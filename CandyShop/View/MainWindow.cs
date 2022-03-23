using System;
using System.Windows.Forms;
using CandyShop.Controller;

namespace CandyShop.View
{
    partial class MainWindow : Form, IMainWindowView
    {
        // remove and sort usings
        // check namepsaces
        // consistent behavior between controls
        // use controller for all methods in here
        // use consistent naming for private members
        // use consistent layout privates -> constructor -> properties -> methods https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
        // https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1201.md
        // clean names of string table

        private IMainWindowController Controller;

        public MainWindow(IMainWindowController candyShopController)
        {
            Controller = candyShopController;
            InitializeComponent();

            Text = String.Format(Properties.Strings.Form_Title, Application.ProductName, Application.ProductVersion);
            
            MenuExtrasCreateTask.CheckedChanged += new EventHandler((sender, e) => CreateTaskEnabledChanged?.Invoke(sender, e));
        }

        public event EventHandler CreateTaskEnabledChanged;
        public event EventHandler CancelPressed;

        public IInstalledPageView InstalledPackagesPage => InstalledPage;
        public IUpgradePageView UpgradePackagesPage => UpgradePage;

        public bool CreateTaskEnabled
        {
            get
            {
                return MenuExtrasCreateTask.Checked;
            }
            set
            {
                MenuExtrasCreateTask.Checked = true;
            }
        }

        public void DisplayError(string msg)
        {
            MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

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
            // register upgrade page handlers
            UpgradePage.UpgradeAllClick += UpgradePage_UpgradeAllClick;
            UpgradePage.UpgradeSelectedClick += UpgradePage_UpgradeSelectedClick;
            UpgradePage.CancelClick += UpgradePage_CancelClick;

            this.Activate();
        }

        private void MenuEditSelectAll_Click(object sender, EventArgs e)
        {
            UpgradePage.CheckAllItems();
        }

        private void MenuEditSelectRelevant_Click(object sender, EventArgs e)
        {
            Controller.SmartSelectPackages();
        }

        private void MenuEditDeselectAll_Click(object sender, EventArgs e)
        {
            UpgradePage.UncheckAllItems();
        }

        private void MenuHelpGithub_Click(object sender, EventArgs e)
        {
            Controller.ShowGithub();
        }

        private void MenuHelpLicense_Click(object sender, EventArgs e)
        {
            Controller.ShowLicenses();
        }

        private void MenuHelpMetaPackages_Click(object sender, EventArgs e)
        {
            Controller.ShowMetaPackageHelp();
        }

        private void MenuExtrasCreateTask_CheckedChanged(object sender, EventArgs e)
        {
            Controller.ToggleCreateTask();
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
            CancelPressed?.Invoke(sender, e);
        }
    }
}
