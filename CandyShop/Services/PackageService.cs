using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Serilog;
using System;
using CandyShop.PackageCore;
using System.IO;
using System.Windows.Forms;
using System.Collections.Immutable;

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

        // TODO remove duplicate calls to pin

        public PackageService(AbstractPackageManager packageManager, ShortcutService shortcutService)
        {
            PackageManager = packageManager;
            ShortcutService = shortcutService;
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task<GenericPackage[]> GetInstalledPackagesAsync()
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

                    // fetch and resolve pinned packages
                    var pinned = (await fetchPinned).ToList();
                    if (PackageManager.RequiresNameResolution)
                    {
                        var unresolved = RemoveUnresolvedPackages(pinned);
                        var resolved = await PackageManager.ResolveAbbreviatedNamesAsync(unresolved);
                        pinned.AddRange(resolved);
                    }

                    // fetch and resolve installed packages
                    var installed = (await fetchInstalled).ToList();
                    if (PackageManager.RequiresNameResolution)
                    {
                        var unresolved = RemoveUnresolvedPackages(installed);
                        var resolved = await PackageManager.ResolveAbbreviatedNamesAsync(unresolved);
                        installed.AddRange(resolved);
                    }

                    // merge pin info
                    var pinnedNames = pinned.Select(package => package.Name).ToImmutableHashSet();
                    installed.ForEach(package =>
                    {
                        package.Pinned = pinnedNames.Contains(package.Name);
                    });

                    // merge
                    installed.ForEach(p => InstalledPckgCache[p.Name] = p);
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

            return InstalledPckgCache.Values.ToArray();
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task<GenericPackage[]> GetOutdatedPackagesAsync()
        {
            Log.Debug($"PackageService [{Environment.CurrentManagedThreadId}]: Get outdated packages.");

            if (PackageManager == null) return [];

            await OutdatedPckgLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (OutdatedPckgCache.Count == 0)
                {
                    List<GenericPackage> outdated = null;
                    if (!PackageManager.SupportsFetchingOutdated)
                    {
                        // fallback to installed packages
                        outdated = (await GetInstalledPackagesAsync())
                            .Where(p => !string.IsNullOrEmpty(p.AvailVer))
                            .Where(p => !p.CurrVer.Equals(p.AvailVer))
                            .ToList();
                    }
                    else
                    {
                        // fetch outdated packages and pins
                        var fetchOutdated = PackageManager.FetchOutdatedAsync();
                        var fetchPinned = PackageManager.FetchPinListAsync();

                        // fetch and resolve pinned packages
                        var pinned = (await fetchPinned).ToList();
                        if (PackageManager.RequiresNameResolution)
                        {
                            var unresolved = RemoveUnresolvedPackages(pinned);
                            var resolved = await PackageManager.ResolveAbbreviatedNamesAsync(unresolved);
                            pinned.AddRange(resolved);
                        }

                        // fetch and resolve outdated packages
                        outdated = (await fetchOutdated).ToList();
                        if (PackageManager.RequiresNameResolution)
                        {
                            var unresolved = RemoveUnresolvedPackages(outdated);
                            var resolved = await PackageManager.ResolveAbbreviatedNamesAsync(unresolved);
                            outdated.AddRange(resolved);
                        }

                        // merge pin info
                        var pinnedNames = pinned.Select(package => package.Name).ToImmutableHashSet();
                        outdated.ForEach(package =>
                        {
                            package.Pinned = pinnedNames.Contains(package.Name);
                        });
                    }

                    // merge
                    outdated.ForEach(p => OutdatedPckgCache[p.Name] = p);
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

            return OutdatedPckgCache.Values.ToArray();
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

        public int GetNonPinnedCount(List<GenericPackage> packages)
        {
            if (packages == null)
                return 0;

            return packages.Count(p => !p.Pinned.GetValueOrDefault(false));
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

        /// <summary>
        /// Removes packages from the list if their name is incomplete (ie. contains an ellipsis)
        /// and they have a source. Returns the removed packages.
        /// </summary>
        /// <param name="packages"></param>
        private List<GenericPackage> RemoveUnresolvedPackages(List<GenericPackage> packages)
        {
            List<GenericPackage> unresolved = [];
            int i = 0;
            while (i < packages.Count)
            {
                var package = packages[i];
                if (package.HasSource && package.Name.Contains('…'))
                {
                    packages.Remove(package);
                    unresolved.Add(package);
                }
                i++;
            }

            return unresolved;
        }
    }
}
