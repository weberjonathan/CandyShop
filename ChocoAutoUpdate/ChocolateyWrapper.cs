using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ChocoAutoUpdate
{
    public class ChocolateyWrapper
    {
        private const string TXT_ERR_PARSE = "Could not parse chocolatey output.";
        private const string CHOCO_BIN = "choco";

        /// <exception cref="ChocolateyException"></exception>
        /// <exception cref="ChocoAutoUpdateException"></exception>
        public static List<ChocolateyPackage> CheckOutdated()
        {
            List<ChocolateyPackage> packages = new List<ChocolateyPackage>();
            
            // launch process
            ProcessStartInfo procInfo = new ProcessStartInfo(CHOCO_BIN, "outdated")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process proc = Process.Start(procInfo);
            string output = proc.StandardOutput.ReadToEnd();

            proc.WaitForExit();
            if (proc.ExitCode != 0)
            {
                throw new ChocolateyException($"choco did not exit cleanly. Returned {proc.ExitCode}. ");
            }

            // parse head
            Queue<string> outputLines = new Queue<string>(output.Split("\r\n"));
            if (!outputLines.Dequeue().StartsWith("Chocolatey v"))
            {
                // TOOD version checks? "Chocolatey v0.10.15"
            }

            if (!outputLines.Dequeue().Equals("Outdated Packages") ||
                !outputLines.Dequeue().Equals(" Output is package name | current version | available version | pinned?") ||
                !outputLines.Dequeue().Equals(""))
            {
                throw new ChocoAutoUpdateException(TXT_ERR_PARSE);
            }

            // parse outdated packages
            while (outputLines.Count > 0)
            {
                string line = outputLines.Dequeue();
                if (line.Equals("")) break;

                // retrieve package
                ChocolateyPackage pckg = new ChocolateyPackage();
                string[] entry = line.Split('|');
                pckg.Name = entry[0];
                pckg.CurrVer = entry[1];
                pckg.AvailVer = entry[2];
                pckg.Pinned = entry[3].Equals("true");
                pckg.Outdated = true;
                packages.Add(pckg);
            }

            // parse summary
            string summaryPattern = @"Chocolatey has determined [0-9]* package\(s\) are outdated\.";
            Match summaryMatch = Regex.Match(outputLines.Dequeue(), summaryPattern);
            if (!summaryMatch.Success)
            {
                throw new ChocoAutoUpdateException(TXT_ERR_PARSE);
            }

            return packages;
        }

        /// <exception cref="ChocolateyException"></exception>
        public static void Upgrade(List<ChocolateyPackage> packages)
        {
            string argument = "";
            foreach (ChocolateyPackage pckg in packages)
            {
                argument += pckg.Name + " ";
            }

            // launch process
            ProcessStartInfo procInfo = new ProcessStartInfo(CHOCO_BIN, $"upgrade {argument} -y")
            {
                UseShellExecute = false,

                // set true to redirect output; will prevent stuff from being printed in allocated console (see ApplicationContext)
                // RedirectStandardOutput = true
            };

            Process proc = new Process()
            {
                StartInfo = procInfo
            };

            // redirect output handler
            //proc.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            //{
            //    UpgradeOutput.Add(e.Data);
            //});

            proc.Start();
            // proc.BeginOutputReadLine(); // needed to redirect output

            proc.WaitForExit();

            if (proc.ExitCode != 0)
            {
                throw new ChocolateyException($"choco did not exit cleanly. Returned {proc.ExitCode}. ");
            }
        }
    }
}
