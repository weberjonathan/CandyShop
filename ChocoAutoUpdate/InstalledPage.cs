using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ChocoAutoUpdate
{
    public partial class InstalledPage : UserControl
    {
        private List<ChocolateyPackage> _Packages = new List<ChocolateyPackage>();

        public InstalledPage()
        {
            InitializeComponent();

            LstPackages.Resize += new EventHandler((sender, e) =>
            {
                int availWidth = LstPackages.Width - LstPackages.Margin.Left - LstPackages.Margin.Right - SystemInformation.VerticalScrollBarWidth;

                LstPackages.Columns[0].Width = (int)Math.Floor(availWidth * .6);
                LstPackages.Columns[1].Width = (int)Math.Floor(availWidth * .4);
            });
        }

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
    }
}
