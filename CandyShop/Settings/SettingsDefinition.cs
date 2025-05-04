using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CandyShop.Settings
{
    internal class PackageManagerDefinition
    {
        public string Name { get; set; }
        public bool Enabled { get; set; } // TODO use this but validate so that only one is allowed right now
        public bool UpgradeAsAdmin { get; set; } // TODO
        public string Filepath { get; set; }
        public List<int> ValidExitCodes { get; set; }
    }

    internal class GsudoDefinition
    {
        public bool Enabled { get; set; }
        public string Filepath { get; set; }
        public bool CachePrivileges { get; set; }
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

        public List<PackageManagerDefinition> PackageManagers { get; set; } = [
            new PackageManagerDefinition()
            {
                Name = "Winget",
                Enabled = true,
                UpgradeAsAdmin = true,
                Filepath = "winget",
                ValidExitCodes = [0]
            },
            new PackageManagerDefinition()
            {
                Name = "Chocolatey",
                Enabled = false,
                UpgradeAsAdmin = true,
                Filepath = "chocolatey",
                ValidExitCodes = [0, 1641, 3010, 350, 1604]
            }
        ];

        public GsudoDefinition Gsudo { get; set; } = new()
        {
            Enabled = true,
            Filepath = "gsudo",
            CachePrivileges = false
        };

        public bool ElevateOnDemand { get; set; } = true; // deprecate this and use combination of Gsudo.Enabled and field of active package manager
        public bool CleanShortcuts { get; set; } = false;
        public bool CloseAfterUpgrade { get; set; } = false;
        public bool SupressNoRightsWarning { get; set; } = false; // TODO if active pm has upgradeAsAdmin true and we are not launched as admin and we do not have gsudo, this is a configuration error and no longer a warning; this can be tested at the beginning of the upgrade process
        public string FileVersion { get; set; } = "1";

        [JsonIgnore]
        public PackageManagerDefinition Winget => PackageManagers.First(pm => pm.Name.Equals("Winget"));

        [JsonIgnore]
        public PackageManagerDefinition Chocolatey => PackageManagers.First(pm => pm.Name.Equals("Chocolatey"));

        [JsonIgnore]
        public List<PackageManagerDefinition> EnabledPackageManagers => PackageManagers.Where(pm => pm.Enabled).ToList();
    }
}
