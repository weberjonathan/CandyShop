using CandyShop.View;

namespace CandyShop.Controller
{
    interface IInstalledPageController
    {
        void InjectView(IInstalledPageView view);
        void InitView();
    }
}
