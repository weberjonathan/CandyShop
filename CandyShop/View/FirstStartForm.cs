using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            btnContinue.DialogResult = DialogResult.OK;
            cmbSource.SelectedIndex = 0;
            checkCacheAdmin.CheckedChanged += new EventHandler((sender, e) =>
            {
                if (!cacheConfirmation)
                {
                    var result =
                    MessageBox.Show("I know what I'm doing.", Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
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
