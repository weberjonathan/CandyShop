using System;
using System.Linq;
using System.Security.Principal;

namespace CandyShop
{
    class CandyShopContext
    {
        internal const string OPTION_BACKGROUND = "--background";
        internal const string OPTION_BACKGROUND_SHORT = "-b";

        internal static bool ContainsArguments(params string[] arguments)
        {
            return Environment.GetCommandLineArgs().Intersect(arguments).Count() > 0;
        }

        public CandyShopContext()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                HasAdminPrivileges = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            LaunchedMinimized = ContainsArguments(OPTION_BACKGROUND, OPTION_BACKGROUND_SHORT);
        }

        internal bool LaunchedMinimized { get; set; } = false;

        internal bool HasAdminPrivileges { get; set; } = false;
    }
}
