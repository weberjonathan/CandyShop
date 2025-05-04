using CandyShop.Settings;
using System;

namespace CandyShop.Components
{
    internal interface IPackageFilterContext
    {
        bool HideSuffixedEnabled { get; }
        bool RequireSourceEnabled { get; }
    }

    internal class WingetPackageFilterContext : IPackageFilterContext
    {
        public bool HideSuffixedEnabled => false;
        public bool RequireSourceEnabled => true;
    }

    internal class ChocoPackageFilterContext : IPackageFilterContext
    {
        public bool HideSuffixedEnabled => true;
        public bool RequireSourceEnabled => false;
    }

    internal class PackageFilterContextFactory
    {
        public static IPackageFilterContext Create(SettingsDefinition settings)
        {
            if (settings.Winget.Enabled)
                return new WingetPackageFilterContext();
            else if (settings.Chocolatey.Enabled)
                return new ChocoPackageFilterContext();
            else
                throw new ArgumentException($"Unknown active package manager.");
        }
    }
}
