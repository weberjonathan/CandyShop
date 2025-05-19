using CandyShop.PackageCore;
using CandyShop.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CandyShop.Services
{
    internal class SettingsService
    {
        private static readonly string _AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CandyShop");
        private static readonly string _ConfigFilepath = Path.Combine(_AppDataDir, "CandyShop.config");
        private static readonly JsonSerializerOptions JsonDisallowUnknownMembers = new()
        {
            UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow
        };

        private List<ISettingsListener> Listeners = [];
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

        public void ToggleActivePackageManager()
        {
            if (CurrentSettings.PackageManagers.Count != 2)
            {
                Log.Warning($"Cannot toggle between package managers because more than two exist.");
                return;
            }

            foreach (var pm in CurrentSettings.PackageManagers)
            {
                pm.Enabled = !pm.Enabled;
            }
        }

        public bool HaveEnabledPackageManagersChanged(SettingsDefinition newSettings)
        {
            var current = CurrentSettings.EnabledPackageManagers;
            var updated = newSettings.EnabledPackageManagers;
            return !current.Zip(updated).All(pms => pms.First.Name.Equals(pms.Second.Name));
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

        public void UpdateSettings(SettingsDefinition settings)
        {
            CurrentSettings = settings;
            Listeners.ForEach(listener => listener.OnSettingsChanged(CurrentSettings));
        }

        public SettingsDefinition CreateSettings()
        {
            CurrentSettings = ValidateAndFixSettingsDefinition(new());
            Listeners.ForEach(listener => listener.OnSettingsChanged(CurrentSettings));
            return CurrentSettings;
        }

        public SettingsDefinition Load(out bool requiresReview)
        {
            requiresReview = false;

            // read settings file to json text
            string json = ReadSettingsFile();
            if (json == null)
            {
                requiresReview = true;
                return null;
            }

            // parse json
            SettingsDefinition settings = SettingsDefinition.FromJson(json, JsonDisallowUnknownMembers);
            if (settings == null)
            {
                requiresReview = true;
                var legacySettings = LegacySettingsDefinition.FromJson(json);
                settings = legacySettings?.ToDefinition();
            }

            if (settings == null)
            {
                requiresReview = true;
                return null;
            }

            settings = ValidateAndFixSettingsDefinition(settings);

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
        public async Task<string> ValidateActiveSource(SettingsDefinition settings)
        {
            var pm = PackageManagerFactory.Active(settings);
            return await ValidatePmBinary(pm);
        }

        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PackageManagerException"></exception>
        public async Task<string> ValidateWinget(SettingsDefinition settings = null)
        {
            settings ??= CurrentSettings;
            var pm = PackageManagerFactory.Winget(settings);
            return await ValidatePmBinary(pm);
        }

        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PackageManagerException"></exception>
        public async Task<string> ValidateChocolatey(SettingsDefinition settings = null)
        {
            settings ??= CurrentSettings;
            var pm = PackageManagerFactory.Chocolatey(settings);
            return await ValidatePmBinary(pm);
        }

        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PackageManagerException"></exception>
        public async Task<string> ValidateGsudo(SettingsDefinition settings = null)
        {
            settings ??= CurrentSettings;

            if (!PathUtil.FileExists(settings.Gsudo.Filepath))
                throw new FileNotFoundException();

            return await Task.Run(() =>
            {
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
            });
        }

        /// <summary>
        /// True if the enabled package manager requires admin privileges,
        /// but the application was launched without them
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public bool RequireGsudoForUpgrade(SettingsDefinition settings = null)
        {
            settings ??= CurrentSettings;
            return !Util.IsAdmin() && settings.EnabledPackageManagers.First().UpgradeAsAdmin;
        }

        public bool RequireAdminPrivilegesForUpgrade(SettingsDefinition settings = null)
        {
            settings ??= CurrentSettings;
            return settings.EnabledPackageManagers.First().UpgradeAsAdmin;
        }

        public bool IsGsudoEnabled(SettingsDefinition settings = null)
        {
            settings ??= CurrentSettings;
            return settings.Gsudo.Enabled;
        }

        public bool IsGsudoCacheEnabled(SettingsDefinition settings = null)
        {
            settings ??= CurrentSettings;
            return settings.Gsudo.CachePrivileges;
        }

        private string ReadSettingsFile()
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

        private SettingsDefinition ValidateAndFixSettingsDefinition(SettingsDefinition settings)
        {
            // validate winget
            if (settings.Winget == null)
            {
                settings.PackageManagers.Insert(0, PackageManagerDefinition.BuildWinget());
            }

            // validate chocolatey
            if (settings.Chocolatey == null)
            {
                settings.PackageManagers.Add(PackageManagerDefinition.BuildChocolatey());
            }

            // remove unknown package managers
            settings.PackageManagers = settings.PackageManagers
                .Where(pm => pm.Name.Equals(settings.Winget.Name) || pm.Name.Equals(settings.Chocolatey.Name))
                .ToList();

            // ensure at least one package source is enabled
            if (settings.EnabledPackageManagers.Count == 0)
            {
                Log.Warning($"No package manager is enabled, defaulting to winget.");
                settings.Winget.Enabled = true;
            }

            // disable chocolatey if both are enabled
            if (settings.EnabledPackageManagers.Count > 1)
            {
                Log.Warning($"Too many package managers are enabled, defaulting to winget.");
                foreach (var pm in settings.EnabledPackageManagers)
                    if (!pm.Name.Equals(settings.Winget.Name))
                        pm.Enabled = false;
            }

            return settings;
        }

        /// <exception cref="PackageManagerException"></exception>
        private async Task<string> ValidatePmBinary(AbstractPackageManager pm)
        {
            return await Task.Run(pm.ValidateExec);
        }
    }
}
