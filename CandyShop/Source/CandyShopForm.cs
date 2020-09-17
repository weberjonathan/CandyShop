using CandyShop.Chocolatey;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Windows.Forms;

namespace CandyShop
{
    public partial class CandyShopForm : Form
    {
        private Dictionary<string, string> InstalledPackageDetails = new Dictionary<string, string>();

        public CandyShopForm()
        {
            InitializeComponent();

            GetInstalledAsync();
            GetOutdatedAsync();

            UpgradePage.UpgradeAllClick += UpgradePage_UpgradeAllClick;
            UpgradePage.UpgradeSelectedClick += UpgradePage_UpgradeSelectedClick;
            UpgradePage.CancelClick += UpgradePage_CancelClick;

            InstalledPage.SelectedPackageChanged += InstalledPage_SelectedPackageChanged;
        }

        public List<ChocolateyPackage> SelectedPackages { get; set; }

        private void ChocoAutoUpdateForm_Load(object sender, EventArgs e)
        {
            if (HasAdminPrivileges())
            {
                TopPanel.Visible = false;
                this.Text = $"{Application.ProductName} v{Application.ProductVersion}";
            }
            else
            {
                TopPanel.Visible = true;
                this.Text = $"{Application.ProductName} v{Application.ProductVersion} (no administrator privileges)";
            }

            this.Activate();
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
            this.DialogResult = DialogResult.OK;
            this.Close();
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
                ShowErrorDialog(Properties.strings.Form_Err_CheckOutdated);
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
