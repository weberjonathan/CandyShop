using System.Windows.Forms;

namespace CandyShop.Controls
{
    internal interface ICommon
    {
        PackageListBoxColumn[] GetUpgradeColumns();
        PackageListBoxColumn[] GetInstalledColumns();
        Control GetSearchBar();
        
        // TODO context menu, menu items
    }
}
