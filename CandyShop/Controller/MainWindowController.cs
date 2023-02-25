using CandyShop.Chocolatey;
using CandyShop.Properties;
using CandyShop.Services;
using CandyShop.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CandyShop.Controller
{
    internal class MainWindowController
    {
        private readonly WindowsTaskService WindowsTaskService;
        private readonly CandyShopContext CandyShopContext;
        private IMainWindowView MainView;

        public MainWindowController(WindowsTaskService windowsTaskService, CandyShopContext candyShopContext)
        {
            WindowsTaskService = windowsTaskService;
            CandyShopContext = candyShopContext;
        }

        public void InjectView(IMainWindowView mainView)
        {
            MainView = mainView;
        }

        public void InitView()
        {
            if (MainView == null) throw new InvalidOperationException("Set a view before intialising it!");

            MainView.CreateTaskEnabled = WindowsTaskService.LaunchTaskExists();

            if (CandyShopContext.HasAdminPrivileges) MainView.ClearAdminHints();
            else MainView.ShowAdminHints();

            // exit application on 'X'
            MainView.ToForm().FormClosed += new FormClosedEventHandler((sender, e) =>
            {
                Program.Exit();
            });

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
            form.ShowDialog();
        }

        public void ToggleCreateTask()
        {
            if (!MainView.CreateTaskEnabled && !WindowsTaskService.LaunchTaskExists())
            {
                WindowsTaskService.CreateLaunchTask();
                MainView.CreateTaskEnabled = true;
            }
            else if (MainView.CreateTaskEnabled && WindowsTaskService.LaunchTaskExists())
            {
                try
                {
                    WindowsTaskService.RemoveLaunchTask();
                }
                catch (CandyShopException)
                {
                    MainView?.DisplayError("An error occurred while trying to delete the Windows task.");
                }
            }

            MainView.CreateTaskEnabled = WindowsTaskService.LaunchTaskExists();
        }

        public void ShowChocoLogFolder()
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
