using CandyShop.Chocolatey;
using CandyShop.Controls;
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
        private readonly ChocolateyService _ChocolateyService;
        private readonly WindowsTaskService _WindowsTaskService;
        private CandyShopForm _CandyShopForm;

        private Dictionary<string, ChocolateyPackage> _InstalledPackages;
        private List<ChocolateyPackage> _OutdatedPackages;

        public CandyShopController(ChocolateyService chocolateyService, WindowsTaskService windowsTaskService)
        {
            _ChocolateyService = chocolateyService;
            _WindowsTaskService = windowsTaskService;

            _CandyShopForm = new CandyShopForm(this);
        }

        public bool LaunchedMinimized { get; set; } = false;
        
        private bool? _HasAdminPrivileges;
        public bool HasAdminPrivileges
        {
            get
            {
                if (_HasAdminPrivileges == null)
                {
                    using WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    _HasAdminPrivileges = principal.IsInRole(WindowsBuiltInRole.Administrator);
                }

                return _HasAdminPrivileges.Value;
            }
        }

        /// <exception cref="ChocolateyException"></exception>
        public async void GetPackageDetailsAsync(string packageName, Action<string> callback)
        {
            ChocolateyPackage packageMock = new ChocolateyPackage()
            {
                Name = packageName
            };

            string details = await _ChocolateyService.GetOrFetchInfo(packageMock);
            callback(details);
        }

        public void LaunchUrl(string url)
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
            List<ChocolateyPackage> packages = GetPackagesByName(packageNames);
            return SelectNormalAndMetaPackages(packages);
        }

        // TODO eval
        public List<ChocolateyPackage> GetPackagesByName(List<string> names)
        {
            // TODO fetch shit if needed, but lock
            // make it safe

            List<ChocolateyPackage> packages =
                names.Select(name => _InstalledPackages[name]).ToList();

            return packages;
        }

        public void PerformUpgrade(List<ChocolateyPackage> packages)
        {
            _CandyShopForm.Hide();

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
                    _ChocolateyService.Upgrade(packages);
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
            if (LaunchedMinimized)
            {
                _CandyShopForm.Hide();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        public void SetOutdatedPackages(List<ChocolateyPackage> outdatedPackages)
        {
            _CandyShopForm.UpdateOutdatedView(outdatedPackages);
        }

        public void SetInstalledPackages(List<ChocolateyPackage> outdatedPackages)
        {
            _CandyShopForm.UpdateInstalledView(outdatedPackages);
        }

        public void ShowForm()
        {
            if (!LaunchedMinimized)
            {
                LoadPackages();
            }

            _CandyShopForm.Show();
        }

        private async void LoadPackages()
        {
            List<ChocolateyPackage> outdatedPackages = null;
            List<ChocolateyPackage> installedPackages = null;

            try
            {
                outdatedPackages = await _ChocolateyService.FetchOutdatedAsync();
            }
            catch (ChocolateyException)
            {
                // TODO
            }

            try
            {
                installedPackages = await _ChocolateyService.FetchInstalledAsync();
            }
            catch (ChocolateyException)
            {

                throw;
            }

            SetOutdatedPackages(outdatedPackages);
            SetInstalledPackages(installedPackages);
        }
    }
}
