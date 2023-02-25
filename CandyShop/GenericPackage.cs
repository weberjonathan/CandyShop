using CandyShop.Chocolatey;
using CandyShop.Winget;
using System;

namespace CandyShop
{
    internal class GenericPackage
    {
        public GenericPackage(ChocolateyPackage package)
        {
            Name = package.Name;
            CurrVer = package.CurrVer;
            AvailVer = package.AvailVer;
            Pinned = package.Pinned;
            IsTopLevelPackage = package.IsTopLevelPackage;
        }

        public GenericPackage(WingetPackage package)
        {
            Name = package.Name;
            Id = package.Id;
            Source = package.Source;
            CurrVer = package.Version;
            AvailVer = package.AvailableVersion;
            Pinned = false;
            IsTopLevelPackage = true;
        }

        public string Name { get; set; }

        public string Id { get; set; }

        public string Source { get; set; }

        public string CurrVer { get; set; }

        public string AvailVer { get; set; }

        public bool? Pinned { get; set; }

        public bool IsTopLevelPackage { get; set; } = true;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (GenericPackage)obj;
            return String.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : base.GetHashCode();
        }
    }
}
