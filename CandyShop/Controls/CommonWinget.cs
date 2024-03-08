﻿using CandyShop.Properties;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    internal class CommonWinget : AbstractCommon
    {
        public override PackageListBoxColumn[] GetUpgradeColumns()
        {
            return [
                new(null,     LocaleEN.TEXT_COL_NAME,      .3f, PackageListBoxSize.Percent),
                new(null,     "ID",                        .3f, PackageListBoxSize.Percent),
                new(null,     LocaleEN.TEXT_COL_CURRENT,   .2f, PackageListBoxSize.Percent),
                new(null,     LocaleEN.TEXT_COL_AVAILABLE, .2f, PackageListBoxSize.Percent),
                new("Pinned", LocaleEN.TEXT_COL_PINNED,    60f, PackageListBoxSize.Fixed),
                new(null,     LocaleEN.TEXT_COL_SOURCE,    65f, PackageListBoxSize.Fixed)
            ];
        }

        public override PackageListBoxColumn[] GetInstalledColumns()
        {
            return [
                new(null, LocaleEN.TEXT_COL_NAME,    .4f, PackageListBoxSize.Percent),
                new(null, LocaleEN.TEXT_COL_VERSION, .3f, PackageListBoxSize.Percent),
                new(null, LocaleEN.TEXT_COL_SOURCE,  .3f, PackageListBoxSize.Percent)
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

        public override MenuStrip GetMenuStrip()
        {
            var menu = GetCommonMenuStrip();
            ResolveMenuItem(menu, "Extras", "Logs").Text = string.Format(LocaleEN.TEXT_MENU_LOGS, "Winget");
            ResolveMenuItem(menu, "Help", "Meta").Visible = false;
            ResolveMenuItem(menu, "Edit", "SelectTop").Visible = false;
            return menu;
        }
    }
}
