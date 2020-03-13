using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ChocoUpgrade
{
    class Program
    {
        private static string chocoError = "";
        
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

        static int Main(string[] args)
        {
            // get desktop shortcuts
            bool FLAG_CLEANUP;
            List<string> desktopShortcuts = null;
            if (args.Length > 0 && (args[0].Equals("-c") || args[0].Equals("--cleanup")))
            {
                FLAG_CLEANUP = true;
                desktopShortcuts = new List<string>(Directory.GetFiles(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "*.lnk",
                    SearchOption.TopDirectoryOnly));
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

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                Console.WriteLine($"choco did not exit cleanly ({p.ExitCode}).\n");
                Console.WriteLine(chocoError);
            }

            // remove new shortcuts
            if (FLAG_CLEANUP)
            {
                Queue<string> newShortcuts = new Queue<string>(Directory.GetFiles(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "*.lnk",
                    SearchOption.TopDirectoryOnly));

                while (desktopShortcuts != null && newShortcuts.Count > 0)
                {
                    string newS = newShortcuts.Dequeue();
                    if (!desktopShortcuts.Contains(newS))
                    {
                        Console.WriteLine($"Deleting shortcut {newS}");
                        File.Delete(newS);
                    }
                }
            }

            Console.Write("Press any key to terminate... ");
            Console.ReadKey();
            return p.ExitCode;
        }
    }
}
