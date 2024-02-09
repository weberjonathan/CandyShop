using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace CandyShop.Winget
{
    public class WingetProcess
    {
        public WingetProcess(string binary, string args)
        {
            Binary = binary;
            Args = args;
        }

        public string Output { get; private set; } = "";

        public string Binary { get; private set; }

        public string Args { get; private set; } = "";

        public int ExitCode { get; private set; }

        /// <summary>
        ///     Executes the winget process without creating a window
        ///     and writes stdout to the Output property after execution
        /// </summary>
        /// <exception cref="PackageManagerException"></exception>
        public void ExecuteHidden()
        {
            ProcessStartInfo procInfo = new ProcessStartInfo(Binary, Args)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.Default
            };

            try
            {
                Process proc = Process.Start(procInfo);
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                Output = output;
                ExitCode = proc.ExitCode;

                if (proc.ExitCode != 0)
                {
                    // TODO what's in output => add property for stderr? put stderr in output?
                    throw new WingetException($"winget did not exit cleanly ({proc.ExitCode})\n\n{Output}");
                }
            }
            catch (Win32Exception e)
            {
                // TODO what's in output => add property for stderr? put stderr in output?
                throw new WingetException("An error occurred while running choco.", e);
            }
        }

        /// <summary>
        ///     Executes the winget process in a new console
        /// </summary>
        /// <exception cref="PackageManagerException"></exception>
        public void Execute()
        {
            // TODO potentially redirect output and expose events

            ProcessStartInfo procInfo = new ProcessStartInfo(Binary, Args)
            {
                UseShellExecute = false,
            };

            try
            {
                Process proc = Process.Start(procInfo);
                proc.WaitForExit();
                ExitCode = proc.ExitCode;

                if (proc.ExitCode != 0)
                {
                    throw new WingetException($"winget did not exit cleanly. Returned {proc.ExitCode}.");
                }
            }
            catch (Win32Exception e)
            {
                throw new WingetException("An error occurred while running choco.", e);
            }
        }
    }
}
