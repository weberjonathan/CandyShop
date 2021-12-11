using CandyShop.Chocolatey;
using CandyShop.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CandyShop
{
    internal class CandyShopController
    {
        private readonly ChocolateyService _ChocolateyService;
        private readonly WindowsTaskService _WindowsTaskService;
        private readonly CandyShopForm _CandyShopForm;

        private List<ChocolateyPackage> _InstalledPackages;
        private List<ChocolateyPackage> _OutdatedPackages;

        public CandyShopController(ChocolateyService chocolateyService, WindowsTaskService windowsTaskService)
        {
            _ChocolateyService = chocolateyService;
            _WindowsTaskService = windowsTaskService;
        }

        private bool? _HasAdminPrivileges;
        public bool HasAdminPrivileges
        {
            get
            {
                if (_HasAdminPrivileges == null)
                {
                    using WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    _HasAdminPrivileges = principal.IsInRole(WindowsBuiltInRole.Administrator);
                }

                return _HasAdminPrivileges.Value;
            }
        }

        public async void GetInstalledPackages(Action<List<ChocolateyPackage>> callback)
        {
            _InstalledPackages ??= await _ChocolateyService.FetchInstalledAsync(); // TODO test, see outdated packages
            callback(_InstalledPackages);
        }

        /// <exception cref="ChocolateyException"></exception>
        public async void GetOutdatedPackagesAsync(Action<List<ChocolateyPackage>> callback)
        {
            _OutdatedPackages ??= await _ChocolateyService.FetchOutdatedAsync(); // TODO test null check; should only assign if _OutdatedPackages is null
            callback(_OutdatedPackages);
        }

        /// <exception cref="ChocolateyException"></exception>
        public async void GetPackageDetails(string packageName, Action<string> callback)
        {
            ChocolateyPackage packageMock = new ChocolateyPackage()
            {
                Name = packageName
            };

            string details = await _ChocolateyService.GetInfo(packageMock);
            callback(details);
        }

        public void LaunchUrl(string url)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = "cmd",
                Arguments = $"/c start {url}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(info);
        }

        public void ShowLicenseForm()
        {
            using LicenseForm form = new LicenseForm();
            form.ShowDialog();
        }
    }
}
