using CandyShop.Settings;
using System;

namespace CandyShop.Controls.Factory
{
    internal class UiComponentsFactory
    {
        public static IUiComponents Create(SettingsDefinition settings)
        {
            if (settings.Winget.Enabled)
                return new WingetUiComponents();
            else if (settings.Chocolatey.Enabled)
                return new ChocoUiComponents();
            else
                throw new ArgumentException($"Unknown active package manager.");
        }
    }
}
