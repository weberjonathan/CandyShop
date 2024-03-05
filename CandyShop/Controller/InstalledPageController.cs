using CandyShop.Chocolatey;
using System;
using System.Collections.Generic;
using CandyShop.View;
using CandyShop.Properties;
using CandyShop.Services;
using Serilog;

namespace CandyShop.Controller
{
    class InstalledPageController
    {
        private readonly IPackageService PackageService;
        private IInstalledPageView View;

        public InstalledPageController(IPackageService service)
        {
            PackageService = service;
        }

        public void InjectView(IInstalledPageView view)
        {
            View = view;
            View.EnableTopLevelToggle = !ContextSingleton.Get.WingetMode;

            View.FilterTextChanged += new EventHandler((sender, e) => SyncListView());
            View.ShowTopLevelOnlyChanged += new EventHandler((sender, e) => SyncListView());
            View.SelectedItemChanged += OnSelectedItemChanged;
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

            View.ClearItems();
            View.LoadingPackages = false;
            packages.ForEach(p => View.AppendItem(p.Name, p.CurrVer));
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

            if (!String.IsNullOrEmpty(details) && name.Equals(View.SelectedItem))
            {
                View.UpdateDetails(details);
            }
        }

        private async void SyncListView()
        {
            string filterName = View.FilterText;
            bool hideSuffixed = View.ShowTopLevelOnly;

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

                if (!String.IsNullOrEmpty(filterName) && !package.Name.Contains(filterName))
                {
                    packageAllowed = false;
                }

                bool isDisplayed = View.Items.Contains(package.Name);

                // act
                if (isDisplayed && !packageAllowed) View.RemoveItem(package.Name);
                else if (!isDisplayed && packageAllowed) InsertItem(package, packages);
            }
        }

        public void InsertItem(GenericPackage package, List<GenericPackage> referenceList)
        {
            int latestPossibleIndex = referenceList.IndexOf(package);
            string lastVisibilePackage = null;

            // find package that is supposed to be directly above it
            for (int j = 0; j < latestPossibleIndex; j++)
            {
                if (View.Items.Contains(referenceList[j].Name))
                {
                    lastVisibilePackage = referenceList[j].Name;
                }
            }

            // insert
            int index = 0;
            if (lastVisibilePackage != null)
            {
                index = View.Items.IndexOf(lastVisibilePackage) + 1;
            }

            View.InsertItem(index, package.Name, package.CurrVer);
        }
    }
}
