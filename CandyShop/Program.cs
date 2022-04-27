using CandyShop.Properties;
using Serilog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CandyShop
{
    static class Program
    {
        private static readonly CandyShopContext context = new CandyShopContext();

        public static void Exit(int code = 0)
        {
            context?.Save();
            Environment.Exit(code);
        }

        [STAThread]
        static void Main(string[] args)
        {
            // TODO create logfile with suffix if cant get access to log file

            // configure logger
            var logToFileConfig = new LoggerConfiguration().WriteTo.File(context.LogFilepath);
            if (context.DebugEnabled)
            {
                logToFileConfig.MinimumLevel.Debug();
            }
            else
            {
                logToFileConfig.MinimumLevel.Information();
            }

            var logToDebugConsoleConfig = new LoggerConfiguration().WriteTo.Debug().MinimumLevel.Debug();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Logger(logToDebugConsoleConfig.CreateLogger())
                .WriteTo.Logger(logToFileConfig.CreateLogger())
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

            CandyShopApplicationContext appContext = new CandyShopApplicationContext(context);
            Application.Run(appContext);
        }
    }
}
