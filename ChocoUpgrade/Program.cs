using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ChocoUpgrade
{
    class Program
    {
        private static string chocoError = "";
        private static Queue<string> newShortcuts = new Queue<string>();
        
        private static void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null && e.Data.StartsWith("Progress: "))
            {
                // also removes last line above progress bar (mirrored from choco)
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }
            Console.WriteLine(e.Data);
        }

        private static void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            chocoError += e.Data + "\n";
        }

        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            newShortcuts.Enqueue(e.FullPath);
        }

        static int Main(string[] args)
        {

            // get desktop shortcuts
            bool FLAG_CLEANUP;
            if (args.Length > 0 && (args[0].Equals("-c") || args[0].Equals("--cleanup")))
            {
                FLAG_CLEANUP = true;
            }
            else
            {
                FLAG_CLEANUP = false;
            }
            
            // upgrade
            Console.WriteLine("> choco upgrade all -y");

            ProcessStartInfo procInfo = new ProcessStartInfo("choco", "upgrade all -y");
            procInfo.RedirectStandardOutput = true;
            procInfo.RedirectStandardError = true;
            Process p = new Process() { StartInfo = procInfo };
            p.OutputDataReceived += OutputDataReceived;
            p.ErrorDataReceived += ErrorDataReceived;
            p.Start();

            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                watcher.Filter = "*.lnk";
                watcher.IncludeSubdirectories = false;
                watcher.NotifyFilter = NotifyFilters.FileName;
                watcher.Created += Watcher_Created;
                watcher.EnableRaisingEvents = true;

                Console.ReadKey();

                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                    Console.WriteLine($"choco did not exit cleanly ({p.ExitCode}).\n");
                    Console.WriteLine(chocoError);
                }
            }

            // remove new shortcuts
            if (FLAG_CLEANUP)
            {
                while (newShortcuts.Count > 0)
                {
                    string shortcut = newShortcuts.Dequeue();
                    Console.WriteLine($"Deleting shortcut {shortcut}");
                    try
                    {
                        File.Delete(shortcut);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR: Could not delete shortcut. (${e.Message})");
                    }
                }
            }

            Console.Write("Press any key to terminate... ");
            Console.ReadKey();
            return p.ExitCode;
        }
    }
}
