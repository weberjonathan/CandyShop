using CandyShop.Controls;
using CandyShop.Controls.Factory;
using CandyShop.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CandyShop.View
{
    partial class UpgradePage : UserControl, ITabPage, IPinSupport, IPackageViewer
    {
        public UpgradePage()
        {
            InitializeComponent();

            // configure PackageListBox
            LstPackages.Hint = LocaleEN.TEXT_LOADING_OUTDATED;

            // labels
            BtnUpgradeSelected.Text = LocaleEN.TEXT_UPGRADE_SELECTED;
            BtnUpgradeAll.Text = LocaleEN.TEXT_UPGRADE_ALL;
            LblSelected.Text = string.Empty;

            // event handlers
            LstPackages.ItemChecked += LstPackages_ItemChecked;
            LstPackages.PinChangeRequest += new EventHandler<PinnedChangedArgs>((sender, e) => PinnedChanged?.Invoke(this, e));
            BtnUpgradeAll.Click += new EventHandler((sender, e) => { UpgradeAllClick?.Invoke(this, e); });
            BtnUpgradeSelected.Click += new EventHandler((sender, e) => { UpgradeSelectedClick?.Invoke(this, e); });
        }

        public event EventHandler<PinnedChangedArgs> PinnedChanged;
        public event EventHandler UpgradeAllClick;
        public event EventHandler UpgradeSelectedClick;
        public event EventHandler CleanShortcutsChanged;
        public event EventHandler CloseAfterUpgradeChanged;
        public event EventHandler CheckTopLevelClicked;
        public event EventHandler RefreshClicked;
        public event EventHandler PackagesAdded;

        public string[] Items
        {
            get
            {
                List<string> items = [];
                foreach (DataGridViewRow row in LstPackages.Other.Rows)
                {
                    items.Add((string)row.Cells[LstPackages.NameCol.Index].Value);
                }

                return items.ToArray();
            }
        }

        public string[] CheckedItems
        {
            get
            {
                List<string> checkedItems = [];
                foreach (DataGridViewRow row in LstPackages.Other.Rows)
                {
                    DataGridViewCell checkedCell = row.Cells[LstPackages.CheckedCol.Index];
                    if ((bool)checkedCell.Value) checkedItems.Add((string)row.Cells[LstPackages.NameCol.Index].Value);
                }

                return checkedItems.ToArray();
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

        public void BuildControls(IControlsFactory provider)
        {
            LstPackages.ColumnHeaders = provider.GetUpgradeColumns();
            LstPackages.CheckBoxes = true;

            var ts = provider.GetUpgradePageToolBar();
            ts.Items["Refresh"].Click += new EventHandler((sender, e) => RefreshClicked?.Invoke(sender, e));
            ts.Items["Select"].Click += new EventHandler((sender, e) => CheckAllItems());
            ts.Items["Deselect"].Click += new EventHandler((sender, e) => UncheckAllItems());
            ts.Items["Pin"].Click += new EventHandler((sender, e) =>
            {
                var packageName = LstPackages.SelectedName;
                if (packageName != null)
                {
                    PinnedChanged?.Invoke(this, new PinnedChangedArgs() { Name = packageName });
                }
            });

            var selectTopLevel = ts.Items["SelectTopLevel"];
            if (selectTopLevel != null)
                selectTopLevel.Click += new EventHandler((sender, e) => CheckTopLevelItems());

            Controls.Add(ts);
        }

        public void AddPackages(List<object[]> data)
        {
            if (Loading) Loading = false;

            if (data.Count == 0)
                DisplayEmpty();
            else
                data.ForEach(LstPackages.AddItem);

            PackagesAdded?.Invoke(this, EventArgs.Empty);
        }

        public void ClearPackages()
        {
            LstPackages.Other.Rows.Clear();
        }

        public void CheckAllItems()
        {
            var rows = LstPackages.Other.Rows;
            foreach (DataGridViewRow row in rows)
            {
                row.Cells[LstPackages.CheckedCol.Index].Value = true;
            }
        }

        public void UncheckAllItems()
        {
            var rows = LstPackages.Other.Rows;
            foreach (DataGridViewRow row in rows)
            {
                row.Cells[LstPackages.CheckedCol.Index].Value = false;
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
                var cellName = (string)row.Cells[LstPackages.NameCol.Index].Value;
                if (name.Equals(cellName))
                {
                    row.Cells[LstPackages.CheckedCol.Index].Value = state;
                    return;
                }
            }
        }

        public void UpdatePinnedState(string name, bool pinned)
        {
            LstPackages.SetPinned(name, pinned);
        }

        public void DisplayEmpty()
        {
            object[] item = [
                false,
                null,
                LocaleEN.TEXT_NO_OUTDATED_PCKGS,
            ];
            var index = LstPackages.Other.Rows.Add(item);
            var row = LstPackages.Other.Rows[index];
            row.DefaultCellStyle.ForeColor = SystemColors.GrayText;
            row.DefaultCellStyle.Font = new Font(LstPackages.Other.DefaultCellStyle.Font, FontStyle.Italic);
            row.Cells[LstPackages.CheckedCol.Index].Value = false;

            LstPackages.NoPackages = true;
            if (Loading) Loading = false;
        }

        private void LstPackages_ItemChecked(object sender, DataGridViewCellEventArgs e)
        {
            // update package count status text
            LblSelected.Text = string.Format(LocaleEN.TEXT_SELECTED_PACKAGE_COUNT, CheckedItems.Length);
        }

        private void CheckDeleteShortcuts_CheckedChanged(object sender, EventArgs e)
        {
            CleanShortcutsChanged?.Invoke(sender, e);
        }

        private void CheckCloseAfterUpgrade_CheckedChanged(object sender, EventArgs e)
        {
            CloseAfterUpgradeChanged?.Invoke(sender, e);
        }
    }
}
