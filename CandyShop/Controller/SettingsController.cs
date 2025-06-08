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

        public void InjectView(SettingsWindow settingsView, MainWindow mainView)
        {
            // Settings view
            SettingsView = settingsView;
            SettingsView.OkClicked += async (sender, e) =>
            {
                SettingsView.Locked = true;
                var success = await ApplySettings();
                SettingsView.Locked = false;
                if (success)
                {
                    SettingsView.DialogResult = DialogResult.OK;
                    SettingsView.Close();
                }
            };

            SettingsView.ApplyClicked += async (sender, e) =>
            {
                SettingsView.Locked = true;
                await ApplySettings();
                SettingsView.Locked = false;
            };

            SettingsView.WingetBinaryChanged += OnWingetBinaryChanged;
            SettingsView.ChocolateyBinaryChanged += OnChocolateyBinaryChanged;
            SettingsView.GSudoBinaryChanged += OnGsudoBinaryChanged;

            // Main view
            MainView = mainView;
            MainView.OpenSettingsClicked += (sender, e) => ShowSettingsWindow();
            MainView.OpenSettingsDirClicked += (sender, e) =>
            {
                try
                {
                    SettingsService.OpenSettingsDirectory();
                }
                catch (CandyShopException ex)
                {
                    MainView.DisplayError("Failed to open settings directory: {0}", ex.Message);
                }
            };

            MainView.TogglePackageSourceClicked += (sender, e) =>
            {
                SettingsService.ToggleActivePackageManager();
                Program.Restart(saveProperties: true);
            };

            // upgrade view
            MainView.UpgradePackagesPage.CleanShortcutsChanged += new EventHandler((sender, e) => SettingsService.SetCleanShortcuts(MainView.UpgradePackagesPage.CleanShortcuts));
            MainView.UpgradePackagesPage.CloseAfterUpgradeChanged += new EventHandler((sender, e) => SettingsService.SetCloseAfterUpgrade(MainView.UpgradePackagesPage.CloseAfterUpgrade));
        }

        public bool ShowSettingsWindow(bool displayFirstStartBanner = false)
        {
            SettingsView.DisplayFirstStartBanner = displayFirstStartBanner;
            DialogResult result = SettingsView.ShowDialog();
            return result.Equals(DialogResult.OK);
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
                    "Winget" => SettingsView?.WingetBinary,
                    "Chocolatey" => SettingsView?.ChocolateyBinary,
                    _ => throw new ArgumentException("Uknown package manager")
                };
                await SettingsService.ValidateActiveSource(settings);
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
            if (settings.Gsudo.Enabled)
            {
                try
                {
                    await SettingsService.ValidateGsudo(settings);
                }
                catch (Exception)
                {
                    ErrorHandler.ShowError("The gsudo configuration is invalid! Please fix the path to the executable or disable gsudo.");
                    return false;
                }
            }

            // check if restart is required
            if (SettingsService.HaveEnabledPackageManagersChanged(settings))
            {
                var result = MessageBox.Show(
                    "A restart is required to apply your settings. Press OK to restart the application.",
                    MetaInfo.Name,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
                if (result != DialogResult.OK)
                    return false;

                SettingsView.FormClosed += RestartEventHandler;
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

            SettingsView?.SetWingetBinaryStatus(status);
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

            SettingsView?.SetChocoBinaryStatus(status);
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
            try
            {
                status = await SettingsService.ValidateGsudo(settings);
            }
            catch (FileNotFoundException)
            {
                status = "File not found";
            }
            catch (PackageManagerException)
            {
                status = "Validation failed";
            }

            SettingsView?.SetGSudoBinaryStatus(status);
        }

        private void RestartEventHandler(object sender, EventArgs e)
        {
            Program.Restart();
        }

        private SettingsDefinition BuildPartialSettingsFromView()
        {
            SettingsDefinition settings = new();
            foreach (var pm in settings.PackageManagers)
            {
                pm.Enabled = pm.Name.Equals(SettingsView.ActivePackageSource);
                pm.UpgradeAsAdmin = SettingsView.UpgradeAsAdmin;
            }
            settings.Gsudo.Enabled = SettingsView.EnableGsudo;
            settings.Gsudo.Filepath = SettingsView.GSudoBinary;
            settings.Gsudo.CachePrivileges = SettingsView.CacheAdminPrivileges;
            settings.Winget.Filepath = SettingsView.WingetBinary;
            settings.Chocolatey.Filepath = SettingsView.ChocolateyBinary;

            return settings;
        }
    }
}
