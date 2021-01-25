using CandyShop.Chocolatey;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace CandyShop
{
    public partial class UpgradePage : UserControl
    {
        private List<ChocolateyPackage> _OutdatedPackages = new List<ChocolateyPackage>();

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

        public bool ShowAdminWarning {
            get {
                return PanelTop.Visible;
            }
            set {
                PanelTop.Visible = value;
            }
        }

        public List<ChocolateyPackage> OutdatedPackages {
            get => _OutdatedPackages;
            set {
                _OutdatedPackages.AddRange(value);

                LblLoading.Visible = false;

                if (value.Count > 0)
                {
                    BtnUpgradeSelected.Enabled = true;
                    BtnUpgradeAll.Enabled = true;

                    foreach (ChocolateyPackage pckg in value)
                    {
                        ListViewItem item = new ListViewItem(new string[]
                        {
                        pckg.Name,
                        pckg.CurrVer,
                        pckg.AvailVer,
                        pckg.Pinned.ToString()
                        });

                        item.Checked = true;
                        LstPackages.Items.Add(item);
                    }
                }
            }
        }

        public List<ChocolateyPackage> SelectedPackages {
            get {
                List<ChocolateyPackage> rtn = new List<ChocolateyPackage>();
                foreach (ListViewItem item in LstPackages.CheckedItems)
                {
                    rtn.Add(_OutdatedPackages.Find(pckg => pckg.Name.Equals(item.Text)));
                }
                return rtn;
            }
        }

        private void LstPackages_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            BtnUpgradeSelected.Text = $"Upgrade ({LstPackages.CheckedItems.Count})";
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
