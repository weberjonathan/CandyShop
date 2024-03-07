using CandyShop.Chocolatey;
using CandyShop.Winget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CandyShop.PackageManager
{
    internal class WingetManager : AbstractPackageManager
    {
        public override string FetchInfo(GenericPackage package)
        {
            if (!package.HasSource)
            {
                return "No sources available for the selected package.";
            }

            WingetProcess p = ProcessFactory.Winget($"show --id \"{package.Id}\" --exact");
            p.ExecuteHidden();

            int start = p.Output.IndexOf("Gefunden ") + "Gefunden ".Length; // TODO make locale indepdent
            return start > 0 ? p.Output[start..] : p.Output;
        }

        public override List<GenericPackage> FetchInstalled()
        {
            // TODO make sure dequeues do not throw bc of missing elements
            // TODO make sure its locale indepent

            // launch process
            WingetProcess p = ProcessFactory.Winget($"list");
            p.ExecuteHidden();

            // remove download indicators from output by skipping to first mention of "Name"
            int i = 0;
            while (p.Output[i] != 'N')
            {
                i++;
            }

            Queue<string> output = new Queue<string>(p.Output[i..].Split(Environment.NewLine));

            string header = output.Dequeue();
            if (!header.StartsWith("Name"))
            {
                throw new ChocolateyException("Failed to parse winget output: Could not find header.");
            }

            int nameIndex = 0;
            int idIndex = GetNextColumnIndex(header, nameIndex);
            int versionIndex = GetNextColumnIndex(header, idIndex);
            int availableIndex = GetNextColumnIndex(header, versionIndex);
            int sourceIndex = GetNextColumnIndex(header, availableIndex);

            if (idIndex == 0 || versionIndex == 0 || availableIndex == 0 || sourceIndex == 0)
            {
                throw new ChocolateyException("Failed to parse winget output: Could not find offsets.");
            }

            string divider = output.Dequeue();
            if (!divider.StartsWith('-') && sourceIndex < divider.Length)
            {
                throw new ChocolateyException(); // TODO
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

        public override List<GenericPackage> FetchOutdated()
        {
            var packages = FetchInstalled();
            return packages.Where(p => !p.CurrVer.Equals(p.AvailVer)).ToList();
        }

        public override async Task<List<GenericPackage>> FetchOutdatedAsync()
        {
            var packages = await FetchInstalledAsync();
            return packages.Where(p => !string.IsNullOrEmpty(p.AvailVer) && !p.CurrVer.Equals(p.AvailVer)).ToList();
        }

        public override void Pin(GenericPackage package)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override void Unpin(GenericPackage package)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override void Upgrade(List<GenericPackage> packages)
        {
            if (packages.Count == 0) return;

            List<WingetProcess> upgradeCommands = new();
            foreach (var pckg in packages)
            {
                string command = $"upgrade --id \"{pckg.Id}\" --silent --exact";
                var cmd = ProcessFactory.Winget(command);
                upgradeCommands.Add(cmd);
            }

            var p = ProcessFactory.WingetPrivileged(upgradeCommands);
            p.FailOnNonZeroExitCode = false;
            p.Execute();

            if (p.ExitCode != 0)
                throw new ChocolateyException($"Winget did not exit cleanly. Returned {p.ExitCode}.");
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
    }
}
