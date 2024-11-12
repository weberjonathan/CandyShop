using CandyShop.Properties;
using CandyShop.Services;
using CandyShop.View;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Serilog;
using CandyShop.Controls;
using CandyShop.PackageCore;

namespace CandyShop.Controller
{
    internal class MainWindowController
    {
        private readonly SystemStartService WindowsTaskService;
        private readonly CandyShopContext CandyShopContext;
        private IMainWindowView MainView;

        public MainWindowController(SystemStartService windowsTaskService, CandyShopContext candyShopContext)
        {
            WindowsTaskService = windowsTaskService;
            CandyShopContext = candyShopContext;
        }

        public void InjectView(IMainWindowView mainView)
        {
            MainView = mainView;

            // TODO use single provider instance accross all controllers
            AbstractCommon provider = ContextSingleton.Get.WingetMode ? new CommonWinget() : new CommonChocolatey();
            MainView.BuildControls(provider);
        }

        public void InitView()
        {
            Log.Information("Initializing UI");

            if (MainView == null) throw new InvalidOperationException("Set a view before intialising it!");

            MainView.LaunchOnSystemStartEnabled = WindowsTaskService.IsLaunchOnStartup();
            MainView.ShowAdminWarning =
                !CandyShopContext.HasAdminPrivileges &&
                !CandyShopContext.ElevateOnDemand &&
                !CandyShopContext.SupressAdminWarning;

            MainView.HideAdminWarningClicked += new EventHandler((sender, e) =>
            {
                CandyShopContext.SupressAdminWarning = true;
            });

            // exit application on 'X'
            MainView.ToForm().FormClosed += new FormClosedEventHandler((sender, e) =>
            {
                Program.Exit();
            });

            // set app title
            MainView.ToForm().Text = MetaInfo.WindowTitle;

            MainView.ToForm().Show();
        }

        public void ShowGithub()
        {
            OpenUrl(LocaleEN.URL_GITHUB);
        }

        public void ShowMetaPackageHelp()
        {
            OpenUrl(LocaleEN.URL_META_PACKAGES);
        }

        public void ShowLicenses()
        {
            using LicenseForm form = new LicenseForm();
            form.LinkClicked += new LinkClickedEventHandler((sender, e) => OpenUrl(e.LinkText));
            form.ShowDialog();
        }

        public void ToggleLaunchOnSystemStart()
        {
            if (!MainView.LaunchOnSystemStartEnabled && !WindowsTaskService.IsLaunchOnStartup())
            {
                WindowsTaskService.RegisterOnStartup();
                MainView.LaunchOnSystemStartEnabled = true;
            }
            else if (MainView.LaunchOnSystemStartEnabled && WindowsTaskService.IsLaunchOnStartup())
            {
                try
                {
                    WindowsTaskService.UnregisterOnStartup();
                }
                catch (CandyShopException e)
                {
                    Log.Error($"Failed to disabled Candy Shop launch with system start: {e.Message}");
                    MainView?.DisplayError(LocaleEN.ERROR_DISABLED_START_WITH_SYSTEM);
                }
            }

            MainView.LaunchOnSystemStartEnabled = WindowsTaskService.IsLaunchOnStartup();
        }

        public void ShowLogFolder()
        {
            if (ContextSingleton.Get.WingetMode)
            {
                PackageManagerProcess proc = new(ContextSingleton.Get.WingetBinary, "--logs");
                proc.ExecuteHidden();
            }
            else
            {
                string path = Path.GetFullPath(CandyShopContext.CholoateyLogFolder);
                if (Directory.Exists(path))
                {
                    try
                    {
                        Process.Start("explorer.exe", path);
                    }
                    catch (Win32Exception e)
                    {
                        MainView.DisplayError("An unknown error occurred: {0}", e.Message);
                    }

                }
                else
                {
                    MainView.DisplayError("Cannot find directory for Chocolatey logs: {0}", path);
                }
            }
        }

        public void ShowCandyShopConfigFolder()
        {
            if (Directory.Exists(CandyShopContext.ConfigFolder))
            {
                try
                {
                    Process.Start("explorer.exe", CandyShopContext.ConfigFolder);
                }
                catch (Win32Exception e)
                {
                    MainView.DisplayError("An unknown error occurred: {0}", e.Message);
                }
            }
            else
            {
                MainView.DisplayError("Cannot find CandyShop configuration directory at '{0}'", CandyShopContext.ConfigFolder);
            }
        }

        public void TogglePackageSource()
        {
            CandyShopContext.WingetMode = !CandyShopContext.WingetMode;
            Program.Restart();
    }

        private void OpenUrl(string url)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = "cmd",
                Arguments = $"/c start {url}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(info);
        }
    }
}
