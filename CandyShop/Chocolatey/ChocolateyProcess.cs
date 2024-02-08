using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace CandyShop.Chocolatey
{
    public class ChocolateyProcess
    {
        private readonly string CHOCO_BIN = ContextSingleton.Get.ChocolateyBinary;

        public ChocolateyProcess(string args)
        {
            Arguments = args;
        }

        public string Arguments { get; private set; }
        
        public string Output { get; private set; } = "";

        public List<string[]> OutputBySection { get; private set; } = new List<string[]>();

        public int ExitCode { get; private set; }

        public bool FailOnNonZeroExitCode { get; set; } = true;

        /// <exception cref="ChocolateyException"></exception>
        public void ExecuteHidden()
        {
            ProcessStartInfo procInfo = new ProcessStartInfo(CHOCO_BIN, Arguments)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                Process proc = Process.Start(procInfo);
                string output = proc.StandardOutput.ReadToEnd();

                proc.WaitForExit();
                Output = output;
                ExitCode = proc.ExitCode;

                if (FailOnNonZeroExitCode && proc.ExitCode != 0)
                {
                    throw new ChocolateyException($"Chocolatey did not exit cleanly. Returned {proc.ExitCode}.");
                }
            }
            catch (Win32Exception e)
            {
                throw new ChocolateyException("An error occurred while running choco.", e);
            }

            OutputBySection = ParseSectionsFromOutput(Output);
        }

        /// <exception cref="ChocolateyException"></exception>
        public void Execute()
        {
            // TODO potentially redirect output and expose events

            ProcessStartInfo procInfo = new ProcessStartInfo(CHOCO_BIN, Arguments)
            {
                UseShellExecute = false,
            };

            try
            {
                Process proc = Process.Start(procInfo);
                proc.WaitForExit();
                ExitCode = proc.ExitCode;

                if (FailOnNonZeroExitCode && proc.ExitCode != 0)
                {
                    throw new ChocolateyException($"choco did not exit cleanly. Returned {proc.ExitCode}.");
                }
            }
            catch (Win32Exception e)
            {
                throw new ChocolateyException("An error occurred while running choco.", e);
            }
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
