using System.Linq;

namespace CandyShop.Chocolatey
{
    public class ChocolateyPackage
    {
        public string Name { get; set; }
        public string CurrVer { get; set; }
        public string AvailVer { get; set; }
        public bool Pinned { get; set; }

        public bool IsMetaPackage {
            get {
                int i = Name.LastIndexOf('.');
                if (i > 0)
                {
                    string suffix = Name.Substring(i);
                    return suffix.Equals(".install")
                        || suffix.Equals(".portable")
                        || suffix.Equals(".app")
                        || suffix.Equals(".commandline")
                        || suffix.Equals(".tool");
                }

                return false;
            }
        }
    }
}
