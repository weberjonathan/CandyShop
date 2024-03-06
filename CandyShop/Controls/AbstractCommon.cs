using CandyShop.Properties;
using System.Drawing;
using System;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    internal abstract class AbstractCommon
    {
        abstract public PackageListBoxColumn[] GetUpgradeColumns();
        abstract public PackageListBoxColumn[] GetInstalledColumns();
        abstract public CommonSearchBar GetSearchBar();
        abstract public ToolStrip GetUpgradePageToolBar();
        abstract public string GetLogsMenuItemText();

        protected ToolStrip GetCommonUpgradePageToolBar()
        {
            var tsRefresh = new ToolStripButton
            {
                Name = "Refresh",
                Text = LocaleEN.TEXT_TS_REFRESH,
                Image = Resources.ic_refresh,
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };

            var tsSelectAll = new ToolStripButton
            {
                Name = "Select",
                Text = LocaleEN.TEXT_TS_SELECT_ALL,
                Image = Resources.ic_check_all,
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };

            var tsDeselect = new ToolStripButton
            {
                Name = "Deselect",
                Text = LocaleEN.TEXT_TS_DESELECT,
                Image = Resources.ic_check_none,
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };

            ToolStrip ts = new()
            {
                BackColor = SystemColors.Window,
                GripStyle = ToolStripGripStyle.Hidden
            };
            ts.Items.Add(tsRefresh);
            ts.Items.Add(new ToolStripSeparator());
            ts.Items.Add(new ToolStripLabel("Select:"));
            ts.Items.Add(tsSelectAll);
            ts.Items.Add(tsDeselect);

            return ts;
        }

        // TODO context menu, menu items
    }
}
