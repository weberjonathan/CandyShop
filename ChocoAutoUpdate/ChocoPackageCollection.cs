using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ChocoAutoUpdate
{
    public class ChocoPackageCollection : Dictionary<string, ChocoPackage>
    {
        public ChocoPackageCollection MarkedPackages
        {
            get
            {
                ChocoPackageCollection rtn = new ChocoPackageCollection();
                foreach (ChocoPackage package in this.Values)
                {
                    if (package.MarkedForUpdate) rtn.Add(package.Name, package);
                }
                return rtn;
            }
        }

        public string GetPackagesAsString()
        {
            StringBuilder rtn = new StringBuilder();
            foreach (ChocoPackage package in this.Values)
            {
                rtn.Append($"{package.Name} ");
            }
            return rtn.ToString();
        }
    }
}
