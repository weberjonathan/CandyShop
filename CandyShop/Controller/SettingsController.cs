using CandyShop.Services;
using CandyShop.View;
using System;
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

            SettingsView.WingetBinaryChanged += new EventHandler((sender, e) =>
            {
                ValidateBinary(SettingsView.WingetBinary, SettingsService.GetWingetVersion, SettingsView.SetWingetBinaryStatus);
            });

            SettingsView.ChocolateyBinaryChanged += new EventHandler((sender, e) =>
            {
                ValidateBinary(SettingsView.ChocolateyBinary, SettingsService.GetChocoVersion, SettingsView.SetChocoBinaryStatus);
            });

            SettingsView.GSudoBinaryChanged += new EventHandler((sender, e) =>
            {
                var validated = ValidateBinary(SettingsView.GSudoBinary, SettingsService.GetGsudoVersion, SettingsView.SetGSudoBinaryStatus);
                SettingsView.EnableGsudoConfig = validated;
            });
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

            ValidateBinary(SettingsView.WingetBinary, SettingsService.GetWingetVersion, SettingsView.SetWingetBinaryStatus);
            ValidateBinary(SettingsView.ChocolateyBinary, SettingsService.GetChocoVersion, SettingsView.SetChocoBinaryStatus);
            var hasGsduo = ValidateBinary(SettingsView.GSudoBinary, SettingsService.GetGsudoVersion, SettingsView.SetGSudoBinaryStatus);
            SettingsView.EnableGsudoConfig = hasGsduo;

            SettingsView.Show();
        }

        private bool ValidateBinary(string binary, Func<string, string> getVersion, Action<string> updateStatus)
        {
            if (SettingsService.FileExists(binary))
            {
                updateStatus("Validating executable");
                var version = getVersion(binary);
                if (version == null)
                {
                    updateStatus("Failed to validate executable.");
                    return false;
                }
                else
                {
                    updateStatus(version);
                    return true;
                }
            }
            else
            {
                updateStatus("File not found");
                return false;
            }
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

            // validate configuration
            if (SettingsView.ActivePackageSource.Equals("Winget") && (!SettingsService.FileExists(SettingsView.WingetBinary) || SettingsService.GetWingetVersion(SettingsView.WingetBinary) == null))
            {
                MessageBox.Show(
                    "Winget was selected as active package source but the executable is not viable.",
                    MetaInfo.Name,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            if (SettingsView.ActivePackageSource.Equals("Chocolatey") && (!SettingsService.FileExists(SettingsView.ChocolateyBinary) || SettingsService.GetChocoVersion(SettingsView.ChocolateyBinary) == null))
            {
                MessageBox.Show(
                    "Chocolatey was selected as active package source but the executable is not viable.",
                    MetaInfo.Name,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            if (SettingsView.EnableGsudoConfig && (!SettingsService.FileExists(SettingsView.GSudoBinary) || SettingsService.GetGsudoVersion(SettingsView.GSudoBinary) == null))
            {
                MessageBox.Show(
                    "Administrator rights cannot be required without Gsudo and the Gsudo executable is not viable.",
                    MetaInfo.Name,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            // TODO apply to context (via service)
            // TODO save context (via service)

            return true;
        }
    }
}
