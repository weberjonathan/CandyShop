using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CandyShop.PackageCore
{
    internal class WingetManager(bool suppressLogWarning) : AbstractPackageManager
    {
        private readonly List<string> VALIDATE_SEARCHES = ["Found", "Gefunden"];
        private readonly List<string> VALIDATE_PIN_LIST = ["There are no pins configured.", "Es sind keine Pins konfiguriert."];
        private readonly List<string> VALIDATE_FIRST_COLUMN = ["Name"];

        private readonly bool SuppressLogWarnings = suppressLogWarning;

        /// <exception cref="PackageManagerException"></exception>
        public override string FetchInfo(GenericPackage package)
        {
            if (!package.HasSource)
            {
                return "No sources available for the selected package.";
            }

            PackageManagerProcess p = ProcessFactory.Winget($"show --id \"{package.Id}\" --exact");
            p.ExecuteHidden();
            if (p.ExitCode != 0)
                throw new PackageManagerException($"Winget did not exit cleanly. Returned {p.ExitCode}.");

            var output = TrimProgressChars(p.Output);
            var first_word_index = output.IndexOf(' ');
            if (first_word_index < 0) first_word_index = 0;
            var first_word = output[..first_word_index].Trim();
            HandleValidateOutputElement(first_word, VALIDATE_SEARCHES, "Could not validate first word of winget output for \"winget show\"");
            return output[(first_word_index + 1)..];
        }

        /// <exception cref="PackageManagerException"></exception>
        public override List<GenericPackage> FetchInstalled()
        {
            // launch process
            PackageManagerProcess p = ProcessFactory.Winget($"list");
            p.ExecuteHidden();
            if (p.ExitCode != 0)
                throw new PackageManagerException($"Winget did not exit cleanly. Returned {p.ExitCode}.");

            Queue<string> output = new Queue<string>(p.Output.Split(Environment.NewLine));

            // find header and divider; divider consists exclusively of '----' and (localized) header is previous line 
            string header = output.Dequeue();
            string divider = output.Dequeue();
            bool isDivider(string value) => ("-".Equals(new string(value.Distinct().ToArray())));

            while (!isDivider(divider))
            {
                if (output.Count == 0)
                {
                    // winget shows no divider or header for empty lists, so assume the list is empty
                    // this could be verified by parsing the output message (like "Es sind keine Pins konfiguriert." for "winget pin list"),
                    // but that is difficult bc of various languages
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
            HandleValidateOutputElement(firstColName, VALIDATE_FIRST_COLUMN, "Could not validate first column name of winget output for \"winget list\"");

            int nameIndex = 0;
            int idIndex = GetNextColumnIndex(header, nameIndex);
            int versionIndex = GetNextColumnIndex(header, idIndex);
            int availableIndex = GetNextColumnIndex(header, versionIndex);
            int sourceIndex = GetNextColumnIndex(header, availableIndex);

            if (idIndex == 0 || versionIndex == 0 || availableIndex == 0 || sourceIndex == 0)
            {
                throw new PackageManagerException("Failed to parse winget output: Could not find column offsets.");
            }

            List<GenericPackage> packages = [];
            while (output.Count > 0)
            {
                string row = output.Dequeue();
                if (string.IsNullOrEmpty(row))
                {
                    continue;
                }

                packages.Add(new()
                {
                    Name = row.Substring(nameIndex, idIndex).Trim(),
                    Id = row.Substring(idIndex, versionIndex - idIndex).Trim(),
                    CurrVer = row.Substring(versionIndex, availableIndex - versionIndex).Trim(),
                    AvailVer = row[availableIndex..sourceIndex].Trim(),
                    Source = row.Substring(sourceIndex).Trim()
                });
            }

            return packages;
        }

        /// <exception cref="PackageManagerException"></exception>
        public override List<GenericPackage> FetchOutdated()
        {
            var packages = FetchInstalled();
            return packages.Where(p => !p.CurrVer.Equals(p.AvailVer)).ToList();
        }

        /// <exception cref="PackageManagerException"></exception>
        public override async Task<List<GenericPackage>> FetchOutdatedAsync()
        {
            var packages = await FetchInstalledAsync();
            return packages.Where(p => !string.IsNullOrEmpty(p.AvailVer) && !p.CurrVer.Equals(p.AvailVer)).ToList();
        }

        /// <exception cref="PackageManagerException"></exception>
        public override void Pin(GenericPackage package)
        {
            // TODO
            throw new PackageManagerException("Pin has not been implemented yet for Winget.");
        }

        /// <exception cref="PackageManagerException"></exception>
        public override void Unpin(GenericPackage package)
        {
            // TODO
            throw new PackageManagerException("Pin has not been implemented yet for Winget.");
        }

        /// <exception cref="PackageManagerException"></exception>
        public override void Upgrade(List<GenericPackage> packages)
        {
            if (packages.Count == 0) return;

            var arguments = packages.Select(p => $"upgrade --id \"{p.Id}\" --silent --exact").ToList();
            var p = ProcessFactory.WingetBatchPrivileged(arguments);
            p.Execute();

            if (p.ExitCode != 0)
                throw new PackageManagerException($"Winget did not exit cleanly. Returned {p.ExitCode}.");
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
                    Log.Warning("{0}: \"{1}\". This message will be suppressed for future occurences.",
                        message, value);
                VALIDATE_SEARCHES.Add(value);
            }
        }
    }
}
