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
using System.Windows.Forms;

namespace CandyShop.Controller
{
    internal class MainWindowController : IMainWindowController
    {
        private readonly ChocolateyService ChocolateyService;
        private readonly WindowsTaskService WindowsTaskService;
        private readonly CandyShopContext CandyShopContext;
        private IMainWindowView MainView;

        public MainWindowController(ChocolateyService chocolateyService, WindowsTaskService windowsTaskService, CandyShopContext candyShopContext)
        {
            ChocolateyService = chocolateyService;
            WindowsTaskService = windowsTaskService;
            CandyShopContext = candyShopContext;
        }

        public void InjectView(IMainWindowView mainView)
        {
            MainView = mainView;
        }

        public void PerformUpgrade(List<ChocolateyPackage> packages)
        {
            MainView?.ToForm().Hide();

            // setup watcher for desktop shortcuts
            Queue<string> shortcuts = new Queue<string>();
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.BeginInit();

                watcher.Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                watcher.Filter = "*.lnk";
                watcher.IncludeSubdirectories = false;
                watcher.NotifyFilter = NotifyFilters.FileName;
                watcher.EnableRaisingEvents = true;
                watcher.Created += new FileSystemEventHandler((sender, e) =>
                {
                    shortcuts.Enqueue(e.FullPath);
                });

                watcher.EndInit();

                // upgrade
                WindowsConsole.AllocConsole();
                Console.CursorVisible = false;

                try
                {
                    ChocolateyService.Upgrade(packages);
                }
                catch (ChocolateyException e)
                {
                    // TODO eval
                    MessageBox.Show(
                        $"An error occurred while executing Chocolatey: \"{e.Message}\"",
                        $"{Application.ProductName} Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    return;
                }
            }

            // display results
            IntPtr handle = WindowsConsole.GetConsoleWindow();
            if (!IntPtr.Zero.Equals(handle))
            {
                WindowsConsole.SetForegroundWindow(handle);
            }
            Console.CursorVisible = false;
            Console.Write("\nPress any key to continue... ");
            Console.ReadKey();

            // remove shortcuts
            if (shortcuts.Count > 0)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append($"During the upgrade process {shortcuts.Count} new desktop shortcut(s) were created:\n\n");
                foreach (string shortcut in shortcuts)
                {
                    msg.Append($"- {Path.GetFileNameWithoutExtension(shortcut)}\n");
                }
                msg.Append($"\nDo you want to delete all {shortcuts.Count} shortcut(s)?");

                DialogResult result = MessageBox.Show(
                    msg.ToString(),
                    Application.ProductName,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);

                if (result.Equals(DialogResult.Yes))
                {
                    while (shortcuts.Count > 0)
                    {
                        string shortcut = shortcuts.Dequeue();
                        try
                        {
                            File.Delete(shortcut);
                        }
                        catch (IOException)
                        {
                            // TODO
                        }
                    }
                }
            }

            // TODO eval
            Environment.Exit(0);
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
                Environment.Exit(0);
            });

            // exit or hide application on 'Cancel', depending on how it was created
            MainView.CancelPressed += new EventHandler((sender, e) =>
            {
                if (CandyShopContext.LaunchedMinimized) MainView.ToForm().Hide();
                else Environment.Exit(0);
            });

            MainView.CreateTaskEnabled = WindowsTaskService.TaskExists();

            MainView.ToForm().Show();
        }

        public void SmartSelectPackages()
        {
            string[] displayedItemNames = MainView.UpgradePackagesPage.Items;
            List<ChocolateyPackage> packages = ChocolateyService.GetInstalledPackagesByName(displayedItemNames.ToList());

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
            if (MainView.CreateTaskEnabled && !WindowsTaskService.TaskExists())
            {
                WindowsTaskService.CreateTask();
            }

            if (!MainView.CreateTaskEnabled && WindowsTaskService.TaskExists())
            {
                WindowsTaskService.RemoveTask();
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
                    MainView.DisplayError("An unknown error occurred: [1}", e.Message);
                }
                
            }
            else
            {
                MainView.DisplayError("Cannot find directory for Chocolatey logs: {1}", path);
            }
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
