using System;
using System.Diagnostics;

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

            Console.Write("Press any key to terminate... ");
            Console.ReadKey();
            return p.ExitCode;
        }
    }
}
