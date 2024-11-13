using CandyShop.Properties;
using CandyShop.Util;
using System.Drawing;
using System.Windows.Forms;

namespace CandyShop.Controls.Factory
{
    internal abstract class AbstractControlsFactory
    {
        abstract public PackageListBoxColumn[] GetUpgradeColumns();
        abstract public PackageListBoxColumn[] GetInstalledColumns();
        abstract public SearchBar GetSearchBar();
        abstract public ToolStrip GetUpgradePageToolBar();
        abstract public MenuStrip GetMenuStrip();

        public ToolStripMenuItem ResolveMenuItem(MenuStrip menu, string level1, string level2)
        {
            ToolStripMenuItem parent = (ToolStripMenuItem)menu.Items[level1];
            return (ToolStripMenuItem)parent.DropDownItems[level2];
        }

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
                GripStyle = ToolStripGripStyle.Hidden,
                RenderMode = ToolStripRenderMode.ManagerRenderMode,
                Renderer = new CandyShopTsRenderer()

            };
            ts.Items.Add(tsRefresh);
            ts.Items.Add(new ToolStripSeparator());
            ts.Items.Add(new ToolStripLabel("Select:"));
            ts.Items.Add(tsSelectAll);
            ts.Items.Add(tsDeselect);

            return ts;
        }

        protected MenuStrip GetCommonMenuStrip()
        {
            ToolStripMenuItem edit = new()
            {
                Name = "Edit",
                Text = "&Edit",
            };
            edit.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem() {
                    Name = "Refresh",
                    Text = LocaleEN.TEXT_TS_REFRESH,
                    ShortcutKeys = Keys.F5
                },
                new ToolStripMenuItem() {
                    Name = "RefreshInstalled",
                    Text = LocaleEN.TEXT_TS_REFRESH_INSTALLED,
                    ShortcutKeys = Keys.Control | Keys.F5,
                    Visible = false
                },
                new ToolStripSeparator(),
                new ToolStripMenuItem()
                {
                    Name = "SelectAll",
                    Text = LocaleEN.TEXT_TS_SELECT_ALL,
                    ShortcutKeys = Keys.Control | Keys.A
                },
                new ToolStripMenuItem()
                {
                    Name = "SelectTop",
                    Text = LocaleEN.TEXT_TS_SELECT_SMART
                },
                new ToolStripMenuItem()
                {
                    Name = "DeselectAll",
                    Text = LocaleEN.TEXT_TS_DESELECT
                },
            });

            ToolStripMenuItem extras = new()
            {
                Name = "Extras",
                Text = "&Extras",
            };
            extras.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem() {
                    Name = "SwitchMode",
                    Text = string.Format(LocaleEN.TEXT_MENU_SWITCH, "MODE")
                },
                new ToolStripSeparator(),
                new ToolStripMenuItem() {
                    Name = "StartWithSystem",
                    Text = "&Display notification for outdated packages on login"
                },
                new ToolStripMenuItem()
                {
                    Name = "Settings",
                    Text = "Open &Settings directory",
                    ShortcutKeys = Keys.Control | Keys.P
                },
                new ToolStripSeparator(),
                new ToolStripMenuItem()
                {
                    Name = "Logs",
                    Text = "Open package manager &logs",
                    ShortcutKeys = Keys.Control | Keys.L
                }
            });

            ToolStripMenuItem help = new()
            {
                Name = "Help",
                Text = "&Help",
            };
            help.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem() {
                    Name = "Github",
                    Text = "&Github"
                },
                new ToolStripMenuItem()
                {
                    Name = "License",
                    Text = "&License"
                },
                new ToolStripMenuItem()
                {
                    Name = "Meta",
                    Text = "Chocolatey &Meta packages"
                }
            });

            MenuStrip menu = new() { BackColor = Color.White };
            menu.Items.AddRange(new ToolStripItem[]
            {
                edit, extras, help
            });

            return menu;
        }

        // TODO context menu
    }
}
