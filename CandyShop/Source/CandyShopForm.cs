using CandyShop.Chocolatey;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;

namespace CandyShop
{
    public partial class CandyShopForm : Form
    {
        private const string TASKNAME = "CandyShopLaunch";
        private bool WindowsTaskExists; // TODO create Context or properties class that contains information like this
        private Dictionary<string, string> InstalledPackageDetails = new Dictionary<string, string>();

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
            InstalledPage.SelectedPackageChanged += InstalledPage_SelectedPackageChanged;

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
                    definition.Actions.Add(executable, "-s", dir);
                    definition.Principal.RunLevel = TaskRunLevel.Highest;

                    TaskService.Instance.RootFolder.RegisterTaskDefinition(TASKNAME, definition);

                    WindowsTaskExists = true;
                }

                MenuExtrasCreateTask.Checked = WindowsTaskExists;
            }
        }

        private void MenuHelpGithub_Click(object sender, EventArgs e)
        {
            string url = "https://github.com/weberjonathan/CandyShop";
            ProcessStartInfo info = new ProcessStartInfo() {
                FileName = "cmd",
                Arguments = $"/c start {url}",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            Process.Start(info);
        }

        private void MenuHelpLicense_Click(object sender, EventArgs e)
        {
            using (LicenseForm form = new LicenseForm())
            {
                form.ShowDialog();
            }
        }

        private void UpgradePage_UpgradeAllClick(object sender, EventArgs e)
        {
            SelectedPackages = UpgradePage.OutdatedPackages;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void UpgradePage_UpgradeSelectedClick(object sender, EventArgs e)
        {
            SelectedPackages = UpgradePage.SelectedPackages;
            if (SelectedPackages.Count > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void UpgradePage_CancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private async void InstalledPage_SelectedPackageChanged(object sender, EventArgs e)
        {
            if (InstalledPage.SelectedPackage == null) return;
            
            string selectedPackageName = InstalledPage.SelectedPackage.Name;
            string details;
            if (!InstalledPackageDetails.TryGetValue(selectedPackageName, out details))
            {
                try
                {
                    details = await ChocolateyWrapper.GetInfoAsync(InstalledPage.SelectedPackage);
                    if (!InstalledPackageDetails.ContainsKey(selectedPackageName))
                    {
                        InstalledPackageDetails.Add(selectedPackageName, details);
                    }
                }
                catch (ChocolateyException)
                {
                    details = Properties.strings.Form_Err_GetInfo;
                }
            }
            
            // check if package whose info was waited on is still selected
            if (InstalledPage.SelectedPackage != null)
            {
                if (InstalledPage.SelectedPackage.Name.Equals(selectedPackageName))
                {
                    InstalledPage.DetailsText = details;
                }
            }
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
    }
}
