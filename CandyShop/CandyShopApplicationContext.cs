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
using CandyShop.PackageCore;
using System.Globalization;
using CandyShop.Controls.Factory;
using CandyShop.Components;
using System.Linq;

namespace CandyShop
{
    internal class CandyShopApplicationContext : ApplicationContext
    {
        // TODO pinning in Choco without admin currently fails silently
        public CandyShopApplicationContext(SettingsService settingsService, CandyShopContext context)
        {
            Log.Information("--- Launching CandyShop ---");

            // load and apply settings
            var settings = settingsService.Load(context);
            // TODO overwrite active pm with eitehr winget or chocolatey, if it is different -> ie validate config
            MetaInfo.ActiveSource = settings.ActivePackageManager;

            IPackageFilterContext packageListSyncContext = PackageFilterContextFactory.Create(settings.ActivePackageManager);

            //
            string cwd = Directory.GetParent(Process.GetCurrentProcess().MainModule.FileName).FullName;
            Log.Debug($"cwd: {cwd}; elevated: {context.HasAdminPrivileges}; elevateOnDemand: {context.ElevateOnDemand}; debug: {context.DebugEnabled}");

            if (context.FirstStart)
            {
                // TODO
            }

            // validate selected package manager
            AbstractPackageManager activePackageManager = null;
            try
            {
                settingsService.ValidateActiveSource(out activePackageManager);
            }
            catch (Exception)
            {
                // TODO an error here will obviously lead to null pointer exceptions -> needs handling (eg show settings akin to first start, noop PM, null checks where PM is used)
                ErrorHandler.ShowError("{0} is selected as package source, but the executable is not viable. Please fix your settings.", settings.ActivePackageManager); // TODO
            }

            // validate gsudo
            if (settingsService.IsGsudoRequired())
            {
                try
                {
                    settingsService.ValidateGsudo();
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

            IControlsFactory controlsFactory =
                context.WingetMode ? new WingetControlsFactory() : new ChocoControlsFactory();

            // init controller
            MainWindowController mainWindowController = new(context, packageService, windowsTaskService, controlsFactory);
            InstalledPageController installedPageController = new(packageService, controlsFactory, packageListSyncContext);
            UpgradePageController upgradePageController = new(context, packageService, controlsFactory);
            PinController pinController = new(packageService);
            PackageController packageController = new(packageService, controlsFactory);
            SettingsController settingsController = new(context, settingsService);

            // init views
            MainWindow mainPage = new(mainWindowController);
            InstalledPage installedPage = mainPage.InstalledPackagesPage;
            UpgradePage upgradePage = mainPage.UpgradePackagesPage;
            installedPageController.InjectView(installedPage);
            upgradePageController.InjectViews(mainPage, upgradePage);
            mainWindowController.InjectView(mainPage);
            pinController.InjectView(installedPage, upgradePage);
            packageController.InjectViews(mainPage, upgradePage, installedPage);
            settingsController.InjectView(mainPage);

            // declare notification handler, so if needed, it lives during the entire lifecycle
            NotificationShowHandler notifificationHandler;

            // launch with form or in tray
            if (context.LaunchedMinimized)
            {
                notifificationHandler = new();
                // creates a tray icon, displays a notification if outdated packages
                // are found and opens the upgrade UI on click
                RunInBackground(mainWindowController, packageController, packageService, notifificationHandler, packageService, settings.CleanShortcuts);
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
                                           bool cleanShortcuts)
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
                ShowNotification(count, icon);
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
                    await packageService.Upgrade(packages, cleanShortcuts);
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

        private void ShowNotification(int packageCount, NotifyIcon icon)
        {
            if (!AppNotificationManager.IsSupported())
            {
                Log.Warning("AppNotificationManager is not supported. Is the Windows App SDK runtime available?");
                return;
            }

            var uri = new Uri("file:///%localappdata%/CandyShop/CandyShop.png");

            string text = packageCount == 1 ? LocaleEN.NOT_TEXT_SINGLE : LocaleEN.NOT_TEXT_MULTI;
            var builder = new AppNotificationBuilder()
                .AddText(string.Format(text, packageCount, MetaInfo.ActiveSource))
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
