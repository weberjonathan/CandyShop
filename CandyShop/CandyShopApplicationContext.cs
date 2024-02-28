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
using System.Diagnostics;

namespace CandyShop
{
    internal class CandyShopApplicationContext : ApplicationContext
    {
        public CandyShopApplicationContext(CandyShopContext context)
        {
            Log.Information("--- Launching CandyShop ---");

            //
            string cwd = Directory.GetParent(Process.GetCurrentProcess().MainModule.FileName).FullName;
            Log.Debug($"Current working directory: {cwd}");

            // init services
            IPackageService packageService;
            SystemStartService windowsTaskService = new SystemStartService();
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

            // declare notification handler, so if needed, it lives during the entire lifecycle
            NotificationShowHandler notifificationHandler;

            // launch with form or in tray
            if (context.LaunchedMinimized)
            {
                notifificationHandler = new();
                // creates a tray icon, displays a notification if outdated packages
                // are found and opens the upgrade UI on click
                RunInBackground(candyShopController, packageService, context, notifificationHandler);
            }
            else
            {
                // launch window
                candyShopController.InitView();
                installedPageController.InitView();

                // attempt removal of legacy task
                if (windowsTaskService.LaunchTaskExists())
                {
                    var result = MessageBox.Show("The launch task, that was used to execute Candy Shop on system start in earlier versions of the program, has been replaced by a shortcut. It is recommended to remove the older task to prevent redundant processes. Do you wish to remove the legacy task? If not, this prompt will appear again next time you launch the program. Please note that CandyShop requires administrator privileges to remove the legacy task.", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            windowsTaskService.RemoveLegacyTask();
                        }
                        catch (CandyShopException e)
                        {
                            ErrorHandler.ShowError(e.Message);
                        }
                    }
                }
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

        private async void RunInBackground(MainWindowController controller, IPackageService service, CandyShopContext context, NotificationShowHandler notifificationHandler)
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
                icon.BalloonTipTitle = String.Format(LocaleEN.TEXT_APP_TITLE, Application.ProductName, context.ApplicationVersion);
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

            string text = packageCount == 1 ? LocaleEN.NOT_TEXT_SINGLE : LocaleEN.NOT_TEXT_MULTI;
            string provider = ContextSingleton.Get.WingetMode ? "Winget" : "Chocolatey";
            var builder = new AppNotificationBuilder()
                .AddText(string.Format(text, packageCount, provider))
                .AddButton(new AppNotificationButton(LocaleEN.NOT_SHOW)
                    .AddArgument("action", "show"))
                .AddButton(new AppNotificationButton(LocaleEN.NOT_IGNORE)
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
