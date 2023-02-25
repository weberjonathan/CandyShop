using System;

namespace CandyShop.Winget
{
    public class WingetPackage
    {
        // TODO do all fields that the package manifest knows

        public string Name { get; set; }
        public String Id { get; set; }
        public string Version { get; set; }
        public string AvailableVersion { get; set; }
        public string Source { get; set; }
        public bool HasSource => !String.IsNullOrEmpty(Source);
    }
}
