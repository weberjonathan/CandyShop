using Serilog;
using System.Diagnostics;
using System.Text;

namespace CandyShop.PackageCore
{
    public class PackageManagerProcess
    {
        public PackageManagerProcess(string binary, string args)
        {
            Binary = binary;
            Arguments = args;
        }

        public string Output { get; private set; } = "";

        public string Binary { get; private set; }

        public string Arguments { get; private set; } = "";

        public int ExitCode { get; private set; }

        public void ExecuteHidden()
        {
            ProcessStartInfo procInfo = new(Binary, Arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.Default
            };

            Process proc = Process.Start(procInfo);
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            
            Output = output;
            ExitCode = proc.ExitCode;

            string stdout = $"Stdout:\n{Output}";
            string stderr = proc.StandardError.ReadToEnd().Trim();
            if (!stderr.Equals(""))
                stderr = $"\nStderr:\n{stderr}";

            Log.Debug($"Process \"{Binary} {Arguments}\" finished with {ExitCode}. {stdout}{stderr}");
        }

        public void Execute()
        {
            ProcessStartInfo procInfo = new(Binary, Arguments)
            {
                UseShellExecute = false,
            };

            Process proc = Process.Start(procInfo);
            proc.WaitForExit();
            ExitCode = proc.ExitCode;
        }
    }
}
