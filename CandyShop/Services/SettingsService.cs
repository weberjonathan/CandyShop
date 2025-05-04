using CandyShop.PackageCore;
using CandyShop.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace CandyShop.Services
{
    // TODO validation methods should be awaitable
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
            // read settings file to json text
            string json = ReadSettingsFile();
            if (json == null)
                return null;

            // parse json
            SettingsDefinition settings = SettingsDefinition.FromJson(json, JsonDisallowUnknownMembers);
            if (settings == null)
            {
                var legacySettings = LegacySettingsDefinition.FromJson(json);
                settings = legacySettings.ToDefinition();
            }

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
