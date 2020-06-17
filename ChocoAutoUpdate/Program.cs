using System;
using System.Security.Principal;
using System.Windows.Forms;

namespace ChocoAutoUpdate
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // check whether app is elevated (admin privileges)
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            // check admin option
            bool hideAdminWarn = false;
            foreach (string arg in args)
            {
                if (arg.Equals("--hide-admin-warn"))
                {
                    hideAdminWarn = true;
                }
            }

            

            // launch application
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApplicationContext appContext = new ApplicationContext
            {
                HideAdminWarn = hideAdminWarn,
                IsElevated = isElevated
            };
            Application.Run(appContext);
        }
    }
}
