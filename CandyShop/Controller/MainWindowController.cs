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

            UpdateOutdatedPackageListAsync();

            MainView.CreateTaskEnabled = WindowsTaskService.LaunchTaskExists();

            if (CandyShopContext.HasAdminPrivileges) MainView.ClearAdminHints();
            else MainView.ShowAdminHints();

            // exit application on 'X'
            MainView.ToForm().FormClosed += new FormClosedEventHandler((sender, e) =>
            {
                Program.Exit();
            });

            MainView.ToForm().Show();
        }

        public void SmartSelectPackages()
        {
            string[] displayedItemNames = MainView.UpgradePackagesPage.Items;
            List<ChocolateyPackage> packages = ChocolateyService.GetPackagesByName(displayedItemNames.ToList());

            List<string> newSelection = packages
                .Where(p => !(p.HasMetaPackage && p.HasSuffix))
                .Where(p => p.Pinned.HasValue && !p.Pinned.Value)
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
            if (!MainView.CreateTaskEnabled && !WindowsTaskService.LaunchTaskExists())
            {
                WindowsTaskService.CreateLaunchTask();
                MainView.CreateTaskEnabled = true;
            }
            else if (MainView.CreateTaskEnabled && WindowsTaskService.LaunchTaskExists())
            {
                try
                {
                    WindowsTaskService.RemoveLaunchTask();
                }
                catch (CandyShopException)
                {
                    MainView?.DisplayError("An error occurred while trying to delete the Windows task.");
                }
            }

            MainView.CreateTaskEnabled = WindowsTaskService.LaunchTaskExists();
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

        private async void UpdateOutdatedPackageListAsync()
        {
            List<ChocolateyPackage> packages = new List<ChocolateyPackage>();
            try
            {
                packages = await ChocolateyService.GetOutdatedPackagesAsync();
            }
            catch (ChocolateyException e)
            {
                MainView?.DisplayError(LocaleEN.ERROR_RETRIEVING_OUTDATED_PACKAGES, e.Message);
            }

            MainView.UpgradePackagesPage.Loading = false;

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
