using CandyShop.Properties;
using System;

namespace CandyShop.Controls
{
    internal class CommonChocolatey : ICommon
    {
        public PackageListBoxColumn[] GetUpgradeColumns()
        {
            return [
                new(LocaleEN.TEXT_COL_NAME, .4f, PackageListBoxSize.Percent),
                new(LocaleEN.TEXT_COL_CURRENT, .3f, PackageListBoxSize.Percent),
                new(LocaleEN.TEXT_COL_AVAILABLE, .3f, PackageListBoxSize.Percent),
                new(LocaleEN.TEXT_COL_PINNED, 60f, PackageListBoxSize.Fixed)
            ];
        }

        public PackageListBoxColumn[] GetInstalledColumns()
        {
            return [
                new(LocaleEN.TEXT_COL_NAME, .6f, PackageListBoxSize.Percent),
                new(LocaleEN.TEXT_COL_VERSION, .4f, PackageListBoxSize.Percent)
            ];
        }

        public CommonSearchBar GetSearchBar()
        {
            return new ChocolateySearchBar();
        }
    }
}
