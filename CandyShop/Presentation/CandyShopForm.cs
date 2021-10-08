using CandyShop.Chocolatey;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace CandyShop.Presentation
{
    public partial class CandyShopForm : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const string TASKNAME = "CandyShopLaunch";
        private bool WindowsTaskExists; // TODO create Context or properties class that contains information like this

        public CandyShopForm()
        {
            InitializeComponent();
            GetInstalledAsync();
            GetOutdatedAsync();
        }

        public CandyShopForm(List<ChocolateyPackage> outdatedPackages)
        {
            InitializeComponent();
            GetInstalledAsync();
            UpgradePage.OutdatedPackages = outdatedPackages;
            // TODO implement refresh that prompts on new outdated packages
        }

        public List<ChocolateyPackage> SelectedPackages { get; set; }

        private void ChocoAutoUpdateForm_Load(object sender, EventArgs e)
        {
            // register handlers
            UpgradePage.UpgradeAllClick += UpgradePage_UpgradeAllClick;
            UpgradePage.UpgradeSelectedClick += UpgradePage_UpgradeSelectedClick;
            UpgradePage.CancelClick += UpgradePage_CancelClick;

            // display admin warning or not
            if (HasAdminPrivileges())
            {
                UpgradePage.ShowAdminWarning = false;
                this.Text = $"{Application.ProductName} v{Application.ProductVersion}";
            }
            else
            {
                UpgradePage.ShowAdminWarning = true;
                this.Text = $"{Application.ProductName} v{Application.ProductVersion} (no administrator privileges)";
            }

            // check task entry or not
            using (TaskService ts = new TaskService())
            {
                WindowsTaskExists = ts.GetTask(TASKNAME) != null;
            }
            MenuExtrasCreateTask.Checked = WindowsTaskExists;

            this.Activate();
        }

        private void MenuEditSelectAll_Click(object sender, EventArgs e)
        {
            UpgradePage.CheckAllItems();
        }

        private void MenuEditSelectRelevant_Click(object sender, EventArgs e)
        {
            UpgradePage.CheckNormalAndMetaItems();
        }

        private void MenuEditDeselectAll_Click(object sender, EventArgs e)
        {
            UpgradePage.UncheckAllItems();
        }

        private void MenuExtrasCreateTask_Click(object sender, EventArgs e)
        {
            if (!HasAdminPrivileges())
            {
                ShowErrorDialog(Properties.strings.Form_Err_RequireAdmin);
                return;
            }

            using (TaskService ts = new TaskService())
            {
                if (WindowsTaskExists)
                {
                    // remove task
                    Task task = ts.GetTask(TASKNAME);
                    if (task != null)
                    {
                        ts.RootFolder.DeleteTask(TASKNAME);
                    }

                    WindowsTaskExists = false;
                }
                else
                {
                    // create task
                    string executable = Process.GetCurrentProcess().MainModule.FileName;
                    string dir = Directory.GetParent(executable).FullName;

                    TaskDefinition definition = TaskService.Instance.NewTask();
                    definition.RegistrationInfo.Description = "Launch CandyShop with elevated privileges to display outdated packages on login.";
                    definition.Principal.LogonType = TaskLogonType.InteractiveToken;

                    LogonTrigger trigger = new LogonTrigger();

                    definition.Triggers.Add(trigger);
                    definition.Actions.Add(executable, "--background", dir);
                    definition.Principal.RunLevel = TaskRunLevel.Highest;

                    TaskService.Instance.RootFolder.RegisterTaskDefinition(TASKNAME, definition);

                    WindowsTaskExists = true;
                }
            }
        }

        private void MenuHelpGithub_Click(object sender, EventArgs e)
        {
            LaunchUrl("https://github.com/weberjonathan/CandyShop");
        }

        private void MenuHelpLicense_Click(object sender, EventArgs e)
        {
            using (LicenseForm form = new LicenseForm())
            {
                form.ShowDialog();
            }
        }

        private void MenuHelpMetaPackages_Click(object sender, EventArgs e)
        {
            LaunchUrl("https://docs.chocolatey.org/en-us/faqs#what-is-the-difference-between-packages-no-suffix-as-compared-to.install.portable");
        }

        private void UpgradePage_UpgradeAllClick(object sender, EventArgs e)
        {
            SelectedPackages = UpgradePage.OutdatedPackages;
            LaunchUpgradeConsole(SelectedPackages);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void UpgradePage_UpgradeSelectedClick(object sender, EventArgs e)
        {
            SelectedPackages = UpgradePage.SelectedPackages;
            if (SelectedPackages.Count > 0)
            {
                LaunchUpgradeConsole(SelectedPackages);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void UpgradePage_CancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        

        private async void GetOutdatedAsync()
        {
            try
            {
                List<ChocolateyPackage> packages = await ChocolateyWrapper.CheckOutdatedAsync();
                UpgradePage.OutdatedPackages = packages;
            }
            catch (ChocolateyException)
            {
                ShowErrorDialog(Properties.strings.Err_CheckOutdated);
            }
        }

        private async void GetInstalledAsync()
        {
            try
            {
                List<ChocolateyPackage> packages = await ChocolateyWrapper.ListInstalledAsync();
                InstalledPage.Packages = packages;
            }
            catch (ChocolateyException)
            {
                ShowErrorDialog(Properties.strings.Form_Err_ListInstalled);
            }
        }

        private bool HasAdminPrivileges()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private void ShowErrorDialog(string msg)
        {
            MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void LaunchUrl(string url)
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

        private void LaunchUpgradeConsole(List<ChocolateyPackage> packages)
        {
            this.Hide();
            
            // setup watcher for desktop shortcuts
            Queue<string> shortcuts = new Queue<string>();
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.BeginInit();

                watcher.Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                watcher.Filter = "*.lnk";
                watcher.IncludeSubdirectories = false;
                watcher.NotifyFilter = NotifyFilters.FileName;
                watcher.InternalBufferSize = 65536; // TODO test; incurs performance penalty so remove if not useful
                watcher.EnableRaisingEvents = true;
                watcher.Created += new FileSystemEventHandler((sender, e) =>
                {
                    shortcuts.Enqueue(e.FullPath);
                });

                watcher.EndInit();

                // upgrade
                AllocConsole();
                Console.CursorVisible = false;

                try
                {
                    ChocolateyWrapper.Upgrade(packages);
                }
                catch (ChocolateyException e)
                {
                    MessageBox.Show(
                        $"An error occurred while executing Chocolatey: \"{e.Message}\"",
                        $"{Application.ProductName} Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    Application.Exit(); // TODO test
                }
            }

            // display results
            IntPtr handle = GetConsoleWindow();
            if (!IntPtr.Zero.Equals(handle))
            {
                SetForegroundWindow(handle);
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

            // exit
            Application.Exit();
        }
    }
}
