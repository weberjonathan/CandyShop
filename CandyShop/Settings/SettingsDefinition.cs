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
        public bool Enabled { get; set; }
        public bool UpgradeAsAdmin { get; set; }
        public string Filepath { get; set; }
        public List<int> ValidExitCodes { get; set; }

        public static PackageManagerDefinition BuildWinget()
        {
            return new()
            {
                Name = "Winget",
                Enabled = true,
                UpgradeAsAdmin = true,
                Filepath = "winget",
                ValidExitCodes = [0]
            };
        }

        public static PackageManagerDefinition BuildChocolatey()
        {
            return new()
            {
                Name = "Chocolatey",
                Enabled = false,
                UpgradeAsAdmin = true,
                Filepath = "chocolatey",
                ValidExitCodes = [0, 1641, 3010, 350, 1604]
            };
        }
    }

    internal class GsudoDefinition
    {
        public bool Enabled { get; set; }
        public string Filepath { get; set; }
        public bool CachePrivileges { get; set; }
        public bool OverwriteCacheDuration { get; set; } = false;
        public int CacheDurationInSeconds { get; set; } = 5*60;
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
            PackageManagerDefinition.BuildWinget(),
            PackageManagerDefinition.BuildChocolatey()
        ];

        public GsudoDefinition Gsudo { get; set; } = new()
        {
            Enabled = true,
            Filepath = "gsudo",
            CachePrivileges = false,
            CacheDurationInSeconds = 5*60,
            OverwriteCacheDuration = false,
        };

        public bool CleanShortcuts { get; set; } = false;
        public bool CloseAfterUpgrade { get; set; } = false;
        public string FileVersion { get; set; } = "1";

        [JsonIgnore]
        public PackageManagerDefinition Winget => PackageManagers.First(pm => pm.Name.Equals("Winget"));

        [JsonIgnore]
        public PackageManagerDefinition Chocolatey => PackageManagers.First(pm => pm.Name.Equals("Chocolatey"));

        [JsonIgnore]
        public List<PackageManagerDefinition> EnabledPackageManagers => PackageManagers.Where(pm => pm.Enabled).ToList();
    }
}
