using CandyShop.View;

namespace CandyShop.Controller
{
    interface IMainWindowController
    {
        void InjectView(IMainWindowView mainView);
        void InitView();
        void SmartSelectPackages();
        void ShowGithub();
        void ShowMetaPackageHelp();
        void ShowLicenses();
        void ToggleCreateTask();
        void ShowChocoLogFolder();
        void ShowCandyShopConfigFolder();
    }
}
