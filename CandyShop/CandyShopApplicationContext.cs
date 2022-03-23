using CandyShop.Chocolatey;
using CandyShop.View;
using CandyShop.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using CandyShop.Controller;
using Serilog;

namespace CandyShop
{
    public class CandyShopApplicationContext : ApplicationContext
    {
        public CandyShopApplicationContext()
        {
            CandyShopContext context = new CandyShopContext();
            
            ChocolateyService chocolateyService = new ChocolateyService();
            BeginLoadingOutdatedPackages(chocolateyService);

            WindowsTaskService windowsTaskService = new WindowsTaskService();
            MainWindowController candyShopController = new MainWindowController(chocolateyService, windowsTaskService, context);
            
            IMainWindow mainView = new MainWindow(candyShopController);
            IInstalledPage pageView = mainView.InstalledPackagesPage;

            InstalledPageController installedPageController = new InstalledPageController(chocolateyService, pageView);

            candyShopController.SetView(mainView);

            // launch with form or in tray
            if (context.LaunchedMinimized)
            {
                // creates a tray icon, displays a notification if outdated packages
                // are found and opens the upgrade UI on click
                RunInBackground(candyShopController, chocolateyService);
            }
            else
            {
                // launch window
                candyShopController.InitView();
            }
        }

        private async void BeginLoadingOutdatedPackages(ChocolateyService service)
        {
            await service.GetOutdatedPackagesAsync();
        }

        private async void RunInBackground(MainWindowController controller, ChocolateyService service)
        {
            List<ChocolateyPackage> packages = null;

            // create tray icon
            NotifyIcon icon = InitTrayIcon();

            // obtain outdated packages
            try
            {
                packages = await service.GetOutdatedPackagesAsync();
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

            // create click handlers
            icon.BalloonTipClicked += new EventHandler((sender, e) =>
            {
                controller.InitView();
            });

            icon.MouseClick += new MouseEventHandler((sender, e) =>
            {
                controller.InitView();
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
