using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CandyShop
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

            // prepare launch 
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // check if Chocolatey is in path
            try
            {
                ProcessStartInfo pi = new ProcessStartInfo("choco", "--version")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process p = Process.Start(pi);
                p.WaitForExit();
            }
            catch (Win32Exception)
            {
                MessageBox.Show(
                    "Error: An error occurred while starting the Chocolatey application. Please make sure it is installed and in PATH.",
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // launch application
            if (SILENT_MODE)
            {
                Application.Run(new ChocoAutoUpdateTray());
            }
            else
            {
                Application.Run(new CandyShopForm());
            }
        }
    }
}
