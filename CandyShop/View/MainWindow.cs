using System;
using System.Windows.Forms;
using CandyShop.Controller;
using CandyShop.Controls;
using CandyShop.Controls.Factory;
using CandyShop.Properties;

namespace CandyShop.View
{
    // TODO remove "select top level"-menu item in winget mode
    // TODO add menu item -> Restart in other mode

    partial class MainWindow : Form, ITabPage
    {
        private MainWindowController Controller;

        private ToolStripMenuItem StartWithSystemCheckBox;

        public MainWindow(MainWindowController candyShopController)
        {
            Controller = candyShopController;
            InitializeComponent();

            AdminBanner.Visible = false;
            AdminBanner.Text = LocaleEN.TEXT_NO_ADMIN_HINT;
            AdminBanner.Closing += new EventHandler((sender, e) =>
            {
                var result = MessageBox.Show(LocaleEN.TEXT_HIDE_PERMANENTLY,
                    MetaInfo.Name,
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                switch (result)
                {
                    case DialogResult.Yes:
                        ShowAdminWarning = false;
                        HideAdminWarningClicked?.Invoke(this, e);
                        break;
                    case DialogResult.No:
                        ShowAdminWarning = false;
                        break;
                    default:
                        break;
                }
            });
        }

        public event EventHandler RefreshClicked;
        public event EventHandler HideAdminWarningClicked;

        public InstalledPage InstalledPackagesPage => InstalledPage;
        public UpgradePage UpgradePackagesPage => UpgradePage;

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

        public bool ShowAdminWarning
        {
            get
            {
                return AdminBanner.Visible;
            }
            set
            {
                AdminBanner.Visible = value;
            }
        }

        public void BuildControls(IControlsFactory provider)
        {
            CandyShopMenuStrip menu = provider.GetMenuStrip();
            MainMenuStrip = menu;
            Controls.Add(MainMenuStrip);

            menu.ItemAt("Edit", "Refresh").Click     += new EventHandler((sender, e) => RefreshClicked?.Invoke(sender, e));
            menu.ItemAt("Edit", "SelectAll").Click   += new EventHandler((sender, e) => UpgradePage.CheckAllItems());
            menu.ItemAt("Edit", "SelectTop").Click   += new EventHandler((sender, e) => UpgradePage.CheckTopLevelItems());
            menu.ItemAt("Edit", "DeselectAll").Click += new EventHandler((sender, e) => UpgradePage.UncheckAllItems());

            menu.ItemAt("Extras", "SwitchMode").Click      += new EventHandler((sender, e) => Controller.TogglePackageSource());
            menu.ItemAt("Extras", "StartWithSystem").Click += new EventHandler((sender, e) => Controller.ToggleLaunchOnSystemStart());
            menu.ItemAt("Extras", "Settings").Click        += new EventHandler((sender, e) => Controller.ShowCandyShopConfigFolder());
            menu.ItemAt("Extras", "Logs").Click            += new EventHandler((sender, e) => Controller.ShowLogFolder());

            menu.ItemAt("Help", "Github").Click  += new EventHandler((sender, e) => Controller.ShowGithub());
            menu.ItemAt("Help", "License").Click += new EventHandler((sender, e) => Controller.ShowLicenses());
            menu.ItemAt("Help", "Meta").Click    += new EventHandler((sender, e) => Controller.ShowMetaPackageHelp());

            StartWithSystemCheckBox = menu.ItemAt("Extras", "StartWithSystem");
        }

        public void DisplayError(string msg, params string[] args)
        {
            if (args != null && args.Length > 0) msg = String.Format(msg, args);
            MessageBox.Show(msg, MetaInfo.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ChocoAutoUpdateForm_Load(object sender, EventArgs e)
        {
            this.Activate();
        }
    }
}
