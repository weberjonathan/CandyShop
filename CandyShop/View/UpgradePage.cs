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
                itemPin.Click += new EventHandler((sender, e) =>
                {
                    if (LstPackages.SelectedItems.Count > 0)
                    {
                        var name = LstPackages.SelectedItems[0].Text;
                        PinnedChanged?.Invoke(this, new PinnedChangedArgs() { Name = name });
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
        }

        public event EventHandler<PinnedChangedArgs> PinnedChanged;
        public event EventHandler UpgradeAllClick;
        public event EventHandler UpgradeSelectedClick;
        public event EventHandler CancelClick;
        public event EventHandler CleanShortcutsChanged;
        public event EventHandler CloseAfterUpgradeChanged;
        public event EventHandler CheckTopLevelClicked;
        public event EventHandler AlwaysHideAdminWarningClicked;

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

        public string[] SelectedItems
        {
            get
            {
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
            ApplyPinnedStyle(item, Boolean.Parse(data[3]));

            if (Loading) Loading = false;
        }

        public void CheckAllItems()
        {
            foreach (ListViewItem item in LstPackages.Items)
            {
                item.Checked = true;
            }
        }

        public void UncheckAllItems()
        {
            foreach (ListViewItem item in LstPackages.Items)
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
            ListViewItem item = LstPackages.FindItemWithText(name);
            if (item != null)
            {
                item.Checked = state;
            }
        }

        public void SetPinned(string name, bool pinned)
        {
            var item = LstPackages.FindItemWithText(name);
            item.SubItems[3].Text = pinned.ToString();
            ApplyPinnedStyle(item, pinned);
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
