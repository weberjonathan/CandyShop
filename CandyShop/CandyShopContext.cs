using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text.Json;

namespace CandyShop
{
    /// <summary>
    /// Determines and contains relevant information for the execution of CandyShop, such as command-line options and settings
    /// </summary>
    internal class CandyShopContext
    {
        private class PropertiesFileContent
        {
            public string ChocolateyLogs { get; set; } = "C:\\ProgramData\\chocolatey\\logs";
            public bool CleanShortcuts { get; set; } = false;
            public List<int> ValidExitCodes { get; set; } = new List<int> { 0, 1641, 3010, 350, 1604 };
        }

        private const string OPTION_BACKGROUND = "--background";
        private const string OPTION_BACKGROUND_SHORT = "-b";
        private const string OPTION_DEBUG = "--debug";

        private static readonly string _AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CandyShop");
        private static readonly string _ConfigFilepath = Path.Combine(_AppDataDir, "CandyShop.config");
        private static readonly string _LogFilepath = Path.Combine(_AppDataDir, "CandyShop.log");

        public CandyShopContext()
        {
            if (!Directory.Exists(_AppDataDir)) Directory.CreateDirectory(_AppDataDir);

            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                HasAdminPrivileges = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            ParseArguments();
            ParseProperties();
        }

        public string ConfigFolder => _AppDataDir;

        public string LogFilepath => _LogFilepath;

        public bool HasAdminPrivileges { get; set; } = false;

        // ----------------- set through arguments -----------------

        public bool LaunchedMinimized { get; set; } = false;

        public bool DebugEnabled { get; private set; } = false;

        // -------------- set through properties file --------------

        public string CholoateyLogFolder { get; private set; }

        public bool CleanShortcuts { get; set; }

        public List<int> ValidExitCodes { get; set; }

        // ---------------------------------------------------------

        public void Save()
        {
            PropertiesFileContent content = new PropertiesFileContent
            {
                ChocolateyLogs = this.CholoateyLogFolder,
                CleanShortcuts = this.CleanShortcuts,
                ValidExitCodes = this.ValidExitCodes
            };
            
            WriteProperties(content);
        }

        private void ParseArguments()
        {
            Queue<string> arguments = new Queue<string>(Environment.GetCommandLineArgs());
            while (arguments.Count > 0)
            {
                string arg = arguments.Dequeue();
                switch (arg)
                {
                    case OPTION_BACKGROUND:
                        LaunchedMinimized = true;
                        break;
                    case OPTION_BACKGROUND_SHORT:
                        LaunchedMinimized = true;
                        break;
                    case OPTION_DEBUG:
                        DebugEnabled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ParseProperties()
        {
            // initialize with defaults
            PropertiesFileContent content = new PropertiesFileContent();

            // read properties file content
            if (File.Exists(_ConfigFilepath))
            {
                try
                {
                    string json = File.ReadAllText(_ConfigFilepath);
                    PropertiesFileContent parsedContent = JsonSerializer.Deserialize<PropertiesFileContent>(json);
                    content = parsedContent;
                }
                catch (Exception e)
                {
                    Log.Error($"An error occurred while reading properties from {_ConfigFilepath}: {e.Message}");
                }
            }
            else
            {
                WriteProperties(content);
            }

            // apply properties from content
            CholoateyLogFolder = content.ChocolateyLogs;
            CleanShortcuts = content.CleanShortcuts;
            ValidExitCodes = content.ValidExitCodes;
        }

        private void WriteProperties(PropertiesFileContent content)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(content, options);
                File.WriteAllText(_ConfigFilepath, json);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred while saving properties to {_ConfigFilepath}: {e.Message}");
            }
        }
    }
}
