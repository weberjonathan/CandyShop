using CandyShop.Chocolatey;
using CandyShop.Winget;

namespace CandyShop
{
    internal class ProcessFactory
    {
        private static readonly ProcessFactory Instance = new ProcessFactory();

        public static ChocolateyProcess Choco(string args)
        {
            return Instance.CreateChocoProcess(args);
        }

        public static WingetProcess Winget(string args)
        {
            return Instance.CreateWingetProcess(args);
        }

        public string ChocoBinary { get; set; }
        
        public string WingetBinary { get; set; }

        public bool RequireSudo { get; set; }

        public ProcessFactory()
        {
            ChocoBinary = "choco";
            WingetBinary = "winget";
            RequireSudo = true;
        }

        public ProcessFactory(CandyShopContext context)
        {
            ChocoBinary = context.ChocolateyBinary;
            WingetBinary = "winget";
            RequireSudo = true;
        }

        private ChocolateyProcess CreateChocoProcess(string args)
        {
            return new ChocolateyProcess(ChocoBinary, args);
        }

        private WingetProcess CreateWingetProcess(string args)
        {
            return new WingetProcess(WingetBinary, args);
        }
    }
}
