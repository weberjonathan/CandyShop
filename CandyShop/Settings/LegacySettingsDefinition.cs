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
            SettingsDefinition loaded = new();
            loaded.Winget.Filepath = WingetBinary;
            loaded.Chocolatey.Filepath = ChocolateyBinary;
            loaded.Chocolatey.ValidExitCodes = ValidExitCodes;
            loaded.ActivePackageManager = WingetMode ? "Winget" : "Chocolatey";
            loaded.Gsudo.CachePrivileges = AllowGsudoCache;
            loaded.ElevateOnDemand = ElevateOnDemand;
            loaded.CleanShortcuts = CleanShortcuts;
            loaded.CloseAfterUpgrade = CloseAfterUpgrade;
            loaded.SupressNoRightsWarning = SupressAdminWarning;

            return loaded;
        }
    }
}
