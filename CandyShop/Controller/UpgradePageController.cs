using CandyShop.Controls.Factory;
using CandyShop.PackageCore;
using CandyShop.Properties;
using CandyShop.Services;
using CandyShop.View;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CandyShop.Controller
{
    class UpgradePageController
    {
        private readonly CandyShopContext Context;
        private readonly PackageService PackageService;
        private IMainWindowView MainWindow;
        private IUpgradePageView View;

        public UpgradePageController(CandyShopContext context, PackageService packageService)
        {
            Context = context;
            PackageService = packageService;
        }

        public void InjectViews(IMainWindowView mainWindow, IUpgradePageView upgradePage)
        {
            MainWindow = mainWindow;
            View = upgradePage;

            IControlsFactory provider = ContextSingleton.Get.WingetMode ? new WingetControlsFactory() : new ChocoControlsFactory(); // TODO
            View.BuildControls(provider);

            View.PinnedChanged += new EventHandler<PinnedChangedArgs>((sender, e) => TogglePin(e.Name));
            View.CleanShortcutsChanged += new EventHandler((sender, e) => Context.CleanShortcuts = View.CleanShortcuts);
            View.CloseAfterUpgradeChanged += new EventHandler((sender, e) => Context.CloseAfterUpgrade = View.CloseAfterUpgrade);
            View.CleanShortcuts = Context.CleanShortcuts;
            View.CloseAfterUpgrade = Context.CloseAfterUpgrade;
            View.RefreshClicked += new EventHandler((sender, e) => UpdateOutdatedPackageDisplayAsync(forceFetch: true));
            MainWindow.RefreshClicked += new EventHandler((sender, e) => UpdateOutdatedPackageDisplayAsync(forceFetch: true));

            View.UpgradeAllClick += new EventHandler((sender, e) =>
            {
                PerformUpgrade(View.Items);
            });

            View.UpgradeSelectedClick += new EventHandler((sender, e) =>
            {
                var packages = View.SelectedItems;
                if (packages.Length > 0) PerformUpgrade(packages);
            });

            View.CheckTopLevelClicked += new EventHandler((sender, e) =>
            {
                CheckTopLevelPackages();
            });

            View.AllowPinnedUacIon = !Context.WingetMode;
            View.ShowUacIcons = Context.ElevateOnDemand && !Context.HasAdminPrivileges;

            // update UI if is properties file is updated
            Context.InitConfigFileWatcher();
        }

        public async void UpdateOutdatedPackageDisplayAsync(bool forceFetch = false)
        {
            View.Loading = true;
            if (forceFetch) await PackageService.ClearOutdatedPackages();

            List<GenericPackage> packages = new List<GenericPackage>();
            try
            {
                packages = await PackageService.GetOutdatedPackagesAsync();
            }
            catch (PackageManagerException e)
            {
                Log.Error(LocaleEN.ERROR_RETRIEVING_OUTDATED_PACKAGES, e.Message);
            }

            View.ClearItems();
            // TODO unknown packages can only be upgraded with a specific flag; give user control over that
            IEnumerable<object[]> items = packages.Where(p => !"Unknown".Equals(p.CurrVer)).Select(BuildDisplayItem);
            foreach (var item in items)
                View.AddItem(item);

            if (packages.Count == 0)
            {
                View.DisplayEmpty();
            }

            CheckTopLevelPackages();
        }

        private object[] BuildDisplayItem(GenericPackage package)
        {
            if (Context.WingetMode)
                return [
                    true,
                    package.Pinned.GetValueOrDefault(false) ? Resources.ic_pin : null,
                    package.Name,
                    package.Id,
                    package.CurrVer,
                    package.AvailVer,
                    package.Source
                ];
            else
                return [
                    true,
                    package.Pinned.GetValueOrDefault(false) ? Resources.ic_pin : null,
                    package.Name,
                    package.CurrVer,
                    package.AvailVer,
                ];
        }

        private void CheckTopLevelPackages()
        {
            string[] displayedItemNames = View.Items;
            List<GenericPackage> packages = PackageService.GetPackagesByName(displayedItemNames.ToList());

            foreach (var p in packages)
            {
                bool check = p.IsTopLevelPackage && !p.Pinned.GetValueOrDefault(false);
                View.SetItemCheckState(p.Name, check);
            }
        }

        private async void PerformUpgrade(string[] packageNames)
        {
            MainWindow?.ToForm().Hide();

            // upgrade
            bool closeAfterUpgrade = Context.CloseAfterUpgrade;

            try
            {
                var packages = PackageService.GetPackagesByName(packageNames.ToList());
                await PackageService.Upgrade(packages);
            }
            catch (PackageManagerException e)
            {
                MainWindow?.DisplayError(LocaleEN.ERROR_UPGRADING_OUTDATED_PACKAGES_SHORT, e.Message.TrimEnd('.'));
                closeAfterUpgrade = false;
            }
            catch (CandyShopException e)
            {
                MainWindow?.DisplayError(LocaleEN.ERROR_UPGRADING_OUTDATED_PACKAGES_SHORT, e.Message.TrimEnd('.'));
                closeAfterUpgrade = false;
            }

            if (closeAfterUpgrade)
            {
                MainWindow?.ToForm().Dispose();
                Program.Exit();
            }
            else
            {
                UpdateOutdatedPackageDisplayAsync();
                MainWindow?.ToForm().Show();
            }
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

                View.SetPinned(package.Name, package.Pinned.Value);
            }
            else
            {
                ErrorHandler.ShowError("Failed to determine the pinned state of package '{0}'.", package.Name);
            }
        }
    }
}
