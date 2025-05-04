using CandyShop.Settings;
using System;

namespace CandyShop.Controls.Factory
{
    internal class UiComponentsFactory
    {
        public static IUiComponents Create(SettingsDefinition settings)
        {
            return settings.ActivePackageManager switch
            {
                "Winget" => new WingetUiComponents(),
                "Chocolatey" => new ChocoUiComponents(),
                _ => throw new ArgumentException(),
            };
        }
    }
}
