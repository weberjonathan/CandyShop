using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text.Json;
using System.Threading;

namespace CandyShop
{
    // TODO either inject context or use singleton; not both
    internal class ContextSingleton
    {
        private static CandyShopContext Instance = new CandyShopContext();
        public static CandyShopContext Get => Instance;
    }

    /// <summary>
    /// Determines and contains relevant information for the execution of CandyShop, such as command-line options and settings
    /// </summary>
    internal class CandyShopContext
    {
        private class PropertiesFileContent
        {
            public string ChocolateyBinary { get; set; } = "C:/ProgramData/chocolatey/bin/choco.exe";
            public string ChocolateyLogs { get; set; } = "C:/ProgramData/chocolatey/logs";
            public bool WingetMode { get; set; } = false;
            public bool CleanShortcuts { get; set; } = false;
            public bool ElevateOnDemand { get; set; } = true;
            public bool SupressAdminWarning { get; set; } = false;
            public List<int> ValidExitCodes { get; set; } = new List<int> { 0, 1641, 3010, 350, 1604 };
        }

        private const string OPTION_BACKGROUND = "--background";
        private const string OPTION_BACKGROUND_SHORT = "-b";
        private const string OPTION_DEBUG = "--debug";

        private static readonly string _AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CandyShop");
        private static readonly string _ConfigFilepath = Path.Combine(_AppDataDir, "CandyShop.config");
        private static readonly string _LogFilepath = Path.Combine(_AppDataDir, "CandyShop.log");

        private FileSystemWatcher _ConfigFileWatcher;

        public CandyShopContext()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            ApplicationVersion = $"{v.Major}.{v.Minor}.{v.Build}";


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

        public string ApplicationVersion { get; private set; }

        // ----------------- set through arguments -----------------

        public bool LaunchedMinimized { get; set; } = false;

        public bool DebugEnabled { get; private set; } = false;

        // -------------- set through properties file --------------

        public string ChocolateyBinary { get; private set; }

        public string CholoateyLogFolder { get; private set; }

        public bool CleanShortcuts { get; set; }
        
        public bool ElevateOnDemand { get; set; }

        public bool SupressAdminWarning { get; set; }

        public bool WingetMode { get; private set; }

        public List<int> ValidExitCodes { get; set; }

        // ---------------------------------------------------------

        public void SaveProperties()
        {
            PropertiesFileContent content = new PropertiesFileContent
            {
                ChocolateyBinary = this.ChocolateyBinary,
                ChocolateyLogs = this.CholoateyLogFolder,
                CleanShortcuts = this.CleanShortcuts,
                ElevateOnDemand = this.ElevateOnDemand,
                SupressAdminWarning = this.SupressAdminWarning,
                WingetMode = this.WingetMode,
                ValidExitCodes = this.ValidExitCodes
            };

            WriteProperties(content);
        }

        /// <summary>
        /// Executes the callback when the properties file changed on the disk.
        /// </summary>
        public void OnPropertiesFileChanged(Action callback)
        {
            // init file watcher
            if (_ConfigFileWatcher == null)
            {
                _ConfigFileWatcher = new FileSystemWatcher();
                _ConfigFileWatcher.BeginInit();
                _ConfigFileWatcher.Path = ConfigFolder;
                _ConfigFileWatcher.Filter = Path.GetFileName(_ConfigFilepath);
                _ConfigFileWatcher.IncludeSubdirectories = false;
                _ConfigFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                _ConfigFileWatcher.EnableRaisingEvents = true;
                _ConfigFileWatcher.EndInit();
            }

            // Changed event may be invoked twice on single save due to possibility
            // of multiple write operations by editor used to change the file.
            // To prevent exceptions, delay parsing of properties file
            // also see https://failingfast.io/a-robust-solution-for-filesystemwatcher-firing-events-multiple-times/
            Timer timer = new Timer((state) =>
            {
                Log.Debug("Attempt reloading properties from file.");
                ParseProperties();
                callback.Invoke();
            }, null, Timeout.Infinite, Timeout.Infinite);

            _ConfigFileWatcher.Changed += new FileSystemEventHandler((sender, e) =>
            {
                Log.Information("Changes in config file detected.");
                timer.Change(500, Timeout.Infinite);
            });
        }

        public void StopPropertiesFileWatcher()
        {
            _ConfigFileWatcher?.Dispose();
            _ConfigFileWatcher = null;
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
                    string json = "";
                    using (FileStream fs = new FileStream(_ConfigFilepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        
                        using StreamReader sr = new StreamReader(fs);
                        json = sr.ReadToEnd();
                    }
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
            ChocolateyBinary = content.ChocolateyBinary;
            CholoateyLogFolder = content.ChocolateyLogs;
            CleanShortcuts = content.CleanShortcuts;
            ElevateOnDemand = content.ElevateOnDemand;
            SupressAdminWarning = content.SupressAdminWarning;
            WingetMode = content.WingetMode;
            ValidExitCodes = content.ValidExitCodes;
        }

        private void WriteProperties(PropertiesFileContent content)
        {
            // use forward-slash as path separator
            content.ChocolateyLogs = content.ChocolateyLogs.Replace('\\', '/');
            content.ChocolateyBinary = content.ChocolateyBinary.Replace('\\', '/');

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
