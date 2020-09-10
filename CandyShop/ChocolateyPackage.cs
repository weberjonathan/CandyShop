namespace CandyShop
{
    public class ChocolateyPackage
    {
        public string Name { get; set; }
        public string CurrVer { get; set; }
        public string AvailVer { get; set; }
        public bool Pinned { get; set; }
        public bool Outdated { get; set; }
    }
}
