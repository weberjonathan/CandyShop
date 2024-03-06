using CandyShop.Chocolatey;
using System;
using System.Collections.Generic;
using CandyShop.View;
using CandyShop.Properties;
using CandyShop.Services;
using Serilog;
using CandyShop.Controls;
using Windows.Services.Maps;

namespace CandyShop.Controller
{
    class InstalledPageController
    {
        private readonly IPackageService PackageService;
        private IMainWindowView MainWindow;
        private IInstalledPageView View;

        public InstalledPageController(IPackageService service)
        {
            PackageService = service;
        }

        public void InjectView(IMainWindowView mainWindow, IInstalledPageView view)
        {
            MainWindow = mainWindow;
            View = view;

            ICommon provider = ContextSingleton.Get.WingetMode ? new CommonWinget() : new CommonChocolatey();
            View.BuildControls(provider);

            View.SearchBar.SearchChanged += new EventHandler((sender, e) => SyncListView());
            View.SearchBar.FilterRequireSourceChanged += new EventHandler((sender, e) => SyncListView());
            View.SearchBar.FilterTopLevelOnlyChanged += new EventHandler((sender, e) => SyncListView());
            View.SelectedItemChanged += OnSelectedItemChanged;
            MainWindow.RefreshClicked += new EventHandler((sender, e) => UpdateInstalledPackagesDisplayAsync(forceFetch: true));
        }

        public async void UpdateInstalledPackagesDisplayAsync(bool forceFetch = false)
        {
            View.LoadingPackages = true;
            if (forceFetch) await PackageService.ClearInstalledPackages();

            List<GenericPackage> packages = new List<GenericPackage>();
            try
            {
                packages = await PackageService.GetInstalledPackagesAsync();
            }
            catch (ChocolateyException e)
            {
                Log.Error($"An error occurred while retrieving installed packages: {e.Message}");
            }

            if (forceFetch) await PackageService.ClearPackageDetails();

            View.ClearItems();
            View.LoadingPackages = false;
            packages.ForEach(p => View.AppendItem([p.Name, p.CurrVer, p.Source]));
            SyncListView();
        }

        private async void OnSelectedItemChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(View.SelectedItem))
            {
                View.UpdateDetails(String.Empty);
                return;
            }

            View.LoadingDetails = true;

            string name = View.SelectedItem;
            string details;
            try
            {
                details = await PackageService.GetPackageDetailsAsync(name);
            }
            catch (ChocolateyException exception)
            {
                details = String.Format(LocaleEN.ERROR_RETRIEVING_PACKAGE_DETAILS, exception.Message);
            }

            if (!String.IsNullOrEmpty(details) && name.Equals(View.SelectedItem) && !View.LoadingPackages)
            {
                View.UpdateDetails(details);
            }
        }

        private async void SyncListView()
        {
            string filterName = View.SearchBar.Text;
            bool hideSuffixed = View.SearchBar.FilterTopLevelOnly;
            bool requireSource = View.SearchBar.FilterRequireSource;

            List<GenericPackage> packages = new List<GenericPackage>();
            try
            {
                packages = await PackageService.GetInstalledPackagesAsync();
            }
            catch (ChocolateyException e)
            {
                Log.Error(e.Message);
            }

            foreach (var package in packages)
            {
                bool packageAllowed = true;

                // determine whether package should be displayed
                if (hideSuffixed && !package.IsTopLevelPackage)
                {
                    packageAllowed = false;
                }

                if (requireSource && string.IsNullOrEmpty(package.Source))
                    packageAllowed = false;

                if (!string.IsNullOrEmpty(filterName) && !package.Name.Contains(filterName))
                {
                    packageAllowed = false;
                }

                bool isDisplayed = View.Items.Contains(package.Name);

                // act
                if (isDisplayed && !packageAllowed) View.RemoveItem(package.Name);
                else if (!isDisplayed && packageAllowed) InsertItem(package, packages);
            }
        }

        private void InsertItem(GenericPackage package, List<GenericPackage> referenceList)
        {
            int latestPossibleIndex = referenceList.IndexOf(package);
            string lastVisibilePackage = null;

            // find package that is supposed to be directly above it
            for (int j = latestPossibleIndex - 1; j >= 0; j--)
            {
                if (View.Items.Contains(referenceList[j].Name))
                {
                    lastVisibilePackage = referenceList[j].Name;
                    break;
                }
            }

            // insert
            int index = 0;
            if (lastVisibilePackage != null)
            {
                index = View.Items.IndexOf(lastVisibilePackage) + 1;
            }

            View.InsertItem(index, [
                package.Name,
                package.CurrVer,
                package.Source
            ]);
        }
    }
}
