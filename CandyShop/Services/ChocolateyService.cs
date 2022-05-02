using CandyShop.Chocolatey;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Serilog;

namespace CandyShop.Services
{
    /// <summary>
    /// This service class allows asynchronous, non-blocking access to Chocolatey and implements a cache.
    /// </summary>
    internal class ChocolateyService
    {
        private readonly SemaphoreSlim OutdatedPckgLock = new SemaphoreSlim(1);
        private readonly SemaphoreSlim InstalledPckgLock = new SemaphoreSlim(1);

        private readonly Dictionary<string, string> PckgDetailsCache = new Dictionary<string, string>();
        private readonly Dictionary<string, ChocolateyPackage> InstalledPckgCache = new Dictionary<string, ChocolateyPackage>();
        private readonly Dictionary<string, ChocolateyPackage> OutdatedPckgCache = new Dictionary<string, ChocolateyPackage>();

        /// <exception cref="ChocolateyException"></exception>
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
            catch (ChocolateyException)
            {
                throw;
            }
            finally
            {
                OutdatedPckgLock.Release();
                Log.Debug("Released lock for outdated packages.");
            }

            return OutdatedPckgCache.Values.ToList();
        }

        /// <exception cref="ChocolateyException"></exception>
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
            catch (ChocolateyException)
            {
                throw;
            }
            finally
            {
                InstalledPckgLock.Release();
            }

            return InstalledPckgCache.Values.ToList();
        }

        /// <exception cref="ChocolateyException"></exception>
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
            return names
                .Where(name => InstalledPckgCache.ContainsKey(name))
                .Select(name => InstalledPckgCache[name])
                .ToList();
        }

        public List<ChocolateyPackage> GetPackageByName(List<string> names)
        {
            List<ChocolateyPackage> rtn = new List<ChocolateyPackage>();

            foreach (string name in names)
            {
                if (InstalledPckgCache.ContainsKey(name))
                {
                    rtn.Add(InstalledPckgCache[name]);
                }
                else if (OutdatedPckgCache.ContainsKey(name))
                {
                    rtn.Add(OutdatedPckgCache[name]);
                }
            }

            return rtn;
        }

        /// <summary>
        /// Upgrades a collection of Chocolatey packages. If the <c>validExitCodes</c> parameter
        /// is not present, only zero is considered valid. When specified, the array should
        /// include zero.
        /// If the upgrade process exited with a non-valid code, a <see cref="ChocolateyException"/>
        /// is thrown. 
        /// </summary>
        /// <param name="packages"></param>
        /// <param name="validExitCodes">test</param>
        /// <exception cref="ChocolateyException"></exception>
        public void Upgrade(List<ChocolateyPackage> packages, int[] validExitCodes = null)
        {
            int exitCode = ChocolateyWrapper.Upgrade(packages);
            
            if (validExitCodes == null) validExitCodes = new int[] { 0 };
            if (!validExitCodes.Contains(exitCode))
            {
                throw new ChocolateyException($"choco did not exit cleanly. Returned {exitCode}.");
            }
        }
    }
}
