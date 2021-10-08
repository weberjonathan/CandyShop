using CandyShop.Chocolatey;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CandyShop
{
    interface IPackageService
    {
        public List<ChocolateyPackage> InstalledPackages { get; }

        public List<ChocolateyPackage> OutdatedPackages { get; }

        public void Upgrade(List<ChocolateyPackage> packages);
        
        public Task FetchInstalledPackagesAsync();

        public Task FetchOutdatedPackagesAsync();

        public Task<string> GetPackageDetails(ChocolateyPackage package);
    }
}