using CandyShop.Chocolatey;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;
using CandyShop.Properties;
using System.Linq;
using Serilog;

namespace CandyShop.View
{
    partial class CandyShopForm : Form, IMainWindow
    {
        // TODO remove WindowsTaskService, use controller
        // remove and sort usings
        // check namepsaces
        // consistent behavior between controls
        // use controller for all methods in here
        // use consistent naming for private members
        // use consistent layout privates -> constructor -> properties -> methods https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
        // https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1201.md

        private WindowsTaskService WindowsTaskService; // TODO remove
        private readonly CandyShopController CandyShopController; // TODO remove

        public CandyShopForm(CandyShopController candyShopController)
        {
            WindowsTaskService = new WindowsTaskService();
            CandyShopController = candyShopController;
            InitializeComponent();

            Text = String.Format(Properties.Strings.Form_Title, Application.ProductName, Application.ProductVersion);
            
            FormClosed += new FormClosedEventHandler((sender, e) => CandyShopController.CloseForm());
            MenuExtrasCreateTask.CheckedChanged += new EventHandler((sender, e) => CreateTaskEnabledChanged?.Invoke(sender, e));
        }

        public event EventHandler CreateTaskEnabledChanged;

        public IInstalledPage InstalledPackagesPage => InstalledPage;
        public IUpgradePage UpgradePackagesPage => UpgradePage;

        public bool CreateTaskEnabled => MenuExtrasCreateTask.Checked;

        public void ShowAdminHints()
        {
            UpgradePage.ShowAdminWarning = true;
            this.Text = String.Format(Properties.Strings.Form_Title, Application.ProductName, Application.ProductVersion) + Properties.Strings.Form_Title_AdminHint;
        }

        public void ClearAdminHints()
        {
            UpgradePage.ShowAdminWarning = false;
            this.Text = String.Format(Properties.Strings.Form_Title, Application.ProductName, Application.ProductVersion);
        }

        private void ChocoAutoUpdateForm_Load(object sender, EventArgs e)
        {
            // register handlers
            UpgradePage.UpgradeAllClick += UpgradePage_UpgradeAllClick;
            UpgradePage.UpgradeSelectedClick += UpgradePage_UpgradeSelectedClick;
            UpgradePage.CancelClick += UpgradePage_CancelClick;

            // check task entry or not
            MenuExtrasCreateTask.Checked = WindowsTaskService.TaskExists();

            this.Activate();
        }

        private void MenuEditSelectAll_Click(object sender, EventArgs e)
        {
            UpgradePage.CheckAllItems();
        }

        private void MenuEditSelectRelevant_Click(object sender, EventArgs e)
        {
            List<string> packageNames = UpgradePage.Items.ToList();
            packageNames = CandyShopController.SelectNormalAndMetaPackages(packageNames);
            UpgradePage.CheckItemsByText(packageNames);
        }

        private void MenuEditDeselectAll_Click(object sender, EventArgs e)
        {
            UpgradePage.UncheckAllItems();
        }

        private void MenuHelpGithub_Click(object sender, EventArgs e)
        {
            CandyShopController.OpenUrl(Strings.Url_Github);
        }

        private void MenuHelpLicense_Click(object sender, EventArgs e)
        {
            CandyShopController.ShowLicenseForm();
        }

        private void MenuHelpMetaPackages_Click(object sender, EventArgs e)
        {
            CandyShopController.OpenUrl(Strings.Url_MetaPackages);
        }

        private void UpgradePage_UpgradeAllClick(object sender, EventArgs e)
        {
            //List<ChocolateyPackage> packages =
            //    CandyShopController.GetPackagesByName(UpgradePage.Items.ToList());
            //CandyShopController.PerformUpgrade(packages);
        }

        private void UpgradePage_UpgradeSelectedClick(object sender, EventArgs e)
        {
            //List<ChocolateyPackage> packages =
            //    CandyShopController.GetPackagesByName(UpgradePage.SelectedItems.ToList());

            //if (packages.Count > 0) CandyShopController.PerformUpgrade(packages);
        }

        private void UpgradePage_CancelClick(object sender, EventArgs e)
        {
            CandyShopController.CancelForm();
        }

        private void ShowError(string msg)
        {
            MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
