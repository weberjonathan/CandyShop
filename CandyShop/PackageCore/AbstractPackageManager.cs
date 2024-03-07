using System.Collections.Generic;
using System.Threading.Tasks;

namespace CandyShop.PackageCore
{
    internal abstract class AbstractPackageManager
    {
        public abstract List<GenericPackage> FetchInstalled();
        public abstract List<GenericPackage> FetchOutdated();
        public abstract string FetchInfo(GenericPackage package);
        public abstract void Pin(GenericPackage package);
        public abstract void Unpin(GenericPackage package);
        public abstract void Upgrade(List<GenericPackage> packages);

        public async Task<List<GenericPackage>> FetchInstalledAsync()
        {
            return await Task.Run(FetchInstalled);
        }

        public virtual async Task<List<GenericPackage>> FetchOutdatedAsync()
        {
            return await Task.Run(FetchOutdated);
        }

        public async Task<string> FetchInfoAsync(GenericPackage package)
        {
            return await Task.Run(() => FetchInfo(package));
        }

        public async Task PinAsync(GenericPackage package)
        {
            await Task.Run(() => Pin(package));
        }

        public async Task UnpinAsync(GenericPackage package)
        {
            await Task.Run(() => Unpin(package));
        }
    }
}
