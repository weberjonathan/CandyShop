using System.Collections.Generic;
using System.Threading.Tasks;

namespace CandyShop.PackageCore
{
    internal abstract class AbstractPackageManager
    {
        /// <exception cref="PackageManagerException"></exception>
        public abstract List<GenericPackage> FetchInstalled();

        /// <exception cref="PackageManagerException"></exception>
        public abstract List<GenericPackage> FetchOutdated();

        /// <exception cref="PackageManagerException"></exception>
        public abstract string FetchInfo(GenericPackage package);

        /// <exception cref="PackageManagerException"></exception>
        public abstract void Pin(GenericPackage package);

        /// <exception cref="PackageManagerException"></exception>
        public abstract void Unpin(GenericPackage package);

        /// <exception cref="PackageManagerException"></exception>
        public abstract void Upgrade(List<GenericPackage> packages);

        /// <exception cref="PackageManagerException"></exception>
        public async Task<List<GenericPackage>> FetchInstalledAsync()
        {
            return await Task.Run(FetchInstalled);
        }

        /// <exception cref="PackageManagerException"></exception>
        public virtual async Task<List<GenericPackage>> FetchOutdatedAsync()
        {
            return await Task.Run(FetchOutdated);
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task<string> FetchInfoAsync(GenericPackage package)
        {
            return await Task.Run(() => FetchInfo(package));
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task PinAsync(GenericPackage package)
        {
            await Task.Run(() => Pin(package));
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task UnpinAsync(GenericPackage package)
        {
            await Task.Run(() => Unpin(package));
        }
    }
}
