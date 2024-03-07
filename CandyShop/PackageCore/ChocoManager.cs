using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CandyShop.PackageCore
{
    internal class ChocoManager : AbstractPackageManager
    {
        // TODO annoate throwing exceptions

        private readonly List<int> ValidExitCodesOnUpgrade;

        public ChocoManager()
        {
            ValidExitCodesOnUpgrade = [ 0 ];
        }
        
        public ChocoManager(List<int> validExitCodesOnUpgrade)
        {
            ValidExitCodesOnUpgrade = validExitCodesOnUpgrade;
            if (!validExitCodesOnUpgrade.Contains(0))
                Log.Warning("List of valid exit codes does not contain '0'. This looks like a mistake in the configuration file.");
        }

        public override string FetchInfo(GenericPackage package)
        {
            StringBuilder rtn = new();

            // launch process
            ChocolateyProcess p = ProcessFactory.Choco($"info {package.Name}");
            p.ExecuteHidden();

            // parse
            List<string[]> sections = p.OutputBySection;
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

        public override List<GenericPackage> FetchInstalled()
        {
            Log.Information("Fetching installed packages from Chocolatey");

            List<GenericPackage> packages = new();

            // launch process
            var args = ChocolateyProcess.MajorVersion < 2 ? "list --local-only" : "list";
            ChocolateyProcess p = ProcessFactory.Choco(args);
            p.ExecuteHidden();

            // parse output
            List<string[]> sections = p.OutputBySection;
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

        public override List<GenericPackage> FetchOutdated()
        {
            Log.Information("Fetching outdated packages from Chocolatey");

            List<GenericPackage> packages = [];

            // launch process
            ChocolateyProcess p = ProcessFactory.Choco("outdated");
            p.ExecuteHidden();

            // TODO throw exceptions for parsing errors
            // account for optional extra sections at the start
            // and end of sections header, package list and summary

            // parse (find header section, followed by package section, followed by summary section)
            List<string[]> sections = p.OutputBySection;
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

        // TODO this used to return exit code; now probably maybe have to catch exxception?
        public override void Pin(GenericPackage package)
        {
            var args = $"pin add --name=\"{package.Name}\" --version=\"{package.CurrVer}\"";
            ChocolateyProcess p = ProcessFactory.ChocoPrivileged(args);
            p.ExecuteHidden();
        }

        // TODO this used to return exit code; now probably maybe have to catch exxception?
        public override void Unpin(GenericPackage package)
        {
            ChocolateyProcess p = ProcessFactory.ChocoPrivileged($"pin remove --name=\"{package.Name}\"");
            p.ExecuteHidden();
        }

        public override void Upgrade(List<GenericPackage> packages)
        {
            string argument = "";
            string arg2 = string.Join(' ', packages.Select(p => p.Name));
            foreach (var pckg in packages)
            {
                argument += pckg.Name + " ";
            }
            // TODO test if argument and arg2 are the same

            // launch process
            ChocolateyProcess p = ProcessFactory.ChocoPrivileged($"upgrade {argument} -y");
            p.FailOnNonZeroExitCode = false;
            p.Execute();

            if (!ValidExitCodesOnUpgrade.Contains(p.ExitCode))
            {
                throw new ChocolateyException($"choco did not exit cleanly. Returned {p.ExitCode}.");
            }
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
    }
}
