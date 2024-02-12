using CandyShop.Chocolatey;
using CandyShop.Properties;
using CandyShop.Services;
using CandyShop.View;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CandyShop.Controller
{
    class UpgradePageController
    {
        private readonly CandyShopContext Context;
        private readonly IPackageService PackageService;
        private readonly ShortcutService ShortcutService;
        private IMainWindowView MainWindow;
        private IUpgradePageView View;

        public UpgradePageController(CandyShopContext context, IPackageService packageService, ShortcutService shortcutService)
        {
            Context = context;
            PackageService = packageService;
            ShortcutService = shortcutService;
        }

        public void InjectViews(IMainWindowView mainWindow, IUpgradePageView upgradePage)
        {
            MainWindow = mainWindow;
            View = upgradePage;
            
            View.PinnedChanged += new EventHandler<PinnedChangedArgs>((sender, e) => TogglePin(e.Name));
            View.CleanShortcutsChanged += new EventHandler((sender, e) => Context.CleanShortcuts = View.CleanShortcuts);
            View.CleanShortcuts = Context.CleanShortcuts;

            View.UpgradeAllClick += new EventHandler((sender, e) =>
            {
                PerformUpgrade(View.Items);
            });

            View.UpgradeSelectedClick += new EventHandler((sender, e) =>
            {
                var packages = View.SelectedItems;
                if (packages.Length > 0) PerformUpgrade(packages);
            });

            View.CancelClick += new EventHandler((sender, e) =>
            {
                // exit or hide application on 'Cancel', depending on how it was created
                if (Context.LaunchedMinimized)
                {
                    MainWindow.ToForm().Hide();
                }
                else
                {
                    MainWindow.ToForm().Dispose();
                    Program.Exit();
                }
            });

            View.CheckTopLevelClicked += new EventHandler((sender, e) =>
            {
                CheckTopLevelPackages();
            });

            View.AlwaysHideAdminWarningClicked += new EventHandler((sender, e) =>
            {
                Context.SupressAdminWarning = true;
            });

            // update UI if is properties file is updated
            Context.OnPropertiesFileChanged(() =>
            {
                Action<bool> checkDelegate = isChecked =>
                {
                    View.CleanShortcuts = isChecked;
                };
                var ctrl = (System.Windows.Forms.Control)View;
                ctrl.Invoke(checkDelegate, Context.CleanShortcuts);
                // TODO message with require restart depending on the property
            });

            UpdateOutdatedPackageListAsync();
        }

        private async void UpdateOutdatedPackageListAsync()
        {
            List<GenericPackage> packages = new List<GenericPackage>();
            try
            {
                packages = await PackageService.GetOutdatedPackagesAsync();
            }
            catch (ChocolateyException e)
            {
                Log.Error(LocaleEN.ERROR_RETRIEVING_OUTDATED_PACKAGES, e.Message);
            }

            View.Loading = false;

            packages.ForEach(p => View.AddItem(new string[]
            {
                p.Name,
                p.CurrVer,
                p.AvailVer,
                p.Pinned.ToString()
            }));

            CheckTopLevelPackages();
        }

        private void CheckTopLevelPackages()
        {
            string[] displayedItemNames = View.Items;
            List<GenericPackage> packages = PackageService.GetPackagesByName(displayedItemNames.ToList());

            foreach (var p in packages)
            {
                bool check = p.IsTopLevelPackage && !p.Pinned.GetValueOrDefault(false);
                View.SetItemCheckState(p.Name, check);
            }
        }

        private async void PerformUpgrade(string[] packages)
        {
            MainWindow?.ToForm().Hide();

            List<string> shortcuts = new List<string>();
            ShortcutService?.WatchDesktops(shortcut => shortcuts.Add(shortcut));

            // upgrade
            WindowsConsole.AllocConsole();
            Console.CursorVisible = false;

            try
            {
                PackageService.Upgrade(packages);
            }
            catch (ChocolateyException e)
            {
                MainWindow?.DisplayError(LocaleEN.ERROR_UPGRADING_OUTDATED_PACKAGES, e.Message);
                return; // TODO why return? shortcuts should be deleted even if chocolatey fails to upgrade some packages (others may have been upgraded and added a shortcut)
            }

            // display results
            Task minDelay = Task.Run(() => Thread.Sleep(3 * 1000));

            IntPtr handle = WindowsConsole.GetConsoleWindow();
            if (!IntPtr.Zero.Equals(handle))
            {
                WindowsConsole.SetForegroundWindow(handle);
            }
            Console.CursorVisible = false;
            Console.Write("\nPress any key to continue... ");
            Console.ReadKey();

            // delete shortcuts
            ShortcutService?.DisposeWatchers();
            if (View.CleanShortcuts)
            {
                await minDelay; // wait for shortcuts to be created
                ShortcutService?.DeleteShortcuts(shortcuts);
            }

            // TODO if there still are outdated packages, return to MainView
            // TODO if there was an error, offer to open log folder? go back to application?
            MainWindow?.ToForm().Dispose();
            Program.Exit();
        }

        private async void TogglePin(string packageName)
        {
            try
            {
                GenericPackage package = PackageService.GetPackageByName(packageName);
                if (package == null)
                {
                    ErrorHandler.ShowError($"Could not find package '{packageName}'");
                    return;
                }

                if (package.Pinned.HasValue)
                {
                    if (package.Pinned.Value)
                    {
                        await PackageService.UnpinAsync(package);
                    }
                    else
                    {
                        await PackageService.PinAsync(package);
                    }

                    View.SetPinned(package.Name, package.Pinned.Value);
                }
                else
                {
                    // TODO error
                }
            }
            catch (ChocolateyException e)
            {
                ErrorHandler.ShowError("An unknown error occurred: {0}", e.Message);
            }
        }
    }
}
