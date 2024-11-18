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

namespace CandyShop
{
    internal class CandyShopApplicationContext : ApplicationContext
    {
        public CandyShopApplicationContext(CandyShopContext context)
        {
            Log.Information("--- Launching CandyShop ---");

            //
            string cwd = Directory.GetParent(Process.GetCurrentProcess().MainModule.FileName).FullName;
            Log.Debug($"cwd: {cwd}; elevated: {context.HasAdminPrivileges}; elevateOnDemand: {context.ElevateOnDemand}; debug: {context.DebugEnabled}");

            if (context.FirstStart)
            {
                FirstStartForm f = new();
                var result = f.ShowDialog();
                if (result == DialogResult.OK)
                {
                    context.FirstStart = false;
                    context.ElevateOnDemand = f.RequireAdmin;
                    context.AllowGsudoCache = f.CacheAdmin;
                    context.WingetMode = f.WingetMode;
                    context.SaveProperties();
                }
                else
                {
                    Program.Exit(saveProperties: false);
                }
            }

            // determine winget or choco and test executables
            bool requireManualElevation = context.ElevateOnDemand && !context.HasAdminPrivileges;
            AbstractPackageManager packageManager;
            if (context.WingetMode)
            {
                // determine locale
                if (!context.SupressLocaleLogWarning)
                {
                    var ci = CultureInfo.CurrentCulture;
                    List<string> supported = ["en", "de"];
                    if (!supported.Contains(ci.TwoLetterISOLanguageName))
                        Log.Warning($"Detected unsupported locale \"{ci.TwoLetterISOLanguageName}\". This may lead to parsing errors. See https://github.com/weberjonathan/CandyShop/blob/master/docs/lcoales.md for more.");
                }

                packageManager = new WingetManager(context.SupressLocaleLogWarning, context.WingetBinary, requireManualElevation, context.AllowGsudoCache);
                var p = new PackageManagerProcess(context.WingetBinary, "--version");
                try
                {
                    p.ExecuteHidden();
                    if (p.ExitCode != 0)
                        throw new PackageManagerException();
                }
                catch (Exception)
                {
                    ErrorHandler.ShowError(LocaleEN.ERROR_WINGET_PATH);
                    packageManager = null;
                }
            }
            else
            {
                var chocoManager = new ChocoManager(2, context.ValidExitCodes, context.ChocolateyBinary, requireManualElevation, context.AllowGsudoCache);
                var p = new PackageManagerProcess(context.ChocolateyBinary, "--version");
                try
                {
                    p.ExecuteHidden();
                    if (p.ExitCode == 0)
                    {
                        string majorString = p.Output.Trim().Split('.')[0];
                        if (!int.TryParse(majorString, out int majorVersion))
                            Log.Error($"Failed to parse the version string from Chocolatey, assume minimum version 2.x. Output was: {p.Output}");

                        chocoManager.ChocoVersionMajor = majorVersion;
                    }
                    else
                    {
                        throw new PackageManagerException();
                    }
                }
                catch (Exception)
                {
                    ErrorHandler.ShowError(LocaleEN.ERROR_CHOCO_PATH);
                    chocoManager = null;
                }

                packageManager = chocoManager;
            }

            // init services
            ShortcutService shortcutService = new();
            PackageService packageService = new(packageManager, shortcutService);
            SystemStartService windowsTaskService = new();

            LoadOutdatedPackagesAsync(packageService);

            IControlsFactory controlsFactory =
                context.WingetMode ? new WingetControlsFactory() : new ChocoControlsFactory();

            // init controller
            MainWindowController mainWindowController = new(context, windowsTaskService, controlsFactory);
            InstalledPageController installedPageController = new(packageService, controlsFactory);
            UpgradePageController upgradePageController = new(context, packageService, controlsFactory);
            PinController pinController = new(packageService);
            PackageController packageController = new(packageService, controlsFactory);

            // init views
            MainWindow mainPage = new(mainWindowController);
            InstalledPage installedPage = mainPage.InstalledPackagesPage;
            UpgradePage upgradePage = mainPage.UpgradePackagesPage;
            installedPageController.InjectView(installedPage);
            upgradePageController.InjectViews(mainPage, upgradePage);
            mainWindowController.InjectView(mainPage);
            pinController.InjectView(installedPage, upgradePage);
            packageController.InjectViews(mainPage, upgradePage, installedPage);

            // declare notification handler, so if needed, it lives during the entire lifecycle
            NotificationShowHandler notifificationHandler;

            // launch with form or in tray
            if (context.LaunchedMinimized)
            {
                notifificationHandler = new();
                // creates a tray icon, displays a notification if outdated packages
                // are found and opens the upgrade UI on click
                RunInBackground(mainWindowController, packageController, packageService, notifificationHandler, packageService);
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
                                           PackageService packageService)
        {
            List<GenericPackage> packages = null;

            // create tray icon
            NotifyIcon icon = InitTrayIcon();

            // obtain outdated packages
            try
            {
                packages = await service.GetOutdatedPackagesAsync();
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
                mainWindowController.InitView();
                packageController.UpdatePackageDisplaysAsync();
            }
            else if (result.Equals(NotificationResult.UpgradeAll))
            {
                try
                {
                    await packageService.Upgrade(packages);
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
            string provider = ContextSingleton.Get.WingetMode ? "Winget" : "Chocolatey";
            var builder = new AppNotificationBuilder()
                .AddText(string.Format(text, packageCount, provider))
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
