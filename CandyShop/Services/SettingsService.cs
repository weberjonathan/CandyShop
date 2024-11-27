using CandyShop.PackageCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        // TODO replace packageManagers with list and provide simple, strongly-tpyed, transient accessors for Choco and Winget
        public Dictionary<string, PackageManagerDefinition> PackageManagers { get; set; } = new() {
            {
                "Winget", new PackageManagerDefinition()
                {
                    Name = "Winget",
                    Filepath = "winget",
                    ValidExitCodes = [ 0 ]
                }
            },
            {
                "Chocolatey", new PackageManagerDefinition()
                {
                    Name = "Chocolatey",
                    Filepath = "chocolatey",
                    ValidExitCodes = [0, 1641, 3010, 350, 1604]
                }
            }
        };

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

    // TODO validation methods should be awaitable
    internal class SettingsService
    {
        // TODO
        private static readonly string _AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CandyShop");
        private static readonly string _ConfigFilepath = Path.Combine(_AppDataDir, "CandyShop.config");

        private SettingsDefinition CurrentSettings;

        /// <exception cref="CandyShopException"></exception>
        public void OpenSettingsDirectory()
        {
            if (!Directory.Exists(_AppDataDir))
                throw new CandyShopException("Candy Shop appdata directory does not exist.");

            try
            {
                Process.Start("explorer.exe", _AppDataDir);
            }
            catch (Exception ex)
            {
                throw new CandyShopException(ex.Message);
            }
        }

        public SettingsDefinition GetCurrentSettings()
        {
            return CurrentSettings;
        }

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
            context.CleanShortcuts = settings.CleanShortcuts;
            context.ElevateOnDemand = settings.ElevateOnDemand;
            context.SupressAdminWarning = settings.SupressNoRightsWarning;
            context.SupressLocaleLogWarning = false;
            context.CloseAfterUpgrade = settings.CloseAfterUpgrade;
            context.WingetMode = settings.ActivePackageManager.Equals("Winget");

            CurrentSettings = settings;
            return settings;
        }

        public void Write()
        {
            // Write CurrentSettings field
        }

        /// <exception cref="PackageManagerException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public string ValidateActiveSource(out AbstractPackageManager validatedPm)
        {
            return ValidateActiveSource(CurrentSettings, out validatedPm);
        }

        /// <exception cref="PackageManagerException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public string ValidateActiveSource(SettingsDefinition settings, out AbstractPackageManager validatedPm)
        {
            return ValidatePmBinary(settings.ActivePackageManager, settings, out validatedPm);
        }

        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PackageManagerException"></exception>
        public string ValidateWinget(SettingsDefinition settings = null)
        {
            settings ??= CurrentSettings;
            return ValidatePmBinary("Winget", settings, out _);
        }

        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PackageManagerException"></exception>
        public string ValidateChocolatey(SettingsDefinition settings = null)
        {
            settings ??= CurrentSettings;
            return ValidatePmBinary("Chocolatey", settings, out _);
        }

        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PackageManagerException"></exception>
        public string ValidateGsudo(SettingsDefinition settings = null)
        {
            settings ??= CurrentSettings;

            if (!PathUtil.FileExists(settings.Gsudo.Filepath))
                throw new FileNotFoundException();

            var p = new PackageManagerProcess(settings.Gsudo.Filepath, "--version"); // TODO this should not abuse the package manager process
            try
            {
                p.ExecuteHidden();
            }
            catch (Exception)
            {
                throw new PackageManagerException("Failed to execute gsudo");
            }

            if (p.ExitCode != 0)
                throw new PackageManagerException($"Gsudo did not exit cleanly: {p.ExitCode}");

            var output = p.Output.Trim().Split(' ');
            if (output.Length > 1 && output[0].Equals("gsudo") && output[1].StartsWith('v') && Util.HasDots(output[1], 2) && Util.IsNumeric(output[1][1..]))
                return output[1];

            throw new PackageManagerException("Failed to parse gsudo version");
        }

        public bool IsGsudoRequired(SettingsDefinition settings = null)
        {
            settings ??= CurrentSettings;
            return !Util.IsAdmin() && settings.ElevateOnDemand;
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
                loaded.PackageManagers["Winget"].Filepath = legacy.WingetBinary;
                loaded.PackageManagers["Chocolatey"].Filepath = legacy.ChocolateyBinary;
                loaded.PackageManagers["Chocolatey"].ValidExitCodes = legacy.ValidExitCodes;
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

        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PackageManagerException"></exception>
        private string ValidatePmBinary(string name, SettingsDefinition settings, out AbstractPackageManager validatedPm)
        {
            if (!settings.PackageManagers.TryGetValue(name, out PackageManagerDefinition pmDefinition))
                throw new ArgumentOutOfRangeException(nameof(name));

            if (!PathUtil.FileExists(pmDefinition.Filepath))
                throw new FileNotFoundException();

            var pm = PackageManagerFactory.Create(pmDefinition, settings);
            var version = pm.ValidateExec();
            validatedPm = pm;
            return version;
        }
    }
}
