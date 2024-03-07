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

        public static PackageManagerProcess WingetPrivileged(List<PackageManagerProcess> processes)
        {
            return Instance.CreatePrivilegedUnifiedWingetProcess(processes);
        }

        public string ChocoBinary { get; set; }
        
        public string WingetBinary { get; set; }

        public bool ElevateOnDemand { get; set; }

        private ProcessFactory()
        {
            ChocoBinary = "choco";
            WingetBinary = "winget";
            ElevateOnDemand = true;
        }

        private ProcessFactory(CandyShopContext context)
        {
            ChocoBinary = context.ChocolateyBinary;
            WingetBinary = "winget";
            ElevateOnDemand = context.ElevateOnDemand && !context.HasAdminPrivileges;
        }

        private PackageManagerProcess CreateChocoProcess(string args, bool elevate)
        {
            if (elevate && ElevateOnDemand)
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
            if (elevate && ElevateOnDemand)
                return new PackageManagerProcess("powershell.exe", "gsudo {" + WingetBinary + " " + args + "}");
            else
                return new PackageManagerProcess(WingetBinary, args);
        }

        private PackageManagerProcess CreatePrivilegedUnifiedWingetProcess(List<PackageManagerProcess> processes)
        {
            string body = "";
            foreach (var item in processes)
            {
                var cmd = $"{item.Binary} {item.Arguments}";
                body += $"Write-Host \"$ {cmd}`n\"; {cmd}; Write-Host \"Returned $LastExitCode`n\"; $exit = $exit -and $?;";
            }
            return new PackageManagerProcess("powershell.exe", $"$exit = $true; gsudo {{ {body} }}; Exit (-Not $exit)");
        }
    }
}
