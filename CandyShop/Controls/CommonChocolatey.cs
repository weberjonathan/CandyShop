using CandyShop.Properties;
using System;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    internal class CommonChocolatey : AbstractCommon
    {
        public override PackageListBoxColumn[] GetUpgradeColumns()
        {
            return [
                new(null,     LocaleEN.TEXT_COL_NAME,      .4f, PackageListBoxSize.Percent),
                new(null,     LocaleEN.TEXT_COL_CURRENT,   .3f, PackageListBoxSize.Percent),
                new(null,     LocaleEN.TEXT_COL_AVAILABLE, .3f, PackageListBoxSize.Percent),
                new("Pinned", LocaleEN.TEXT_COL_PINNED,    60f, PackageListBoxSize.Fixed)
            ];
        }

        public override PackageListBoxColumn[] GetInstalledColumns()
        {
            return [
                new(null, LocaleEN.TEXT_COL_NAME,    .6f, PackageListBoxSize.Percent),
                new(null, LocaleEN.TEXT_COL_VERSION, .4f, PackageListBoxSize.Percent)
            ];
        }

        public override CommonSearchBar GetSearchBar()
        {
            return new ChocolateySearchBar();
        }

        public override ToolStrip GetUpgradePageToolBar()
        {
            var ts = GetCommonUpgradePageToolBar();
            var index = ts.Items.IndexOfKey("Select") + 1;

            var tsSelectSmart = new ToolStripButton
            {
                Name = "SelectTopLevel",
                Text = LocaleEN.TEXT_TS_SELECT_SMART,
                Image = Resources.ic_check_smart,
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };

            ts.Items.Insert(index, tsSelectSmart);
            return ts;
        }

        public override MenuStrip GetMenuStrip()
        {
            var menu = GetCommonMenuStrip();
            ResolveMenuItem(menu, "Extras", "Logs").Text = string.Format(LocaleEN.TEXT_MENU_LOGS, "Chocolatey");
            return menu;
        }
    }
}
