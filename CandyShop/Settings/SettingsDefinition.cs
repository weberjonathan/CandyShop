using Serilog;
using System.Collections.Generic;
using System.Text.Json;

namespace CandyShop.Settings
{
    internal class PackageManagerDefinition
    {
        public bool Enabled { get; set; } // TODO use this but validate so that only one is allowed right now
        public bool UpgradeAsAdmin { get; set; } // TODO
        public string Filepath { get; set; }
        public List<int> ValidExitCodes { get; set; }
    }

    internal class GsudoDefinition
    {
        public bool Enabled { get; set; }
        public string Filepath { get; set; }
        public bool CachePrivileges { get; set; } // TODO rename to CachePrivileges
    }

    internal class SettingsDefinition
    {
        public static SettingsDefinition FromJson(string json, JsonSerializerOptions options = null)
        {
            SettingsDefinition definition = null;
            try
            {
                definition = JsonSerializer.Deserialize<SettingsDefinition>(json, options);
            }
            catch (JsonException)
            {
                Log.Error("Failed to parse settings file using modern format.");
            }

            return definition;
        }

        public PackageManagerDefinition Winget { get; set; } = new()
        {
            Enabled = true,
            UpgradeAsAdmin = true,
            Filepath = "winget",
            ValidExitCodes = [0]
        };

        public PackageManagerDefinition Chocolatey { get; set; } = new()
        {
            Enabled = false,
            UpgradeAsAdmin = true,
            Filepath = "chocolatey",
            ValidExitCodes = [0, 1641, 3010, 350, 1604]
        };

        public GsudoDefinition Gsudo { get; set; } = new()
        {
            Enabled = true,
            Filepath = "gsudo",
            CachePrivileges = false
        };

        public string ActivePackageManager { get; set; } = "Winget"; // deprecate this in favor of the enabled stuff on pm defintion
        public bool ElevateOnDemand { get; set; } = true; // deprecate this and use combination of Gsudo.Enabled and field of active package manager
        public bool CleanShortcuts { get; set; } = false;
        public bool CloseAfterUpgrade { get; set; } = false;
        public bool SupressNoRightsWarning { get; set; } = false; // TODO if active pm has upgradeAsAdmin true and we are not launched as admin and we do not have gsudo, this is a configuration error and no longer a warning; this can be tested at the beginning of the upgrade process
        public string FileVersion { get; set; } = "1";

        public PackageManagerDefinition GetActivePm() // TODO can be removed; instead turn it into the getter of active package manager with new enabled fields in pm defintion
        {
            return ActivePackageManager switch
            {
                "Winget" => Winget,
                "Chocolatey" => Chocolatey,
                _ => null,
            };
        }
    }
}
