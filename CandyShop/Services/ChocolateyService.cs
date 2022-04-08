using CandyShop.Chocolatey;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace CandyShop.Services
{
    /// <summary>
    /// This service class allows asynchronous access to Chocolatey and implements a cache.
    /// </summary>
    internal class ChocolateyService
    {
        private readonly SemaphoreSlim OutdatedPckgLock = new SemaphoreSlim(1);
        private readonly SemaphoreSlim InstalledPckgLock = new SemaphoreSlim(1);

        private readonly Dictionary<string, string> PckgDetailsCache = new Dictionary<string, string>();
        private readonly Dictionary<string, ChocolateyPackage> InstalledPckgCache = new Dictionary<string, ChocolateyPackage>();
        private readonly Dictionary<string, ChocolateyPackage> OutdatedPckgCache = new Dictionary<string, ChocolateyPackage>();

        public async Task<List<ChocolateyPackage>> GetOutdatedPackagesAsync()
        {
            await OutdatedPckgLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (OutdatedPckgCache.Count <= 0)
                {
                    List<ChocolateyPackage> outdatedPckgs = await ChocolateyWrapper.FetchOutdatedAsync();
                    outdatedPckgs.ForEach(p => OutdatedPckgCache[p.Name] = p);
                }
            }
            finally
            {
                OutdatedPckgLock.Release();
            }

            return OutdatedPckgCache.Values.ToList();
        }

        public async Task<List<ChocolateyPackage>> GetInstalledPackagesAsync()
        {
            await InstalledPckgLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (InstalledPckgCache.Count <= 0)
                {
                    List<ChocolateyPackage> installedPckgs = await ChocolateyWrapper.FetchInstalledAsync();
                    installedPckgs.ForEach(p => InstalledPckgCache[p.Name] = p);
                }
            }
            finally
            {
                InstalledPckgLock.Release();
            }

            return InstalledPckgCache.Values.ToList();
        }

        public async Task<string> GetPackageDetails(ChocolateyPackage package)
        {
            if (!PckgDetailsCache.TryGetValue(package.Name, out string details))
            {
                details = await ChocolateyWrapper.FetchInfoAsync(package);
                PckgDetailsCache[package.Name] = details;
            }

            return details;
        }

        public List<ChocolateyPackage> GetInstalledPackagesByName(List<string> names)
        {
            List<ChocolateyPackage> rtn = names.Select(name =>
            {
                InstalledPckgCache.TryGetValue(name, out ChocolateyPackage pckg);
                return pckg;
            }).ToList();

            return rtn;
        }

        /// <exception cref="ChocolateyException"></exception>
        public void Upgrade(List<ChocolateyPackage> packages)
        {
            ChocolateyWrapper.Upgrade(packages);
        }
    }
}
