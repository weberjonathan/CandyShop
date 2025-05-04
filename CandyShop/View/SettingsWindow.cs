using System;
using System.Security;
using System.Windows.Forms;

namespace CandyShop.View
{
    public partial class SettingsWindow : Form
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
        
        public bool RequireAdminPrivileges
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

        public bool EnableGsudoConfig
        {
            get { return checkRequireAdmin.Enabled; }
            set
            {
                if (!value)
                    checkRequireAdmin.Checked = false;
                checkRequireAdmin.Enabled = value;
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

        public event EventHandler ApplyClicked;
        public event EventHandler OkClicked;
        public event EventHandler WingetBinaryChanged;
        public event EventHandler ChocolateyBinaryChanged;
        public event EventHandler GSudoBinaryChanged;

        public SettingsWindow()
        {
            InitializeComponent();

            EnableGsudoConfig = false;
            Text = $"{MetaInfo.Name} | Settings";

            btnApply.Click += new EventHandler((sender, e) => ApplyClicked?.Invoke(sender, e));
            btnOk.Click += new EventHandler((sender, e) => OkClicked?.Invoke(sender, e));

            btnWingetBinary.Click += new EventHandler((sender, e)
                => UpdateBinaryTextbox(txtWingetBinary, WingetBinaryChanged, "Winget"));
            btnChocoBinary.Click += new EventHandler((sender, e)
                => UpdateBinaryTextbox(txtChocoBinary, ChocolateyBinaryChanged, "Chocolatey"));
            btnGSudoBinary.Click += new EventHandler((sender, e)
                => UpdateBinaryTextbox(txtGSudoBinary, GSudoBinaryChanged, "Gsudo"));

            txtWingetBinary.TextChanged += new EventHandler((sender, e) => WingetBinaryChanged?.Invoke(sender, e));
            txtChocoBinary.TextChanged += new EventHandler((sender, e) => ChocolateyBinaryChanged?.Invoke(sender, e));
            txtGSudoBinary.TextChanged += new EventHandler((sender, e) => GSudoBinaryChanged?.Invoke(sender, e));

            checkRequireAdmin.CheckedChanged += new EventHandler((sender, e) =>
            {
                if (!checkRequireAdmin.Checked)
                    checkCacheAdmin.Checked = false;

                checkCacheAdmin.Enabled = checkRequireAdmin.Checked;
            });
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
