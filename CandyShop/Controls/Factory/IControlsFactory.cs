using CandyShop.PackageCore;
using System.Windows.Forms;

namespace CandyShop.Controls.Factory
{
    interface IControlsFactory
    {
        PackageListBoxColumn[] GetUpgradeColumns();
        PackageListBoxColumn[] GetInstalledColumns();
        object[] BuildUpgradeItem(GenericPackage package);
        object[] BuildInstalledItem(GenericPackage package);
        SearchBar GetSearchBar();
        ToolStrip GetUpgradePageToolBar();
        CandyShopMenuStrip GetMenuStrip();
    }
}
