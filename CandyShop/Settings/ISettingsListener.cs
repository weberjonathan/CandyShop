namespace CandyShop.Settings
{
    internal interface ISettingsListener
    {
        void OnSettingsChanged(SettingsDefinition settings);
    }
}
