using CandyShop.Chocolatey;
using CandyShop.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace CandyShop.Controller
{
    class UpgradePageController : IUpgradePageController
    {
        private readonly ChocolateyService ChocolateyService;

        public UpgradePageController(ChocolateyService chocolateyService)
        {
            ChocolateyService = chocolateyService;
        }

        public void TogglePin(string packageName, Action<bool> onSuccess)
        {
            try
            {
                var package = ChocolateyService.GetPackageByName(packageName);
                if (package == null)
                {
                    ErrorHandler.ShowError($"Could not find package '{packageName}'");
                    return;
                }

                if (package.Pinned.HasValue)
                {
                    if (package.Pinned.Value)
                    {
                        ChocolateyService.Unpin(package);
                    }
                    else
                    {
                        ChocolateyService.Pin(package);
                    }

                    onSuccess.Invoke(package.Pinned.Value);
                }
                else
                {
                    // TODO error
                }
            }
            catch (ChocolateyException e)
            {
                ErrorHandler.ShowError("An unknown error occurred: {0}", e.Message);
            }
        }
    }
}
