using CandyShop.Chocolatey;
using CandyShop.Winget;
using System.Collections.Generic;

namespace CandyShop
{
    internal class ProcessFactory
    {
        private static ProcessFactory Instance = new ProcessFactory();

        public static void Config(CandyShopContext context)
        {
            Instance = new ProcessFactory(context);
        }

        public static ChocolateyProcess Choco(string args)
        {
            return Instance.CreateChocoProcess(args, false);
        }

        public static ChocolateyProcess ChocoPrivileged(string args)
        {
            return Instance.CreateChocoProcess(args, true);
        }

        public static WingetProcess Winget(string args)
        {
            return Instance.CreateWingetProcess(args, false);
        }

        public static WingetProcess WingetPrivileged(string args)
        {
            return Instance.CreateWingetProcess(args, true);
        }

        public static WingetProcess WingetPrivileged(List<WingetProcess> processes)
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

        private ChocolateyProcess CreateChocoProcess(string args, bool elevate)
        {
            if (elevate && ElevateOnDemand)
            {
                return new ChocolateyProcess("powershell.exe", "gsudo {" + ChocoBinary + " " + args + "}");
            }
            else
            {
                return new ChocolateyProcess(ChocoBinary, args);
            }
        }

        private WingetProcess CreateWingetProcess(string args, bool elevate)
        {
            if (elevate && ElevateOnDemand)
                return new WingetProcess("powershell.exe", "gsudo {" + WingetBinary + " " + args + "}");
            else
                return new WingetProcess(WingetBinary, args);
        }

        private WingetProcess CreatePrivilegedUnifiedWingetProcess(List<WingetProcess> processes)
        {
            string body = "";
            foreach (var item in processes)
            {
                var cmd = $"{item.Binary} {item.Args}";
                body += $"Write-Host \"$ {cmd}`n\"; {cmd}; Write-Host \"Returned $LastExitCode`n\"; $exit = $exit -and $?;";
            }
            return new WingetProcess("powershell.exe", $"$exit = $true; gsudo {{ {body} }}; Exit (-Not $exit)");
        }
    }
}
