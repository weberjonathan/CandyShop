using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace ChocoAutoUpdate
{
    public class Choco
    {
        private const string EX_PARSE_ERR = "Could not parse chocolatey output.";

        public List<string> Outdated { get; private set; }

        public string OutdatedOutput { get; private set; }

        private readonly Queue<string> _NewShortcuts = new Queue<string>();
        public string[] NewShortcuts => _NewShortcuts.ToArray();

        /// <exception cref="ChocolateyException"></exception>
        /// <exception cref="ChocoAutoUpdateException"></exception>
        public Choco()
        {
            CheckOutdated();
        }

        /// <exception cref="ChocolateyException"></exception>
        /// <exception cref="ChocoAutoUpdateException"></exception>
        public void CheckOutdated()
        {
            OutdatedOutput = "";
            Outdated = new List<string>();
            
            // launch process
            ProcessStartInfo procInfo = new ProcessStartInfo("choco", "outdated")
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
            OutdatedOutput = output;

            // parse head
            Queue<string> outputLines = new Queue<string>(output.Split("\r\n"));
            
            if (!outputLines.Dequeue().StartsWith("Chocolatey v"))
            {
                // TOOD version checks? "Chocolatey v0.10.15"
            }

            if (!outputLines.Dequeue().Equals("Outdated Packages") ||
                !outputLines.Dequeue().Equals(" Output is package name | current version | available version | pinned?"))
            {
                throw new ChocoAutoUpdateException(EX_PARSE_ERR);
            }

            // parse outdated packages
            while (outputLines.Count > 0)
            {
                string line = outputLines.Dequeue();
                // TODO test; choco outdated prints two empty lines if no packages are outdated
                if (line.Equals(""))
                {
                    if (outputLines.Count > 0 && outputLines.Dequeue().Equals(""))
                    {
                        break;
                    }
                    else
                    {
                        throw new ChocoAutoUpdateException(EX_PARSE_ERR);
                    }
                }
                
                // TODO parse
                Outdated.Add(line);
            }
            
            // parse summary
            string summaryPattern = @"Chocolatey has determined [0-9]* package\(s\) are outdated\.";
            Match summaryMatch = Regex.Match(outputLines.Dequeue(), summaryPattern);
            if (!summaryMatch.Success)
            {
                throw new ChocoAutoUpdateException(EX_PARSE_ERR);
            }
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            _NewShortcuts.Enqueue(e.FullPath);
        }

        /// <exception cref="ChocolateyException"></exception>
        public void Upgrade()
        {
            _NewShortcuts.Clear();
            
            ProcessStartInfo procInfo = new ProcessStartInfo("choco", "upgrade all -y")
            {
                UseShellExecute = false
            };

            Process proc = Process.Start(procInfo);
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                watcher.Filter = "*.lnk";
                watcher.IncludeSubdirectories = false;
                watcher.NotifyFilter = NotifyFilters.FileName;
                watcher.Created += Watcher_Created;
                watcher.EnableRaisingEvents = true;
                proc.WaitForExit();

                if (proc.ExitCode != 0)
                {
                    throw new ChocolateyException($"choco did not exit cleanly. Returned {proc.ExitCode}. ");
                }
            }
        }
    }
}
