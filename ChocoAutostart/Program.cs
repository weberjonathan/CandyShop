using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace ChocoAutostart
{
    class Program
    {
        private static readonly string UPGRADE_TOOL = "ChocoUpgrade.exe";
        private static readonly string CHOCO_EXEC = "choco";
        private static readonly string CHOCO_CHECK = "outdated";

#if DEBUG
        private static bool DEBUG = false;
#else
        private static bool DEBUG = false;
#endif

        private static void Pause()
        {
            Console.Write("Press any key to continue... ");
            Console.ReadKey();
        }

        private static void Warning(string msg)
        {
            ConsoleColor tmp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"WARNING: {msg}");
            Console.ForegroundColor = tmp;
        }

        private static void Debug(string msg)
        {
            Console.WriteLine($"DEBUG: {msg}");
        }
        
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].Equals("-d")) DEBUG = true;
            string assemblyLocation =
                Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            if (DEBUG) Debug($"Executing in '{assemblyLocation}'");

            /* retrieve upgrade tool
             *   (1) upgrade tool is referenced as dependency and copied locally
             *   (2) chocoAutostart is published as a single file exec
             *   ==> chocoAutostart + dependencies are extracted to tmp location by dotnet runtime
             *       thats where the upgrade tool is found, not in working dir
             * */
            string upgradeTool = Path.Combine(assemblyLocation, UPGRADE_TOOL);
            if (!File.Exists(upgradeTool))
            {
                Warning("Upgrade tool 'ChocoUpgrade.exe' not found.");
                if (DEBUG) Debug($"{Path.GetFullPath(upgradeTool)} does not exist.");
            }

            // check for updates
            Console.WriteLine($"> {CHOCO_EXEC} {CHOCO_CHECK}");
            ProcessStartInfo checkProcInfo = new ProcessStartInfo(CHOCO_EXEC, CHOCO_CHECK);
            checkProcInfo.RedirectStandardOutput = true;
            Process checkProc = Process.Start(checkProcInfo);
            string output = checkProc.StandardOutput.ReadToEnd();
            checkProc.WaitForExit();
            if (checkProc.ExitCode == 0)
            {
                Console.WriteLine(output);
            }
            else
            {
                Console.Write($"choco did not exit cleanly ({checkProc.ExitCode}). ");
                Pause();
                return;
            }

            string summaryPattern = @"Chocolatey has determined [0-9]* package\(s\) are outdated\.";
            Match summaryMatch = Regex.Match(output, summaryPattern);
            if (!summaryMatch.Success)
            {
                Console.Write("Could not parse chocolatey output. ");
                Pause();
                return;
            }

            int outdatedCount = Convert.ToInt32(Regex.Replace(summaryMatch.Value, "[^0-9]", ""));

            // execute upgrades
            if (outdatedCount > 0 && File.Exists(upgradeTool))
            {
                Warning("Do not change or rename shortcuts on your desktop while the installation is ongoing!");
                Console.Write("Do you wish to update all outdated packages? [y] ");
                if (Console.ReadLine().ToLower().Equals("y"))
                {
                    // launch upgrade tool and exit
                    Console.WriteLine("Launching upgrade process...");
                    ProcessStartInfo upgradeProcInfo = new ProcessStartInfo(upgradeTool);
                    upgradeProcInfo.Arguments = "--cleanup";
                    upgradeProcInfo.UseShellExecute = true;
                    try
                    {
                        Process p = Process.Start(upgradeProcInfo);
                    }
                    catch (System.ComponentModel.Win32Exception)
                    {
                        Console.Write("An error occourred. Upgrade aborted. ");
                        Pause();
                    }
                    return;
                }
            }
            else
            {
                Pause();
            }
        }
    }
}
