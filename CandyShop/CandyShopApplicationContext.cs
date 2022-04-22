using CandyShop.Chocolatey;
using CandyShop.View;
using CandyShop.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CandyShop.Controller;
using CandyShop.Services;

namespace CandyShop
{
    internal class CandyShopApplicationContext : ApplicationContext
    {
        public CandyShopApplicationContext(CandyShopContext context)
        {
            // init services
            ChocolateyService chocolateyService = new ChocolateyService();
            WindowsTaskService windowsTaskService = new WindowsTaskService();
            ShortcutService shortcutService = new ShortcutService();

            //
            LoadOutdatedPackagesAsync(chocolateyService);

            // init controller
            MainWindowController candyShopController = new MainWindowController(chocolateyService, windowsTaskService, shortcutService, context);
            InstalledPageController installedPageController = new InstalledPageController(chocolateyService);

            // init views
            IMainWindowView mainView = new MainWindow(candyShopController);
            IInstalledPageView pageView = mainView.InstalledPackagesPage;
            installedPageController.InjectView(pageView);
            candyShopController.InjectView(mainView);

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
                installedPageController.InitView();
            }
        }

        private async void LoadOutdatedPackagesAsync(ChocolateyService service)
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
            catch (ChocolateyException e)
            {
                icon.BalloonTipIcon = ToolTipIcon.Error;
                icon.Text = Application.ProductName;
                icon.BalloonTipTitle = String.Format(LocaleEN.TEXT_APP_TITLE, Application.ProductName, Application.ProductVersion);
                icon.BalloonTipText = String.Format(LocaleEN.ERROR_RETRIEVING_OUTDATED_PACKAGES, e.Message);
                icon.ShowBalloonTip(2000);
                Program.Exit();
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
                Program.Exit();
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
