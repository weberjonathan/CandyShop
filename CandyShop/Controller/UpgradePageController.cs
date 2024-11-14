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
        private readonly IControlsFactory ControlsFactory;
        private MainWindow MainWindow;
        private UpgradePage View;

        public UpgradePageController(CandyShopContext context, PackageService packageService, IControlsFactory controlsFactory)
        {
            Context = context;
            PackageService = packageService;
            ControlsFactory = controlsFactory;
        }

        public void InjectViews(MainWindow mainWindow, UpgradePage upgradePage)
        {
            MainWindow = mainWindow;
            View = upgradePage;

            View.BuildControls(ControlsFactory);

            View.CleanShortcutsChanged += new EventHandler((sender, e) => Context.CleanShortcuts = View.CleanShortcuts);
            View.CloseAfterUpgradeChanged += new EventHandler((sender, e) => Context.CloseAfterUpgrade = View.CloseAfterUpgrade);
            View.CleanShortcuts = Context.CleanShortcuts;
            View.CloseAfterUpgrade = Context.CloseAfterUpgrade;
            View.PackagesAdded += new EventHandler((sender, e) => CheckTopLevelPackages());

            View.UpgradeAllClick += new EventHandler((sender, e) =>
            {
                PerformUpgrade(View.Items);
            });

            View.UpgradeSelectedClick += new EventHandler((sender, e) =>
            {
                var packages = View.CheckedItems;
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
            MainWindow?.Hide();

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
                MainWindow?.Dispose();
                Program.Exit();
            }
            else
            {
                MainWindow?.ShowAndRefresh();
            }
        }
    }
}
