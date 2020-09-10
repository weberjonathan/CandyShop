using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ChocoAutoUpdate
{
    public partial class ChocoAutoUpdateForm : Form
    {
        public ChocoAutoUpdateForm()
        {
            InitializeComponent();

            GetInstalledAsync();
            GetOutdatedAsync();

            UpgradePage.UpgradeAllClick += UpgradePage_UpgradeAllClick;
            UpgradePage.UpgradeSelectedClick += UpgradePage_UpgradeSelectedClick;
            UpgradePage.CancelClick += UpgradePage_CancelClick;
        }

        public List<ChocolateyPackage> SelectedPackages { get; set; }

        private void ChocoAutoUpdateForm_Load(object sender, EventArgs e)
        {
            if (Program.IsElevated())
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

        private async void GetOutdatedAsync()
        {
            var packages = await ChocolateyWrapper.CheckOutdatedAsync();
            UpgradePage.OutdatedPackages = packages;
        }

        private async void GetInstalledAsync()
        {
            List<ChocolateyPackage> packages = await ChocolateyWrapper.ListInstalledAsync();
            InstalledPage.Packages = packages;
        }
    }
}
