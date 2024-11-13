﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Serilog;
using System;
using CandyShop.PackageCore;
using System.IO;

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
        private readonly ShortcutService ShortcutService;

        public PackageService(AbstractPackageManager packageManager, ShortcutService shortcutService)
        {
            PackageManager = packageManager;
            ShortcutService = shortcutService;
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task<List<GenericPackage>> GetInstalledPackagesAsync()
        {
            Log.Debug($"{Environment.CurrentManagedThreadId} GetInstalledPackagesAsync()");

            if (PackageManager == null) return [];

            List<GenericPackage> installed = [];
            List<GenericPackage> pinned = [];

            // get installed packages
            await InstalledPckgLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (InstalledPckgCache.Count <= 0)
                {
                    // TODO
                    // currently, fetching installed and fetching pin lists in winget
                    // will cause separate executions of 'winget show' for abbreviated
                    // packages; the output of those commands is discarded aside
                    // from the full name.
                    // Instead, the flag PackageManager.SupportsNameResolution
                    // may be added (similar to PackageManager.SupportsFetchingOutdated)
                    // and the resolution could be implemented here, thus removing
                    // redundant winget executions and populating the details cache

                    var fetchInstalled = PackageManager.FetchInstalledAsync();
                    var fetchPinned = PackageManager.FetchPinListAsync();

                    installed = await fetchInstalled;
                    installed.ForEach(p => p.Pinned = false);
                    installed.ForEach(p => InstalledPckgCache[p.Name] = p);
                    
                    pinned = await fetchPinned;
                    pinned.ForEach(p =>
                    {
                        if (InstalledPckgCache.TryGetValue(p.Name, out GenericPackage cachePackage))
                            cachePackage.Pinned = p.Pinned;
                    });
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
            Log.Debug($"{Environment.CurrentManagedThreadId} GetOutdatedPackagesAsync()");

            if (PackageManager == null) return [];

            await OutdatedPckgLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (OutdatedPckgCache.Count <= 0)
                {
                    List<GenericPackage> packages = [];
                    if (PackageManager.SupportsFetchingOutdated)
                    {
                        packages = await PackageManager.FetchOutdatedAsync();
                    }
                    else
                    {
                        packages = await GetInstalledPackagesAsync();
                        packages = packages
                            .Where(p => !string.IsNullOrEmpty(p.AvailVer))
                            .Where(p => !p.CurrVer.Equals(p.AvailVer))
                            .ToList();
                    }
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
            if (PackageManager == null) return "";

            var package = GetPackageByName(name);
            if (package == null) return "";
            
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
            if (PackageManager == null) return;

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
            if (PackageManager == null) return;

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
        /// <exception cref="CandyShopException"></exception>
        public async Task Upgrade(List<GenericPackage> packages)
        {
            if (PackageManager == null) return;

            packages = packages.Where(p => !p.Pinned.GetValueOrDefault(false)).ToList();
            if (packages.Count <= 0) return;

            List<string> shortcuts = [];
            ShortcutService?.WatchDesktops(shortcut =>
            {
                shortcuts.Add(shortcut);
                Log.Information($"Detected new shortcut: {shortcut}");
            });

            WindowsConsole.AllocConsole();
            var StdOut = new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            };
            Console.SetOut(StdOut);
            Console.CursorVisible = false;
            Console.Title = $"{MetaInfo.WindowTitle} | Upgrade in process";

            try
            {
                PackageManager.Upgrade(packages);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await ClearOutdatedPackages();
                await PckgDetailsLock.WaitAsync().ConfigureAwait(false);
                packages.ForEach(p => PckgDetailsCache.Remove(p.Name));
                PckgDetailsLock.Release();

                // display results
                Task minDelay = Task.Run(() => Thread.Sleep(3 * 1000));

                IntPtr handle = WindowsConsole.GetConsoleWindow();
                if (!IntPtr.Zero.Equals(handle))
                {
                    WindowsConsole.SetForegroundWindow(handle);
                }
                Console.CursorVisible = false;
                Console.Write("\nPress any key to continue... ");
                Console.CursorVisible = true;
                Console.ReadKey();
                Log.Debug("Read key press after upgrading");
                Console.Write("\nClosing terminal");
                Console.CursorVisible = false;

                // delete shortcuts
                ShortcutService?.DisposeWatchers();
                if (ContextSingleton.Get.CleanShortcuts)
                {
                    await minDelay; // wait for shortcuts to be created
                    ShortcutService?.DeleteShortcuts(shortcuts);
                }

                Log.Debug("Attempt to free console");
                StdOut.Close();
                StdOut.Dispose();
                if (WindowsConsole.FreeConsole())
                    Log.Debug("Console freed successfully.");
                else
                    Log.Debug("Failed to free console.");
            }
        }

        public async Task ClearOutdatedPackages()
        {
            await OutdatedPckgLock.WaitAsync().ConfigureAwait(false);
            OutdatedPckgCache.Clear();
            OutdatedPckgLock.Release();

            if (PackageManager != null && !PackageManager.SupportsFetchingOutdated)
            {
                await ClearInstalledPackages();
            }
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
            var cachedPackage = OutdatedPckgCache[package.Name];
            cachedPackage.Name = package.Name;
            cachedPackage.Id = package.Id;
            cachedPackage.Source = package.Source;
            cachedPackage.CurrVer = package.CurrVer;
            cachedPackage.AvailVer = package.AvailVer;
            cachedPackage.Pinned = package.Pinned;
            cachedPackage.Parent = package.Parent;
            OutdatedPckgLock.Release();
        }
    }
}
