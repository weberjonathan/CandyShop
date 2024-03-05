using CandyShop.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CandyShop.View
{
    partial class UpgradePage : UserControl, IUpgradePageView
    {
        public UpgradePage()
        {
            InitializeComponent();
            PanelTop.Visible = false;

            // tool bar
            var tsRefresh = new ToolStripButton
            {
                Text = LocaleEN.TEXT_TS_REFRESH,
                Image = Resources.ic_refresh,
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };
            tsRefresh.Click += new EventHandler((sender, e) => RefreshClicked?.Invoke(sender, e));

            var tsSelectAll = new ToolStripButton
            {
                Text = LocaleEN.TEXT_TS_SELECT_ALL,
                Image = Resources.ic_check_all,
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };
            tsSelectAll.Click += new EventHandler((sender, e) => CheckAllItems());

            var tsSelectSmart = new ToolStripButton
            {
                Text = LocaleEN.TEXT_TS_SELECT_SMART,
                Image = Resources.ic_check_smart,
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };
            tsSelectSmart.Click += new EventHandler((sender, e) => CheckTopLevelItems());

            var tsDeselect = new ToolStripButton
            {
                Text = LocaleEN.TEXT_TS_DESELECT,
                Image = Resources.ic_check_none,
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };
            tsDeselect.Click += new EventHandler((sender, e) => UncheckAllItems());

            ToolStrip ts = new()
            {
                BackColor = SystemColors.Window,
                GripStyle = ToolStripGripStyle.Hidden
            };
            ts.Items.Add(tsRefresh);
            ts.Items.Add(new ToolStripSeparator());
            ts.Items.Add(new ToolStripLabel("Select:"));
            ts.Items.Add(tsSelectAll);
            ts.Items.Add(tsSelectSmart);
            ts.Items.Add(tsDeselect);
            this.Controls.Add(ts);

            // labels
            BtnUpgradeSelected.Text = LocaleEN.TEXT_UPGRADE_SELECTED;
            BtnUpgradeAll.Text = LocaleEN.TEXT_UPGRADE_ALL;
            LblAdmin.Text = LocaleEN.TEXT_NO_ADMIN_HINT;
            LblSelected.Text = string.Empty;

            // event handlers
            LstPackages.Other.ItemChecked += LstPackages_ItemChecked;
            BtnUpgradeAll.Click += new EventHandler((sender, e) => { UpgradeAllClick?.Invoke(this, e); });
            BtnUpgradeSelected.Click += new EventHandler((sender, e) => { UpgradeSelectedClick?.Invoke(this, e); });
            BtnHideWarning.Click += new EventHandler((sender, e) =>
            {
                var result = MessageBox.Show("Always hide this warning?",
                    Application.ProductName,
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                switch (result)
                {
                    case DialogResult.Yes:
                        ShowAdminWarning = false;
                        AlwaysHideAdminWarningClicked?.Invoke(this, e);
                        break;
                    case DialogResult.No:
                        ShowAdminWarning = false;
                        break;
                    default:
                        break;
                }
            });

            // context menu
            if (!ContextSingleton.Get.WingetMode)
            {
                var itemPin = new ToolStripMenuItem("&Pin package");
                itemPin.Name = "Pin";
                itemPin.Click += new EventHandler((sender, e) =>
                {
                    if (LstPackages.Other.SelectedItems.Count > 0)
                    {
                        var name = LstPackages.Other.SelectedItems[0].Text;
                        PinnedChanged?.Invoke(this, new PinnedChangedArgs() { Name = name });
                    }
                });

                var contextMenu = new ContextMenuStrip();
                contextMenu.Opening += new System.ComponentModel.CancelEventHandler((sender, e) =>
                {
                    itemPin.Checked = bool.Parse(LstPackages.Other.SelectedItems[0].SubItems[3].Text);
                });

                contextMenu.Items.Add(itemPin);
                LstPackages.ContextMenuStrip = contextMenu;
            }
        }

        public event EventHandler<PinnedChangedArgs> PinnedChanged;
        public event EventHandler UpgradeAllClick;
        public event EventHandler UpgradeSelectedClick;
        public event EventHandler CleanShortcutsChanged;
        public event EventHandler CloseAfterUpgradeChanged;
        public event EventHandler CheckTopLevelClicked;
        public event EventHandler AlwaysHideAdminWarningClicked;
        public event EventHandler RefreshClicked;

        public string[] Items
        {
            get
            {
                string[] rtn = new string[LstPackages.Other.Items.Count];
                for (int i = 0; i < rtn.Length; i++)
                {
                    rtn[i] = LstPackages.Other.Items[i].Text;
                }

                return rtn;
            }
        }

        public string[] SelectedItems
        {
            get
            {
                string[] rtn = new string[LstPackages.Other.CheckedItems.Count];
                for (int i = 0; i < rtn.Length; i++)
                {
                    rtn[i] = LstPackages.Other.CheckedItems[i].Text;
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

        public bool CloseAfterUpgrade
        {
            get
            {
                return CheckCloseAfterUpgrade.Checked;
            }
            set
            {
                CheckCloseAfterUpgrade.Checked = value;
            }
        }

        public bool Loading
        {
            get
            {
                return LstPackages.Loading;
            }
            set
            {
                LstPackages.Loading = value;
                BtnUpgradeSelected.Enabled = !value;
                BtnUpgradeAll.Enabled = !value;
                LblSelected.Visible = !value;
            }
        }

        public bool ShowUacIcons
        {
            get
            {
                return BtnUpgradeSelected.Image != null;
            }
            set
            {
                if (value)
                {
                    BtnUpgradeSelected.Image = Resources.ic_uac;
                    BtnUpgradeAll.Image = Resources.ic_uac;
                    var item = LstPackages.ContextMenuStrip.Items["Pin"];
                    item.Image = Resources.ic_uac;
                }
                else
                {
                    BtnUpgradeSelected.Image = null;
                    BtnUpgradeAll.Image = null;
                    var item = LstPackages.ContextMenuStrip.Items["Pin"];
                    item.Image = null;
                }
            }
        }

        public void AddItem(string[] data)
        {
            ListViewItem item = new ListViewItem(data);
            LstPackages.Other.Items.Add(item);
            ApplyPinnedStyle(item, bool.Parse(data[3]));

            if (Loading) Loading = false;
        }

        public void ClearItems()
        {
            LstPackages.Other.Items.Clear();
        }

        public void CheckAllItems()
        {
            var items = LstPackages.Other.Items;
            foreach (ListViewItem item in items)
            {
                item.Checked = true;
            }
        }

        public void UncheckAllItems()
        {
            var items = LstPackages.Other.Items;
            foreach (ListViewItem item in items)
            {
                item.Checked = false;
            }
        }

        public void CheckTopLevelItems()
        {
            CheckTopLevelClicked?.Invoke(this, EventArgs.Empty);
        }

        public void SetItemCheckState(string name, bool state)
        {
            ListViewItem item = LstPackages.Other.FindItemWithText(name);
            if (item != null)
            {
                item.Checked = state;
            }
        }

        public void SetPinned(string name, bool pinned)
        {
            var item = LstPackages.Other.FindItemWithText(name);
            item.SubItems[3].Text = pinned.ToString();
            ApplyPinnedStyle(item, pinned);
        }

        private void LstPackages_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            bool pinned = bool.Parse(e.Item.SubItems[3].Text);
            if (pinned) e.Item.Checked = false;

            LblSelected.Text = string.Format(LocaleEN.TEXT_SELECTED_PACKAGE_COUNT, LstPackages.Other.CheckedItems.Count);
        }

        private void CheckDeleteShortcuts_CheckedChanged(object sender, EventArgs e)
        {
            CleanShortcutsChanged?.Invoke(sender, e);
        }

        private void CheckCloseAfterUpgrade_CheckedChanged(object sender, EventArgs e)
        {
            CloseAfterUpgradeChanged?.Invoke(sender, e);
        }

        private void ApplyPinnedStyle(ListViewItem item, bool pinned)
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
