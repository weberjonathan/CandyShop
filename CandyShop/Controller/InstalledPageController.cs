using CandyShop.Chocolatey;
using System;
using System.Collections.Generic;
using CandyShop.View;
using CandyShop.Properties;
using CandyShop.Services;

namespace CandyShop.Controller
{
    class InstalledPageController
    {
        private readonly ChocolateyService ChocolateyService;
        private IInstalledPageView View;

        public InstalledPageController(ChocolateyService chocolateyService)
        {
            ChocolateyService = chocolateyService;
            RequestInstalledPackagesAsync();
        }

        public void InjectView(IInstalledPageView view)
        {
            View = view;

            View.FilterTextChanged += new EventHandler((sender, e) => SyncListView());
            View.HideDependenciesChanged += new EventHandler((sender, e) => SyncListView());
            View.SelectedItemChanged += OnSelectedItemChanged;
        }

        private async void RequestInstalledPackagesAsync()
        {
            List<ChocolateyPackage> packages = new List<ChocolateyPackage>();
            try
            {
                packages = await ChocolateyService.GetInstalledPackagesAsync();
            }
            catch (ChocolateyException)
            {
                throw;
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

            View.UpdateDetails(Strings.Txt_Loading);

            ChocolateyPackage packageMock = new ChocolateyPackage()
            {
                Name = View.SelectedItem
            };

            string details;
            try
            {
                details = await ChocolateyService.GetPackageDetails(packageMock);
            }
            catch (ChocolateyException)
            {
                throw;
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
                if (hideSuffixed && package.HasSuffix)
                {
                    packageAllowed = false;
                }

                if (!String.IsNullOrEmpty(filterName) && !package.Name.Contains(filterName))
                {
                    packageAllowed = false;
                }

                // determine whether it is displayed
                if (!View.Items.Contains(package.Name))
                {
                    if (packageAllowed)
                    {
                        InsertItem(package, packages);
                    }
                }
                else
                {
                    if (!packageAllowed)
                    {
                        View.RemoveItem(package.Name);
                    }
                }
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
