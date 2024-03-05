using System;
using System.Reflection;
using System.Windows.Forms;
using CandyShop.Controller;
using CandyShop.Properties;

namespace CandyShop.View
{
    // TODO remove "select top level"-menu item in winget mode
    // TODO add menu item -> Restart in other mode

    partial class MainWindow : Form, IMainWindowView
    {
        private MainWindowController Controller;

        public MainWindow(MainWindowController candyShopController)
        {
            Controller = candyShopController;
            InitializeComponent();
            MenuEditSelectRelevant.Text = LocaleEN.TEXT_TS_SELECT_SMART;
            MenuEditSelectAll.Text = LocaleEN.TEXT_TS_SELECT_ALL;
            MenuEditDeselectAll.Text = LocaleEN.TEXT_TS_DESELECT;
            MenuEditRefresh.Text = LocaleEN.TEXT_TS_REFRESH;
            MenuEditRefresh.Click += new EventHandler((sender, e) => RefreshClicked?.Invoke(sender, e));
            MenuEditRefreshInstalled.Text = LocaleEN.TEXT_TS_REFRESH_INSTALLED;
            MenuEditRefreshInstalled.Visible = false;
        }

        public event EventHandler RefreshClicked;

        public IInstalledPageView InstalledPackagesPage => InstalledPage;
        public IUpgradePageView UpgradePackagesPage => UpgradePage;

        public bool LaunchOnSystemStartEnabled
        {
            get
            {
                return MenuExtrasCreateTask.Checked;
            }
            set
            {
                MenuExtrasCreateTask.Checked = value;
            }
        }

        public void DisplayError(string msg, params string[] args)
        {
            if (args != null && args.Length > 0) msg = String.Format(msg, args);
            MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowAdminHints()
        {
            UpgradePage.ShowAdminWarning = true;
        }

        public void ClearAdminHints()
        {
            UpgradePage.ShowAdminWarning = false;
        }

        private void ChocoAutoUpdateForm_Load(object sender, EventArgs e)
        {
            this.Activate();
        }

        private void MenuEditSelectAll_Click(object sender, EventArgs e)
        {
            UpgradePage.CheckAllItems();
        }

        private void MenuEditSelectRelevant_Click(object sender, EventArgs e)
        {
            UpgradePage.CheckTopLevelItems();
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

        private void MenuExtrasCreateTask_Click(object sender, EventArgs e)
        {
            Controller.ToggleLaunchOnSystemStart();
        }

        private void MenuExtrasOpenLogs_Click(object sender, EventArgs e)
        {
            Controller.ShowChocoLogFolder();
        }

        private void MenuExtrasOpenCandyShopConfig_Click(object sender, EventArgs e)
        {
            Controller.ShowCandyShopConfigFolder();
        }
    }
}
