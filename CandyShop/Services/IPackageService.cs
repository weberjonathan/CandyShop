using System.Collections.Generic;
using System.Threading.Tasks;

namespace CandyShop.Services
{
    internal interface IPackageService
    {
        public Task<List<GenericPackage>> GetInstalledPackagesAsync();
        public Task<List<GenericPackage>> GetOutdatedPackagesAsync();
        public Task ClearOutdatedPackages();
        public Task ClearInstalledPackages();
        public Task ClearPackageDetails();
        public Task<string> GetPackageDetailsAsync(string name);
        public GenericPackage GetPackageByName(string name);
        public List<GenericPackage> GetPackagesByName(List<string> names);
        Task PinAsync(string name);
        Task UnpinAsync(string name);
        void Upgrade(string[] names);
    }
}
