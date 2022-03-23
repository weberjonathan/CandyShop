using CandyShop.Chocolatey;
using CandyShop.Controller;
using CandyShop.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CandyShop
{
    internal class CandyShopController
    {
        private readonly ChocolateyService ChocolateyService;
        private readonly WindowsTaskService WindowsTaskService;
        private readonly CandyShopContext CandyShopContext;
        private IMainWindow MainView;

        public CandyShopController(ChocolateyService chocolateyService, WindowsTaskService windowsTaskService, CandyShopContext candyShopContext)
        {
            ChocolateyService = chocolateyService;
            WindowsTaskService = windowsTaskService;
            CandyShopContext = candyShopContext;
        }

        public void SetView(IMainWindow mainView)
        {
            MainView = mainView;
        }

        public void OpenUrl(string url)
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

        public void ShowLicenseForm()
        {
            using LicenseForm form = new LicenseForm();
            form.ShowDialog();
        }

        public List<string> SelectNormalAndMetaPackages(List<ChocolateyPackage> packages)
        {
            return packages
                .Where(p => !(p.HasMetaPackage && p.HasSuffix))
                .Select(p => p.Name)
                .ToList();
        }

        public List<string> SelectNormalAndMetaPackages(List<string> packageNames)
        {
            List<ChocolateyPackage> packages = ChocolateyService.GetInstalledPackagesByName(packageNames);
            return SelectNormalAndMetaPackages(packages);
        }

        public void PerformUpgrade(List<ChocolateyPackage> packages)
        {
            MainView.ToForm().Hide();

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
                ConsoleManager.AllocConsole();
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
            IntPtr handle = ConsoleManager.GetConsoleWindow();
            if (!IntPtr.Zero.Equals(handle))
            {
                ConsoleManager.SetForegroundWindow(handle);
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

        public void CloseForm()
        {
            Environment.Exit(0);
        }

        public void CancelForm()
        {
            if (CandyShopContext.LaunchedMinimized)
            {
                MainView.ToForm().Hide();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        public void InitView()
        {
            RequestOutdatedPackagesAsync();

            if (CandyShopContext.HasAdminPrivileges)
                MainView.ClearAdminHints();
            else
                MainView.ShowAdminHints();

            MainView.ToForm().Show();
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
                throw;
            }

            packages.ForEach(p => MainView.UpgradePackagesPage.AddItem(new string[]
            {
                p.Name,
                p.CurrVer,
                p.AvailVer,
                p.Pinned.ToString()
            }));

            MainView.UpgradePackagesPage.CheckItemsByText(SelectNormalAndMetaPackages(packages));
        }
    }
}
