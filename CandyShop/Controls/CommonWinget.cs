using CandyShop.Properties;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    internal class CommonWinget: AbstractCommon
    {
        public override PackageListBoxColumn[] GetUpgradeColumns()
        {
            return [
                new(LocaleEN.TEXT_COL_NAME, .4f, PackageListBoxSize.Percent),
                new(LocaleEN.TEXT_COL_CURRENT, .3f, PackageListBoxSize.Percent),
                new(LocaleEN.TEXT_COL_AVAILABLE, .3f, PackageListBoxSize.Percent),
                new(LocaleEN.TEXT_COL_PINNED, 60f, PackageListBoxSize.Fixed),
                new(LocaleEN.TEXT_COL_SOURCE, 65f, PackageListBoxSize.Fixed)
            ];
        }

        public override PackageListBoxColumn[] GetInstalledColumns()
        {
            return [
                new(LocaleEN.TEXT_COL_NAME, .4f, PackageListBoxSize.Percent),
                new(LocaleEN.TEXT_COL_VERSION, .3f, PackageListBoxSize.Percent),
                new(LocaleEN.TEXT_COL_SOURCE, .3f, PackageListBoxSize.Percent)
            ];
        }

        public override CommonSearchBar GetSearchBar()
        {
            return new WingetSearchBar();
        }

        public override ToolStrip GetUpgradePageToolBar()
        {
            return GetCommonUpgradePageToolBar();
        }
    }
}
