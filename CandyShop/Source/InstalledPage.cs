using CandyShop.Chocolatey;
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
                    if (!(CheckHideMeta.Checked && pckg.IsMetaPackage))
                    {
                        LstPackages.Items.Add(PackageToListView(pckg));
                    }
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

        private void CheckHideMeta_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < _Packages.Count; i++)
            {
                if (_Packages[i].IsMetaPackage)
                {
                    if (CheckHideMeta.Checked)
                    {
                        LstPackages.Items[_Packages[i].Name].Remove();
                    }
                    else
                    {
                        LstPackages.Items.Insert(i, PackageToListView(_Packages[i]));
                    }
                }
            }
        }

        private ListViewItem PackageToListView(ChocolateyPackage pckg)
        {
            ListViewItem rtn = new ListViewItem(new string[]
            {
                pckg.Name,
                pckg.CurrVer
            });

            rtn.Name = pckg.Name;
            return rtn;
        }
    }
}
