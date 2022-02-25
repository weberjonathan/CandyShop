using CandyShop.Chocolatey;
using CandyShop.Controls;
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
        private const string OPTION_BACKGROUND = "--background";
        private const string OPTION_BACKGROUND_SHORT = "-b";

        private readonly CandyShopController _CandyShopController;

        private readonly ChocolateyService _ChocolateyService;

        public CandyShopApplicationContext()
        {
            _ChocolateyService = new ChocolateyService();
            _CandyShopController = new CandyShopController(_ChocolateyService, null); // TODO

            // determine silent mode
            List<string> args = new List<string>(Environment.GetCommandLineArgs());
            _CandyShopController.LaunchedMinimized = args.Find(s => s.Equals(OPTION_BACKGROUND) ||
                                                  s.Equals(OPTION_BACKGROUND_SHORT)) != null; // TODO eval != null

            // launch with form or in tray
            if (_CandyShopController.LaunchedMinimized)
            {
                // creates a tray icon, displays a notification if outdated packages
                // are found and opens the upgrade UI on click
                RunInBackground();
            }
            else
            {
                _CandyShopController.ShowForm();
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
                packages = await _ChocolateyService.FetchOutdatedAsync();
            }
            catch (ChocolateyException)
            {
                icon.BalloonTipIcon = ToolTipIcon.Error;
                icon.Text = Application.ProductName;
                icon.BalloonTipTitle = String.Format(Strings.Form_Title, Application.ProductName, Application.ProductVersion);
                icon.BalloonTipText = Strings.Err_CheckOutdated;
                icon.ShowBalloonTip(2000);
                Environment.Exit(0);
            }

            _CandyShopController.SetOutdatedPackages(packages);

            // create click handlers
            icon.BalloonTipClicked += new EventHandler((sender, e) =>
            {
                _CandyShopController.ShowForm();
            });

            icon.MouseClick += new MouseEventHandler((sender, e) =>
            {
                _CandyShopController.ShowForm();
            });


            if (packages.Count > 0)
            {
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
