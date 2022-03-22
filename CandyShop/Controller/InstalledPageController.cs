using CandyShop.Chocolatey;
using System;
using System.Collections.Generic;
using CandyShop.View;
using CandyShop.Properties;

namespace CandyShop.Controller
{
    class InstalledPageController
    {
        private readonly ChocolateyService ChocolateyService;
        private readonly IInstalledPageView View;

        public InstalledPageController(ChocolateyService chocolateyService, IInstalledPageView view)
        {
            ChocolateyService = chocolateyService;
            View = view;

            View.FilterTextChanged += new EventHandler((sender, e) => SyncListView());
            View.HideDependenciesChanged += new EventHandler((sender, e) => SyncListView());
            View.SelectedItemChanged += OnSelectedItemChanged;

            RequestInstalledPackages();
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
                details = await ChocolateyService.GetOrFetchInfo(packageMock);
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

        private async void RequestInstalledPackages()
        {
            List<ChocolateyPackage> packages = new List<ChocolateyPackage>();
            try
            {
                packages = await ChocolateyService.FetchInstalledAsync();
            }
            catch (ChocolateyException)
            {
                throw;
            }

            packages.ForEach(p => View.AppendItem(p.Name, p.CurrVer));
        }

        private void SyncListView()
        {
            string filterName = View.FilterText;
            bool hideSuffixed = View.HideDependencies;

            IEnumerable<ChocolateyPackage> packages = ChocolateyService.GetPackagesByName(View.Items);

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
                if (View.Items.Contains(package.Name))
                {
                    if (packageAllowed)
                    {
                        View.InsertItem(package.Name, package.CurrVer);
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
    }
}
