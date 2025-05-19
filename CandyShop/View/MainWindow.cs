using System;
using System.Linq;
using System.Windows.Forms;
using CandyShop.Controls;
using CandyShop.Controls.Factory;
using CandyShop.Settings;

namespace CandyShop.View
{
    partial class MainWindow : Form, ITabPage, ISettingsListener
    {
        private ToolStripMenuItem StartWithSystemCheckBox;

        public MainWindow()
        {
            InitializeComponent();
        }

        public event EventHandler RefreshClicked;
        public event EventHandler OpenLogsClicked;
        public event EventHandler OpenSettingsClicked;
        public event EventHandler OpenSettingsDirClicked;
        public event EventHandler LaunchOnSystemStartClicked;
        public event EventHandler ShowGithubClicked;
        public event EventHandler ShowLicenseClicked;
        public event EventHandler ShowMetaPackageHelpClicked;
        public event EventHandler TogglePackageSourceClicked;

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

        public void BuildControls(IUiComponents provider)
        {
            CandyShopMenuStrip menu = provider.GetMenuStrip();
            MainMenuStrip = menu;
            Controls.Add(MainMenuStrip);

            menu.ItemAt("Edit", "Refresh").Click     += (sender, e) => RefreshClicked?.Invoke(sender, e);
            menu.ItemAt("Edit", "SelectAll").Click   += (sender, e) => UpgradePage.CheckAllItems();
            menu.ItemAt("Edit", "SelectTop").Click   += (sender, e) => UpgradePage.CheckTopLevelItems();
            menu.ItemAt("Edit", "DeselectAll").Click += (sender, e) => UpgradePage.UncheckAllItems();

            menu.ItemAt("Extras", "SwitchMode").Click      += (sender, e) => TogglePackageSourceClicked?.Invoke(sender, e);
            menu.ItemAt("Extras", "StartWithSystem").Click += (sender, e) => LaunchOnSystemStartClicked?.Invoke(sender, e);
            menu.ItemAt("Extras", "SettingsWindow").Click  += (sender, e) => OpenSettingsClicked?.Invoke(sender, e);
            menu.ItemAt("Extras", "SettingsDir").Click     += (sender, e) => OpenSettingsDirClicked?.Invoke(sender, e);
            menu.ItemAt("Extras", "Logs").Click            += (sender, e) => OpenLogsClicked?.Invoke(sender, e);

            menu.ItemAt("Help", "Github").Click  += (sender, e) => ShowGithubClicked?.Invoke(sender, e);
            menu.ItemAt("Help", "License").Click += (sender, e) => ShowLicenseClicked?.Invoke(sender, e);
            menu.ItemAt("Help", "Meta").Click    += (sender, e) => ShowMetaPackageHelpClicked?.Invoke(sender, e);

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
        }
    }
}
