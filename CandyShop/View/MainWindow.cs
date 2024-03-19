using System;
using System.Windows.Forms;
using CandyShop.Controller;
using CandyShop.Controls;

namespace CandyShop.View
{
    // TODO remove "select top level"-menu item in winget mode
    // TODO add menu item -> Restart in other mode

    partial class MainWindow : Form, IMainWindowView, ITabPage
    {
        private MainWindowController Controller;

        private ToolStripMenuItem StartWithSystemCheckBox;

        public MainWindow(MainWindowController candyShopController)
        {
            Controller = candyShopController;
            InitializeComponent();
        }

        public event EventHandler RefreshClicked;

        public IInstalledPageView InstalledPackagesPage => InstalledPage;
        public IUpgradePageView UpgradePackagesPage => UpgradePage;

        public bool LaunchOnSystemStartEnabled
        {
            get
            {
                return StartWithSystemCheckBox.Checked;
            }
            set
            {
                StartWithSystemCheckBox.Checked = value;
            }
        }

        public void BuildControls(AbstractCommon provider)
        {
            MainMenuStrip = provider.GetMenuStrip();
            Controls.Add(MainMenuStrip);

            ToolStripItem item(string s, string s2) => provider.ResolveMenuItem(MainMenuStrip, s, s2);

            item("Edit", "Refresh").Click     += new EventHandler((sender, e) => RefreshClicked?.Invoke(sender, e));
            item("Edit", "SelectAll").Click   += new EventHandler((sender, e) => UpgradePage.CheckAllItems());
            item("Edit", "SelectTop").Click   += new EventHandler((sender, e) => UpgradePage.CheckTopLevelItems());
            item("Edit", "DeselectAll").Click += new EventHandler((sender, e) => UpgradePage.UncheckAllItems());

            item("Extras", "StartWithSystem").Click += new EventHandler((sender, e) => Controller.ToggleLaunchOnSystemStart());
            item("Extras", "Settings").Click        += new EventHandler((sender, e) => Controller.ShowCandyShopConfigFolder());
            item("Extras", "Logs").Click            += new EventHandler((sender, e) => Controller.ShowLogFolder());

            item("Help", "Github").Click  += new EventHandler((sender, e) => Controller.ShowGithub());
            item("Help", "License").Click += new EventHandler((sender, e) => Controller.ShowLicenses());
            item("Help", "Meta").Click    += new EventHandler((sender, e) => Controller.ShowMetaPackageHelp());

            StartWithSystemCheckBox = (ToolStripMenuItem)item("Extras", "StartWithSystem");
        }

        public void DisplayError(string msg, params string[] args)
        {
            if (args != null && args.Length > 0) msg = String.Format(msg, args);
            MessageBox.Show(msg, MetaInfo.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
