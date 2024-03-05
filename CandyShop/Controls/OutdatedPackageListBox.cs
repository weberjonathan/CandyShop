using CandyShop.Properties;
using System;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    internal class OutdatedPackageListBox : AbstractLoadingControl<ListView>
    {
        public OutdatedPackageListBox()
        {
            Other = new ListView()
            {
                Name = "ListView",
                CheckBoxes = true,
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
                new(LocaleEN.TEXT_COL_CURRENT) { Text = LocaleEN.TEXT_COL_CURRENT },
                new(LocaleEN.TEXT_COL_AVAILABLE) { Text = LocaleEN.TEXT_COL_AVAILABLE },
                new(LocaleEN.TEXT_COL_PINNED) { Text = LocaleEN.TEXT_COL_PINNED }
            });

            Resize += new EventHandler((sender, e) =>
            {
                const int pinnedWidth = 60;
                int availWidth = Width - pinnedWidth - Margin.Left - Margin.Right - SystemInformation.VerticalScrollBarWidth;

                Other.Columns[0].Width = (int)Math.Floor(availWidth * .4);
                Other.Columns[1].Width = (int)Math.Floor(availWidth * .3);
                Other.Columns[2].Width = (int)Math.Floor(availWidth * .3);
                Other.Columns[3].Width = pinnedWidth;
            });

            SpinnerCtl.Text = LocaleEN.TEXT_LOADING_OUTDATED;
        }
    }
}
