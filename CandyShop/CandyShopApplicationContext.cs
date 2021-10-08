using CandyShop.Chocolatey;
using CandyShop.Presentation;
using CandyShop.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CandyShop
{
    public class CandyShopApplicationContext : ApplicationContext
    {
        public CandyShopApplicationContext()
        {
            // determine silent mode
            List<string> args = new List<string>(Environment.GetCommandLineArgs());
            bool launchMinimized = args.Find(s => s.Equals("--background") ||
                                                s.Equals("-b")) != null;

            // launch with form or in tray
            if (launchMinimized)
            {
                // creates a tray icon, displays a notification if outdated packages
                // are found and opens the upgrade UI on click
                RunInBackground();
            }
            else
            {
                new CandyShopForm().Show();
            }
        }

        private async void RunInBackground()
        {
            List<ChocolateyPackage> packages = null;

            // create tray icon
            NotifyIcon icon = InitTrayIcon();

            // obtain outdated packages
            try
            {
                packages = await ChocolateyWrapper.CheckOutdatedAsync();
            }
            catch (ChocolateyException)
            {
                icon.BalloonTipIcon = ToolTipIcon.Error;
                icon.Text = Application.ProductName;
                icon.BalloonTipTitle = $"Candy Shop";
                icon.BalloonTipText = Properties.strings.Err_CheckOutdated;
                icon.ShowBalloonTip(2000);
                Environment.Exit(0);
            }

            // create click handlers
            icon.BalloonTipClicked += new EventHandler((sender, e) =>
            {
                new CandyShopForm(packages).Show();
            });

            icon.MouseClick += new MouseEventHandler((sender, e) =>
            {
                if (e.Button.Equals(MouseButtons.Left))
                {
                    new CandyShopForm(packages).Show();
                }
            });


            if (packages.Count > 0)
            {
                CandyShopForm form = new CandyShopForm(packages);
                ShowNotification(packages.Count, icon);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private NotifyIcon InitTrayIcon()
        {
            // create context menu
            ToolStripItem exitItem = new ToolStripMenuItem
            {
                Text = "Exit",
            };
            exitItem.Click += new EventHandler((sender, e) => { ExitThread(); });

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(exitItem);

            // Initialize Tray Icon
            NotifyIcon rtn = new NotifyIcon()
            {
                // Icon = Resources.AppIcon,
                Icon = Resources.IconNew,
                Visible = true,
                ContextMenuStrip = contextMenu
            };

            // make sure tray icon is removed on application exit
            Application.ApplicationExit += new EventHandler((sender, e) =>
            {
                if (rtn != null) rtn.Visible = false;
            });

            return rtn;
        }

        private void ShowNotification(int packageCount, NotifyIcon icon)
        {
            icon.BalloonTipIcon = ToolTipIcon.Info;
            icon.Text = Application.ProductName;
            icon.BalloonTipTitle = $"{packageCount} package{(packageCount == 1 ? " is" : "s are")} outdated.";
            icon.BalloonTipText = $"To upgrade click here or the tray icon later.";
            icon.ShowBalloonTip(2000);
        }
    }
}
