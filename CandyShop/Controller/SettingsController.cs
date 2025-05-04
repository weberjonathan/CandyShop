using CandyShop.PackageCore;
using CandyShop.Services;
using CandyShop.Settings;
using CandyShop.View;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CandyShop.Controller
{
    // TOOD update SettingsView through observer pattern
    internal class SettingsController
    {
        private readonly SettingsService SettingsService;
        private MainWindow MainView;
        private SettingsWindow SettingsView;

        public SettingsController(SettingsService settingsService)
        {
            SettingsService = settingsService;
        }

        public void InjectView(MainWindow mainView)
        {
            MainView = mainView;

            MainView.OpenSettingsClicked += new EventHandler((sender, e) => ShowSettingsWindow());
            MainView.HideAdminWarningClicked += new EventHandler((sender, e) => SettingsService.SetSupressNoRightsWarning(true)); // TODO test this behavior and see if I like it; does the banenr still exist in main window?
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

            // TODO inject upgrade page I guess
            MainView.UpgradePackagesPage.CleanShortcutsChanged += new EventHandler((sender, e) => SettingsService.SetCleanShortcuts(MainView.UpgradePackagesPage.CleanShortcuts));
            MainView.UpgradePackagesPage.CloseAfterUpgradeChanged += new EventHandler((sender, e) => SettingsService.SetCloseAfterUpgrade(MainView.UpgradePackagesPage.CloseAfterUpgrade));
        }

        public void ShowSettingsWindow(bool displayFirstStartBanner = false)
        {
            SettingsView = new SettingsWindow
            {
                DisplayFirstStartBanner = displayFirstStartBanner
            };

            SettingsView.OkClicked += async (sender, e) =>
            {
                SettingsView.Locked = true;
                var success = await ApplySettings();
                SettingsView.Locked = false;
                if (success)
                {
                    SettingsView.Close();
                }
            };

            SettingsView.ApplyClicked += async (sender, e) =>
            {
                SettingsView.Locked = true;
                await ApplySettings();
                SettingsView.Locked = false;
            };

            SettingsView.FormClosed += (sender, e) =>
            {
                SettingsView.Dispose();
                SettingsView = null;
            };

            SettingsView.WingetBinaryChanged += OnWingetBinaryChanged;
            SettingsView.ChocolateyBinaryChanged += OnChocolateyBinaryChanged;
            SettingsView.GSudoBinaryChanged += OnGsudoBinaryChanged;

            // update view with current settings
            var settings = SettingsService.GetCurrentSettings();
            SettingsView.ActivePackageSource = settings.EnabledPackageManagers.First().Name;
            SettingsView.WingetBinary = settings.Winget.Filepath;
            SettingsView.ChocolateyBinary = settings.Chocolatey.Filepath;
            SettingsView.GSudoBinary = settings.Gsudo.Filepath;
            SettingsView.RequireAdminPrivileges = settings.ElevateOnDemand;
            SettingsView.CacheAdminPrivileges = settings.Gsudo.CachePrivileges;

            SettingsView.ShowDialog();
        }

        private async Task<bool> ApplySettings()
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
            var settings = BuildPartialSettingsFromView();

            // validate package manager
            try
            {
                var active = SettingsView.ActivePackageSource;
                var binary = active switch
                {
                    "Winget" => SettingsView.WingetBinary,
                    "Chocolatey" => SettingsView?.ChocolateyBinary,
                    _ => throw new ArgumentException("Uknown package manager")
                };
                await SettingsService.ValidateActiveSource(settings); // TODO if this is changed, update packageService
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
                    await SettingsService.ValidateGsudo(settings);
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

            SettingsService.UpdateSettings(settings);
            return true;
        }

        private async void OnWingetBinaryChanged(object sender, EventArgs e)
        {
            SettingsDefinition settings = BuildPartialSettingsFromView();
            if (!PathUtil.FileExists(settings.Winget.Filepath))
            {
                SettingsView.SetWingetBinaryStatus("File not found");
                return;
            }

            SettingsView.SetWingetBinaryStatus(string.Empty);

            string status;
            try
            {
                status = await SettingsService.ValidateWinget(settings);
            }
            catch (Exception)
            {
                status = "Validation failed";
            }

            SettingsView.SetWingetBinaryStatus(status);
        }

        private async void OnChocolateyBinaryChanged(object sender, EventArgs e)
        {
            SettingsDefinition settings = BuildPartialSettingsFromView();
            if (!PathUtil.FileExists(settings.Chocolatey.Filepath))
            {
                SettingsView.SetChocoBinaryStatus("File not found");
                return;
            }

            SettingsView.SetChocoBinaryStatus(string.Empty);

            string status;
            try
            {
                status = await SettingsService.ValidateChocolatey(settings);
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

        private async void OnGsudoBinaryChanged(object sender, EventArgs e)
        {
            SettingsDefinition settings = BuildPartialSettingsFromView();
            if (!PathUtil.FileExists(settings.Gsudo.Filepath))
            {
                SettingsView.SetGSudoBinaryStatus("File not found");
                return;
            }

            SettingsView.SetGSudoBinaryStatus(string.Empty);

            string status;
            bool valid = false;
            try
            {
                status = await SettingsService.ValidateGsudo(settings);
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

        private SettingsDefinition BuildPartialSettingsFromView()
        {
            SettingsDefinition settings = new();
            foreach (var pm in settings.PackageManagers)
            {
                pm.Enabled = pm.Name.Equals(SettingsView.ActivePackageSource);
                pm.UpgradeAsAdmin = SettingsView.RequireAdminPrivileges;
            }
            settings.Gsudo.Filepath = SettingsView.GSudoBinary;
            settings.Gsudo.CachePrivileges = SettingsView.CacheAdminPrivileges;
            settings.Winget.Filepath = SettingsView.WingetBinary;
            settings.Chocolatey.Filepath = SettingsView.ChocolateyBinary;

            return settings;
        }
    }
}
