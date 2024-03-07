using System.Diagnostics;
using System.Text;

namespace CandyShop.PackageCore
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
        public void ExecuteHidden()
        {
            ProcessStartInfo procInfo = new(Binary, Args)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.Default
            };

            Process proc = Process.Start(procInfo);
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            
            Output = output;
            ExitCode = proc.ExitCode;
        }

        /// <summary>Executes the winget process in a new console</summary>
        public void Execute()
        {
            ProcessStartInfo procInfo = new(Binary, Args)
            {
                UseShellExecute = false,
            };

            Process proc = Process.Start(procInfo);
            proc.WaitForExit();
            ExitCode = proc.ExitCode;
        }
    }
}
