using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace ChocoAutoUpdate
{
    public partial class ChocoAutoUpdateForm : Form
    {
        private readonly Choco _Choco;
        private readonly BackgroundWorker _BackgroundWorker;

        public bool IsElevated { get; set; } = false;

        public ChocoAutoUpdateForm(Choco choco)
        {
            _Choco = choco ?? throw new ArgumentNullException("choco");
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
            this.Text = $"{Application.ProductName} v{Application.ProductVersion}";
        }

        private void _BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _Choco.CheckOutdated();
        }

        private void _BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                TxtLoading.Visible = false;
                BtnCancel.Enabled = true;
                BtnUpgrade.Text = $"{BtnUpgrade.Text} ({_Choco.Outdated.Count})";
                BtnUpgrade.Enabled = true;
                foreach (ChocoPackage pckg in _Choco.Outdated)
                {
                    LstPackages.Items.Add(new ListViewItem(
                        new string[] { pckg.Name, pckg.CurrVer, pckg.AvailVer, pckg.Pinned.ToString() })
                    );
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

        private void BtnUpgrade_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
