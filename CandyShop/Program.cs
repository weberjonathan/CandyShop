using System;
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
                Application.Run(new CandyShopForm());
            }
        }
    }
}