using CandyShop.PackageCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace CandyShop.Services
{
    // TODO move interface and definitions out into own settings namespace
    // TODO move legacy stuff out into own LegacySettings classes
    // TODO add config file version
    internal interface ISettingsListener
    {
        void OnSettingsChanged(SettingsDefinition settings);
    }

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

        public GsudoDefinition Gsudo { get; set; } = new() {
            Enabled = true,
            Filepath = "gsudo",
            CachePrivileges = false
        };

        public string ActivePackageManager { get; set; } = "Winget"; // deprecate this in favor of the enabled stuff on pm defintion
        public bool ElevateOnDemand { get; set; } = true; // deprecate this and use combination of Gsudo.Enabled and field of active package manager
        public bool CleanShortcuts { get; set; } = false;
        public bool CloseAfterUpgrade { get; set; } = false;
        public bool SupressNoRightsWarning { get; set; } = false; // TODO if active pm has upgradeAsAdmin true and we are not launched as admin and we do not have gsudo, this is a configuration error and no longer a warning; this can be tested at the beginning of the upgrade process

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
        private List<ISettingsListener> Listeners = [];

        // TODO
        private static readonly string _AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CandyShop");
        private static readonly string _ConfigFilepath = Path.Combine(_AppDataDir, "CandyShop.config");

        private SettingsDefinition CurrentSettings;

        public SettingsService()
        {
            if (!Directory.Exists(_AppDataDir))
                Directory.CreateDirectory(_AppDataDir);
        }

        public void RegisterListener(ISettingsListener listener)
        {
            Listeners.Add(listener);
        }

        public void SetSupressNoRightsWarning(bool value)
        {
            CurrentSettings.SupressNoRightsWarning = value;
            Listeners.ForEach(listener => listener.OnSettingsChanged(CurrentSettings));
        }

        public void SetCleanShortcuts(bool value)
        {
            CurrentSettings.CleanShortcuts = value;
            Listeners.ForEach(listener => listener.OnSettingsChanged(CurrentSettings));
        }

        public void SetCloseAfterUpgrade(bool value)
        {
            CurrentSettings.CloseAfterUpgrade = value;
            Listeners.ForEach(listener => listener.OnSettingsChanged(CurrentSettings));
        }

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

        // TODO remove this and use OnSettingsChanged to update settings view
        public SettingsDefinition GetCurrentSettings()
        {
            return CurrentSettings;
        }

        public void UpdateSettings(SettingsDefinition settings)
        {
            CurrentSettings = settings;
            Listeners.ForEach(listener => listener.OnSettingsChanged(CurrentSettings));
        }

        public SettingsDefinition CreateSettings()
        {
            // TODO ensure valid package manager selection
            CurrentSettings = new();
            Listeners.ForEach(listener => listener.OnSettingsChanged(CurrentSettings));
            return CurrentSettings;
        }

        public SettingsDefinition Load()
        {
            // load settings
            string json = LoadSettingsRaw(); // TODO null check
            SettingsDefinition settings = ParseSettings(json);

            if (settings == null)
                settings = ParseLegacySettings(json);

            // TODO ensure valid package manager selection

            CurrentSettings = settings;
            Listeners.ForEach(listener => listener.OnSettingsChanged(CurrentSettings));
            return CurrentSettings;
        }

        public void Write()
        {
            SettingsDefinition settings = CurrentSettings;

            try
            {
                JsonSerializerOptions options = new()
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(_ConfigFilepath, json);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred while saving properties to {_ConfigFilepath}: {e.Message}");
            }
        }

        /// <exception cref="PackageManagerException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public string ValidateActiveSource(SettingsDefinition settings)
        {
            var pm = PackageManagerFactory.Active(settings);
            return ValidatePmBinary(pm);
        }

        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PackageManagerException"></exception>
        public string ValidateWinget(SettingsDefinition settings = null)
        {
            settings ??= CurrentSettings;
            var pm = PackageManagerFactory.Winget(settings);
            return ValidatePmBinary(pm);
        }

        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PackageManagerException"></exception>
        public string ValidateChocolatey(SettingsDefinition settings = null)
        {
            settings ??= CurrentSettings;
            var pm = PackageManagerFactory.Chocolatey(settings);
            return ValidatePmBinary(pm);
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
            return !Util.IsAdmin() && settings.ElevateOnDemand; // TODO for choco pinning it is also required
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

            return "";
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
                loaded.Winget.Filepath = legacy.WingetBinary;
                loaded.Chocolatey.Filepath = legacy.ChocolateyBinary;
                loaded.Chocolatey.ValidExitCodes = legacy.ValidExitCodes;
                loaded.ActivePackageManager = legacy.WingetMode ? "Winget" : "Chocolatey";
                loaded.Gsudo.CachePrivileges = legacy.AllowGsudoCache;
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
        private string ValidatePmBinary(AbstractPackageManager pm)
        {
            if (!PathUtil.FileExists(pm.Binary))
                throw new FileNotFoundException();

            var version = pm.ValidateExec();
            return version;
        }
    }
}
