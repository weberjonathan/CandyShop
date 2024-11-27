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
        private SettingsWindow SettingsView;

        // TODO disable cache checkbox if require checkbox is unchecked
        public SettingsController(CandyShopContext context, SettingsService settingsService)
        {
            Context = context;
            SettingsService = settingsService;
        }

        public void InjectView(SettingsWindow settingsView)
        {
            SettingsView = settingsView;

            SettingsView.OkClicked += new EventHandler((sender, e) =>
            {
                var succses = ApplySettings();
                if (succses)
                {
                    SettingsView.Close();
                    SettingsView.Dispose();
                }
            });

            SettingsView.ApplyClicked += new EventHandler((sender, e) =>
            {
                ApplySettings();
            });

            SettingsView.WingetBinaryChanged += OnWingetBinaryChanged;
            SettingsView.WingetBinaryChanged += OnChocolateyBinaryChanged;
            SettingsView.GSudoBinaryChanged += OnGsudoBinaryChanged;
        }

        public void ShowView()
        {
            // TODO init view here, not in app context; do not inject view here either

            SettingsView.ActivePackageSource = Context.WingetMode ? "Winget" : "Chocolatey";
            SettingsView.WingetBinary = Context.WingetBinary;
            SettingsView.ChocolateyBinary = Context.ChocolateyBinary;
            SettingsView.GSudoBinary = "gsudo"; // TODO
            SettingsView.RequireAdminPrivileges = Context.ElevateOnDemand;
            SettingsView.CacheAdminPrivileges = Context.AllowGsudoCache;

            OnWingetBinaryChanged(this, EventArgs.Empty);
            OnChocolateyBinaryChanged(this, EventArgs.Empty);
            OnGsudoBinaryChanged(this, EventArgs.Empty);

            SettingsView.Show();
        }

        private bool ApplySettings()
        {
            // ask to resolve any binaries defined through environment variables
            var wingetViaEnvPath = SettingsService.TryDetectOnPath(SettingsView.WingetBinary, out var wingetResolved);
            var chocoViaEnvPath = SettingsService.TryDetectOnPath(SettingsView.ChocolateyBinary, out var chocoResolved);
            var gsudoViaEnvPath = SettingsService.TryDetectOnPath(SettingsView.GSudoBinary, out var gsudoResolved);

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
                }
                return false;
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
