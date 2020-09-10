using System;
using System.Security.Principal;
using System.Windows.Forms;

namespace ChocoAutoUpdate
{
    static class Program
    {
        private static bool ADMIN_MODE;
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
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                ADMIN_MODE = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            // launch application
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (SILENT_MODE)
            {
                Application.Run(new ChocoAutoUpdateTray());
            }
            else
            {
                Application.Run(new ChocoAutoUpdateForm());
            }
        }

        public static bool IsElevated()
        {
            return ADMIN_MODE;
        }
    }
}
