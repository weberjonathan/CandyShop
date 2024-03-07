﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Serilog;
using System;
using CandyShop.PackageCore;

namespace CandyShop.Services
{
    /// <summary>
    /// This service class allows asynchronous, non-blocking access to packages of any source
    /// </summary>
    internal class PackageService
    {
        private readonly SemaphoreSlim OutdatedPckgLock = new(1);
        private readonly SemaphoreSlim InstalledPckgLock = new(1);
        private readonly SemaphoreSlim PckgDetailsLock = new(1);

        private readonly Dictionary<string, string> PckgDetailsCache = [];
        private readonly Dictionary<string, GenericPackage> InstalledPckgCache = [];
        private readonly Dictionary<string, GenericPackage> OutdatedPckgCache = [];

        private readonly AbstractPackageManager PackageManager;

        public PackageService(AbstractPackageManager packageManager)
        {
            PackageManager = packageManager;
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task<List<GenericPackage>> GetInstalledPackagesAsync()
        {
            await InstalledPckgLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (InstalledPckgCache.Count <= 0)
                {
                    List<GenericPackage> packages = await PackageManager.FetchInstalledAsync();
                    packages.ForEach(p => InstalledPckgCache[p.Name] = p);
                }
            }
            catch (PackageManagerException)
            {
                throw;
            }
            finally
            {
                InstalledPckgLock.Release();
            }

            return InstalledPckgCache.Values.ToList();
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task<List<GenericPackage>> GetOutdatedPackagesAsync()
        {
            await OutdatedPckgLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (OutdatedPckgCache.Count <= 0)
                {
                    List<GenericPackage> packages = await PackageManager.FetchOutdatedAsync();
                    packages.ForEach(p => OutdatedPckgCache[p.Name] = p);
                }
            }
            catch (PackageManagerException)
            {
                throw;
            }
            finally
            {
                OutdatedPckgLock.Release();
            }

            return OutdatedPckgCache.Values.ToList();
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task<string> GetPackageDetailsAsync(string name)
        {
            var package = GetPackageByName(name);
            if (package == null) return "";
            
            // TODO ensure lock release with exception handling !!!
            await PckgDetailsLock.WaitAsync().ConfigureAwait(false);
            if (!PckgDetailsCache.TryGetValue(name, out string details))
            {
                PckgDetailsLock.Release();
                details = await PackageManager.FetchInfoAsync(package);

                await PckgDetailsLock.WaitAsync().ConfigureAwait(false);
                PckgDetailsCache[name] = details;
            }
            PckgDetailsLock.Release();

            return details;
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task PinAsync(string name)
        {
            // TODO exception handling; pinasync needs to check return code and throw; needs to be handled here

            Log.Information($"Attempting to pin package {name}.");

            var package = GetPackageByName(name);
            if (package == null)
            {
                Log.Error($"Could not find package to pin with name: {name}");
                return;
            }

            try
            {
                await PackageManager.PinAsync(package);
            }
            catch (PackageManagerException)
            {
                throw;
            }

            package.Pinned = true;
            await UpdateCachedItem(package);
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task UnpinAsync(string name)
        {
            Log.Information($"Attempting to unpin package {name}.");

            var package = GetPackageByName(name);
            if (package == null)
            {
                Log.Error($"Could not find package to unpin with name: {name}");
                return;
            }

            try
            {
                await PackageManager.UnpinAsync(package);
            }
            catch (PackageManagerException)
            {
                throw;
            }

            package.Pinned = false;
            await UpdateCachedItem(package);
        }

        /// <exception cref="PackageManagerException"></exception>
        public async void Upgrade(string[] names)
        {
            List<GenericPackage> packages = GetPackagesByName(names.ToList());
            packages = packages.Where(p => !p.Pinned.GetValueOrDefault(false)).ToList();

            if (packages.Count <= 0) return;

            try
            {
                PackageManager.Upgrade(packages);
            }
            catch (PackageManagerException)
            {
                throw;
            }
            finally
            {
                await ClearOutdatedPackages();
                await PckgDetailsLock.WaitAsync().ConfigureAwait(false);
                names.ToList().ForEach(name => PckgDetailsCache.Remove(name));
                PckgDetailsLock.Release();
            }
        }

        public async Task ClearOutdatedPackages()
        {
            await OutdatedPckgLock.WaitAsync().ConfigureAwait(false);
            OutdatedPckgCache.Clear();
            OutdatedPckgLock.Release();
        }

        public async Task ClearInstalledPackages()
        {
            await InstalledPckgLock.WaitAsync().ConfigureAwait(false);
            InstalledPckgCache.Clear();
            InstalledPckgLock.Release();
        }

        public async Task ClearPackageDetails()
        {
            await PckgDetailsLock.WaitAsync().ConfigureAwait(false);
            PckgDetailsCache.Clear();
            PckgDetailsLock.Release();
        }

        public GenericPackage GetPackageByName(string name)
        {
            if (OutdatedPckgCache.TryGetValue(name, out GenericPackage outdated))
                return outdated;
            else if (InstalledPckgCache.TryGetValue(name, out GenericPackage installed))
                return installed;

            return null;
        }

        public List<GenericPackage> GetPackagesByName(List<string> names)
        {
            return names
                .Select(GetPackageByName)
                .Where(package => package != null)
                .ToList();
        }

        private async Task UpdateCachedItem(GenericPackage package)
        {
            await OutdatedPckgLock.WaitAsync().ConfigureAwait(false);
            package = OutdatedPckgCache[package.Name];
            package.AvailVer = package.AvailVer;
            package.CurrVer = package.CurrVer;
            package.Name = package.Name;
            package.Pinned = package.Pinned;
            // TODO rest of properties
            OutdatedPckgLock.Release();
        }
    }
}
