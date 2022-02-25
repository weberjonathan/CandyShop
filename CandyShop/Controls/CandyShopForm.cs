using CandyShop.Chocolatey;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;
using CandyShop.Properties;
using System.Linq;

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

            this.FormClosed += new FormClosedEventHandler((sender, e) => CandyShopController.CloseForm());
        }

        public void UpdateInstalledView(List<ChocolateyPackage> packages)
        {
            InstalledPage.UpdatePackageView(packages);
        }

        public void UpdateOutdatedView(List<ChocolateyPackage> packages)
        {
            if (packages == null)
            {
                ShowError(Strings.Err_CheckOutdated);
            }

            string[][] items = packages.Select(package => new string[] {
                package.Name,
                package.CurrVer,
                package.AvailVer,
                package.Pinned.ToString()
            }).ToArray();

            UpgradePage.UpdatePackageView(items);
            UpgradePage.CheckItemsByName(CandyShopController.SelectNormalAndMetaPackages(packages));
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
            List<string> packageNames = UpgradePage.ItemNames.ToList();
            packageNames = CandyShopController.SelectNormalAndMetaPackages(packageNames);
            UpgradePage.CheckItemsByName(packageNames);
        }

        private void MenuEditDeselectAll_Click(object sender, EventArgs e)
        {
            UpgradePage.UncheckAllItems();
        }

        private void MenuExtrasCreateTask_Click(object sender, EventArgs e)
        {
            if (CandyShopController.HasAdminPrivileges)
            {
                ShowError(Strings.Err_RequireAdmin);
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
            List<ChocolateyPackage> packages =
                CandyShopController.GetPackagesByName(UpgradePage.ItemNames.ToList());
            CandyShopController.PerformUpgrade(packages);
        }

        private void UpgradePage_UpgradeSelectedClick(object sender, EventArgs e)
        {
            List<ChocolateyPackage> packages =
                CandyShopController.GetPackagesByName(UpgradePage.SelectedItemNames.ToList());

            if (packages.Count > 0) CandyShopController.PerformUpgrade(packages);
        }

        private void UpgradePage_CancelClick(object sender, EventArgs e)
        {
            CandyShopController.CancelForm();
        }

        private void InstallPage_SelectedPackageChanged(object sender, PackageChangedEventArgs e)
        {
            // all of this can go in InstalledPage, if installed page gets access to controller
            // just pass package name here, not entire package; saves weird shit in InstalledPage
            try
            {
                CandyShopController.GetPackageDetailsAsync(e.SelectedPackage.Name, (details) =>
                {
                    InstalledPage.UpdatePackageDetails(e.SelectedPackage, details);
                });
            }
            catch (CandyShopException)
            {
                InstalledPage.UpdatePackageDetails(e.SelectedPackage, String.Empty);
            }
        }

        private void ShowError(string msg)
        {
            MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
