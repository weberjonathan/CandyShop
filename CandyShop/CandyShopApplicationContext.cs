using CandyShop.Chocolatey;
using CandyShop.View;
using CandyShop.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CandyShop.Controller;
using CandyShop.Services;
using Serilog;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;
using System.IO;

namespace CandyShop
{
    internal class CandyShopApplicationContext : ApplicationContext
    {
        public CandyShopApplicationContext(CandyShopContext context)
        {
            Log.Debug("Launched CandyShop.");

            // init services
            IPackageService packageService;
            WindowsTaskService windowsTaskService = new WindowsTaskService();
            ShortcutService shortcutService = new ShortcutService();
            if (context.WingetMode) packageService = new WingetService();
            else packageService = new ChocolateyService();

            LoadOutdatedPackagesAsync(packageService);

            // init controller
            MainWindowController candyShopController = new MainWindowController(windowsTaskService, context);
            InstalledPageController installedPageController = new InstalledPageController(packageService);
            UpgradePageController upgradePageController = new UpgradePageController(context, packageService, shortcutService);

            // init views
            IMainWindowView mainPage = new MainWindow(candyShopController);
            IInstalledPageView installedPage = mainPage.InstalledPackagesPage;
            IUpgradePageView upgradePage = mainPage.UpgradePackagesPage;
            installedPageController.InjectView(installedPage);
            upgradePageController.InjectViews(mainPage, upgradePage);
            candyShopController.InjectView(mainPage);

            // launch with form or in tray
            if (context.LaunchedMinimized)
            {
                // creates a tray icon, displays a notification if outdated packages
                // are found and opens the upgrade UI on click
                RunInBackground(candyShopController, packageService);
            }
            else
            {
                // launch window
                candyShopController.InitView();
                installedPageController.InitView();
            }
        }

        private async void LoadOutdatedPackagesAsync(IPackageService service)
        {
            try
            {
                await service.GetOutdatedPackagesAsync();
            }
            catch (ChocolateyException e)
            {
                Log.Error(LocaleEN.ERROR_RETRIEVING_OUTDATED_PACKAGES, e.Message);
            }
        }

        private async void RunInBackground(MainWindowController controller, IPackageService service)
        {
            List<GenericPackage> packages = null;

            // create tray icon
            NotifyIcon icon = InitTrayIcon();

            // obtain outdated packages
            try
            {
                packages = await service.GetOutdatedPackagesAsync();
                // dummy for testing
                //packages = new List<GenericPackage>();
                //packages.Add(new GenericPackage(new ChocolateyPackage()));
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

            NotificationShowHandler notifificationHandler = new();
            var result = await notifificationHandler.AwaitResult();
            if (result.Equals(NotificationResult.Show))
            {
                controller.InitView();
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
            if (!AppNotificationManager.IsSupported())
            {
                Log.Warning("AppNotificationManager is not supported. Is the Windows App SDK runtime available?");
                return;
            }

            var uri = new Uri("file:///%localappdata%/CandyShop/CandyShop.png");

            var builder = new AppNotificationBuilder()
                // TODO locale
                .AddText($"{packageCount} {(ContextSingleton.Get.WingetMode ? "Winget" : "Chocolatey")} package{(packageCount == 1 ? " is" : "s are")} outdated.") // TODO locale
                .AddButton(new AppNotificationButton("More details")
                    .AddArgument("action", "show"))
                .AddButton(new AppNotificationButton("Ignore")
                    .AddArgument("action", "ignore"));

            var p = uri.LocalPath[1..];
            p = Environment.ExpandEnvironmentVariables(p);
            p = Path.GetFullPath(p);
            if (!File.Exists(p))
            {
                FileStream fs = null;
                try
                {
                    fs = new(p, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                    Resources.CandyShop.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                    fs.Close();
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to store notification thumbnail at \"{p}\": {e.Message}");
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }

            if (File.Exists(p))
                builder.SetAppLogoOverride(uri, AppNotificationImageCrop.Default, "CandyShop");

            var notific = builder.BuildNotification();
            notific.ExpiresOnReboot = true;
            AppNotificationManager.Default.Show(notific);
        }
    }
}
