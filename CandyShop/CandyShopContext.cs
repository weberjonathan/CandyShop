using CandyShop.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;

namespace CandyShop
{
    public class MetaInfo
    {
        private static readonly Version VersionObject = Assembly.GetExecutingAssembly().GetName().Version;

        public static string Name => Application.ProductName;
        public static string ActiveSource { get; set; } = "Unknown";
        public static string WindowTitle => string.Format(LocaleEN.TEXT_APP_TITLE, Name, ActiveSource, Version);
        public static string Version = $"{VersionObject.Major}.{VersionObject.Minor}";
    }

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
        private const string OPTION_BACKGROUND = "--silent";
        private const string OPTION_BACKGROUND_SHORT = "-s";
        private const string OPTION_BACKGROUND_LEGACY = "--background";
        private const string OPTION_BACKGROUND_LEGACY_SHORT = "-b";
        private const string OPTION_DEBUG = "--debug";
        private const string OPTION_ENABLE_SELF_UPDATE = "--enable-self-update";

        private static readonly string _LogFilepath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CandyShop"), "CandyShop.log");

        public CandyShopContext()
        {
            // TODO
            //if (!Directory.Exists(_AppDataDir)) Directory.CreateDirectory(_AppDataDir);

            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                HasAdminPrivileges = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            ParseArguments();
        }

        public string ConfigFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CandyShop"); // TODO

        public string LogFilepath => _LogFilepath;

        public bool HasAdminPrivileges { get; set; } = false;

        public bool FirstStart { get; set; } = false;

        // ----------------- set through arguments -----------------

        public bool LaunchedMinimized { get; set; } = false;

        public bool DebugEnabled { get; private set; } = false;

        public bool SelfUpdateEnabled { get; private set; } = false;

        // -------------- set through properties file --------------

        public bool CleanShortcuts { get; set; }
        
        public bool ElevateOnDemand { get; set; }

        public bool SupressAdminWarning { get; set; }

        public bool CloseAfterUpgrade { get; set; }

        public bool WingetMode { get; set; }

        // ---------------------------------------------------------

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
                    case OPTION_BACKGROUND_LEGACY:
                        LaunchedMinimized = true;
                        break;
                    case OPTION_BACKGROUND_LEGACY_SHORT:
                        LaunchedMinimized = true;
                        break;
                    case OPTION_DEBUG:
                        DebugEnabled = true;
                        break;
                    case OPTION_ENABLE_SELF_UPDATE:
                        SelfUpdateEnabled = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
