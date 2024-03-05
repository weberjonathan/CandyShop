using CandyShop.Chocolatey;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CandyShop.Services
{
    internal interface IPackageService
    {
        public Task<List<GenericPackage>> GetInstalledPackagesAsync();
        public Task<List<GenericPackage>> GetOutdatedPackagesAsync();
        public Task ClearOutdatedPackages();
        public Task ClearInstalledPackages();
        public Task<string> GetPackageDetailsAsync(string name);
        public GenericPackage GetPackageByName(string name);
        public List<GenericPackage> GetPackagesByName(List<string> names);
        Task PinAsync(GenericPackage package);
        Task UnpinAsync(GenericPackage package);
        void Upgrade(string[] names);
    }
}
