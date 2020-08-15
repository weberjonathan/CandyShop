using System;
using System.Collections.Generic;
using System.Text;

namespace ChocoAutoUpdate
{
    public class ChocoPackage
    {
        public string Name { get; set; }
        public string CurrVer { get; set; }
        public string AvailVer { get; set; }
        public bool Pinned { get; set; }
        public bool Outdated { get; set; }
        public bool MarkedForUpdate { get; set; } = false;
    }
}
