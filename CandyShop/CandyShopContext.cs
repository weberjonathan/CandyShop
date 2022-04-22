using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace CandyShop
{
    /// <summary>
    /// Determines and contains relevant information for the execution of CandyShop, such as command-line options
    /// </summary>
    internal class CandyShopContext
    {
        private const string OPTION_BACKGROUND = "--background";
        private const string OPTION_BACKGROUND_SHORT = "-b";
        private readonly string _LogFilepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CandyShop.log");

        public CandyShopContext()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                HasAdminPrivileges = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            ParseArguments();
        }

        public string LogFilepath => _LogFilepath;

        public bool LaunchedMinimized { get; set; } = false;

        public bool HasAdminPrivileges { get; set; } = false;

        public string CholoateyLogFolder { get; set; } = "C:\\ProgramData\\chocolatey\\logs";

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
                    default:
                        break;
                }
            }
        }
    }
}
