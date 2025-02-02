using System;
using System.Collections.Generic;

namespace CandyShop.PackageCore
{
    internal class GenericPackage
    {
        public static GenericPackage FromArgs(string[] args)
        {
            if (args.Length == 4)
            {
                return new GenericPackage(args[0], args[1], args[2], args[3]);
            }
            else if (args.Length == 5)
            {
                return new GenericPackage(args[0], args[1], args[2], args[3], args[4]);
            }
            else
            {
                throw new ArgumentException($"Expected 4 or 5 arguments, found {args.Length}.");
            }
        }

        private readonly List<string> SUFFIXES = [
            ".install",
            ".portable",
            ".app",
            ".commandline",
            ".tool"
        ];

        public GenericPackage() { }

        public GenericPackage(string name)
        {
            Name = name;
        }

        public GenericPackage(string name, string id, string currentVersion, string source)
        {
            Name = name;
            Id = id;
            CurrVer = currentVersion;
            Source = source;
        }

        public GenericPackage(string name, string id, string currentVersion, string availableVersion, string source)
        {
            Name = name;
            Id = id;
            CurrVer = currentVersion;
            AvailVer = availableVersion;
            Source = source;
        }

        public string Name { get; set; }

        public string Id { get; set; }

        public string Source { get; set; }

        public bool HasSource => !string.IsNullOrEmpty(Source);

        public string CurrVer { get; set; }

        public string AvailVer { get; set; }

        public bool? Pinned { get; set; }

        /// <summary>Parent package (Chocolatey only)</summary>
        public GenericPackage Parent { get; set; }

        /// <summary>Whether the package has a parent (Chocolatey only)</summary>
        public bool IsTopLevelPackage => Parent == null;

        /// <summary>Whether the package has a valid suffix (Chocolatey only)</summary>
        public bool HasChocolateySuffix
            => Name.Contains('.') && SUFFIXES.Contains(Name[Name.LastIndexOf('.')..]);

        /// <summary>Package name without suffix, if present (Chocolatey only)</summary>
        public string ClearName
            => HasChocolateySuffix ? Name[..Name.LastIndexOf('.')] : Name;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (GenericPackage)obj;
            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : base.GetHashCode();
        }
    }
}
