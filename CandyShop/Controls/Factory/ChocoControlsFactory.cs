using CandyShop.Properties;
using System.Windows.Forms;

namespace CandyShop.Controls.Factory
{
    internal class ChocoControlsFactory : IControlsFactory
    {
        public PackageListBoxColumn[] GetUpgradeColumns()
        {
            return [
                new(ColumnType.Pinned),
                new(null,     LocaleEN.TEXT_COL_NAME,      .4f, PackageListBoxSize.Percent),
                new(null,     LocaleEN.TEXT_COL_CURRENT,   .3f, PackageListBoxSize.Percent),
                new(null,     LocaleEN.TEXT_COL_AVAILABLE, .3f, PackageListBoxSize.Percent),
            ];
        }

        public PackageListBoxColumn[] GetInstalledColumns()
        {
            return [
                new(null, LocaleEN.TEXT_COL_NAME,    .6f, PackageListBoxSize.Percent),
                new(null, LocaleEN.TEXT_COL_VERSION, .4f, PackageListBoxSize.Percent)
            ];
        }

        public SearchBar GetSearchBar()
        {
            return new SearchBar()
            {
                CheckboxText = LocaleEN.TEXT_SEARCH_TOP_LEVEL
            };
        }

        public ToolStrip GetUpgradePageToolBar()
        {
            var ts = CommonBase.GetCommonUpgradePageToolBar();
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

        public CandyShopMenuStrip GetMenuStrip()
        {
            var menu = CommonBase.GetCommonMenuStrip();
            menu.ItemAt("Extras", "Logs").Text = string.Format(LocaleEN.TEXT_MENU_LOGS, "Chocolatey");
            menu.ItemAt("Extras", "SwitchMode").Text = string.Format(LocaleEN.TEXT_MENU_SWITCH, "Winget");
            return menu;
        }
    }
}
