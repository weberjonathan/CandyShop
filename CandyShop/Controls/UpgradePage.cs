using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    public partial class UpgradePage : UserControl
    {
        public UpgradePage()
        {
            InitializeComponent();

            LstPackages.ItemChecked += LstPackages_ItemChecked;
            LstPackages.Resize += LstPackages_Resize;
            BtnUpgradeAll.Click += new EventHandler((sender, e) => { UpgradeAllClick?.Invoke(this, e); });
            BtnUpgradeSelected.Click += new EventHandler((sender, e) => { UpgradeSelectedClick?.Invoke(this, e); });
            BtnCancel.Click += new EventHandler((sender, e) => { CancelClick?.Invoke(this, e); });
        }

        public event EventHandler UpgradeAllClick;

        public event EventHandler UpgradeSelectedClick;

        public event EventHandler CancelClick;

        public string[] ItemNames
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

        public bool ShowAdminWarning {
            get {
                return PanelTop.Visible;
            }
            set {
                PanelTop.Visible = value;
            }
        }

        public string[] SelectedItemNames {
            get {
                string[] rtn = new string[LstPackages.CheckedItems.Count];
                for (int i = 0; i < rtn.Length; i++)
                {
                    rtn[i] = LstPackages.CheckedItems[i].Text;
                }

                return rtn;
            }
        }

        public void UpdatePackageView(string[][] items)
        {
            LblLoading.Visible = false;

            if (items.Length > 0)
            {
                BtnUpgradeSelected.Enabled = true;
                BtnUpgradeAll.Enabled = true;

                ListViewItem[] listViewItems = items
                    .Select(item => new ListViewItem(item)
                    {
                        Name = item[0]
                    })
                    .ToArray();
                LstPackages.Items.AddRange(listViewItems);
            }
        }

        public void CheckAllItems()
        {
            foreach (ListViewItem item in LstPackages.Items)
            {
                item.Checked = true;
            }
        }

        public void CheckItemsByName(List<string> items)
        {
            foreach (string key in items)
            {
                if (LstPackages.Items.ContainsKey(key))
                {
                    LstPackages.Items[key].Checked = true;
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
            BtnUpgradeSelected.Text = String.Format(Properties.Strings.Btn_UpgradeSelected, LstPackages.CheckedItems.Count);
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
    }
}
