using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChocoAutoUpdate
{
    public partial class ChocoAutoUpdateForm : Form
    {
        private readonly BackgroundWorker _BackgroundWorker;

        public List<ChocolateyPackage> SelectedPackages { get; set; }

        public bool IsElevated { get; set; } = false;

        public ChocoAutoUpdateForm()
        {
            InitializeComponent();

            _BackgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true
            };
            _BackgroundWorker.DoWork += _BackgroundWorker_DoWork;
            _BackgroundWorker.RunWorkerCompleted += _BackgroundWorker_RunWorkerCompleted;

            GetInstalledAsync();

            UpgradePage.UpgradeAllClick += UpgradePage_UpgradeAllClick;
            UpgradePage.UpgradeSelectedClick += UpgradePage_UpgradeSelectedClick;
            UpgradePage.CancelClick += UpgradePage_CancelClick;
        }

        private void ChocoAutoUpdateForm_Load(object sender, EventArgs e)
        {
            _BackgroundWorker.RunWorkerAsync();
            TopPanel.Visible = !IsElevated;
            if (IsElevated)
            {
                this.Text = $"{Application.ProductName} v{Application.ProductVersion}";
            }
            else
            {
                this.Text = $"{Application.ProductName} v{Application.ProductVersion} (no administrator privileges)";
            }

            this.Activate();
        }

        private void _BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = ChocolateyWrapper.CheckOutdated();
        }

        /// <exception cref="InvalidOperationException"></exception>
        private void _BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result.GetType() != typeof(List<ChocolateyPackage>))
            {
                throw new InvalidOperationException($"Expected result of type 'List<ChocolateyPackage>'.");
            }

            List<ChocolateyPackage> packages = (List<ChocolateyPackage>) e.Result;
            if (!e.Cancelled && e.Error == null)
            {
                UpgradePage.OutdatedPackages = packages;
            }
            else
            {
                MessageBox.Show(
                    "An unknown error occurred. Please upgrade using the terminal.",
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
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
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void UpgradePage_CancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private async void GetInstalledAsync()
        {
            List<ChocolateyPackage> packages = await Task<List<ChocolateyPackage>>.Run(() =>
            {
                return ChocolateyWrapper.ListInstalled();
            });

            InstalledPage.Packages = packages;
        }
    }
}
