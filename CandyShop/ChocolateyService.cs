using CandyShop.Chocolatey;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CandyShop
{
    class ChocolateyService : IPackageService
    {
        private Dictionary<ChocolateyPackage, string> _PackageDetailsCache = new Dictionary<ChocolateyPackage, string>();

        public List<ChocolateyPackage> InstalledPackages { get; private set; } = new List<ChocolateyPackage>();

        public List<ChocolateyPackage> OutdatedPackages { get; private set; } = new List<ChocolateyPackage>();

        public async Task FetchInstalledPackagesAsync()
        {
            List<ChocolateyPackage> packages = await ChocolateyWrapper.ListInstalledAsync();
            InstalledPackages = packages;
        }

        public async Task FetchOutdatedPackagesAsync()
        {
            List<ChocolateyPackage> packages = await ChocolateyWrapper.CheckOutdatedAsync();
            OutdatedPackages = packages;
        }

        /// <summary>Loads package details through chocolatey or from cache.</summary>
        /// <param name="package"></param>
        /// <returns>Package details as multiline string.</returns>
        /// <exception cref="ChocolateyException"></exception>"
        public async Task<string> GetPackageDetails(ChocolateyPackage package)
        {
            if (!_PackageDetailsCache.TryGetValue(package, out string details))
            {
                details = await ChocolateyWrapper.GetInfoAsync(package);
            }

            return details;
        }

        public void Upgrade(List<ChocolateyPackage> packages)
        {
            ChocolateyWrapper.Upgrade(packages);
        }
    }
}