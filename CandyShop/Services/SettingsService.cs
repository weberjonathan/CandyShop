using CandyShop.PackageCore;
using CandyShop.Properties;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CandyShop.Services
{
    internal class PackageManagerDefinition
    {
        public string Name { get; set; }
        public string Filepath { get; set; }
        public List<int> ValidExitCodes { get; set; }
    }

    internal class GsudoDefinition
    {
        public string Filepath { get; set; }
        public bool EnableCredentialsStore { get; set; }
    }
    
    internal class SettingsDefinition
    {
        public List<PackageManagerDefinition> PackageManagers { get; set; } = [
            new PackageManagerDefinition()
            {
                Name = "Winget",
                Filepath = "winget",
                ValidExitCodes = [ 0 ]
            },
            new PackageManagerDefinition()
            {
                Name = "Chocolatey",
                Filepath = "chocolatey",
                ValidExitCodes = [0, 1641, 3010, 350, 1604]
            }
        ];

        public GsudoDefinition Gsudo { get; set; } = new() {
            Filepath = "gsudo",
            EnableCredentialsStore = false
        };

        public string ActivePackageManager { get; set; } = "Winget";
        public bool ElevateOnDemand { get; set; } = true;
        public bool CleanShortcuts { get; set; } = false;
        public bool CloseAfterUpgrade { get; set; } = false;
        public bool SupressNoRightsWarning { get; set; } = false;
    }

    internal class LegacySettingsDefinition
    {
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
        public List<int> ValidExitCodes { get; set; } = new List<int> { 0, 1641, 3010, 350, 1604 };
    }

    // TODO version methods should be awaitable
    // TODO exception vs null return on error in version methods
    internal class SettingsService
    {
        // TODO
        private static readonly string _AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CandyShop");
        private static readonly string _ConfigFilepath = Path.Combine(_AppDataDir, "CandyShop.config");

        private SettingsDefinition CurrentSettings;

        public SettingsDefinition Load(CandyShopContext context)
        {
            // load settings
            string json = LoadSettingsRaw(); // TODO null check
            SettingsDefinition settings = ParseSettings(json);

            if (settings == null)
            {
                settings = ParseLegacySettings(json);
                context.FirstStart = true;
            }

            if (settings == null)
            {
                settings ??= new();
                context.FirstStart = true;
            }

            // apply to context
            // TODO context needs rework
            context.ChocolateyBinary = settings.PackageManagers.Where(p => p.Name.Equals("Chocolatey")).First().Filepath;
            //context.CholoateyLogFolder = settings.ChocolateyLogs; // TODO
            context.WingetBinary = settings.PackageManagers.Where(p => p.Name.Equals("Winget")).First().Filepath;
            context.AllowGsudoCache = settings.Gsudo.EnableCredentialsStore;
            context.CleanShortcuts = settings.CleanShortcuts;
            context.ElevateOnDemand = settings.ElevateOnDemand;
            context.SupressAdminWarning = settings.SupressNoRightsWarning;
            context.SupressLocaleLogWarning = false;
            context.CloseAfterUpgrade = settings.CloseAfterUpgrade;
            context.WingetMode = settings.ActivePackageManager.Equals("Winget");
            context.ValidExitCodes = settings.PackageManagers.Where(p => p.Name.Equals("Chocolatey")).First().ValidExitCodes;

            CurrentSettings = settings;
            return settings;
        }

        public void Write()
        {
            // Write CurrentSettings field
        }

        public bool FileExists(string path, bool includePath = true)
        {
            return File.Exists(path) || (includePath && TryDetectOnPath(path, out _));
        }

        public bool TryDetectOnPath(string name, out string path)
        {
            path = null;

            if (Path.IsPathFullyQualified(name))
                return false;

            name = Path.GetFileName(name);
            if (!Path.HasExtension(name))
                name = $"{name}.exe";

            path = Environment.GetEnvironmentVariable("PATH") // TODO target
                .Split(';')
                .Select(dir => Path.Combine(dir, name))
                .Where(File.Exists)
                .FirstOrDefault();

            return path != null;
        }

        public string GetWingetVersion(string binary)
        {
            string version = null;
            var p = new PackageManagerProcess(binary, "--version");
            try
            {
                p.ExecuteHidden();
                if (p.ExitCode == 0)
                {
                    version = p.Output.Trim();
                    if (!version.StartsWith('v') || !HasDots(version, 2) || !IsNumeric(version[1..]))
                        version = null;
                }
                else
                {
                    throw new PackageManagerException();
                }
            }
            catch (Exception)
            {
                Log.Error(LocaleEN.ERROR_CHOCO_PATH);
            }

            return version;
        }

        public string GetChocoVersion(string binary)
        {
            string version = null;
            var p = new PackageManagerProcess(binary, "--version --limit-output");
            try
            {
                p.ExecuteHidden();
                if (p.ExitCode == 0)
                {
                    version = p.Output.Trim();
                    if (!HasDots(version, 2) || !IsNumeric(version))
                    {
                        version = null;
                    }
                    else
                    {
                        // TODO manager requires major version
                        version = $"v{version}";
                    }
                }
                else
                {
                    throw new PackageManagerException();
                }
            }
            catch (Exception)
            {
                Log.Error(LocaleEN.ERROR_CHOCO_PATH);
            }

            return version;
        }

        public string GetGsudoVersion(string binary)
        {
            string version = null;
            var p = new PackageManagerProcess(binary, "--version --limit-output");
            try
            {
                p.ExecuteHidden();
                if (p.ExitCode == 0)
                {
                    var output = p.Output.Trim().Split(' ');
                    if (output.Length > 1 && output[0].Equals("gsudo") && output[1].StartsWith('v') && HasDots(output[1], 2) && IsNumeric(output[1][1..]))
                    {
                        version = output[1];
                    }
                    else
                    {
                        version = null;
                    }
                }
                else
                {
                    throw new PackageManagerException();
                }
            }
            catch (Exception)
            {
                Log.Error(LocaleEN.ERROR_CHOCO_PATH);
            }

            return version;
        }

        private string LoadSettingsRaw()
        {
            if (File.Exists(_ConfigFilepath))
            {
                try
                {
                    using FileStream fs = new(_ConfigFilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using StreamReader sr = new(fs);
                    return sr.ReadToEnd();
                }
                catch (Exception e)
                {
                    Log.Error($"An error occurred while reading properties file {_ConfigFilepath}: {e.Message}");
                }
            }

            return null;
        }

        private SettingsDefinition ParseSettings(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<SettingsDefinition>(json, new JsonSerializerOptions()
                {
                    UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow
                });
            }
            catch (JsonException)
            {
                Log.Error("Failed to parse settings file using modern format.");
            }

            return null;
        }
        
        private SettingsDefinition ParseLegacySettings(string json)
        {
            SettingsDefinition loaded = null;
            try
            {
                var legacy = JsonSerializer.Deserialize<LegacySettingsDefinition>(json);
                loaded = new();
                loaded.PackageManagers.Where(p => p.Name.Equals("Winget")).First().Filepath = legacy.WingetBinary;
                loaded.PackageManagers.Where(p => p.Name.Equals("Chocolatey")).First().Filepath = legacy.ChocolateyBinary;
                loaded.PackageManagers.Where(p => p.Name.Equals("Chocolatey")).First().ValidExitCodes = legacy.ValidExitCodes;
                loaded.ActivePackageManager = legacy.WingetMode ? "Winget" : "Chocolatey";
                loaded.Gsudo.EnableCredentialsStore = legacy.AllowGsudoCache;
                loaded.ElevateOnDemand = legacy.ElevateOnDemand;
                loaded.CleanShortcuts = legacy.CleanShortcuts;
                loaded.CloseAfterUpgrade = legacy.CloseAfterUpgrade;
                loaded.SupressNoRightsWarning = legacy.SupressAdminWarning;
            }
            catch (JsonException)
            {
                Log.Error("Failed to parse settings file using legacy format.");
            }

            return loaded;
        }

        private bool HasDots(string value, int n)
        {
            return value.Where(c => c.Equals('.')).Count() == n;
        }

        private bool IsNumeric(string value, char separator = '.')
        {
            return value.Where(c => !c.Equals(separator) || !char.IsNumber(c)).Any();
        }
    }
}
