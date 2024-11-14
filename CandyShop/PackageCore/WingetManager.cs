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

        public override bool SupportsFetchingOutdated => false;
        public override bool RequiresNameResolution => true;

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

            var output = TrimProgressChars(p.Output);
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

            var output = ParseTable5Cols(p.Output, (firstColName) =>
                HandleValidateOutputElement(firstColName, VALIDATE_FIRST_COLUMN, "Could not validate first column name of winget output for \"winget list\""));

            List<GenericPackage> rtn = output
                .Select(row => new GenericPackage()
                {
                    Name = row[0],
                    Id = row[1],
                    CurrVer = row[2],
                    AvailVer = row[3],
                    Source = row[4]
                })
                .OrderBy(p => p.Name)
                .ToList();

            return rtn;
        }

        /// <exception cref="PackageManagerException"></exception>
        protected override List<GenericPackage> FetchOutdated()
        {
            Log.Error("Winget tried to invoke a capabality that is not implemented: Fetch outdated packages.");
            throw new InvalidOperationException();
        }

        protected override List<GenericPackage> FetchPinList()
        {
            // launch process
            PackageManagerProcess p = BuildProcess($"pin list");
            p.ExecuteHidden();
            ThrowOnError(p);

            var output = ParseTable5Cols(p.Output,
                (firstColName) => HandleValidateOutputElement(firstColName, VALIDATE_FIRST_COLUMN, "Could not validate first column name of winget output for \"winget pin list\""),
                (value)        => HandleValidateOutputElement(value,        VALIDATE_PIN_LIST,     "Could not validate empty list of winget output for \"winget pin list\""));

            List<GenericPackage> rtn = output.Select(row => new GenericPackage()
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

        private string TrimProgressChars(string output)
        {
            int i = 0;
            while (!char.IsLetterOrDigit(output[i]))
            {
                if (i >= output.Length)
                {
                    Log.Debug("Winget output did not contain any letters or digits to parse.");
                    break;
                }
                i++;
            }
            return output[i..];
        }

        private int GetNextColumnIndex(string row, int startIndex)
        {
            int i = startIndex;
            while (i < row.Length && char.IsLetterOrDigit(row[i]))
            {
                i++;
            }

            while (i < row.Length && char.IsWhiteSpace(row[i]))
            {
                i++;
            }

            return i;
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

        private List<string[]> ParseTable5Cols(string outputAsString, Action<string> validateHeader, Action<string> validateEmpty = null)
        {
            Queue<string> output = new(outputAsString.Split(Environment.NewLine));

            // find header and divider; divider consists exclusively of '----' and (localized) header is previous line
            string header = output.Dequeue();
            string divider = output.Dequeue();
            bool isDivider(string value) => ("-".Equals(new string(value.Distinct().ToArray())));

            while (!isDivider(divider))
            {
                // if the winget list is empty, header will be a message like "no pins are configured", divider will be "" and the queue will be empty
                // the message may be validated
                if (output.Count == 0)
                {
                    validateEmpty?.Invoke(TrimProgressChars(header));
                    return [];
                }

                header = divider;
                divider = output.Dequeue();
            }

            // strip characters from header that were indicating progress in winget output and validate
            header = header.Substring(header.Length - divider.Length);
            var firstColNameIndex = header.IndexOf(' ');
            if (firstColNameIndex < 0) firstColNameIndex = 0;
            var firstColName = header[..firstColNameIndex].Trim();
            validateHeader.Invoke(firstColName);

            int nameIndex = 0;
            int idIndex = GetNextColumnIndex(header, nameIndex);
            int versionIndex = GetNextColumnIndex(header, idIndex);
            int availableIndex = GetNextColumnIndex(header, versionIndex);
            int sourceIndex = GetNextColumnIndex(header, availableIndex);

            if (idIndex == 0 || versionIndex == 0 || availableIndex == 0 || sourceIndex == 0)
            {
                throw new PackageManagerException("Failed to parse winget output: Could not find column offsets.");
            }

            // parse table content
            List<string[]> rows = [];
            while (output.Count > 0)
            {
                string row = output.Dequeue();
                if (string.IsNullOrEmpty(row))
                {
                    continue;
                }

                rows.Add([
                    row.Substring(nameIndex, idIndex).Trim(),
                    row.Substring(idIndex, versionIndex - idIndex).Trim(),
                    row.Substring(versionIndex, availableIndex - versionIndex).Trim(),
                    row[availableIndex..sourceIndex].Trim(),
                    row.Substring(sourceIndex).Trim()
                ]);
            }

            return rows;
        }

        // TODO try resolve ids
        // TODO move public up in structure
        public override async Task<Dictionary<string, GenericPackage>> ResolveAbbreviatedNamesAsync(List<GenericPackage> packages)
        {
            int total = packages.Count;
            ConcurrentBag<GenericPackage> unresolvedNames = [];
            ConcurrentDictionary<string, GenericPackage> movedPackages = new();

            var candidates = packages.Where(p => p.HasSource && p.Name.Contains('…')).ToList();
            var options = new ParallelOptions() { MaxDegreeOfParallelism = 5 };
            await Parallel.ForEachAsync(candidates, options, async (package, token) =>
            {
                Log.Debug("Started task");
                string fetched;
                try
                {
                    fetched = await FetchInfoAsync(package);
                }
                catch (PackageManagerException e)
                {
                    Log.Error($"Failed to resolve package with name '{package.Name}': {e.Message}");
                    unresolvedNames.Add(package);
                    return;
                }

                var (name, id) = GetMetaInfo(fetched);
                if (name != null && id != null)
                {
                    movedPackages[package.Name] = package;
                    package.Name = name;
                    package.Id = id;
                }
                else
                {
                    unresolvedNames.Add(package);
                }
                Log.Debug("Finished");
            });

            // log unresolved packages
            Log.Debug($"Resolved incomplete package names for {movedPackages.Count} of {candidates.Count} candidates. Total package count was {total}.");
            if (!unresolvedNames.IsEmpty)
            {
                var value = string.Join(", ", unresolvedNames.Select(p => p.Name).ToList());
                Log.Warning($"Failed to resolve {unresolvedNames.Count} package(s): {value}");
            }

            return movedPackages.ToDictionary();
        }

        private (string, string) GetMetaInfo(string fetchInfoResult)
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
