using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text.Json;

namespace CandyShop
{
    /// <summary>
    /// Determines and contains relevant information for the execution of CandyShop, such as command-line options
    /// </summary>
    internal class CandyShopContext
    {
        private class CandyShopProperties
        {
            public string ChocolateyLogs { get; set; } = "C:\\ProgramData\\chocolatey\\logs";
            public bool CleanShortcuts { get; set; } = false;
        }

        private const string OPTION_BACKGROUND = "--background";
        private const string OPTION_BACKGROUND_SHORT = "-b";
        private const string OPTION_DEBUG = "--debug";
        private static readonly string _AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CandyShop");
        private static readonly string _ConfigFilepath = Path.Combine(_AppDataDir, "CandyShop.config");
        private static readonly string _LogFilepath = Path.Combine(_AppDataDir, "CandyShop.log");
        private CandyShopProperties _Properties = new CandyShopProperties();

        public CandyShopContext()
        {
            Directory.CreateDirectory(_AppDataDir);

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

        public bool LaunchedMinimized { get; set; } = false;

        public bool HasAdminPrivileges { get; set; } = false;

        public string CholoateyLogFolder => _Properties.ChocolateyLogs;

        public bool DebugEnabled { get; private set; } = false;

        public bool CleanShortcuts
        {
            get
            {
                return _Properties.CleanShortcuts;
            }
            set
            {
                _Properties.CleanShortcuts = value;
            }
        }

        public void Save() => WriteProperties();

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
            CandyShopProperties p;

            // read properties
            try
            {
                string json = File.ReadAllText(_ConfigFilepath);
                p = JsonSerializer.Deserialize<CandyShopProperties>(json);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred while reading properties from {_ConfigFilepath}: {e.Message}");
                return;
            }

            // apply valid ones
            if (p.ChocolateyLogs != null && Directory.Exists(p.ChocolateyLogs))
                _Properties.ChocolateyLogs = Path.GetFullPath(p.ChocolateyLogs);

            _Properties.CleanShortcuts = p.CleanShortcuts;
        }

        private void WriteProperties()
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true };
                string json = JsonSerializer.Serialize(_Properties, options);
                File.WriteAllText(_ConfigFilepath, json);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred while saving properties to {_ConfigFilepath}: {e.Message}");
            }
        }
    }
}
