using CandyShop.Winget;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Serilog;
using System;

namespace CandyShop.Services

    // TODO use IDs instead of names for dictionaries
{
    /// <summary>
    /// This service class allows asynchronous, non-blocking access to Chocolatey and implements a cache.
    /// </summary>
    internal class WingetService : IPackageService
    {
        private readonly SemaphoreSlim InstalledPckgLock = new SemaphoreSlim(1);

        // TODO use proper package repository instead
        private readonly Dictionary<string, string> PckgDetailsCache = new Dictionary<string, string>();
        private readonly Dictionary<string, WingetPackage> InstalledPckgCache = new Dictionary<string, WingetPackage>();

        // ------------- GENERIC PACKAGE METHODS ------------------------------

        public async Task<List<GenericPackage>> GetInstalledPackagesAsync()
        {
            var chocoPackages = await GetInstalledWingetPackagesAsync();
            return chocoPackages.Select(p => new GenericPackage(p)).ToList();
        }

        /// <exception cref="WingetException"></exception>
        public async Task<List<GenericPackage>> GetOutdatedPackagesAsync()
        {
            var chocoPackages = await GetOutdatedWingetPackagesAsync();
            return chocoPackages.Select(p => new GenericPackage(p)).ToList();
        }

        public async Task ClearOutdatedPackages()
        {
            await ClearInstalledPackages();
        }

        public async Task ClearInstalledPackages()
        {
            await InstalledPckgLock.WaitAsync().ConfigureAwait(false);
            InstalledPckgCache.Clear();
            InstalledPckgLock.Release();
        }

#pragma warning disable CS1998
        public async Task ClearPackageDetails()
#pragma warning restore CS1998
        {
            PckgDetailsCache.Clear();
        }

        public async Task<string> GetPackageDetailsAsync(string name)
        {
            var package = GetPackageByName(name);
            return await GetWingetPackageDetails(new WingetPackage()
            {
                Id = package.Id,
                Name = package.Name,
                Source = package.Source
            });
        }

        public GenericPackage GetPackageByName(string name)
        {
            var chocoPackage = GetWingetPackageByName(name);
            return new GenericPackage(chocoPackage);
        }

        public List<GenericPackage> GetPackagesByName(List<string> names)
        {
            var chocoPackages = GetWingetPackagesByName(names);
            return chocoPackages.Select(p => new GenericPackage(p)).ToList();
        }

        /// <exception cref="WingetException"></exception>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task PinAsync(GenericPackage package)
#pragma warning restore CS1998
        {
            // TODO
            //int exitCode = ChocolateyWrapper.Pin(package.Name, package.CurrVer);
            //Log.Debug($"choco add pin operation for {package.Name} returned with {exitCode}.");
            //if (exitCode != 0)
            //{
            //    throw new WingetException($"choco did not exit cleanly. Returned {exitCode}.");
            //}

            //package.Pinned = true;
        }

        /// <exception cref="WingetException"></exception>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task UnpinAsync(GenericPackage package)
#pragma warning restore CS1998
        {
            // TODO
            //int exitCode = ChocolateyWrapper.Unpin(package.Name);
            //Log.Debug($"choco pin remove operation for {package.Name} returned with {exitCode}.");
            //if (exitCode != 0)
            //{
            //    throw new WingetException($"choco did not exit cleanly. Returned {exitCode}.");
            //}

            //package.Pinned = false;
        }

        public void Upgrade(string[] names)
        {
            List<WingetPackage> chocoPackages = GetWingetPackagesByName(names.ToList());
            if (chocoPackages.Count > 0)
            {
                Upgrade(chocoPackages);
            }

            // TODO refresh stuff; see ChocolateyService.Upgrade()
        }

        // --------------------------------------------------------------------

        /// <exception cref="WingetException"></exception>
        public async Task<List<WingetPackage>> GetOutdatedWingetPackagesAsync()
        {
            var packages = await GetInstalledWingetPackagesAsync();
            return packages.Where(p => !String.IsNullOrEmpty(p.AvailableVersion) &&
                                       !p.Version.Equals(p.AvailableVersion)).ToList();
        }

        /// <exception cref="WingetException"></exception>
        public async Task<List<WingetPackage>> GetInstalledWingetPackagesAsync()
        {
            await InstalledPckgLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (InstalledPckgCache.Count <= 0)
                {
                    List<WingetPackage> installedPckgs = await WingetWrapper.FetchInstalledAsync();
                    installedPckgs.ForEach(p => InstalledPckgCache[p.Name] = p);
                }
            }
            catch (WingetException)
            {
                throw;
            }
            finally
            {
                InstalledPckgLock.Release();
            }

            return InstalledPckgCache.Values.ToList();
        }

        /// <exception cref="WingetException"></exception>
        public async Task<string> GetWingetPackageDetails(WingetPackage package)
        {
            if (!PckgDetailsCache.TryGetValue(package.Name, out string details))
            {
                details = await WingetWrapper.FetchInfoAsync(package);
                PckgDetailsCache[package.Name] = details;
            }

            return details;
        }

        public List<WingetPackage> GetInstalledPackagesByName(List<string> names)
        {
            return names
                .Where(name => InstalledPckgCache.ContainsKey(name))
                .Select(name => InstalledPckgCache[name])
                .ToList();
        }

        /// <returns>The package with the specified name, or null</returns>
        public WingetPackage GetWingetPackageByName(string name)
        {
            if (InstalledPckgCache.ContainsKey(name))
            {
                return InstalledPckgCache[name];
            }

            return null;
        }

        public List<WingetPackage> GetWingetPackagesByName(List<string> names)
        {
            var rtn = names
                .Select(name => GetWingetPackageByName(name))
                .Where(package => package != null)
                .ToList();
            
            return rtn;
        }

        /// <summary>
        /// Upgrades a collection of packages.
        /// </summary>
        /// <param name="packages"></param>
        /// <param name="validExitCodes">test</param>
        /// <exception cref="WingetException"></exception>
        private void Upgrade(List<WingetPackage> packages)
        {
            int exitCode = WingetWrapper.Upgrade(packages);

            if (exitCode != 0)
            {
                throw new WingetException($"Winget did not exit cleanly. Returned {exitCode}.");
            }
        }
    }
}
