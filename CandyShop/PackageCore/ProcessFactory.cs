using System.Collections.Generic;

namespace CandyShop.PackageCore
{
    internal class ProcessFactory
    {
        private static ProcessFactory Instance = new ProcessFactory();

        public static void Config(CandyShopContext context)
        {
            Instance = new ProcessFactory(context);
        }

        public static PackageManagerProcess Choco(string args)
        {
            return Instance.CreateChocoProcess(args, false);
        }

        public static PackageManagerProcess ChocoPrivileged(string args)
        {
            return Instance.CreateChocoProcess(args, true);
        }

        public static PackageManagerProcess Winget(string args)
        {
            return Instance.CreateWingetProcess(args, false);
        }

        public static PackageManagerProcess WingetPrivileged(string args)
        {
            return Instance.CreateWingetProcess(args, true);
        }

        /// <summary>
        /// Combines arguments for multiple Winget launches into a 
        /// powershell script. Returned process has binary powershell.exe
        /// and the assembled script as argument.
        /// </summary>
        public static PackageManagerProcess WingetBatchPrivileged(List<string> arguments)
        {
            return Instance.CreateWingetBatchProcess(arguments, true);
        }

        public string ChocoBinary { get; set; }
        
        public string WingetBinary { get; set; }

        public bool RequireManualElevation { get; set; }

        private ProcessFactory()
        {
            ChocoBinary = "choco";
            WingetBinary = "winget";
            RequireManualElevation = true;
        }

        private ProcessFactory(CandyShopContext context)
        {
            ChocoBinary = context.ChocolateyBinary;
            WingetBinary = "winget";
            RequireManualElevation = context.ElevateOnDemand && !context.HasAdminPrivileges;
        }

        private PackageManagerProcess CreateChocoProcess(string args, bool elevate)
        {
            if (elevate && RequireManualElevation)
            {
                return new PackageManagerProcess("powershell.exe", "gsudo {" + ChocoBinary + " " + args + "}");
            }
            else
            {
                return new PackageManagerProcess(ChocoBinary, args);
            }
        }

        private PackageManagerProcess CreateWingetProcess(string args, bool elevate)
        {
            if (elevate && RequireManualElevation)
                return new PackageManagerProcess("powershell.exe", "gsudo {" + WingetBinary + " " + args + "}");
            else
                return new PackageManagerProcess(WingetBinary, args);
        }

        // TODO test this method
        private PackageManagerProcess CreateWingetBatchProcess(List<string> arguments, bool elevate)
        {
            string body = "";
            foreach (string arg in arguments)
            {
                var cmd = $"{WingetBinary} {arg}";
                body += $"Write-Host \"{cmd}`n\"; {cmd}; Write-Host \"Returned $LastExitCode`n\"; $exit = $exit -and $?;";
            }

            var gsudo = (elevate && RequireManualElevation) ? "gsudo" : "Invoke-Command -ScriptBlock";
            return new PackageManagerProcess("powershell.exe", $"$exit = $true; {gsudo} {{ {body} }}; Exit (-Not $exit)");
        }
    }
}
