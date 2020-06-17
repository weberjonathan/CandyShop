using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace ChocoHelpers
{
    public class Choco
    {
        public int OutdatedCount { get; private set; }
        public string OutdatedDetails { get; private set; }

        private readonly Queue<string> _NewShortcuts = new Queue<string>();
        public string[] NewShortcuts => _NewShortcuts.ToArray();

        /// <exception cref="ChocolateyException"></exception>
        /// <exception cref="ChocoHelpersException"></exception>
        public Choco()
        {
            CheckOutdated();
        }

        /// <exception cref="ChocolateyException"></exception>
        /// <exception cref="ChocoHelpersException"></exception>
        public void CheckOutdated()
        {
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

            string summaryPattern = @"Chocolatey has determined [0-9]* package\(s\) are outdated\.";
            Match summaryMatch = Regex.Match(output, summaryPattern);
            if (!summaryMatch.Success)
            {
                throw new ChocoHelpersException("Could not parse chocolatey output.");
            }

            // TODO test for success, set null otherwise
            OutdatedCount = Convert.ToInt32(Regex.Replace(summaryMatch.Value, "[^0-9]", ""));
            OutdatedDetails = output;
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

        public void RemoveShortcuts()
        {
            while (_NewShortcuts.Count > 0)
            {
                string shortcut = _NewShortcuts.Dequeue();
                try
                {
                    File.Delete(shortcut);
                }
                catch (IOException)
                {
                }
            }
        }
    }
}
