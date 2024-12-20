﻿using CandyShop.Controls.Factory;
using CandyShop.PackageCore;
using CandyShop.Properties;
using CandyShop.Services;
using CandyShop.View;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CandyShop.Controller
{
    internal class PackageController
    {
        private readonly PackageService PackageService;
        private readonly IControlsFactory ControlsFactory;

        private MainWindow MainWindow;
        private IPackageViewer UpgradePage;
        private IPackageViewer InstalledPage;

        public PackageController(PackageService packageService, IControlsFactory controlsFactory)
        {
            PackageService = packageService;
            ControlsFactory = controlsFactory;
        }

        public void InjectViews(MainWindow mainWindow, IPackageViewer upgradePage, IPackageViewer installedPage)
        {
            MainWindow = mainWindow;
            UpgradePage = upgradePage;
            InstalledPage = installedPage;

            MainWindow.RefreshClicked += new EventHandler((sender, e) => UpdatePackageDisplaysAsync(forceFetch: true));
            upgradePage.RefreshClicked += new EventHandler((sender, e) => UpdatePackageDisplaysAsync(forceFetch: true));
            installedPage.RefreshClicked += new EventHandler((sender, e) => UpdatePackageDisplaysAsync(forceFetch: true));
        }

        public async void UpdatePackageDisplaysAsync(bool forceFetch = false)
        {
            UpgradePage.Loading = true;
            InstalledPage.Loading = true;

            if (forceFetch) await PackageService.ClearPackages();

            Task<List<GenericPackage>> installedFuture = null;
            Task<List<GenericPackage>> outdatedFuture = null;
            try
            {
                installedFuture = PackageService.GetInstalledPackagesAsync();
            }
            catch (PackageManagerException e)
            {
                Log.Error(LocaleEN.ERROR_RETRIEVING_OUTDATED_PACKAGES, e.Message);
            }

            try
            {
                outdatedFuture = PackageService.GetOutdatedPackagesAsync();
            }
            catch (PackageManagerException e)
            {
                Log.Error(LocaleEN.ERROR_RETRIEVING_OUTDATED_PACKAGES, e.Message);
            }

            List<GenericPackage> outdated = await outdatedFuture;
            outdated ??= [];
            UpgradePage.ClearPackages();
            UpgradePage.AddPackages(outdated.Select(ControlsFactory.BuildUpgradeItem).ToList());

            List<GenericPackage> installed = await installedFuture;
            installed ??= [];
            InstalledPage.ClearPackages();
            InstalledPage.AddPackages(installed.Select(ControlsFactory.BuildInstalledItem).ToList());
        }
    }
}
