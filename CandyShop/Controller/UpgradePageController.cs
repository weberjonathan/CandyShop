using CandyShop.Chocolatey;
using CandyShop.Properties;
using CandyShop.Services;
using CandyShop.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CandyShop.Controller
{
    class UpgradePageController : IUpgradePageController
    {
        private readonly CandyShopContext Context;
        private readonly ChocolateyService ChocolateyService;
        private readonly ShortcutService ShortcutService;
        private IMainWindowView MainWindow;
        private IUpgradePageView View;

        public UpgradePageController(CandyShopContext context, ChocolateyService chocolateyService, ShortcutService shortcutService)
        {
            Context = context;
            ChocolateyService = chocolateyService;
            ShortcutService = shortcutService;
        }

        public void InjectViews(IMainWindowView mainWindow, IUpgradePageView upgradePage)        {
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

            // update UI if is properties file is updated
            Context.OnPropertiesFileChanged(() =>
            {
                Action<bool> checkDelegate = isChecked =>
                {
                    View.CleanShortcuts = isChecked;
                };
                var ctrl = (System.Windows.Forms.Control)View;
                ctrl.Invoke(checkDelegate, Context.CleanShortcuts);
            });
        }

        public async void PerformUpgrade(string[] packages)
        {
            MainWindow?.ToForm().Hide();

            List<string> shortcuts = new List<string>();
            ShortcutService?.WatchDesktops(shortcut => shortcuts.Add(shortcut));

            // upgrade
            WindowsConsole.AllocConsole();
            Console.CursorVisible = false;

            try
            {
                List<ChocolateyPackage> chocoPackages = ChocolateyService.GetPackagesByName(packages.ToList());
                if (chocoPackages.Count > 0)
                {
                    ChocolateyService.Upgrade(chocoPackages, Context.ValidExitCodes.ToArray());
                }
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

        public void TogglePin(string packageName)
        {
            try
            {
                ChocolateyPackage package = ChocolateyService.GetPackageByName(packageName);
                if (package == null)
                {
                    ErrorHandler.ShowError($"Could not find package '{packageName}'");
                    return;
                }

                if (package.Pinned.HasValue)
                {
                    if (package.Pinned.Value)
                    {
                        ChocolateyService.Unpin(package);
                    }
                    else
                    {
                        ChocolateyService.Pin(package);
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
