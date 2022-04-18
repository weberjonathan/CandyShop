using CandyShop.Properties;
using Serilog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace CandyShop
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // configure logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .CreateLogger();

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
                    LocaleEN.ERROR_CHOCO_PATH,
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // launch application
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CandyShopApplicationContext context = new CandyShopApplicationContext();
            Application.Run(context);
        }
    }
}
