using System;

namespace CandyShop.Controller
{
    internal interface IUpgradePageController
    {
        void TogglePin(string packageName, Action<bool> onSuccess);
    }
}