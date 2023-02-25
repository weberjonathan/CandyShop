using CandyShop.Chocolatey;
using System;
using System.Collections.Generic;
using CandyShop.View;
using CandyShop.Properties;
using CandyShop.Services;
using Serilog;
using System.Threading.Tasks;

namespace CandyShop.Controller
{
    class InstalledPageController : IInstalledPageController
    {
        private readonly ChocolateyService ChocolateyService;
        private IInstalledPageView View;

        public InstalledPageController(ChocolateyService chocolateyService)
        {
            ChocolateyService = chocolateyService;
        }

        public void InjectView(IInstalledPageView view)
        {
            View = view;

            View.FilterTextChanged += new EventHandler((sender, e) => SyncListView());
            View.HideDependenciesChanged += new EventHandler((sender, e) => SyncListView());
            View.SelectedItemChanged += OnSelectedItemChanged;
        }

        public void InitView()
        {
            RequestInstalledPackagesAsync();
        }

        private async void RequestInstalledPackagesAsync()
        {
            List<ChocolateyPackage> packages = new List<ChocolateyPackage>();
            try
            {
                packages = await ChocolateyService.GetInstalledPackagesAsync();
            }
            catch (ChocolateyException e)
            {
                Log.Error($"An error occurred while retrieving installed packages: {e.Message}");
            }

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

            View.UpdateDetails(LocaleEN.TEXT_LOADING);

            ChocolateyPackage packageMock = new ChocolateyPackage()
            {
                Name = View.SelectedItem
            };

            string details;
            try
            {
                details = await ChocolateyService.GetPackageDetails(packageMock);
            }
            catch (ChocolateyException exception)
            {
                details = String.Format(LocaleEN.ERROR_RETRIEVING_PACKAGE_DETAILS, exception.Message);
            }

            if (!String.IsNullOrEmpty(details) && packageMock.Name.Equals(View.SelectedItem))
            {
                View.UpdateDetails(details);
            }
        }

        private async void SyncListView()
        {
            string filterName = View.FilterText;
            bool hideSuffixed = View.HideDependencies;

            List<ChocolateyPackage> packages = await ChocolateyService.GetInstalledPackagesAsync();

            foreach (ChocolateyPackage package in packages)
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

        public void InsertItem(ChocolateyPackage package, List<ChocolateyPackage> referenceList)
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
