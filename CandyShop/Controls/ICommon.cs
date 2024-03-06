namespace CandyShop.Controls
{
    internal interface ICommon
    {
        PackageListBoxColumn[] GetUpgradeColumns();
        PackageListBoxColumn[] GetInstalledColumns();
        CommonSearchBar GetSearchBar();

        // TODO context menu, menu items
    }
}
