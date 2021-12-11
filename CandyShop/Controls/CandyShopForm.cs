using CandyShop.Chocolatey;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;
using CandyShop.Properties;

namespace CandyShop.Controls
{
    partial class CandyShopForm : Form
    {
        // TODO remove WindowsTaskService, use controller
        // remove and sort usings
        // check namepsaces
        // consistent behavior between controls
        // use controller for all methods in here
        // use consistent naming for private members
        // use consistent layout privates -> constructor -> properties -> methods https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
        // https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1201.md

        private WindowsTaskService WindowsTaskService; // TODO remove
        private readonly CandyShopController CandyShopController;

        public CandyShopForm(CandyShopController candyShopController)
        {
            WindowsTaskService = new WindowsTaskService();
            CandyShopController = candyShopController;
            InitializeComponent();
        }

        public List<ChocolateyPackage> SelectedPackages { get; set; }

        public bool HasSelectedPackages => SelectedPackages != null && SelectedPackages.Count > 0;

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
            this.Text = String.Format(Strings.Form_Title, Application.ProductName, Application.ProductVersion);
            if (CandyShopController.HasAdminPrivileges)
            {
                UpgradePage.ShowAdminWarning = false;
            }
            else
            {
                UpgradePage.ShowAdminWarning = true;
                this.Text += Strings.Form_Title;
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
            if (CandyShopController.HasAdminPrivileges)
            {
                ShowErrorDialog(Strings.Err_RequireAdmin);
                return;
            }

            WindowsTaskService.ToggleTask();
        }

        private void MenuHelpGithub_Click(object sender, EventArgs e)
        {
            CandyShopController.LaunchUrl(Strings.Url_Github);
        }

        private void MenuHelpLicense_Click(object sender, EventArgs e)
        {
            CandyShopController.ShowLicenseForm();
        }

        private void MenuHelpMetaPackages_Click(object sender, EventArgs e)
        {
            CandyShopController.LaunchUrl(Strings.Url_MetaPackages);
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

        private void InstallPage_SelectedPackageChanged(object sender, PackageChangedEventArgs e)
        {
            // all of this can go in InstalledPage, if installed page gets access to controller
            // just pass package name here, not entire package; saves weird shit in InstalledPage
            try
            {
                CandyShopController.GetPackageDetails(e.SelectedPackage.Name, (details) =>
                {
                    InstalledPage.SetPackageDetails(e.SelectedPackage, details);
                });
            }
            catch (CandyShopException)
            {
                InstalledPage.SetPackageDetails(e.SelectedPackage, String.Empty);
            }
        }

        private void LoadOutdated()
        {
            // TODO adjust LoadInstalled accordingly; these two methods should probably go into the controller; the form should be created in the controller
            
            try
            {
                CandyShopController.GetOutdatedPackagesAsync((packages) =>
                {
                    UpgradePage.OutdatedPackages = packages; // TODO this should be a method call, or fetching details in InstallPage_SelectedPackageChanged should be a property
                });
            }
            catch (ChocolateyException)
            {
                ShowErrorDialog(Strings.Err_CheckOutdated);
            }
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
                ShowErrorDialog(Strings.Err_ListInstalled);
                packages = new List<ChocolateyPackage>();
            }
            
            InstalledPage.Packages = packages;
        }

        private void ShowErrorDialog(string msg)
        {
            MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
