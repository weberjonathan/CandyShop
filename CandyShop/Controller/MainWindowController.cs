using CandyShop.Properties;
using CandyShop.Services;
using CandyShop.View;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Serilog;
using CandyShop.PackageCore;
using CandyShop.Controls.Factory;

namespace CandyShop.Controller
{
    internal class MainWindowController
    {
        private readonly CandyShopContext Context;
        private readonly PackageService PackageService;
        private readonly SystemStartService WindowsTaskService;
        private readonly IControlsFactory ControlsFactory;
        private MainWindow MainView;

        public MainWindowController(CandyShopContext candyShopContext, PackageService packageService, SystemStartService windowsTaskService, IControlsFactory controlsFactory)
        {
            Context = candyShopContext;
            PackageService = packageService;
            WindowsTaskService = windowsTaskService;
            ControlsFactory = controlsFactory;
        }

        public void InjectView(MainWindow mainView)
        {
            MainView = mainView;
            MainView.BuildControls(ControlsFactory);
        }

        public void InitView()
        {
            Log.Information("Initializing UI");

            if (MainView == null) throw new InvalidOperationException("Set a view before intialising it!");

            MainView.LaunchOnSystemStartEnabled = WindowsTaskService.IsLaunchOnStartup();
            MainView.ShowAdminWarning =
                !Context.HasAdminPrivileges &&
                !Context.ElevateOnDemand &&
                !Context.SupressAdminWarning; // move these to settingsController, bc it also knows the mainView

            MainView.HideAdminWarningClicked += new EventHandler((sender, e) =>
            {
                Context.SupressAdminWarning = true; // move this to settingsController, bc it also knows the mainView TODO
            });

            MainView.OpenLogsClicked += new EventHandler((sender, e) =>
            {
                try
                {
                    PackageService.OpenLogFolder();
                }
                catch (PackageManagerException ex)
                {
                    MainView.DisplayError("Failed to open log folder: {0}", ex.Message);
                }
            });

            // exit application on 'X'
            MainView.FormClosed += new FormClosedEventHandler((sender, e) =>
            {
                Program.Exit();
            });

            // set app title
            MainView.Text = MetaInfo.WindowTitle;

            MainView.Show();
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

        public void TogglePackageSource()
        {
            Context.WingetMode = !Context.WingetMode;
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
