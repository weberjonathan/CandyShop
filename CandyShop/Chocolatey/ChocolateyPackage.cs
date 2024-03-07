using System;
using System.Collections.Generic;

namespace CandyShop.Chocolatey
{
    public class ChocolateyPackage
    {
        private readonly List<string> SUFFIXES = new List<string> {
            ".install",
            ".portable",
            ".app",
            ".commandline",
            ".tool"
        };

        public string Name { get; set; }

        public string CurrVer { get; set; }

        public string AvailVer { get; set; }

        public bool? Pinned { get; set; }

        public bool IsTopLevelPackage { get; set; } = true;

        public ChocolateyPackage Parent { get; set; }

        public bool HasSuffix
        {
            get
            {
                int i = Name.LastIndexOf('.');
                return (i > 0) && SUFFIXES.Contains(Name.Substring(i));
            }
        }

        /// <summary>
        /// The name of the package without any suffixes.
        /// </summary>
        public string ClearName
        {
            get
            {
                int i = Name.LastIndexOf('.');
                return i > 0 ? Name.Substring(0, i) : Name;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (ChocolateyPackage) obj;
            return String.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : base.GetHashCode();
        }
    }
}
