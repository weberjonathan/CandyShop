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
            context?.StopPropertiesFileWatcher();
            context?.SaveProperties();
            Environment.Exit(code);
        }

        [STAThread]
        static void Main(string[] args)
        {
            // configure logger
            var fileLogger = GetFileLoggerConfiguration().CreateLogger();
            var debugLogger = new LoggerConfiguration().WriteTo.Debug().MinimumLevel.Debug().CreateLogger();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Logger(debugLogger)
                .WriteTo.Logger(fileLogger)
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

        private static LoggerConfiguration GetFileLoggerConfiguration()
        {
            string filepath = GetLogFilename();

            LoggerConfiguration config = new LoggerConfiguration()
                .WriteTo.File(
                    filepath,
                    Serilog.Events.LogEventLevel.Debug,
                    "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
            );

            return context.DebugEnabled ? config.MinimumLevel.Debug() : config.MinimumLevel.Information();
        }

        private static string GetLogFilename()
        {
            string path = context.LogFilepath;

            int i = 1;
            while (true)
            {
                if (File.Exists(path) && IsFileLocked(path))
                {
                    // logfile is in use, so try next
                    string dir = Path.GetDirectoryName(Path.GetFullPath(path));
                    string filename = Path.GetFileNameWithoutExtension(context.LogFilepath) + (i++) + Path.GetExtension(context.LogFilepath);
                    path = Path.Combine(dir, filename);
                }
                else
                {
                    break;
                }
            }

            return path;
            
        }

        private static bool IsFileLocked(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    fs.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }
    }
}
