using CandyShop.Chocolatey;
using CandyShop.Properties;
using CandyShop.Services;
using CandyShop.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CandyShop.Controller
{
    internal class MainWindowController : IMainWindowController
    {
        private readonly ChocolateyService ChocolateyService;
        private readonly WindowsTaskService WindowsTaskService;
        private readonly ShortcutService ShortcutService;
        private readonly CandyShopContext CandyShopContext;
        private IMainWindowView MainView;

        public MainWindowController(ChocolateyService chocolateyService, WindowsTaskService windowsTaskService, ShortcutService shortcutService, CandyShopContext candyShopContext)
        {
            ChocolateyService = chocolateyService;
            WindowsTaskService = windowsTaskService;
            ShortcutService = shortcutService;
            CandyShopContext = candyShopContext;
        }

        public void InjectView(IMainWindowView mainView)
        {
            MainView = mainView;
        }

        public void InitView()
        {
            if (MainView == null) throw new InvalidOperationException("Set a view before intialising it!");

            RequestOutdatedPackagesAsync();

            if (CandyShopContext.HasAdminPrivileges) MainView.ClearAdminHints();
            else MainView.ShowAdminHints();

            // exit application on 'X'
            MainView.ToForm().FormClosed += new FormClosedEventHandler((sender, e) =>
            {
                Program.Exit();
            });

            // exit or hide application on 'Cancel', depending on how it was created
            MainView.CancelPressed += new EventHandler((sender, e) =>
            {
                if (CandyShopContext.LaunchedMinimized)
                {
                    MainView.ToForm().Hide();
                }
                else
                {
                    MainView.ToForm().Dispose();
                    Program.Exit();
                }
            });

            // wire upgrade page properties
            MainView.UpgradePackagesPage.CleanShortcutsChanged += new EventHandler((sender, e) => CandyShopContext.CleanShortcuts = MainView.UpgradePackagesPage.CleanShortcuts);

            MainView.CreateTaskEnabled = WindowsTaskService.LaunchTaskExists();
            MainView.UpgradePackagesPage.CleanShortcuts = CandyShopContext.CleanShortcuts;

            MainView.ToForm().Show();
        }

        public void SmartSelectPackages()
        {
            string[] displayedItemNames = MainView.UpgradePackagesPage.Items;
            List<ChocolateyPackage> packages = ChocolateyService.GetPackageByName(displayedItemNames.ToList());

            List<string> smartSelected = packages
                .Where(p => !(p.HasMetaPackage && p.HasSuffix))
                .Select(p => p.Name)
                .ToList();

            List<string> newSelection = packages
                .Where(p => !(p.HasMetaPackage && p.HasSuffix))
                .Select(p => p.Name)
                .ToList();

            MainView.UpgradePackagesPage.CheckItemsByText(newSelection);
        }

        public void ShowGithub()
        {
            OpenUrl(LocaleEN.URL_GITHUB);
        }

        public void ShowMetaPackageHelp()
        {
            OpenUrl(LocaleEN.URL_META_PACKAGES);
        }

        public void ShowLicenses()
        {
            using LicenseForm form = new LicenseForm();
            form.ShowDialog();
        }

        public void ToggleCreateTask()
        {
            if (MainView.CreateTaskEnabled && !WindowsTaskService.LaunchTaskExists())
            {
                WindowsTaskService.CreateLaunchTask();
            }

            if (!MainView.CreateTaskEnabled && WindowsTaskService.LaunchTaskExists())
            {
                WindowsTaskService.RemoveLaunchTask();
            }
        }

        public void ShowChocoLogFolder()
        {
            string path = Path.GetFullPath(CandyShopContext.CholoateyLogFolder);
            if (Directory.Exists(path))
            {
                try
                {
                    Process.Start("explorer.exe", path);
                }
                catch (Win32Exception e)
                {
                    MainView.DisplayError("An unknown error occurred: {0}", e.Message);
                }
                
            }
            else
            {
                MainView.DisplayError("Cannot find directory for Chocolatey logs: {0}", path);
            }
        }

        public void ShowCandyShopConfigFolder()
        {
            if (Directory.Exists(CandyShopContext.ConfigFolder))
            {
                CandyShopContext.Save();

                try
                {
                    Process.Start("explorer.exe", CandyShopContext.ConfigFolder);
                }
                catch (Win32Exception e)
                {
                    MainView.DisplayError("An unknown error occurred: {0}", e.Message);
                }
            }
            else
            {
                MainView.DisplayError("Cannot find CandyShop configuration directory at '{0}'", CandyShopContext.ConfigFolder);
            }
        }

        public async void PerformUpgrade(string[] packages)
        {
            MainView?.ToForm().Hide();

            List<string> shortcuts = new List<string>();
            ShortcutService?.WatchDesktops(shortcut => shortcuts.Add(shortcut));

            // upgrade
            WindowsConsole.AllocConsole();
            Console.CursorVisible = false;

            try
            {
                List<ChocolateyPackage> chocoPackages = ChocolateyService.GetPackageByName(packages.ToList());
                if (chocoPackages.Count > 0)
                {
                    ChocolateyService.Upgrade(chocoPackages);
                }
            }
            catch (ChocolateyException e)
            {
                MainView.DisplayError(LocaleEN.ERROR_UPGRADING_OUTDATED_PACKAGES, e.Message);
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
            if (MainView.UpgradePackagesPage.CleanShortcuts)
            {
                await minDelay; // wait for shortcuts to be created
                ShortcutService?.DeleteShortcuts(shortcuts);
            }

            // TODO if there still are outdated packages, return to MainView
            // TODO if there was an error, offer to open log folder? go back to application?
            MainView?.ToForm().Dispose();
            Program.Exit();
        }

        private async void RequestOutdatedPackagesAsync()
        {
            List<ChocolateyPackage> packages = new List<ChocolateyPackage>();
            try
            {
                packages = await ChocolateyService.GetOutdatedPackagesAsync();
            }
            catch (ChocolateyException)
            {
                MainView?.DisplayError("Failed to retrieve outdated packages: {}");
            }

            if (packages.Count == 0)
            {
                MainView.UpgradePackagesPage.Loading = false;
            }

            packages.ForEach(p => MainView.UpgradePackagesPage.AddItem(new string[]
            {
                p.Name,
                p.CurrVer,
                p.AvailVer,
                p.Pinned.ToString()
            }));

            SmartSelectPackages();
        }

        private void OpenUrl(string url)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = "cmd",
                Arguments = $"/c start {url}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(info);
        }
    }
}
