using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace ChocoAutoUpdate
{
    public partial class ChocoAutoUpdateForm : Form
    {
        private readonly BackgroundWorker _BackgroundWorker;

        private List<ChocolateyPackage> _OutdatedPackages;

        public List<string> PackageNamesForUpgrading { get; private set; } = new List<string>();

        public bool IsElevated { get; set; } = false;

        public ChocoAutoUpdateForm()
        {
            _BackgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true
            };
            _BackgroundWorker.DoWork += _BackgroundWorker_DoWork;
            _BackgroundWorker.RunWorkerCompleted += _BackgroundWorker_RunWorkerCompleted;

            InitializeComponent();
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

            _OutdatedPackages = (List<ChocolateyPackage>) e.Result;
            
            if (!e.Cancelled && e.Error == null)
            {
                TxtLoading.Visible = false;
                BtnCancel.Enabled = true;
                BtnUpgradeChecked.Enabled = true;
                BtnUpgradeAll.Enabled = true;
                foreach (ChocolateyPackage pckg in _OutdatedPackages)
                {
                    ListViewItem item = new ListViewItem(
                        new string[] { pckg.Name, pckg.CurrVer, pckg.AvailVer, pckg.Pinned.ToString()});
                    item.Checked = true;
                    LstPackages.Items.Add(item);
                }
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

        private void LinkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkGithub.LinkVisited = true;
            var url = "https://github.com/weberjonathan/Chocolatey-AutoUpdate";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}"));
        }

        private void BtnUpgradeAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in LstPackages.Items)
            {
                PackageNamesForUpgrading.Add(item.Text);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void LstPackages_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            BtnUpgradeChecked.Text = $"Upgrade ({LstPackages.CheckedItems.Count})";
        }

        private void BtnUpgradeChecked_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in LstPackages.CheckedItems)
            {
                PackageNamesForUpgrading.Add(item.Text);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
