using System;
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

        private static bool ContainsLaunchOption(params string[] options)
        {
            return Environment.GetCommandLineArgs().Intersect(options).Count() > 0;
        }

        public CandyShopContext()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                HasAdminPrivileges = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            LaunchedMinimized = ContainsLaunchOption(OPTION_BACKGROUND, OPTION_BACKGROUND_SHORT);
        }

        public bool LaunchedMinimized { get; set; } = false;

        public bool HasAdminPrivileges { get; set; } = false;

        public string CholoateyLogFolder { get; set; } = "C:\\ProgramData\\chocolatey\\logs";
    }
}
