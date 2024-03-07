using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CandyShop.PackageCore
{
    public class ChocolateyProcess
    {
        public ChocolateyProcess(string binary, string args)
        {
            Binary = binary;
            Arguments = args;
        }

        public string Output { get; private set; } = "";

        public string Binary { get; private set; }

        public string Arguments { get; private set; }

        public int ExitCode { get; private set; }

        public void ExecuteHidden()
        {
            ProcessStartInfo procInfo = new(Binary, Arguments)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process proc = Process.Start(procInfo);
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            
            Output = output;
            ExitCode = proc.ExitCode;
        }

        public void Execute()
        {
            ProcessStartInfo procInfo = new ProcessStartInfo(Binary, Arguments)
            {
                UseShellExecute = false,
            };

            Process proc = Process.Start(procInfo);
            proc.WaitForExit();
            ExitCode = proc.ExitCode;
        }
    }
}
