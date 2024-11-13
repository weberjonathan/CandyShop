using CandyShop.Properties;
using System.Windows.Forms;

namespace CandyShop.Controls.Factory
{
    internal class WingetControlsFactory : IControlsFactory
    {
        public PackageListBoxColumn[] GetUpgradeColumns()
        {
            return [
                new(ColumnType.Pinned),
                new(null,     LocaleEN.TEXT_COL_NAME,      .3f, PackageListBoxSize.Percent),
                new(null,     "ID",                        .3f, PackageListBoxSize.Percent),
                new(null,     LocaleEN.TEXT_COL_CURRENT,   .2f, PackageListBoxSize.Percent),
                new(null,     LocaleEN.TEXT_COL_AVAILABLE, .2f, PackageListBoxSize.Percent),
                new(null,     LocaleEN.TEXT_COL_SOURCE,    65f, PackageListBoxSize.Fixed)
            ];
        }

        public PackageListBoxColumn[] GetInstalledColumns()
        {
            return [
                new(ColumnType.Pinned),
                new(null, LocaleEN.TEXT_COL_NAME,    .4f, PackageListBoxSize.Percent),
                new(null, "ID",                      .4f, PackageListBoxSize.Percent),
                new(null, LocaleEN.TEXT_COL_VERSION, .2f, PackageListBoxSize.Percent),
                new(null, LocaleEN.TEXT_COL_SOURCE,  65f, PackageListBoxSize.Fixed)
            ];
        }

        public SearchBar GetSearchBar()
        {
            return new SearchBar()
            {
                CheckboxText = LocaleEN.TEXT_SEARCH_REQUIRE_SOURCE
            };
        }

        public ToolStrip GetUpgradePageToolBar()
        {
            return CommonBase.GetCommonUpgradePageToolBar();
        }

        public CandyShopMenuStrip GetMenuStrip()
        {
            var menu = CommonBase.GetCommonMenuStrip();
            menu.ItemAt("Extras", "Logs").Text = string.Format(LocaleEN.TEXT_MENU_LOGS, "Winget");
            menu.ItemAt("Extras", "SwitchMode").Text = string.Format(LocaleEN.TEXT_MENU_SWITCH, "Chocolatey");
            menu.ItemAt("Help", "Meta").Visible = false;
            menu.ItemAt("Edit", "SelectTop").Visible = false;
            return menu;
        }
    }
}
