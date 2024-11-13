using System.Windows.Forms;

namespace CandyShop.Controls.Factory
{
    interface IControlsFactory
    {
        PackageListBoxColumn[] GetUpgradeColumns();
        PackageListBoxColumn[] GetInstalledColumns();
        SearchBar GetSearchBar();
        ToolStrip GetUpgradePageToolBar();
        CandyShopMenuStrip GetMenuStrip();
    }
}
