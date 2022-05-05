using CandyShop.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CandyShop.View
{
    partial class UpgradePage : UserControl, IUpgradePageView
    {
        public UpgradePage()
        {
            InitializeComponent();

            PanelTop.Visible = false;

            // labels
            BtnCancel.Text = LocaleEN.TEXT_CANCEL;
            BtnUpgradeSelected.Text = LocaleEN.TEXT_UPGRADE_SELECTED;
            BtnUpgradeAll.Text = LocaleEN.TEXT_UPGRADE_ALL;
            LblAdmin.Text = LocaleEN.TEXT_NO_ADMIN_HINT;
            LblLoading.Text = LocaleEN.TEXT_LOADING;
            LblSelected.Text = String.Empty;
            LstPackages.Columns[0].Text = LocaleEN.TEXT_COL_NAME;
            LstPackages.Columns[1].Text = LocaleEN.TEXT_COL_CURRENT;
            LstPackages.Columns[2].Text = LocaleEN.TEXT_COL_AVAILABLE;
            LstPackages.Columns[3].Text = LocaleEN.TEXT_COL_PINNED;

            LstPackages.ItemChecked += LstPackages_ItemChecked;
            LstPackages.Resize += LstPackages_Resize;
            BtnUpgradeAll.Click += new EventHandler((sender, e) => { UpgradeAllClick?.Invoke(this, e); });
            BtnUpgradeSelected.Click += new EventHandler((sender, e) => { UpgradeSelectedClick?.Invoke(this, e); });
            BtnCancel.Click += new EventHandler((sender, e) => { CancelClick?.Invoke(this, e); });
        }

        public event EventHandler UpgradeAllClick;
        public event EventHandler UpgradeSelectedClick;
        public event EventHandler CancelClick;
        public event EventHandler CleanShortcutsChanged;

        public string[] Items
        {
            get
            {
                string[] rtn = new string[LstPackages.Items.Count];
                for (int i = 0; i < rtn.Length; i++)
                {
                    rtn[i] = LstPackages.Items[i].Text;
                }

                return rtn;
            }
        }
        
        public string[] SelectedItems {
            get {
                string[] rtn = new string[LstPackages.CheckedItems.Count];
                for (int i = 0; i < rtn.Length; i++)
                {
                    rtn[i] = LstPackages.CheckedItems[i].Text;
                }

                return rtn;
            }
        }

        public bool ShowAdminWarning
        {
            get
            {
                return PanelTop.Visible;
            }
            set
            {
                PanelTop.Visible = value;
            }
        }

        public bool CleanShortcuts
        {
            get
            {
                return CheckDeleteShortcuts.Checked;
            }
            set
            {
                CheckDeleteShortcuts.Checked = value;
            }
        }

        public bool Loading
        {
            get
            {
                return LblLoading.Visible;
            }
            set
            {
                LblLoading.Visible = value;
                BtnUpgradeSelected.Enabled = !value;
                BtnUpgradeAll.Enabled = !value;
            }
        }

        public void AddItem(string[] data)
        {
            ListViewItem item = new ListViewItem(data);
            LstPackages.Items.Add(item);
            
            if (Loading) Loading = false;
        }

        public void CheckAllItems()
        {
            foreach (ListViewItem item in LstPackages.Items)
            {
                item.Checked = true;
            }
        }

        public void CheckItemsByText(List<string> texts)
        {
            foreach (string text in texts)
            {
                ListViewItem item = LstPackages.FindItemWithText(text);
                if (item != null)
                {
                    item.Checked = true;
                }
            }
        }

        public void UncheckAllItems()
        {
            foreach (ListViewItem item in LstPackages.Items)
            {
                item.Checked = false;
            }
        }

        private void LstPackages_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            LblSelected.Text = String.Format(LocaleEN.TEXT_SELECTED_PACKAGE_COUNT, LstPackages.CheckedItems.Count);
        }

        private void LstPackages_Resize(object sender, EventArgs e)
        {
            const int pinnedWidth = 60;
            int availWidth = LstPackages.Width - pinnedWidth - LstPackages.Margin.Left - LstPackages.Margin.Right - SystemInformation.VerticalScrollBarWidth;

            LstPackages.Columns[0].Width = (int)Math.Floor(availWidth * .4);
            LstPackages.Columns[1].Width = (int)Math.Floor(availWidth * .3);
            LstPackages.Columns[2].Width = (int)Math.Floor(availWidth * .3);
            LstPackages.Columns[3].Width = pinnedWidth;
        }

        private void CheckDeleteShortcuts_CheckedChanged(object sender, EventArgs e)
        {
            CleanShortcutsChanged?.Invoke(sender, e);
        }
    }
}
