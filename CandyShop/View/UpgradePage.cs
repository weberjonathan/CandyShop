using CandyShop.Controls;
using CandyShop.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CandyShop.View
{
    partial class UpgradePage : UserControl, IUpgradePageView
    {
        // TODO Checked and Name columns could be read-only properties of PackageListBox, which gives the column index as a property of the columns
        // TODO same with pinned column
        private const int COL_NAME_INDEX = 1;
        private const int COL_CHECKED_INDEX = 0;

        public UpgradePage()
        {
            InitializeComponent();
            PanelTop.Visible = false;

            // configure PackageListBox
            LstPackages.Hint = LocaleEN.TEXT_LOADING_OUTDATED;

            // labels
            BtnUpgradeSelected.Text = LocaleEN.TEXT_UPGRADE_SELECTED;
            BtnUpgradeAll.Text = LocaleEN.TEXT_UPGRADE_ALL;
            LblAdmin.Text = LocaleEN.TEXT_NO_ADMIN_HINT;
            LblSelected.Text = string.Empty;

            // event handlers
            LstPackages.ItemChecked += LstPackages_ItemChecked;
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
            var itemPin = new ToolStripMenuItem("&Pin package")
            {
                Name = "Pin",
            };
            itemPin.Click += new EventHandler((sender, e) =>
            {
                if (LstPackages.TryGetSelectedItem(out var row))
                {
                    var name = (string)row.Cells[COL_NAME_INDEX].Value;
                    PinnedChanged?.Invoke(this, new PinnedChangedArgs() { Name = name });
                }
            });

            var contextMenu = new ContextMenuStrip();
            contextMenu.Opening += new System.ComponentModel.CancelEventHandler((sender, e) =>
            {
                var index = LstPackages.IndexOfColumn("Pinned");
                if (LstPackages.TryGetSelectedItem(out var row))
                {
                    string pinnedText = (string)row.Cells[index].Value;
                    if (bool.TryParse(pinnedText, out bool pinned))
                    {
                        itemPin.Checked = pinned;
                        e.Cancel = false;
                        return;
                    }
                }
                e.Cancel = true;
            });

            contextMenu.Items.Add(itemPin);
            LstPackages.ContextMenuStrip = contextMenu;
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
                List<string> items = [];
                foreach (DataGridViewRow row in LstPackages.Other.Rows)
                {
                    // TODO make safe
                    items.Add((string)row.Cells[COL_NAME_INDEX].Value);
                }

                return items.ToArray();
            }
        }

        public string[] SelectedItems // TODO rename to CheckedItems
        {
            get
            {
                List<string> checkedItems = [];
                foreach (DataGridViewRow row in LstPackages.Other.Rows)
                {
                    // TODO make safe
                    DataGridViewCell checkedCell = row.Cells[COL_CHECKED_INDEX];
                    if ((bool)checkedCell.Value) checkedItems.Add((string)row.Cells[COL_NAME_INDEX].Value);
                }

                return checkedItems.ToArray();
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
                    if (LstPackages.ContextMenuStrip != null && AllowPinnedUacIon)
                        if (LstPackages.ContextMenuStrip.Items.ContainsKey("Pin"))
                        {
                            LstPackages.ContextMenuStrip.Items["Pin"].Image = Resources.ic_uac;
                        }
                }
                else
                {
                    BtnUpgradeSelected.Image = null;
                    BtnUpgradeAll.Image = null;
                    if (LstPackages.ContextMenuStrip != null)
                        if (LstPackages.ContextMenuStrip.Items.ContainsKey("Pin"))
                        {
                            LstPackages.ContextMenuStrip.Items["Pin"].Image = null;
                        }
                }
            }
        }

        public bool AllowPinnedUacIon { get; set; }

        public void BuildControls(AbstractCommon provider)
        {
            LstPackages.ColumnHeaders = provider.GetUpgradeColumns();
            LstPackages.CheckBoxes = true;

            var ts = provider.GetUpgradePageToolBar();
            ts.Items["Refresh"].Click += new EventHandler((sender, e) => RefreshClicked?.Invoke(sender, e));
            ts.Items["Select"].Click += new EventHandler((sender, e) => CheckAllItems());
            ts.Items["Deselect"].Click += new EventHandler((sender, e) => UncheckAllItems());
            var selectTopLevel = ts.Items["SelectTopLevel"];
            if (selectTopLevel != null)
                selectTopLevel.Click += new EventHandler((sender, e) => CheckTopLevelItems());
            Controls.Add(ts);
        }

        public void AddItem(string[] data)
        {
            var index = LstPackages.Other.Rows.Add(data);
            if (LstPackages.NoPackages) LstPackages.NoPackages = false;

            int pinnedIndex = LstPackages.IndexOfColumn("Pinned");
            if (bool.TryParse(data[pinnedIndex], out bool pinned))
            {
                ApplyPinnedStyle(LstPackages.Other.Rows[index], pinned);
            }
            else
            {
                ApplyPinnedStyle(LstPackages.Other.Rows[index], true);
            }

            if (Loading) Loading = false;
        }

        public void ClearItems()
        {
            LstPackages.Other.Rows.Clear();
        }

        public void CheckAllItems()
        {
            var rows = LstPackages.Other.Rows;
            foreach (DataGridViewRow row in rows)
            {
                row.Cells[COL_CHECKED_INDEX].Value = true;
            }
        }

        public void UncheckAllItems()
        {
            var rows = LstPackages.Other.Rows;
            foreach (DataGridViewRow row in rows)
            {
                row.Cells[COL_CHECKED_INDEX].Value = false;
            }
        }

        public void CheckTopLevelItems()
        {
            CheckTopLevelClicked?.Invoke(this, EventArgs.Empty);
        }

        public void SetItemCheckState(string name, bool state)
        {
            var rows = LstPackages.Other.Rows;
            foreach (DataGridViewRow row in rows)
            {
                var cellName = (string)row.Cells[COL_NAME_INDEX].Value;
                if (name.Equals(cellName))
                {
                    row.Cells[COL_CHECKED_INDEX].Value = state;
                    return;
                }
            }
        }

        public void SetPinned(string name, bool pinned)
        {
            var rows = LstPackages.Other.Rows;
            foreach (DataGridViewRow row in rows)
            {
                var cellName = (string)row.Cells[COL_NAME_INDEX].Value;
                if (name.Equals(cellName))
                {
                    int index = LstPackages.IndexOfColumn("Pinned");
                    row.Cells[index].Value = pinned.ToString();
                    ApplyPinnedStyle(row, pinned);
                    return;
                }
            }
        }

        public void DisplayEmpty()
        {
            string[] item = [
                "false",
                "All packages are up to date.",
                "",
                "",
                ""
            ];
            var index = LstPackages.Other.Rows.Add(item);
            ApplyPinnedStyle(LstPackages.Other.Rows[index], true);
            LstPackages.NoPackages = true;
            if (Loading) Loading = false;
        }

        private void LstPackages_ItemChecked(object sender, DataGridViewCellEventArgs e)
        {
            int index = LstPackages.IndexOfColumn("Pinned");
            string pinnedText = (string)LstPackages.Other.Rows[e.RowIndex].Cells[index].Value;

            // disallow checking for pinned packages
            if (!bool.TryParse(pinnedText, out bool pinned) || pinned)
            {
                LstPackages.Other.Rows[e.RowIndex].Cells[COL_CHECKED_INDEX].Value = false;
            }

            // update package count status text
            LblSelected.Text = string.Format(LocaleEN.TEXT_SELECTED_PACKAGE_COUNT, SelectedItems.Length);
        }

        private void CheckDeleteShortcuts_CheckedChanged(object sender, EventArgs e)
        {
            CleanShortcutsChanged?.Invoke(sender, e);
        }

        private void CheckCloseAfterUpgrade_CheckedChanged(object sender, EventArgs e)
        {
            CloseAfterUpgradeChanged?.Invoke(sender, e);
        }

        private void ApplyPinnedStyle(DataGridViewRow item, bool pinned)
        {
            if (pinned)
            {

                item.DefaultCellStyle.ForeColor = SystemColors.GrayText;
                item.DefaultCellStyle.Font = new Font(LstPackages.Other.DefaultCellStyle.Font, FontStyle.Italic);
                item.Cells[COL_CHECKED_INDEX].Value = false;
            }
            else
            {
                item.DefaultCellStyle.ForeColor = SystemColors.ControlText;
                item.DefaultCellStyle.Font = new Font(LstPackages.Other.DefaultCellStyle.Font, FontStyle.Regular);
                item.Cells[COL_CHECKED_INDEX].Value = true;
            }
        }
    }
}
