using CandyShop.PackageCore;
using CandyShop.Services;
using CandyShop.View;
using System;
using System.IO;
using System.Windows.Forms;

namespace CandyShop.Controller
{
    internal class SettingsController
    {
        private readonly CandyShopContext Context;
        private readonly SettingsService SettingsService;
        private MainWindow MainView;
        private SettingsWindow SettingsView;

        // TODO disable cache checkbox if require checkbox is unchecked
        public SettingsController(CandyShopContext context, SettingsService settingsService)
        {
            Context = context;
            SettingsService = settingsService;
        }

        public void InjectView(MainWindow mainView)
        {
            MainView = mainView;
            MainView.OpenSettingsClicked += new EventHandler((sender, e) => ShowView());
            MainView.OpenSettingsDirClicked += new EventHandler((sender, e) =>
            {
                try
                {
                    SettingsService.OpenSettingsDirectory();
                }
                catch (CandyShopException ex)
                {
                    MainView.DisplayError("Failed to open settings directory: {0}", ex.Message);
                }
            });
        }

        public void ShowView()
        {
            SettingsView = new SettingsWindow();
            SettingsView.OkClicked += new EventHandler((sender, e) =>
            {
                var succses = ApplySettings();
                if (succses)
                {
                    SettingsView.Hide();
                    SettingsView.Close();
                }
            });

            SettingsView.ApplyClicked += new EventHandler((sender, e) =>
            {
                ApplySettings();
            });

            SettingsView.FormClosed += new FormClosedEventHandler((sender, e) =>
            {
                SettingsView.Dispose();
                SettingsView = null;
            });

            SettingsView.WingetBinaryChanged += OnWingetBinaryChanged;
            SettingsView.ChocolateyBinaryChanged += OnChocolateyBinaryChanged;
            SettingsView.GSudoBinaryChanged += OnGsudoBinaryChanged;

            // update view with current settings
            var settings = SettingsService.GetCurrentSettings();
            SettingsView.ActivePackageSource = settings.ActivePackageManager;
            SettingsView.WingetBinary = settings.PackageManagers["Winget"].Filepath;
            SettingsView.ChocolateyBinary = settings.PackageManagers["Chocolatey"].Filepath;
            SettingsView.GSudoBinary = settings.Gsudo.Filepath;
            SettingsView.RequireAdminPrivileges = settings.ElevateOnDemand;
            SettingsView.CacheAdminPrivileges = settings.Gsudo.EnableCredentialsStore;

            SettingsView.ShowDialog();
        }

        private bool ApplySettings()
        {
            // ask to resolve any binaries defined through environment variables
            var wingetViaEnvPath = PathUtil.FileExistsOnEnvPath(SettingsView.WingetBinary, out var wingetResolved);
            var chocoViaEnvPath = PathUtil.FileExistsOnEnvPath(SettingsView.ChocolateyBinary, out var chocoResolved);
            var gsudoViaEnvPath = PathUtil.FileExistsOnEnvPath(SettingsView.GSudoBinary, out var gsudoResolved);

            if (wingetViaEnvPath || chocoViaEnvPath || gsudoViaEnvPath)
            {
                var result = MessageBox.Show(
                    "It is recommended to explicitly define any executables called from Candy Shop. Do you wish to resolve all executables defined through the environment PATH variable?",
                    MetaInfo.Name,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);

                if (result.Equals(DialogResult.Yes))
                {
                    if (wingetViaEnvPath)
                        SettingsView.WingetBinary = wingetResolved;

                    if (chocoViaEnvPath)
                        SettingsView.ChocolateyBinary = chocoResolved;

                    if (gsudoViaEnvPath)
                        SettingsView.GSudoBinary = gsudoResolved;

                    return false;
                }
            }

            // build settings definition from view
            var settings = BuildSettingsFromView();

            // validate package manager
            AbstractPackageManager activePmInstance = null;
            try
            {
                var active = SettingsView.ActivePackageSource;
                var binary = active switch
                {
                    "Winget" => SettingsView.WingetBinary,
                    "Chocolatey" => SettingsView?.ChocolateyBinary,
                    _ => throw new ArgumentException("Uknown package manager")
                };
                SettingsService.ValidateActiveSource(settings, out activePmInstance); // TODO if this is changed, update packageService
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Winget was selected as active package source but the executable is not viable.",
                    MetaInfo.Name,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            // validate gsudo
            if (SettingsService.IsGsudoRequired(settings)) // TODO this check should respect whether CandyShop was launched as admin or not
            {
                try
                {
                    SettingsService.ValidateGsudo(settings);
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        "Administrator rights cannot be required without Gsudo and the Gsudo executable is not viable.", // TODO
                        MetaInfo.Name,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
            }

            // TODO apply to context (via service)
            // TODO save context (via service)

            return true;
        }

        private void OnWingetBinaryChanged(object sender, EventArgs e)
        {
            SettingsDefinition settings = BuildSettingsFromView();
            string status;
            try
            {
                status = SettingsService.ValidateWinget(settings);
            }
            catch (FileNotFoundException)
            {
                status = "File not found";
            }
            catch (PackageManagerException)
            {
                status = "Validation failed";
            }

            SettingsView.SetWingetBinaryStatus(status);
        }

        private void OnChocolateyBinaryChanged(object sender, EventArgs e)
        {
            SettingsDefinition settings = BuildSettingsFromView();
            string status;
            try
            {
                status = SettingsService.ValidateChocolatey(settings);
            }
            catch (FileNotFoundException)
            {
                status = "File not found";
            }
            catch (PackageManagerException)
            {
                status = "Validation failed";
            }

            SettingsView.SetChocoBinaryStatus(status);
        }

        private void OnGsudoBinaryChanged(object sender, EventArgs e)
        {
            SettingsDefinition settings = BuildSettingsFromView();
            string status;
            bool valid = false;
            try
            {
                status = SettingsService.ValidateGsudo(settings);
                valid = true;
            }
            catch (FileNotFoundException)
            {
                status = "File not found";
            }
            catch (PackageManagerException)
            {
                status = "Validation failed";
            }

            SettingsView.SetGSudoBinaryStatus(status);
            SettingsView.EnableGsudoConfig = valid;
        }

        private SettingsDefinition BuildSettingsFromView()
        {
            // TODO combine with get settings from service to fill out remaining properties

            SettingsDefinition settings = new()
            {
                ActivePackageManager = SettingsView.ActivePackageSource,
                //CleanShortcuts = 
                //CloseAfterUpgrade =
                ElevateOnDemand = SettingsView.RequireAdminPrivileges,
                //SupressNoRightsWarning = 
            };
            settings.Gsudo.Filepath = SettingsView.GSudoBinary;
            settings.Gsudo.EnableCredentialsStore = SettingsView.CacheAdminPrivileges;
            settings.PackageManagers["Winget"].Filepath = SettingsView.WingetBinary;
            settings.PackageManagers["Chocolatey"].Filepath = SettingsView.ChocolateyBinary;

            return settings;
        }
    }
}
