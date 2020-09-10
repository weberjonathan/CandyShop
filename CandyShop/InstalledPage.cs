using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CandyShop
{
    public partial class InstalledPage : UserControl
    {
        private List<ChocolateyPackage> _Packages = new List<ChocolateyPackage>();
        private string _DetailsText;

        public InstalledPage()
        {
            InitializeComponent();
            PanelTop.Visible = false; // TODO # of packages, filter .portable .install etc.
            // SplitContainer.Panel2Collapsed = true;

            LstPackages.SelectedIndexChanged += new EventHandler((sender, e) =>
            {
                DetailsText = "Loading ...";
                SelectedPackageChanged?.Invoke(this, e);
            });

            LstPackages.Resize += new EventHandler((sender, e) =>
            {
                int availWidth = LstPackages.Width - LstPackages.Margin.Left - LstPackages.Margin.Right - SystemInformation.VerticalScrollBarWidth;

                LstPackages.Columns[0].Width = (int)Math.Floor(availWidth * .6);
                LstPackages.Columns[1].Width = (int)Math.Floor(availWidth * .4);
            });
        }

        public event EventHandler SelectedPackageChanged;

        public List<ChocolateyPackage> Packages {
            get => _Packages;
            set {
                _Packages = value;
                foreach (ChocolateyPackage pckg in value)
                {
                    LstPackages.Items.Add(new ListViewItem(new string[]
                    {
                        pckg.Name,
                        pckg.CurrVer
                    }));
                }
            }
        }

        public string DetailsText {
            get => _DetailsText;
            set {
                _DetailsText = value;
                TxtDetails.Text = value;
                // SplitContainer.Panel2Collapsed = String.Empty.Equals(value);
            }
        }

        public ChocolateyPackage SelectedPackage {
            get {
                if (LstPackages.SelectedItems.Count > 0)
                {
                    string pName = LstPackages.SelectedItems[0].Text;
                    return Packages.Find(p => p.Name.Equals(pName));
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
