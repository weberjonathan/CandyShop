using CandyShop.Chocolatey;
using CandyShop.Services;
using CandyShop.View;
using System;
using System.Collections.Generic;
using System.Text;

namespace CandyShop.Controller
{
    class UpgradePageController : IUpgradePageController
    {
        private readonly ChocolateyService ChocolateyService;
        private IUpgradePageView View;

        public UpgradePageController(ChocolateyService chocolateyService)
        {
            ChocolateyService = chocolateyService;
        }

        public void InjectView(IUpgradePageView view)
        {
            View = view;
            View.PinnedChanged += new EventHandler<PinnedChangedArgs>((sender, e) => TogglePin(e.Name));
        }

        public void TogglePin(string packageName)
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

                    View.SetPinned(package.Name, package.Pinned.Value);
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
