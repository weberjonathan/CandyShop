using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Serilog;
using System;
using CandyShop.PackageCore;
using System.IO;
using System.Windows.Forms;

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
            Log.Debug($"PackageService [{Environment.CurrentManagedThreadId}]: Get installed packages.");

            if (PackageManager == null) return [];

            // get installed packages
            await InstalledPckgLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (InstalledPckgCache.Count == 0)
                {
                    var fetchInstalled = PackageManager.FetchInstalledAsync();
                    var fetchPinned = PackageManager.FetchPinListAsync();

                    var installed = await fetchInstalled;
                    installed.ForEach(p => p.Pinned = false);
                    installed.ForEach(p => InstalledPckgCache[p.Name] = p);

                    var pinned = await fetchPinned;

                    // resolve names of installed packages and pinned packages
                    if (PackageManager.RequiresNameResolution)
                    {
                        var movedPackages = await PackageManager.ResolveAbbreviatedNamesAsync(InstalledPckgCache.Values.ToList());
                        MoveInstalledPackagesUnsafe(movedPackages);

                        movedPackages = await PackageManager.ResolveAbbreviatedNamesAsync(pinned);
                        pinned = movedPackages.Values.ToList();
                    }

                    MergePinInfoUnsafe(pinned);

                    var unresolved = InstalledPckgCache
                        .Values
                        .Where(p => p.HasSource && p.Id != null && p.Id.Contains('…'))
                        .Select(p => p.Name)
                        .ToList();
                    if (unresolved.Count > 0)
                    {
                        var value = string.Join(", ", unresolved);
                        Log.Warning($"{unresolved.Count} package(s) have sources and unresolved IDs: {value}");
                    }
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
            Log.Debug($"PackageService [{Environment.CurrentManagedThreadId}]: Get outdated packages.");

            if (PackageManager == null) return [];

            await OutdatedPckgLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (OutdatedPckgCache.Count == 0)
                {
                    List<GenericPackage> packages = null;
                    if (PackageManager.SupportsFetchingOutdated)
                    {
                        packages = await PackageManager.FetchOutdatedAsync();
                    }
                    else
                    {
                        packages = (await GetInstalledPackagesAsync())
                            .Where(p => !string.IsNullOrEmpty(p.AvailVer))
                            .Where(p => !p.CurrVer.Equals(p.AvailVer))
                            .ToList();
                    }

                    // remove packages with unknown version (relevant for winget only)
                    packages = packages.Where(p => !"Unknown".Equals(p.CurrVer)).ToList();
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
            var selfPackages = packages.Where(p => p.Name.Equals("Candy Shop") || p.Name.Equals("CandyShop"));
            if (selfPackages.Any() && !ContextSingleton.Get.SelfUpdateEnabled)
            {
                MessageBox.Show(
                    "Candy Shop currently does not support upgrading itself. This feature is planned in the future. Please upgrade Candy Shop through the terminal instead.",
                    MetaInfo.Name,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                packages.Remove(selfPackages.First());
            }

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
                await ClearPackages();
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

        public async Task ClearPackages()
        {
            await OutdatedPckgLock.WaitAsync().ConfigureAwait(false);
            await InstalledPckgLock.WaitAsync().ConfigureAwait(false);
            Log.Debug($"Clear package caches.");
            OutdatedPckgCache.Clear();
            InstalledPckgCache.Clear();
            OutdatedPckgLock.Release();
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
            if (OutdatedPckgCache.ContainsKey(package.Name))
            {
                await OutdatedPckgLock.WaitAsync().ConfigureAwait(false);
                OutdatedPckgCache[package.Name] = package;
                OutdatedPckgLock.Release();
            }

            if (InstalledPckgCache.ContainsKey(package.Name))
            {
                await InstalledPckgLock.WaitAsync().ConfigureAwait(false);
                InstalledPckgCache[package.Name] = package;
                InstalledPckgLock.Release();
            }
        }

        private void MergePinInfoUnsafe(List<GenericPackage> pinned)
        {
            ConditionalMergePinInfoUnsafe(pinned, (p) => true);
        }

        /// <summary>
        /// Merges the pin info from the supplied packages into the installed package cache
        /// if the supplied condition is met. No lock is acquired during the operation.
        /// Packages, that do not meet the defined condition or have not been cached, are returned.
        /// </summary>
        /// <param name="pinned"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private List<GenericPackage> ConditionalMergePinInfoUnsafe(List<GenericPackage> pinned, Func<GenericPackage, bool> condition)
        {
            List<GenericPackage> rtn = [];
            pinned.ForEach(package =>
            {
                if (condition(package) && InstalledPckgCache.TryGetValue(package.Name, out GenericPackage cachePackage))
                    cachePackage.Pinned = package.Pinned;
                else
                    rtn.Add(package);
            });

            return rtn;
        }

        private void MoveInstalledPackagesUnsafe(Dictionary<string, GenericPackage> movedPackages)
        {
            foreach (var (oldName, package) in movedPackages)
            {
                if (InstalledPckgCache.ContainsKey(oldName))
                {
                    InstalledPckgCache.Remove(oldName);
                    InstalledPckgCache[package.Name] = package;
                }
            }
        }
    }
}
