using CandyShop.Chocolatey;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CandyShop.Services
{
    internal interface IPackageService
    {
        public Task<List<GenericPackage>> GetOutdatedPackagesAsync();
        public GenericPackage GetPackageByName(string name);
        public List<GenericPackage> GetPackagesByName(List<string> names);
        void Pin(GenericPackage package);
        void Unpin(GenericPackage package);
        void Upgrade(string[] names);
    }
}
