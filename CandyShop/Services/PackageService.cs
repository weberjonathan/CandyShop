using System.Collections.Generic;
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
        private readonly SemaphoreSlim IntermediateInstalledPckgLock = new(1);
        private readonly SemaphoreSlim PckgDetailsLock = new(1);

        private readonly Dictionary<string, string> PckgDetailsCache = [];
        private readonly Dictionary<string, GenericPackage> InstalledPckgCache = [];
        private readonly Dictionary<string, GenericPackage> OutdatedPckgCache = [];

        private List<GenericPackage> IntermediateInstalledPackages = [];

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

            bool reportIntermediateResults = PackageManager.RequiresNameResolution && !PackageManager.SupportsFetchingOutdated;
            if (reportIntermediateResults)
            {
                // some package managers abbreviate the package names in their output (winget)
                // and some package manager do not explicitly support fetching outdated packages (winget).
                // Re-using the install fetch for determining outdated packages can be slow
                // in this case, because resolving names is slow. But if no outdated packages
                // require name resolution, they can be posted as intermediate results for consumption
                // by the outdated fetch.
                await IntermediateInstalledPckgLock.WaitAsync().ConfigureAwait(false);
                IntermediateInstalledPackages.Clear();
            }

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

                    // report intermediate results without resolving names if all pin info can be merged into cache
                    if (reportIntermediateResults)
                    {
                        pinned = ConditionalMergePinInfoUnsafe(pinned, (package) => !package.Name.Contains('…'));
                        if (pinned.Count == 0)
                            IntermediateInstalledPackages = InstalledPckgCache.Values.ToList();

                        IntermediateInstalledPckgLock.Release();
                    }

                    // resolve names of installed packages and pinned packages
                    if (PackageManager.RequiresNameResolution)
                    {
                        var movedPackages = await PackageManager.ResolveAbbreviatedNamesAsync(InstalledPckgCache.Values.ToList());
                        MoveInstalledPackagesUnsafe(movedPackages);

                        movedPackages = await PackageManager.ResolveAbbreviatedNamesAsync(pinned);
                        pinned = movedPackages.Values.ToList();
                    }

                    MergePinInfoUnsafe(pinned);

                    var unresolvedIds = InstalledPckgCache.Values.Where(p => p.Id.Contains('…') && p.HasSource).ToList();
                    if (unresolvedIds.Count > 0)
                    {
                        var value = string.Join(", ", unresolvedIds.Select(p => p.Name).ToList());
                        Log.Warning($"{unresolvedIds.Count} package(s) have sources and unresolved IDs: {value}");
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

                if (IntermediateInstalledPckgLock.CurrentCount == 0)
                    IntermediateInstalledPckgLock.Release();
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
                        var packagesFuture = GetInstalledPackagesAsync();

                        // try use intermediate installed package fetch results if (expensive) name resolution is required
                        if (PackageManager.RequiresNameResolution)
                        {
                            // wait until the active install fetch posts intermediate results and check for oudated if the data is workable (ie no abbreviated names exist)
                            if (InstalledPckgLock.CurrentCount == 0 && IntermediateInstalledPckgLock.CurrentCount == 0)
                            {
                                Log.Debug("Package manager is currently fetching installed packages. Waiting for intermediate results.");
                                await IntermediateInstalledPckgLock.WaitAsync().ConfigureAwait(false);
                                packages = IntermediateInstalledPackages
                                    .Where(p => !string.IsNullOrEmpty(p.AvailVer))
                                    .Where(p => !p.CurrVer.Equals(p.AvailVer))
                                    .ToList();

                                if (packages.Where(p => p.Name.Contains('…')).Any())
                                    packages = null;

                                Log.Debug($"Found {packages?.Count} outdated packages through intermediary data.");
                                IntermediateInstalledPackages.Clear();
                                IntermediateInstalledPckgLock.Release();
                            }
                        }

                        // wait on full install package fetch if intermediate results were not workable or no fetch is underway
                        if (packages == null)
                        {
                            packages = await packagesFuture;
                            packages = packages
                                .Where(p => !string.IsNullOrEmpty(p.AvailVer))
                                .Where(p => !p.CurrVer.Equals(p.AvailVer))
                                .ToList();
                            Log.Debug($"Found {packages?.Count} outdated packages through full install data.");
                        }
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

                if (IntermediateInstalledPckgLock.CurrentCount == 0)
                    IntermediateInstalledPckgLock.Release();
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
