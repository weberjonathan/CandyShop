using CandyShop.Chocolatey;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Serilog;
using System;

namespace CandyShop.Services
{
    /// <summary>
    /// This service class allows asynchronous, non-blocking access to Chocolatey and implements a cache.
    /// </summary>
    internal class ChocolateyService : IPackageService
    {
        private readonly SemaphoreSlim OutdatedPckgLock = new SemaphoreSlim(1);
        private readonly SemaphoreSlim InstalledPckgLock = new SemaphoreSlim(1);

        // TODO use proper package repository instead
        private readonly Dictionary<string, string> PckgDetailsCache = new Dictionary<string, string>();
        private readonly Dictionary<string, ChocolateyPackage> InstalledPckgCache = new Dictionary<string, ChocolateyPackage>();
        private readonly Dictionary<string, ChocolateyPackage> OutdatedPckgCache = new Dictionary<string, ChocolateyPackage>();

        // ------------- GENERIC PACKAGE METHODS ------------------------------

        public async Task<List<GenericPackage>> GetInstalledPackagesAsync()
        {
            var chocoPackages = await GetInstalledChocoPackagesAsync();
            return chocoPackages.Select(p => new GenericPackage(p)).ToList();
        }

        /// <exception cref="ChocolateyException"></exception>
        public async Task<List<GenericPackage>> GetOutdatedPackagesAsync()
        {
            var chocoPackages = await GetOutdatedChocoPackagesAsync();
            return chocoPackages.Select(p => new GenericPackage(p)).ToList();
        }

        public async Task ClearOutdatedPackages()
        {
            await OutdatedPckgLock.WaitAsync().ConfigureAwait(false);
            OutdatedPckgCache.Clear();
            OutdatedPckgLock.Release();
        }

        public async Task<string> GetPackageDetailsAsync(string name)
        {
            return await GetChocoPackageDetails(new ChocolateyPackage() { Name = name });
        }

        public GenericPackage GetPackageByName(string name)
        {
            var chocoPackage = GetChocoPackageByName(name);
            return new GenericPackage(chocoPackage);
        }

        public List<GenericPackage> GetPackagesByName(List<string> names)
        {
            var chocoPackages = GetChocoPackagesByName(names);
            return chocoPackages.Select(p => new GenericPackage(p)).ToList();
        }

        /// <exception cref="ChocolateyException"></exception>
        public async Task PinAsync(GenericPackage package)
        {
            Log.Information($"Attempting to pin package {package.Name}.");
            int exitCode = await Task.Run(() => ChocolateyWrapper.Pin(package.Name, package.CurrVer));
            Log.Debug($"choco add pin operation for {package.Name} returned with {exitCode}.");
            if (exitCode != 0)
            {
                throw new ChocolateyException($"choco did not exit cleanly. Returned {exitCode}.");
            }

            package.Pinned = true;
            await UpdateCachedItem(package);
        }

        /// <exception cref="ChocolateyException"></exception>
        public async Task UnpinAsync(GenericPackage package)
        {
            Log.Information($"Attempting to unpin package {package.Name}.");
            int exitCode = await Task.Run(() => ChocolateyWrapper.Unpin(package.Name));
            Log.Debug($"choco pin remove operation for {package.Name} returned with {exitCode}.");
            if (exitCode != 0)
            {
                throw new ChocolateyException($"choco did not exit cleanly. Returned {exitCode}.");
            }

            package.Pinned = false;
            await UpdateCachedItem(package);
        }

        /// <exception cref="ChocolateyException"></exception>
        public void Upgrade(string[] names)
        {
            List<ChocolateyPackage> chocoPackages = GetChocoPackagesByName(names.ToList());
            if (chocoPackages.Count <= 0)
            {
                return;
            }

            try
            {
                Upgrade(chocoPackages, ContextSingleton.Get.ValidExitCodes.ToArray());
            }
            catch (ChocolateyException e)
            {
                throw new ChocolateyException(e.Message, e);
            }
            finally
            {
                ClearOutdatedPackages(); // TODO this should be moved into async method
            }
        }

        // --------------------------------------------------------------------

        /// <exception cref="ChocolateyException"></exception>
        public async Task<List<ChocolateyPackage>> GetOutdatedChocoPackagesAsync()
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
        public async Task<List<ChocolateyPackage>> GetInstalledChocoPackagesAsync()
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
        public async Task<string> GetChocoPackageDetails(ChocolateyPackage package)
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

        /// <returns>The package with the specified name, or null</returns>
        public ChocolateyPackage GetChocoPackageByName(string name)
        {
            // prefer outdated packages as they contain more information, even though hit rate may be less
            if (OutdatedPckgCache.ContainsKey(name))
            {
                return OutdatedPckgCache[name];
            }
            else if (InstalledPckgCache.ContainsKey(name))
            {
                return InstalledPckgCache[name];
            }

            return null;
        }

        public List<ChocolateyPackage> GetChocoPackagesByName(List<string> names)
        {
            var rtn = names
                .Select(name => GetChocoPackageByName(name))
                .Where(package => package != null)
                .ToList();
            
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

        private async Task UpdateCachedItem(GenericPackage package)
        {
            await OutdatedPckgLock.WaitAsync().ConfigureAwait(false);
            var chocoPackage = OutdatedPckgCache[package.Name];
            chocoPackage.AvailVer = package.AvailVer;
            chocoPackage.CurrVer = package.CurrVer;
            chocoPackage.IsTopLevelPackage = package.IsTopLevelPackage;
            chocoPackage.Name = package.Name;
            chocoPackage.Pinned = package.Pinned;
            OutdatedPckgLock.Release();
        }
    }
}
