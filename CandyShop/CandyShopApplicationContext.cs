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
using CandyShop.PackageCore;
using CandyShop.Controls.Factory;
using CandyShop.Components;
using System.Linq;
using CandyShop.Settings;

namespace CandyShop
{
    internal class CandyShopApplicationContext : ApplicationContext
    {
        // TODO settings service pm must be validated so that Winget, Chocolatey exist and that at least one is enabled
        // TODO pinning in Choco without admin currently fails silently
        public CandyShopApplicationContext(SettingsService settingsService, Arguments arguments)
        {
            Log.Information("--- Launching CandyShop ---");

            // init views
            MainWindow mainPage = new();
            InstalledPage installedPage = mainPage.InstalledPackagesPage;
            UpgradePage upgradePage = mainPage.UpgradePackagesPage;
            settingsService.RegisterListener(mainPage);
            settingsService.RegisterListener(upgradePage);


            // load and apply settings
            SettingsController settingsController = new(settingsService);
            SettingsDefinition settings = settingsService.Load();
            if (settings == null)
            {
                settings = settingsService.CreateSettings();
                // TODO check how it behaves if we launched from background
                settingsController.ShowSettingsWindow();
            }

            IPackageFilterContext packageListSyncContext = PackageFilterContextFactory.Create(settings);

            //
            string cwd = Directory.GetParent(Environment.ProcessPath).FullName;
            Log.Debug($"cwd: {cwd}; elevated: {Util.IsAdmin()}; debug: {arguments.DebugEnabled}");

            // validate selected package manager
            // TODO the gsudo param should be elevateOnDemand && !isAdmin or no?
            AbstractPackageManager activePackageManager = PackageManagerFactory.Active(settings);

            try
            {
                activePackageManager.ValidateExec();
            }
            catch (Exception)
            {
                // TODO how to proceed? nothign will work right? but not crash and then settings can be accessed
                ErrorHandler.ShowError("Failed to validate selected package manager.");
            }

            // validate gsudo
            if (settingsService.IsGsudoRequired())
            {
                try
                {
                    settingsService.ValidateGsudo(); // TODO
                }
                catch (Exception)
                {
                    ErrorHandler.ShowError("Validation of the gsudo executable failed. Please fix the filepath of the executable in the settings, or disable gsudo entirely.");
                }
            }

            // init services
            ShortcutService shortcutService = new();
            PackageService packageService = new(activePackageManager, shortcutService);
            SystemStartService windowsTaskService = new();

            LoadOutdatedPackagesAsync(packageService);

            IUiComponents controlsFactory = UiComponentsFactory.Create(settings);

            // init controller
            MainWindowController mainWindowController = new(packageService, windowsTaskService, controlsFactory);
            InstalledPageController installedPageController = new(packageService, controlsFactory, packageListSyncContext);
            UpgradePageController upgradePageController = new(packageService, controlsFactory);
            PinController pinController = new(packageService);
            PackageController packageController = new(packageService, controlsFactory);
            installedPageController.InjectView(installedPage);
            upgradePageController.InjectViews(mainPage, upgradePage);
            mainWindowController.InjectView(mainPage);
            pinController.InjectView(installedPage, upgradePage);
            packageController.InjectViews(mainPage, upgradePage, installedPage);
            settingsController.InjectView(mainPage);

            // declare notification handler, so if needed, it lives during the entire lifecycle
            NotificationShowHandler notifificationHandler;

            // launch with form or in tray
            if (arguments.LaunchedMinimized)
            {
                notifificationHandler = new();
                // creates a tray icon, displays a notification if outdated packages
                // are found and opens the upgrade UI on click
                RunInBackground(mainWindowController, packageController, packageService, notifificationHandler, packageService, settings);
            }
            else
            {
                // launch window
                mainWindowController.InitView();
                packageController.UpdatePackageDisplaysAsync();

                // attempt removal of legacy task
                if (windowsTaskService.LaunchTaskExists())
                {
                    var result = MessageBox.Show("The launch task, that was used to execute Candy Shop on system start in earlier versions of the program, has been replaced by a shortcut. It is recommended to remove the older task to prevent redundant processes. Do you wish to remove the legacy task? If not, this prompt will appear again next time you launch the program. Please note that CandyShop requires administrator privileges to remove the legacy task.", MetaInfo.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

        private async void LoadOutdatedPackagesAsync(PackageService service)
        {
            try
            {
                await service.GetOutdatedPackagesAsync();
            }
            catch (PackageManagerException e)
            {
                ErrorHandler.ShowError(e.Message);
                Log.Error(LocaleEN.ERROR_RETRIEVING_OUTDATED_PACKAGES, e.Message);
            }
        }

        private async void RunInBackground(MainWindowController mainWindowController,
                                           PackageController packageController,
                                           PackageService service,
                                           NotificationShowHandler notifificationHandler,
                                           PackageService packageService,
                                           SettingsDefinition settings)
        {
            List<GenericPackage> packages = null;

            // create tray icon
            NotifyIcon icon = InitTrayIcon();

            // obtain outdated packages
            try
            {
                packages = (await service.GetOutdatedPackagesAsync()).ToList();
            }
            catch (PackageManagerException e)
            {
                ErrorHandler.NotifyError(icon, LocaleEN.ERROR_RETRIEVING_OUTDATED_PACKAGES, e.Message);
                Program.Exit();
            }

            // create click handlers
            icon.MouseClick += new MouseEventHandler((sender, e) =>
            {
                mainWindowController.InitView();
                packageController.UpdatePackageDisplaysAsync();
            });

            int count = service.GetNonPinnedCount(packages);
            if (count > 0)
            {
                ShowNotification(count, icon, settings.EnabledPackageManagers.First().Name);
            }
            else
            {
                Program.Exit();
            }

            var result = await notifificationHandler.AwaitResult();
            if (result.Equals(NotificationResult.Show))
            {
                mainWindowController.InitView();
                packageController.UpdatePackageDisplaysAsync();
            }
            else if (result.Equals(NotificationResult.UpgradeAll))
            {
                try
                {
                    await packageService.Upgrade(packages, settings.CleanShortcuts);
                }
                catch (PackageManagerException e)
                {
                    ErrorHandler.ShowError(LocaleEN.ERROR_UPGRADING_OUTDATED_PACKAGES_SHORT, e.Message.TrimEnd('.'));
                }
                catch (CandyShopException e)
                {
                    ErrorHandler.ShowError(LocaleEN.ERROR_UPGRADING_OUTDATED_PACKAGES_SHORT, e.Message.TrimEnd('.'));
                }
                Program.Exit();
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

        private void ShowNotification(int packageCount, NotifyIcon icon, string packageSource)
        {
            if (!AppNotificationManager.IsSupported())
            {
                Log.Warning("AppNotificationManager is not supported. Is the Windows App SDK runtime available?");
                return;
            }

            var uri = new Uri("file:///%localappdata%/CandyShop/CandyShop.png");

            string text = packageCount == 1 ? LocaleEN.NOT_TEXT_SINGLE : LocaleEN.NOT_TEXT_MULTI;
            var builder = new AppNotificationBuilder()
                .AddText(string.Format(text, packageCount, packageSource))
                .AddButton(new AppNotificationButton(LocaleEN.NOT_SHOW)
                    .AddArgument("action", "show"))
                .AddButton(new AppNotificationButton(LocaleEN.NOT_UPGRADE)
                    .AddArgument("action", "upgrade"));

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
