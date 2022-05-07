using CandyShop.Controller;
using CandyShop.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CandyShop.View
{
    partial class UpgradePage : UserControl, IUpgradePageView
    {
        private IUpgradePageController Controller;

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

            // event handlers
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

        public void InjectController(IUpgradePageController controller)
        {
            Controller = controller;

            // context menu
            var itemPin = new ToolStripMenuItem("&Pin package");
            itemPin.Click += new EventHandler((sender, e) =>
            {
                if (LstPackages.SelectedItems.Count > 0)
                {
                    var name = LstPackages.SelectedItems[0].Text;
                    Controller.TogglePin(name, pinned =>
                    {
                        var item = LstPackages.FindItemWithText(name);
                        item.SubItems[3].Text = pinned.ToString();
                        SetItemStyle(item, pinned);
                    });
                }
            });

            var contextMenu = new ContextMenuStrip();
            contextMenu.Opening += new System.ComponentModel.CancelEventHandler((sender, e) =>
            {
                itemPin.Checked = Boolean.Parse(LstPackages.SelectedItems[0].SubItems[3].Text);
            });

            contextMenu.Items.Add(itemPin);
            LstPackages.ContextMenuStrip = contextMenu;
        }

        public void AddItem(string[] data)
        {
            ListViewItem item = new ListViewItem(data);
            LstPackages.Items.Add(item);
            SetItemStyle(item, Boolean.Parse(data[3]));

            if (Loading) Loading = false;
        }

        public void CheckAllItems()
        {
            foreach (ListViewItem item in LstPackages.Items)
            {
                // TODO dont check pinned items
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

        public void SetPinned(string name, bool pinned)
        {
            var item = LstPackages.FindItemWithText(name);
            item.SubItems[3].Text = pinned.ToString();
            SetItemStyle(item, pinned);
        }

        private void LstPackages_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            bool pinned = Boolean.Parse(e.Item.SubItems[3].Text);
            if (pinned) e.Item.Checked = false;
            
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

        private void SetItemStyle(ListViewItem item, bool pinned)
        {
            if (pinned)
            {
                item.ForeColor = SystemColors.GrayText;
                item.Font = new Font(item.Font, FontStyle.Italic);
                item.Checked = false;
            }
            else
            {
                item.ForeColor = SystemColors.ControlText;
                item.Font = new Font(item.Font, FontStyle.Regular);
                item.Checked = true;
            }
        }
    }
}
