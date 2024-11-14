using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyShop.PackageCore
{
    internal class ChocoManager : AbstractPackageManager
    {
        private readonly List<int> ValidExitCodesOnUpgrade;

        public override bool SupportsFetchingOutdated => true;
        public override bool RequiresNameResolution => false;

        public int ChocoVersionMajor { get; set; } = 2;

        public ChocoManager(int chocoVersionMajor, List<int> validExitCodesOnUpgrade, string binary, bool requireManualElevation, bool allowGsudoCache) : base(binary, requireManualElevation, allowGsudoCache)
        {
            ChocoVersionMajor = chocoVersionMajor;
            ValidExitCodesOnUpgrade = validExitCodesOnUpgrade;
            if (!validExitCodesOnUpgrade.Contains(0))
                Log.Warning("List of valid exit codes does not contain '0'. This looks like a mistake in the configuration file.");
        }

        /// <exception cref="PackageManagerException"></exception>
        protected override string FetchInfo(GenericPackage package)
        {
            StringBuilder rtn = new();

            // launch process
            PackageManagerProcess p = BuildProcess($"info {package.Name}");
            p.ExecuteHidden();
            if (p.ExitCode != 0)
                throw new PackageManagerException($"Chocolatey did not exit cleanly. Returned {p.ExitCode}.");

            // parse
            List<string[]> sections = ParseSections(p.Output);
            foreach (string[] section in sections)
            {
                if (section.Length > 0)
                {
                    for (int i = 0; i < section.Length; i++)
                    {
                        rtn.AppendLine(section[i].Trim());
                    }
                }
            }

            return rtn.ToString();
        }

        /// <exception cref="PackageManagerException"></exception>
        protected override List<GenericPackage> FetchInstalled()
        {
            Log.Information("Fetching installed packages from Chocolatey");

            List<GenericPackage> packages = [];

            // launch process
            var args = ChocoVersionMajor < 2 ? "list --local-only" : "list";
            PackageManagerProcess p = BuildProcess(args);
            p.ExecuteHidden();
            if (p.ExitCode != 0)
                throw new PackageManagerException($"Chocolatey did not exit cleanly. Returned {p.ExitCode}.");

            // parse output
            List<string[]> sections = ParseSections(p.Output);
            foreach (string[] section in sections)
            {
                // check final line and match count
                string summary = section[section.Length - 1];
                if (summary.Contains(' '))
                {
                    int index = summary.IndexOf(' ');
                    string packageCountStr = summary[..index];
                    if (int.TryParse(packageCountStr, out int packageCount))
                    {
                        if (packageCount == section.Length - 1 && summary[index..].Equals(" packages installed."))
                        {
                            // check potential package list entries
                            for (int i = 0; i < section.Length - 1; i++)
                            {
                                if (section[i].Contains(' '))
                                {
                                    // retrieve package
                                    string[] entry = section[i].Split(' ');
                                    if (entry.Length == 2)
                                    {
                                        packages.Add(new GenericPackage()
                                        {
                                            Name = entry[0],
                                            CurrVer = entry[1]
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            packages = ResolveMetaPackages(packages);
            return packages;
        }

        /// <exception cref="PackageManagerException"></exception>
        protected override List<GenericPackage> FetchOutdated()
        {
            Log.Information("Fetching outdated packages from Chocolatey");

            List<GenericPackage> packages = [];

            // launch process
            PackageManagerProcess p = BuildProcess("outdated");
            p.ExecuteHidden();
            if (p.ExitCode != 0)
                throw new PackageManagerException($"Chocolatey did not exit cleanly. Returned {p.ExitCode}.");

            // parse (find header section, followed by package section, followed by summary section)
            List<string[]> sections = ParseSections(p.Output);
            for (int i = 0; i < sections.Count; i++)
            {
                // find header section
                if (sections[i].Length == 2 &&
                    sections[i][0].Equals("Outdated Packages") &&
                    sections[i][1].Equals(" Output is package name | current version | available version | pinned?") &&
                    i + 2 < sections.Count)
                {
                    string[] packageList = sections[i + 1];
                    string[] summary = sections[i + 2];

                    // check summary
                    if (summary.Length > 0 &&
                        summary[0].StartsWith("Chocolatey has determined ") &&
                        summary[0].EndsWith(" package(s) are outdated. "))
                    {
                        /* Do NOT check if count of outdated packages in summary
                         * equals package list size, because they are not equal
                         * in case of faulty Chocolatey packages (which are not
                         * counted towards outdated packages)!
                         * Thanks to Valentin for helping with this issue. */

                        // check packages
                        foreach (string line in packageList)
                        {
                            // retrieve package
                            string[] entry = line.Split('|');
                            if (entry.Length == 4)
                            {
                                GenericPackage pckg = new()
                                {
                                    Name = entry[0],
                                    CurrVer = entry[1],
                                    AvailVer = entry[2],
                                    Pinned = entry[3].Equals("true")
                                };
                                packages.Add(pckg);
                            }
                        }
                    }
                }
            }

            return ResolveMetaPackages(packages);
        }

        protected override List<GenericPackage> FetchPinList()
        {
            PackageManagerProcess p = BuildProcess("pin list --limit-output");
            p.ExecuteHidden();
            if (p.ExitCode != 0)
                throw new PackageManagerException($"Chocolatey did not exit cleanly. Returned {p.ExitCode}.");

            var output = p.Output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            List<GenericPackage> packages = [];
            foreach (var row in output)
            {
                var values = row.Split('|', StringSplitOptions.TrimEntries);
                if (values.Length == 2)
                {
                    packages.Add(new GenericPackage()
                    {
                        Name = values[0],
                        CurrVer = values[1],
                        Pinned = true
                    });
                }
            }
            return packages;
        }

        /// <exception cref="PackageManagerException"></exception>
        protected override void Pin(GenericPackage package)
        {
            var args = $"pin add --name=\"{package.Name}\" --version=\"{package.CurrVer}\"";
            PackageManagerProcess p = BuildProcess(args, useGsudo: RequireManualElevation);
            p.ExecuteHidden();

            if (p.ExitCode != 0)
                throw new PackageManagerException($"Chocolatey did not exit cleanly. Returned {p.ExitCode}.");
        }

        /// <exception cref="PackageManagerException"></exception>
        protected override void Unpin(GenericPackage package)
        {
            PackageManagerProcess p = BuildProcess($"pin remove --name=\"{package.Name}\"", useGsudo: RequireManualElevation);
            p.ExecuteHidden();

            if (p.ExitCode != 0)
                throw new PackageManagerException($"Chocolatey did not exit cleanly. Returned {p.ExitCode}.");
        }

        public override void Upgrade(List<GenericPackage> packages)
        {
            string arg = string.Join(' ', packages.Select(p => p.Name));

            // launch process
            PackageManagerProcess p = BuildProcess($"upgrade {arg} -y", useGsudo: RequireManualElevation);
            p.Execute();

            if (!ValidExitCodesOnUpgrade.Contains(p.ExitCode))
                throw new PackageManagerException($"Chocolatey did not exit cleanly. Returned {p.ExitCode}.");
        }

        /// <summary>
        /// Guess meta packages based on package names.
        /// For every suffixed package (child) where
        /// a meta package (non-suffixed parent) exists 
        /// the parent is registered with the child.
        /// </summary>
        private List<GenericPackage> ResolveMetaPackages(List<GenericPackage> packages)
        {
            foreach (var child in packages)
            {
                if (child.HasChocolateySuffix)
                {
                    foreach (var parent in packages)
                    {
                        if (child.ClearName.ToLower().Equals(parent.Name.ToLower()))
                        {
                            child.Parent = parent;
                        }
                    }
                }
            }

            return packages;
        }

        /// <summary>
        /// Splits the output from a Chocolatey process into sections divided by an empty line
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private List<string[]> ParseSections(string output)
        {
            List<string[]> rtn = [];

            // parse head
            Queue<string> outputLines = new(output.Split("\r\n"));
            if (outputLines.Count > 0)
            {
                if (!outputLines.Dequeue().StartsWith("Chocolatey v"))
                {
                    // TOOD version checks? "Chocolatey v0.10.15"
                }

                // divide into blocks seperated by empty line
                List<string> currentBlock = [];
                while (outputLines.Count > 0)
                {
                    string line = outputLines.Dequeue();
                    if (string.Empty.Equals(line))
                    {
                        rtn.Add(currentBlock.ToArray());
                        currentBlock = [];
                    }
                    else
                    {
                        currentBlock.Add(line);
                    }
                }
            }

            return rtn;
        }

        public override Task<Dictionary<string, GenericPackage>> ResolveAbbreviatedNamesAsync(List<GenericPackage> packages)
        {
            Log.Error("Chocolatey tried to invoke a capabality that is not implemented: Resolving abbreviated names.");
            throw new InvalidOperationException();
        }
    }
}
