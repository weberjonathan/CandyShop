using CandyShop.Chocolatey;
using CandyShop.Presentation;
using CandyShop.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CandyShop
{
    public class CandyShopApplicationContext : ApplicationContext
    {
        public CandyShopApplicationContext()
        {
            // determine silent mode
            List<string> args = new List<string>(Environment.GetCommandLineArgs());
            LaunchInBackground = args.Find(s => s.Equals("--background") ||
                                                s.Equals("-b")) != null;

            // hier ein service, darin outdated fetchen
            // durch event des service in dieser klasse notification showen
            // durch event die Liste in der Form füllen

            // launch with form or in tray
            if (LaunchInBackground)
            {
                NotifyIcon icon = InitTrayIcon();
                FetchOutdatedAndShowNotificationAsync(icon);
            }
            else
            {
                new CandyShopForm().Show();
            }
        }

        public bool LaunchInBackground { get; }

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

        private async void FetchOutdatedAndShowNotificationAsync(NotifyIcon icon)
        {
            List<ChocolateyPackage> packages = null;

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

            // show notification and create click handlers
            if (packages.Count > 0)
            {
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

                icon.BalloonTipIcon = ToolTipIcon.Info;
                icon.Text = Application.ProductName;
                icon.BalloonTipTitle = $"{packages.Count} package{(packages.Count == 1 ? " is" : "s are")} outdated.";
                icon.BalloonTipText = $"To upgrade click here or the tray icon later.";
                icon.ShowBalloonTip(2000);
            }
            else
            {
                // terminate, bc app was launched silently but no packages are outdated and thus nothing is to be done
                Environment.Exit(0);
            }
        }
    }
}
