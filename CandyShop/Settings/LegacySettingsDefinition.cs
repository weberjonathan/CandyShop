using Serilog;
using System.Collections.Generic;
using System.Text.Json;

namespace CandyShop.Settings
{
    internal class LegacySettingsDefinition
    {
        public static LegacySettingsDefinition FromJson(string json)
        {
            LegacySettingsDefinition definition = null;
            try
            {
                definition = JsonSerializer.Deserialize<LegacySettingsDefinition>(json);
            }
            catch (JsonException)
            {
                Log.Error("Failed to parse settings file using legacy format.");
            }

            return definition;
        }

        public string ChocolateyBinary { get; set; } = "choco";
        public string ChocolateyLogs { get; set; } = "C:/ProgramData/chocolatey/logs";
        public string WingetBinary { get; set; } = "winget";
        public bool AllowGsudoCache { get; set; } = false;
        public bool WingetMode { get; set; } = true;
        public bool CleanShortcuts { get; set; } = false;
        public bool ElevateOnDemand { get; set; } = true;
        public bool SupressAdminWarning { get; set; } = false;
        public bool SupressLocaleLogWarning { get; set; } = false;
        public bool CloseAfterUpgrade { get; set; } = false;
        public List<int> ValidExitCodes { get; set; } = [0, 1641, 3010, 350, 1604];

        public LegacySettingsDefinition() { }

        public SettingsDefinition ToDefinition()
        {
            SettingsDefinition definition = new();
            definition.Winget.Enabled = WingetMode;
            definition.Winget.Filepath = WingetBinary;
            definition.Winget.UpgradeAsAdmin = ElevateOnDemand;
            definition.Chocolatey.Enabled = !WingetMode;
            definition.Chocolatey.Filepath = ChocolateyBinary;
            definition.Chocolatey.UpgradeAsAdmin = ElevateOnDemand;
            definition.Chocolatey.ValidExitCodes = ValidExitCodes;
            definition.Gsudo.Enabled = ElevateOnDemand;
            definition.Gsudo.CachePrivileges = AllowGsudoCache;
            definition.CleanShortcuts = CleanShortcuts;
            definition.CloseAfterUpgrade = CloseAfterUpgrade;
            definition.SupressNoRightsWarning = SupressAdminWarning;

            return definition;
        }
    }
}
