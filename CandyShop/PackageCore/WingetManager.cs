using CandyShop.Properties;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CandyShop.PackageCore
{
    internal class WingetManager(bool suppressLogWarning, string binary, bool requireManualElevation, bool allowGsudoCache) : AbstractPackageManager(binary, requireManualElevation, allowGsudoCache)
    {
        private readonly List<string> VALIDATE_SEARCHES = ["Found", "Gefunden"];
        private readonly List<string> VALIDATE_PIN_LIST = ["There are no pins configured.", "Es sind keine Pins konfiguriert."];
        private readonly List<string> VALIDATE_FIRST_COLUMN = ["Name"];

        private readonly bool SuppressLogWarnings = suppressLogWarning;

        public override bool SupportsFetchingOutdated => true;
        public override bool RequiresNameResolution => true;

        /// <exception cref="PackageManagerException"></exception>
        /// <exception cref="CandyShopException"></exception>
        public override void Upgrade(List<GenericPackage> packages)
        {
            if (packages.Count == 0) return;

            // start gsudo cache session
            bool isCacheEnabled = false;
            if (AllowGsudoCache && RequireManualElevation)
            {
                EnableGsudoCache();
                isCacheEnabled = true;
            }

            // perform upgrades
            List<int> nonZeroExitCodes = [];
            foreach (var package in packages)
            {
                var arguments = $"upgrade --id \"{package.Id}\" --silent --exact";
                var p = BuildProcess(arguments, useGsudo: RequireManualElevation);
                try
                {
                    p.Execute();
                    ThrowOnError(p);
                }
                catch (Exception e)
                {
                    Log.Error($"Winget upgrade process \"{p.Binary} {p.Arguments}\" failed with exit code {p.ExitCode}: {e.Message}");
                    nonZeroExitCodes.Add(-1);
                }
            }

            // if the upgrade console is closed during upgrading, Candy Shop exits,
            // which also ends the gsudo cache session bc it is tied to this pid

            // exit gsudo cache session
            if (isCacheEnabled)
                DisableGsudoCache();

            if (nonZeroExitCodes.Count > 0)
                throw new PackageManagerException($"One or more winget upgrade processes failed with exit codes {string.Join(", ", nonZeroExitCodes)}. See log for more information.");
        }

        public override async Task<List<GenericPackage>> ResolveAbbreviatedNamesAsync(List<GenericPackage> unresolved)
        {
            int unresolvedTotal = unresolved.Count;
            ConcurrentBag<GenericPackage> failed = [];
            ConcurrentBag<GenericPackage> resolved = [];

            unresolved = unresolved.Where(p => p.HasSource && p.Name.Contains('…')).ToList();
            var options = new ParallelOptions() { MaxDegreeOfParallelism = 5 };

            await Parallel.ForEachAsync(unresolved, options, async (package, token) =>
            {
                string fetched;
                try
                {
                    fetched = await FetchInfoAsync(package);
                }
                catch (PackageManagerException e)
                {
                    Log.Error($"Failed to resolve package with name '{package.Name}': {e.Message}");
                    failed.Add(package);
                    return;
                }

                var (name, id) = ExtractNameAndIdFromInfo(fetched);
                if (name != null && id != null)
                {
                    package.Name = name;
                    package.Id = id;
                    resolved.Add(package);
                }
                else
                {
                    failed.Add(package);
                }
            });

            // log unresolved packages
            Log.Debug($"WingetManager [{Environment.CurrentManagedThreadId}]: Resolved incomplete package names for {resolved.Count} of {unresolvedTotal} candidates.");
            if (!failed.IsEmpty)
            {
                var value = string.Join(", ", failed.Select(p => p.Name));
                Log.Warning($"Failed to resolve {failed.Count} package(s): {value}");
            }

            return resolved.Concat(failed).ToList();
        }

        protected override PackageManagerProcess BuildProcess(string args, bool useGsudo = false)
        {
            if (!args.Contains("--disable-interactivity"))
                args += " --disable-interactivity";

            return base.BuildProcess(args, useGsudo);
        }

        /// <exception cref="PackageManagerException"></exception>
        protected override string FetchInfo(GenericPackage package)
        {
            if (!package.HasSource)
            {
                return "No sources available for the selected package.";
            }

            PackageManagerProcess p = BuildProcess($"show --id \"{package.Id}\" --exact");
            p.ExecuteHidden();
            ThrowOnError(p);

            var output = WingetParser.TrimProgressChars(p.Output);
            var first_word_index = output.IndexOf(' ');
            if (first_word_index < 0) first_word_index = 0;
            var first_word = output[..first_word_index].Trim();
            HandleValidateOutputElement(first_word, VALIDATE_SEARCHES, "Could not validate first word of winget output for \"winget show\"");
            return output[(first_word_index + 1)..];
        }

        /// <exception cref="PackageManagerException"></exception>
        protected override List<GenericPackage> FetchInstalled()
        {
            Log.Information($"Fetching installed packages from winget.");

            // launch process
            PackageManagerProcess p = BuildProcess($"list");
            p.ExecuteHidden();
            ThrowOnError(p);

            // parse and validate
            WingetParser parser = new(p.Output);
            string[] cols = parser.Columns;
            HandleValidateOutputElement(cols[0], VALIDATE_FIRST_COLUMN, "Could not validate first column name of winget output for \"winget list\""); // TODO eval this, also make it safe
            if (cols.Length != 4 && cols.Length != 5)
            {
                Log.Debug($"WingetManager [{Environment.CurrentManagedThreadId}]: Column layout is '{string.Join(", ", cols)}'");
                throw new PackageManagerException($"Failed to fetch installed packages: Expected 4 or 5 columns, found {cols.Length}");
            }

            // build packages
            List<GenericPackage> rtn = parser.Items
                .Select(GenericPackage.FromArgs)
                .OrderBy(p => p.Name)
                .ToList();

            return rtn;
        }

        /// <exception cref="PackageManagerException"></exception>
        protected override List<GenericPackage> FetchOutdated()
        {
            Log.Information($"Fetching outdated packages from winget.");
            Log.Debug($"WingetManager [{Environment.CurrentManagedThreadId}]: Fetching outdated packages.");

            // launch process
            PackageManagerProcess p = BuildProcess($"list --upgrade-available --include-pinned --include-unknown");
            p.ExecuteHidden();
            ThrowOnError(p);

            // parse and validate
            WingetParser parser = new(p.Output);
            string[] cols = parser.Columns;
            HandleValidateOutputElement(cols[0], VALIDATE_FIRST_COLUMN, "Could not validate first column name of winget output for \"winget list\""); // TODO eval this, also make it safe
            if (cols.Length != 5)
            {
                Log.Debug($"WingetManager [{Environment.CurrentManagedThreadId}]: Column layout is '{string.Join(", ", cols)}'");
                throw new PackageManagerException($"Failed to fetch installed packages: Expected 5 columns, found {cols.Length}");
            }

            // build packages
            List<GenericPackage> rtn = parser.Items
                .Select(GenericPackage.FromArgs)
                .Where(p => !"Unknown".Equals(p.CurrVer))
                .OrderBy(p => p.Name)
                .ToList();

            return rtn;
        }

        // TODO
        protected override List<GenericPackage> FetchPinList()
        {
            // launch process
            PackageManagerProcess p = BuildProcess($"pin list");
            p.ExecuteHidden();
            ThrowOnError(p);

            // parse and validate
            WingetParser parser = new(p.Output);
            string[] cols = parser.Columns;
            if (cols.Length != 5)
            {
                Log.Debug($"WingetManager [{Environment.CurrentManagedThreadId}]: Column layout is '{string.Join(", ", cols)}'");
                throw new PackageManagerException($"Failed to fetch installed packages: Expected 5 columns, found {cols.Length}");
            }

            // TODO
            //(firstColName) => HandleValidateOutputElement(firstColName, VALIDATE_FIRST_COLUMN, "Could not validate first column name of winget output for \"winget pin list\""),
            //    (value)        => HandleValidateOutputElement(value,        VALIDATE_PIN_LIST,     "Could not validate empty list of winget output for \"winget pin list\""));

            List<GenericPackage> rtn = parser.Items.Select(row => new GenericPackage()
            {
                Name = row[0],
                Id = row[1],
                CurrVer = row[2],
                Source = row[3],
                Pinned = true
            }).ToList();

            return rtn;
        }

        /// <exception cref="PackageManagerException"></exception>
        protected override void Pin(GenericPackage package)
        {
            var args = $"pin add --id \"{package.Id}\" --exact";
            PackageManagerProcess p = BuildProcess(args);
            p.ExecuteHidden();
            ThrowOnError(p);
        }

        /// <exception cref="PackageManagerException"></exception>
        protected override void Unpin(GenericPackage package)
        {
            var args = $"pin remove --id \"{package.Id}\" --exact";
            PackageManagerProcess p = BuildProcess(args);
            p.ExecuteHidden();
            ThrowOnError(p);
        }

        private void HandleValidateOutputElement(string value, List<string> valid, string message)
        {
            if (!SuppressLogWarnings)
            {
                if (!valid.Contains(value))
                {
                    Log.Warning("{0}: \"{1}\". This message will be suppressed for future occurences.", message, value);
                    valid.Add(value);
                }
            }
        }

        private (string, string) ExtractNameAndIdFromInfo(string fetchInfoResult)
        {
            string name = null;
            string id = null;

            var lines = fetchInfoResult.Split(Environment.NewLine);
            if (lines.Length > 0)
            {
                string meta = lines[0]; // pattern: "<name> [<id>]"
                var index = meta.Split('[');
                if (index.Length == 2)
                {
                    name = index[0].TrimEnd();
                    id = index[1].TrimEnd(']');
                }
            }

            return (name, id);
        }

        /// <exception cref="PackageManagerException"></exception>
        private void ThrowOnError(PackageManagerProcess p)
        {
            // https://github.com/microsoft/winget-cli/blob/master/doc/windows/package-manager/winget/returnCodes.md
            switch (p.ExitCode)
            {
                case 0:
                    return;
                case -1978335162:
                    throw new PackageManagerException(LocaleEN.ERROR_WINGET_SRC_AGREE);
                default:
                    throw new PackageManagerException($"Winget did not exit cleanly. Returned {p.ExitCode}.");
            }
        }
    }
}
