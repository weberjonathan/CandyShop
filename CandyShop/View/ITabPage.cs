using CandyShop.Controls.Factory;

namespace CandyShop.View
{
    internal interface ITabPage
    {
        void BuildControls(AbstractControlsFactory provider);
    }
}
