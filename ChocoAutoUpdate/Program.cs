using System;
using System.Security.Principal;
using System.Windows.Forms;

namespace ChocoAutoUpdate
{
    static class Program
    {
        private static bool SILENT_MODE = false;

        [STAThread]
        static void Main(string[] args)
        {
            // check arguments
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--silent":
                        SILENT_MODE = true;
                        break;
                    case "-s":
                        SILENT_MODE = true;
                        break;
                    default:
                        break;
                }
            }
            
            // check whether app is elevated (admin privileges)
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            // launch application
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (SILENT_MODE)
            {
                ChocoAutoUpdateTray appContext = new ChocoAutoUpdateTray
                {
                    IsElevated = isElevated
                };
                Application.Run(appContext);
            }
            else
            {
                ChocoAutoUpdateForm form = new ChocoAutoUpdateForm
                {
                    IsElevated = isElevated
                };
                Application.Run(form);
            }

            
        }
    }
}
