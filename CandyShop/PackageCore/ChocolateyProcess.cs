using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CandyShop.PackageCore
{
    public class ChocolateyProcess
    {
        public static int MajorVersion { get; set; }

        public ChocolateyProcess(string binary, string args)
        {
            Binary = binary;
            Arguments = args;
        }

        public string Output { get; private set; } = "";

        public string Binary { get; private set; }

        public string Arguments { get; private set; }

        public List<string[]> OutputBySection { get; private set; } = [];

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
            OutputBySection = ParseSectionsFromOutput(Output);
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

        private List<string[]> ParseSectionsFromOutput(string output)
        {
            List<string[]> rtn = new List<string[]>();

            // parse head
            Queue<string> outputLines = new Queue<string>(output.Split("\r\n"));
            if (outputLines.Count > 0)
            {
                if (!outputLines.Dequeue().StartsWith("Chocolatey v"))
                {
                    // TOOD version checks? "Chocolatey v0.10.15"
                }

                // divide out into blocks seperated by empty line
                List<string> currentBlock = new List<string>();
                while (outputLines.Count > 0)
                {
                    string line = outputLines.Dequeue();
                    if (String.Empty.Equals(line))
                    {
                        rtn.Add(currentBlock.ToArray());
                        currentBlock = new List<string>();
                    }
                    else
                    {
                        currentBlock.Add(line);
                    }
                }
            }

            return rtn;
        }
    }
}
