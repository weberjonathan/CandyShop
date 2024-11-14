using CandyShop.Properties;
using System;
using System.Windows.Forms;

namespace CandyShop.View
{
    public partial class FirstStartForm : Form
    {
        public bool RequireAdmin => checkRequireAdmin.Checked;
        public bool CacheAdmin => checkCacheAdmin.Checked;
        public bool WingetMode => cmbSource.SelectedIndex == 0;

        private bool cacheConfirmation = false;

        public FirstStartForm()
        {
            InitializeComponent();

            checkRequireAdmin.Text = LocaleEN.TEXT_WIZARD_REQUIRE_ADMIN;
            checkCacheAdmin.Text = LocaleEN.TEXT_WIZARD_ENABLE_CACHE;
            lblHint.Text = LocaleEN.TEXT_WIZARD_CHANGE_HINT;
            lblSecurity1.Text = LocaleEN.TEXT_WIZARD_SECURITY1;
            lblSecurity2.Text = LocaleEN.TEXT_WIZARD_SECURITY2;
            lblChooseSource.Text = LocaleEN.TEXT_WIZARD_CHOOSE_SOURCE;
            btnContinue.Text = LocaleEN.TEXT_WIZARD_CONTINUE;

            btnContinue.DialogResult = DialogResult.OK;
            cmbSource.SelectedIndex = 0;
            checkCacheAdmin.CheckedChanged += new EventHandler((sender, e) =>
            {
                if (!cacheConfirmation)
                {
                    var result =
                    MessageBox.Show(LocaleEN.TEXT_WIZARD_CONFIRM_RISKS, Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.OK)
                    {
                        cacheConfirmation = true;
                    }
                    else
                    {
                        checkCacheAdmin.Checked = false;
                    }
                }
            });
        }
    }
}
