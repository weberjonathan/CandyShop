using CandyShop.Properties;
using System;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    // TODO this could be unified with OutdatedPackageListBox into a common control

    internal class InstalledPackageListBox : AbstractLoadingControl<ListView>
    {
        public InstalledPackageListBox()
        {
            Other = new ListView()
            {
                Name = "ListView2",
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                MultiSelect = false,
                ShowItemToolTips = true,
                UseCompatibleStateImageBehavior = false,
                View = System.Windows.Forms.View.Details
            };

            Other.Columns.AddRange(new ColumnHeader[] {
                new(LocaleEN.TEXT_COL_NAME) { Text = LocaleEN.TEXT_COL_NAME },
                new(LocaleEN.TEXT_COL_VERSION) { Text = LocaleEN.TEXT_COL_VERSION }
            });

            Resize += new EventHandler((sender, e) =>
            {
                int availWidth = Width - Margin.Left - Margin.Right - SystemInformation.VerticalScrollBarWidth;
                Other.Columns[0].Width = (int)Math.Floor(availWidth * .6);
                Other.Columns[1].Width = (int)Math.Floor(availWidth * .4);
            });

            SpinnerCtl.Text = LocaleEN.TEXT_LOADING_INSTALLED;
        }
    }
}
