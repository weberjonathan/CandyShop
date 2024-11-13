using CandyShop.Controls;
using CandyShop.PackageCore;
using CandyShop.Services;
using CandyShop.View;
using System.Collections.Generic;
using System.Linq;

namespace CandyShop.Controller
{
    internal class PinController
    {
        readonly PackageService PackageService;
        private List<IPinSupport> Views = [];

        public PinController(PackageService packageService)
        {
            PackageService = packageService;
        }

        public void InjectView(params IPinSupport[] views)
        {
            foreach (var view in Views)
                view.PinnedChanged -= View_PinnedChanged;

            Views = views.ToList();

            foreach (var view in views)
                view.PinnedChanged += View_PinnedChanged;
        }

        private void View_PinnedChanged(object sender, PinnedChangedArgs e)
        {
            TogglePin(e.Name);
        }

        private async void TogglePin(string packageName)
        {
            GenericPackage package = PackageService.GetPackageByName(packageName);
            if (package == null)
            {
                ErrorHandler.ShowError("Could not find package '{0}'", packageName);
                return;
            }

            if (package.Pinned.HasValue)
            {
                try
                {
                    if (package.Pinned.Value)
                        await PackageService.UnpinAsync(package.Name);
                    else
                        await PackageService.PinAsync(package.Name);
                }
                catch (PackageManagerException e)
                {
                    ErrorHandler.ShowError("An unknown error occurred: {0}", e.Message);
                }

                foreach (var view in Views)
                {
                    view.UpdatePinnedState(package.Name, package.Pinned.Value);
                }
            }
            else
            {
                ErrorHandler.ShowError("Failed to determine the pinned state of package '{0}'.", package.Name);
            }
        }
    }
}
