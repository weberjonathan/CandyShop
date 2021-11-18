using CandyShop.Chocolatey;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    partial class CandyShopForm : Form
    {
        private ChocolateyService ChocolateyService;
        private WindowsTaskService WindowsTaskService;

        public CandyShopForm(ChocolateyService chocolateyService)
        {
            ChocolateyService = chocolateyService;
            WindowsTaskService = new WindowsTaskService();
            InitializeComponent();
        }

        public List<ChocolateyPackage> SelectedPackages { get; set; }

        public bool HasSelectedPackages => SelectedPackages != null && SelectedPackages.Count > 0;

        public void LoadPackages()
        {
            LoadInstalledAsync();
            LoadOutdatedAsync();
        }

        public void SetOutdatedPackages(List<ChocolateyPackage> packages)
        {
            UpgradePage.OutdatedPackages = packages;
        }

        private void ChocoAutoUpdateForm_Load(object sender, EventArgs e)
        {
            // register handlers
            UpgradePage.UpgradeAllClick += UpgradePage_UpgradeAllClick;
            UpgradePage.UpgradeSelectedClick += UpgradePage_UpgradeSelectedClick;
            UpgradePage.CancelClick += UpgradePage_CancelClick;
            InstalledPage.SelectedPackageChanged += InstallPage_SelectedPackageChanged;

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
            MenuExtrasCreateTask.Checked = WindowsTaskService.TaskExists();

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

            WindowsTaskService.ToggleTask();
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

        private async void InstallPage_SelectedPackageChanged(object sender, PackageChangedEventArgs e)
        {
            string info = await ChocolateyService.GetInfo(e.SelectedPackage);
            InstalledPage.SetPackageDetails(e.SelectedPackage, info);
        }

        private async void LoadOutdatedAsync()
        {
            List<ChocolateyPackage> packages;
            try
            {
                packages = await ChocolateyService.FetchOutdatedAsync();
                
            }
            catch (ChocolateyException)
            {
                ShowErrorDialog(Properties.strings.Err_CheckOutdated);
                packages = new List<ChocolateyPackage>();
            }

            UpgradePage.OutdatedPackages = packages;
        }

        private async void LoadInstalledAsync()
        {
            List<ChocolateyPackage> packages;

            try
            {
                 packages = await ChocolateyService.FetchInstalledAsync();
            }
            catch (ChocolateyException)
            {
                ShowErrorDialog(Properties.strings.Form_Err_ListInstalled);
                packages = new List<ChocolateyPackage>();
            }
            
            InstalledPage.Packages = packages;
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
    }
}
