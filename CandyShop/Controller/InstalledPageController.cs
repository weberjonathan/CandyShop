using System;
using System.Collections.Generic;
using CandyShop.View;
using CandyShop.Properties;
using CandyShop.Services;
using Serilog;
using CandyShop.PackageCore;
using CandyShop.Controls.Factory;
using CandyShop.Components;
using System.Linq;

namespace CandyShop.Controller
{
    class InstalledPageController
    {
        private readonly PackageService PackageService;
        private readonly IUiComponents ControlsFactory;
        private readonly IPackageFilterContext PackageListSyncContext; // TODO settings listener

        private InstalledPage View;

        public InstalledPageController(PackageService service, IUiComponents controlsFactory, IPackageFilterContext packageListSyncContext)
        {
            PackageService = service;
            ControlsFactory = controlsFactory;
            PackageListSyncContext = packageListSyncContext;
        }

        public void InjectView(InstalledPage view)
        {
            View = view;

            View.BuildControls(ControlsFactory);

            View.SearchBar.SearchChanged += new EventHandler((sender, e) => SyncListView());
            View.SearchBar.CheckedChanged += new EventHandler((sender, e) => SyncListView());
            View.PackagesAdded += new EventHandler((sender, e) => SyncListView());
            View.SelectedItemChanged += OnSelectedItemChanged;
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
            catch (PackageManagerException exception)
            {
                details = String.Format(LocaleEN.ERROR_RETRIEVING_PACKAGE_DETAILS, exception.Message);
            }

            if (!String.IsNullOrEmpty(details) && name.Equals(View.SelectedItem) && !View.Loading)
            {
                View.UpdateDetails(details);
            }
        }

        private async void SyncListView()
        {
            string filterName = View.SearchBar.Text;
            bool hideSuffixed = PackageListSyncContext.HideSuffixedEnabled && View.SearchBar.Checked;
            bool requireSource = PackageListSyncContext.RequireSourceEnabled && View.SearchBar.Checked;

            List<GenericPackage> packages = [];
            try
            {
                packages = (await PackageService.GetInstalledPackagesAsync()).ToList();
            }
            catch (PackageManagerException e)
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

            View.InsertItem(index, ControlsFactory.BuildInstalledItem(package));
        }
    }
}
