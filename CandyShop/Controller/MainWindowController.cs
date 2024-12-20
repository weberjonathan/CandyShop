﻿using CandyShop.Properties;
using CandyShop.Services;
using CandyShop.View;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Serilog;
using CandyShop.PackageCore;
using CandyShop.Controls.Factory;

namespace CandyShop.Controller
{
    internal class MainWindowController
    {
        private readonly CandyShopContext Context;
        private readonly SystemStartService WindowsTaskService;
        private readonly IControlsFactory ControlsFactory;
        private MainWindow MainView;

        public MainWindowController(CandyShopContext candyShopContext, SystemStartService windowsTaskService, IControlsFactory controlsFactory)
        {
            Context = candyShopContext;
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
                !Context.SupressAdminWarning;

            MainView.HideAdminWarningClicked += new EventHandler((sender, e) =>
            {
                Context.SupressAdminWarning = true;
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

        public void ShowLogFolder()
        {
            if (Context.WingetMode)
            {
                PackageManagerProcess proc = new(Context.WingetBinary, "--logs");
                proc.ExecuteHidden();
            }
            else
            {
                string path = Path.GetFullPath(Context.CholoateyLogFolder);
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
            if (Directory.Exists(Context.ConfigFolder))
            {
                try
                {
                    Process.Start("explorer.exe", Context.ConfigFolder);
                }
                catch (Win32Exception e)
                {
                    MainView.DisplayError("An unknown error occurred: {0}", e.Message);
                }
            }
            else
            {
                MainView.DisplayError("Cannot find CandyShop configuration directory at '{0}'", Context.ConfigFolder);
            }
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
