using CandyShop.Settings;
using System;
using System.Linq;
using System.Security;
using System.Windows.Forms;

namespace CandyShop.View
{
    partial class SettingsWindow : Form, ISettingsListener
    {
        public string ActivePackageSource
        {
            get { return cmbSource.SelectedItem.ToString(); }
            set { cmbSource.SelectedItem = value; }
        }

        public string WingetBinary
        {
            get { return txtWingetBinary.Text; }
            set { txtWingetBinary.Text = value; }
        }

        public string ChocolateyBinary
        {
            get { return txtChocoBinary.Text; }
            set { txtChocoBinary.Text = value; }
        }

        public string GSudoBinary
        {
            get { return txtGSudoBinary.Text; }
            set { txtGSudoBinary.Text = value; }
        }
        
        public bool UpgradeAsAdmin
        {
            get { return checkRequireAdmin.Checked; }
            set
            {
                checkRequireAdmin.Checked = value;
            }
        }
        
        public bool CacheAdminPrivileges
        {
            get { return checkCacheAdmin.Checked; }
            set
            {
                checkCacheAdmin.Checked = value;
            }
        }

        public bool EnableGsudo
        {
            get { return checkEnableGsudo.Checked; }
            set
            {
                checkEnableGsudo.Checked = value;

                checkCacheAdmin.Enabled = value;
            }
        }

        public bool DisplayFirstStartBanner
        {
            get { return panel2.Visible; }
            set
            {
                panel2.Visible = value;
            }
        }

        public bool Locked
        {
            get
            {
                return btnOk.Enabled;
            }
            set
            {
                cmbSource.Enabled = !value;
                btnOk.Enabled = !value;
                btnApply.Enabled = !value;
                btnChocoBinary.Enabled = !value;
                btnWingetBinary.Enabled = !value;
                btnGSudoBinary.Enabled = !value;
                checkRequireAdmin.Enabled = !value;
                checkEnableGsudo.Enabled = !value;
                checkCacheAdmin.Enabled = !value && checkEnableGsudo.Checked;
                txtChocoBinary.Enabled = !value;
                txtWingetBinary.Enabled = !value;
                txtGSudoBinary.Enabled = !value;
            }
        }

        public event EventHandler ApplyClicked;
        public event EventHandler OkClicked;
        public event EventHandler WingetBinaryChanged;
        public event EventHandler ChocolateyBinaryChanged;
        public event EventHandler GSudoBinaryChanged;

        public SettingsWindow()
        {
            InitializeComponent();

            Text = $"{MetaInfo.Name} | Settings";

            btnApply.Click += new EventHandler((sender, e) => ApplyClicked?.Invoke(sender, e));
            btnOk.Click += new EventHandler((sender, e) => OkClicked?.Invoke(sender, e));

            btnWingetBinary.Click += (sender, e)
                => UpdateBinaryTextbox(txtWingetBinary, WingetBinaryChanged, "Winget");
            btnChocoBinary.Click += (sender, e)
                => UpdateBinaryTextbox(txtChocoBinary, ChocolateyBinaryChanged, "Chocolatey");
            btnGSudoBinary.Click += (sender, e)
                => UpdateBinaryTextbox(txtGSudoBinary, GSudoBinaryChanged, "Gsudo");

            txtWingetBinary.TextChanged += (sender, e) =>
            {
                lblWingetStatus.Text = string.Empty;
                WingetBinaryChanged?.Invoke(sender, e);
            };

            txtChocoBinary.TextChanged += (sender, e) =>
            {
                lblChocoStatus.Text = string.Empty;
                ChocolateyBinaryChanged?.Invoke(sender, e);
            };

            txtGSudoBinary.TextChanged += (sender, e) =>
            {
                lblGSudoStatus.Text = string.Empty;
                GSudoBinaryChanged?.Invoke(sender, e);
            };

            checkEnableGsudo.CheckedChanged += (sender, e) =>
            {
                if (!checkEnableGsudo.Checked)
                    checkCacheAdmin.Checked = false;

                checkCacheAdmin.Enabled = checkEnableGsudo.Checked;
            };
        }

        public void SetWingetBinaryStatus(string status)
        {
            lblWingetStatus.Text = status;
        }

        public void SetChocoBinaryStatus(string status)
        {
            lblChocoStatus.Text = status;
        }

        public void SetGSudoBinaryStatus(string status)
        {
            lblGSudoStatus.Text = status;
        }

        public void OnSettingsChanged(SettingsDefinition settings)
        {
            ActivePackageSource = settings.EnabledPackageManagers.First().Name;
            WingetBinary = settings.Winget.Filepath;
            ChocolateyBinary = settings.Chocolatey.Filepath;
            GSudoBinary = settings.Gsudo.Filepath;
            UpgradeAsAdmin = settings.EnabledPackageManagers.First().UpgradeAsAdmin;
            EnableGsudo = settings.Gsudo.Enabled;
            CacheAdminPrivileges = settings.Gsudo.CachePrivileges;
        }

        private void UpdateBinaryTextbox(TextBox target, EventHandler handler, string title)
        {
            using var ofd = new OpenFileDialog()
            {
                Filter = "Executables | *.exe",
                DefaultExt = ".exe",
                FilterIndex = 0,
                Title = $"{MetaInfo.Name} | Choose {title} executable",
                Multiselect = false,
                ReadOnlyChecked = true
            };

            var result = ofd.ShowDialog();
            if (result.Equals(DialogResult.OK))
            {

                string filename = null;
                try
                {
                    filename = ofd.FileName;
                }
                catch (SecurityException)
                {
                    MessageBox.Show("hello");
                    return;
                }
                target.Text = filename;
                handler.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
