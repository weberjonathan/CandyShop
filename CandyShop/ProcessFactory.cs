using CandyShop.Chocolatey;
using CandyShop.Winget;

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
            return Instance.CreateWingetProcess(args);
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

        private WingetProcess CreateWingetProcess(string args)
        {
            return new WingetProcess(WingetBinary, args);
        }
    }
}
