using System;
using System.Linq;
using System.Windows.Forms;
using CandyShop.Controls;
using CandyShop.Controls.Factory;
using CandyShop.Properties;
using CandyShop.Settings;

namespace CandyShop.View
{
    partial class MainWindow : Form, ITabPage, ISettingsListener
    {
        private ToolStripMenuItem StartWithSystemCheckBox;

        public MainWindow()
        {
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
        public event EventHandler OpenLogsClicked;
        public event EventHandler OpenSettingsClicked;
        public event EventHandler OpenSettingsDirClicked;
        public event EventHandler LaunchOnSystemStartClicked;
        public event EventHandler ShowGithubClicked;
        public event EventHandler ShowLicenseClicked;
        public event EventHandler ShowMetaPackageHelpClicked;

        public InstalledPage InstalledPackagesPage => InstalledPage; // TODO remove these
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

        public void BuildControls(IUiComponents provider)
        {
            CandyShopMenuStrip menu = provider.GetMenuStrip();
            MainMenuStrip = menu;
            Controls.Add(MainMenuStrip);

            menu.ItemAt("Edit", "Refresh").Click     += new EventHandler((sender, e) => RefreshClicked?.Invoke(sender, e));
            menu.ItemAt("Edit", "SelectAll").Click   += new EventHandler((sender, e) => UpgradePage.CheckAllItems());
            menu.ItemAt("Edit", "SelectTop").Click   += new EventHandler((sender, e) => UpgradePage.CheckTopLevelItems());
            menu.ItemAt("Edit", "DeselectAll").Click += new EventHandler((sender, e) => UpgradePage.UncheckAllItems());

            //menu.ItemAt("Extras", "SwitchMode").Click      += new EventHandler((sender, e) => Controller.TogglePackageSource()); // TODO fully remove item
            menu.ItemAt("Extras", "StartWithSystem").Click += new EventHandler((sender, e) => LaunchOnSystemStartClicked?.Invoke(sender, e));
            menu.ItemAt("Extras", "SettingsWindow").Click  += new EventHandler((sender, e) => OpenSettingsClicked?.Invoke(sender, e));
            menu.ItemAt("Extras", "SettingsDir").Click     += new EventHandler((sender, e) => OpenSettingsDirClicked?.Invoke(sender, e));
            menu.ItemAt("Extras", "Logs").Click            += new EventHandler((sender, e) => OpenLogsClicked?.Invoke(sender, e));

            menu.ItemAt("Help", "Github").Click  += new EventHandler((sender, e) => ShowGithubClicked?.Invoke(sender, e));
            menu.ItemAt("Help", "License").Click += new EventHandler((sender, e) => ShowLicenseClicked?.Invoke(sender, e));
            menu.ItemAt("Help", "Meta").Click    += new EventHandler((sender, e) => ShowMetaPackageHelpClicked?.Invoke(sender, e));

            StartWithSystemCheckBox = menu.ItemAt("Extras", "StartWithSystem");
        }

        public void DisplayError(string msg, params string[] args)
        {
            if (args != null && args.Length > 0) msg = String.Format(msg, args);
            MessageBox.Show(msg, MetaInfo.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowAndRefresh()
        {
            RefreshClicked?.Invoke(this, EventArgs.Empty);
            Show();
        }

        public void OnSettingsChanged(SettingsDefinition settings)
        {
            Text = MetaInfo.GetAppTitle(settings.EnabledPackageManagers.First().Name);

            ShowAdminWarning =
                !Util.IsAdmin() && // TODO could move this to OnSettingsChanged param
                !settings.ElevateOnDemand && // TODO rename requireAdminRights or whatever it says in the settings window
                !settings.SupressNoRightsWarning;
        }
    }
}
