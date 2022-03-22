using CandyShop.Chocolatey;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace CandyShop
{
    class ChocolateyService
    {
        private Dictionary<string, string> PackageInfoCache = new Dictionary<string, string>();

        private Dictionary<string, ChocolateyPackage> InstalledPackages = new Dictionary<string, ChocolateyPackage>();

        public IEnumerable<ChocolateyPackage> GetPackagesByName(IEnumerable<string> names)
        {
            return names.Select(name =>
            {
                InstalledPackages.TryGetValue(name, out ChocolateyPackage rtn);
                return rtn;
            });
        }

        /// <exception cref="ChocolateyException"></exception>
        public async Task<List<ChocolateyPackage>> FetchInstalledAsync()
        {
            return await Task.Run(FetchInstalled);
        }

        /// <exception cref="ChocolateyException"></exception>
        public List<ChocolateyPackage> FetchInstalled()
        {
            List<ChocolateyPackage> packages = new List<ChocolateyPackage>();

            // launch process TODO try catch
            ChocolateyProcess p = new ChocolateyProcess("list --local-only");
            p.ExecuteHidden();

            // parse output (get sections, all wanted info is in single section)
            // conditions
            // - final line looks like "54 packages included."; match count
            // - package list entries look like "name version"
            List<string[]> sections = p.OutputBySection;
            foreach (string[] section in sections)
            {
                // check final line and match count
                string summary = section[section.Length - 1];
                if (summary.Contains(' '))
                {
                    int indexOf = summary.IndexOf(' ');
                    string packageCountStr = summary.Substring(0, indexOf);
                    int packageCount;

                    if (int.TryParse(packageCountStr, out packageCount))
                    {
                        if (packageCount == section.Length - 1 && summary.Substring(indexOf).Equals(" packages installed."))
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
                                        packages.Add(new ChocolateyPackage
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

            packages = GuessMetaPackages(packages);

            InstalledPackages.Clear();
            packages.ForEach(p => InstalledPackages.Add(p.Name, p));

            return packages;
        }

        /// <exception cref="ChocolateyException"></exception>
        public async Task<List<ChocolateyPackage>> FetchOutdatedAsync()
        {
            return await Task.Run(FetchOutdated);
        }

        /// <exception cref="ChocolateyException"></exception>
        public List<ChocolateyPackage> FetchOutdated()
        {
            List<ChocolateyPackage> packages = new List<ChocolateyPackage>();

            // launch process
            ChocolateyProcess p = new ChocolateyProcess("outdated");
            p.ExecuteHidden();

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
                    if (summary.Length == 1 &&
                        summary[0].StartsWith("Chocolatey has determined ") &&
                        summary[0].EndsWith(" package(s) are outdated. ") &&
                        summary[0].Length > 51 &&
                        int.TryParse(summary[0].Substring(26, summary[0].Length - 51), out int outdatedCount))
                    {
                        if (outdatedCount == packageList.Length)
                        {
                            // check packages
                            foreach (string line in packageList)
                            {
                                // retrieve package
                                string[] entry = line.Split('|');
                                if (entry.Length == 4)
                                {
                                    ChocolateyPackage pckg = new ChocolateyPackage
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
            }

            return GuessMetaPackages(packages);
        }

        /// <summary>Get detailed information for a given package. Fetches it, if not available.</summary>
        /// <exception cref="ChocolateyException"></exception>
        public async Task<string> GetOrFetchInfo(ChocolateyPackage package)
        {
            if (!PackageInfoCache.TryGetValue(package.Name, out string info))
            {
                info = await FetchInfoAsync(package);
                if (!PackageInfoCache.ContainsKey(package.Name))
                {
                    PackageInfoCache.Add(package.Name, info);
                }
            }

            return info;
        }

        /// <summary>Fetches detailed information for a given package. Use <see cref="GetOrFetchInfo(ChocolateyPackage)"/> to utilize cached details.</summary>
        /// <exception cref="ChocolateyException"></exception>
        public async Task<string> FetchInfoAsync(ChocolateyPackage package)
        {
            return await Task.Run(() => FetchInfo(package));
        }

        /// <summary>Fetches detailed information for a given package. Use <see cref="GetOrFetchInfo(ChocolateyPackage)"/> to utilize cached details.</summary>
        /// <exception cref="ChocolateyException"></exception>
        public string FetchInfo(ChocolateyPackage package)
        {
            StringBuilder rtn = new StringBuilder();

            // launch process
            ChocolateyProcess p = new ChocolateyProcess($"info {package.Name}");
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

        /// <exception cref="ChocolateyException"></exception>
        public void Upgrade(List<ChocolateyPackage> packages)
        {
            string argument = "";
            foreach (ChocolateyPackage pckg in packages)
            {
                argument += pckg.Name + " ";
            }

            // launch process
            ChocolateyProcess p = new ChocolateyProcess($"upgrade {argument} -y");
            p.FailOnNonZeroExitCode = false;
            p.Execute();

            // check exit
            // TODO send appropriate messages for non-zero valid exit codes
            // no priority because console is displayed and choco displays that information in console
            var validExitCodes = new List<int> { 0, 1641, 3010, 350, 1604 };
            if (!validExitCodes.Contains(p.ExitCode))
            {
                throw new ChocolateyException($"choco did not exit cleanly. Returned {p.ExitCode}.");
            }
        }

        /// <summary>
        /// Guess meta packages based on package names.
        /// A package is considered to be a meta package, if there are other
        /// packages of the same name with a suffix (like *.install).
        /// Ignores case.
        /// </summary>
        private List<ChocolateyPackage> GuessMetaPackages(List<ChocolateyPackage> packages)
        {
            foreach (ChocolateyPackage childPackageCandidate in packages)
            {
                if (childPackageCandidate.HasSuffix)
                {
                    foreach (ChocolateyPackage metaPackageCandidate in packages)
                    {
                        if (childPackageCandidate.ClearName.ToLower().Equals(
                            metaPackageCandidate.Name.ToLower()))
                        {
                            childPackageCandidate.MetaPackage = metaPackageCandidate;
                        }
                    }
                }
            }

            return packages;
        }
    }
}
