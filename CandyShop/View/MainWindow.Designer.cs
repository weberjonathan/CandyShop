using System;

namespace CandyShop.View
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            MainPanel = new System.Windows.Forms.Panel();
            tabControl1 = new System.Windows.Forms.TabControl();
            TabUpgrade = new System.Windows.Forms.TabPage();
            UpgradePage = new UpgradePage();
            TabInstalled = new System.Windows.Forms.TabPage();
            InstalledPage = new InstalledPage();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            CandyShopMenu = new System.Windows.Forms.MenuStrip();
            MenuEdit = new System.Windows.Forms.ToolStripMenuItem();
            MenuEditRefresh = new System.Windows.Forms.ToolStripMenuItem();
            MenuEditRefreshInstalled = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            MenuEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            MenuEditSelectRelevant = new System.Windows.Forms.ToolStripMenuItem();
            MenuEditDeselectAll = new System.Windows.Forms.ToolStripMenuItem();
            MenuExtras = new System.Windows.Forms.ToolStripMenuItem();
            MenuExtrasCreateTask = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            MenuExtrasOpenLogs = new System.Windows.Forms.ToolStripMenuItem();
            MenuExtrasOpenCandyShopConfig = new System.Windows.Forms.ToolStripMenuItem();
            MenuHelp = new System.Windows.Forms.ToolStripMenuItem();
            MenuHelpGithub = new System.Windows.Forms.ToolStripMenuItem();
            MenuHelpLicense = new System.Windows.Forms.ToolStripMenuItem();
            MenuHelpMetaPackages = new System.Windows.Forms.ToolStripMenuItem();
            MainPanel.SuspendLayout();
            tabControl1.SuspendLayout();
            TabUpgrade.SuspendLayout();
            TabInstalled.SuspendLayout();
            CandyShopMenu.SuspendLayout();
            SuspendLayout();
            // 
            // MainPanel
            // 
            MainPanel.BackColor = System.Drawing.SystemColors.Window;
            MainPanel.Controls.Add(tabControl1);
            MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            MainPanel.Location = new System.Drawing.Point(0, 24);
            MainPanel.Name = "MainPanel";
            MainPanel.Size = new System.Drawing.Size(730, 510);
            MainPanel.TabIndex = 1;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(TabUpgrade);
            tabControl1.Controls.Add(TabInstalled);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(730, 510);
            tabControl1.TabIndex = 3;
            // 
            // TabUpgrade
            // 
            TabUpgrade.Controls.Add(UpgradePage);
            TabUpgrade.Location = new System.Drawing.Point(4, 24);
            TabUpgrade.Name = "TabUpgrade";
            TabUpgrade.Padding = new System.Windows.Forms.Padding(3);
            TabUpgrade.Size = new System.Drawing.Size(722, 482);
            TabUpgrade.TabIndex = 0;
            TabUpgrade.Text = "Upgrade";
            TabUpgrade.UseVisualStyleBackColor = true;
            // 
            // UpgradePage
            // 
            UpgradePage.CleanShortcuts = false;
            UpgradePage.CloseAfterUpgrade = false;
            UpgradePage.Dock = System.Windows.Forms.DockStyle.Fill;
            UpgradePage.Loading = true;
            UpgradePage.Location = new System.Drawing.Point(3, 3);
            UpgradePage.Name = "UpgradePage";
            UpgradePage.ShowAdminWarning = true;
            UpgradePage.ShowUacIcons = false;
            UpgradePage.Size = new System.Drawing.Size(716, 476);
            UpgradePage.TabIndex = 2;
            // 
            // TabInstalled
            // 
            TabInstalled.Controls.Add(InstalledPage);
            TabInstalled.Location = new System.Drawing.Point(4, 24);
            TabInstalled.Name = "TabInstalled";
            TabInstalled.Padding = new System.Windows.Forms.Padding(3);
            TabInstalled.Size = new System.Drawing.Size(722, 482);
            TabInstalled.TabIndex = 1;
            TabInstalled.Text = "Installed";
            TabInstalled.UseVisualStyleBackColor = true;
            // 
            // InstalledPage
            // 
            InstalledPage.Dock = System.Windows.Forms.DockStyle.Fill;
            InstalledPage.EnableTopLevelToggle = true;
            InstalledPage.Loading = false;
            InstalledPage.Location = new System.Drawing.Point(3, 3);
            InstalledPage.Name = "InstalledPage";
            InstalledPage.Size = new System.Drawing.Size(716, 476);
            InstalledPage.TabIndex = 0;
            // 
            // CandyShopMenu
            // 
            CandyShopMenu.BackColor = System.Drawing.SystemColors.Window;
            CandyShopMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { MenuEdit, MenuExtras, MenuHelp });
            CandyShopMenu.Location = new System.Drawing.Point(0, 0);
            CandyShopMenu.Name = "CandyShopMenu";
            CandyShopMenu.Size = new System.Drawing.Size(730, 24);
            CandyShopMenu.TabIndex = 3;
            CandyShopMenu.Text = "Menu";
            // 
            // MenuEdit
            // 
            MenuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { MenuEditRefresh, MenuEditRefreshInstalled, toolStripSeparator2, MenuEditSelectAll, MenuEditSelectRelevant, MenuEditDeselectAll });
            MenuEdit.Name = "MenuEdit";
            MenuEdit.Size = new System.Drawing.Size(39, 20);
            MenuEdit.Text = "&Edit";
            // 
            // MenuEditRefresh
            // 
            MenuEditRefresh.Name = "MenuEditRefresh";
            MenuEditRefresh.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R;
            MenuEditRefresh.Size = new System.Drawing.Size(338, 22);
            MenuEditRefresh.Text = "&Refresh all";
            // 
            // MenuEditRefreshInstalled
            // 
            MenuEditRefreshInstalled.Name = "MenuEditRefreshInstalled";
            MenuEditRefreshInstalled.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.R;
            MenuEditRefreshInstalled.Size = new System.Drawing.Size(338, 22);
            MenuEditRefreshInstalled.Text = "Refresh &installed packages";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(335, 6);
            // 
            // MenuEditSelectAll
            // 
            MenuEditSelectAll.Name = "MenuEditSelectAll";
            MenuEditSelectAll.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A;
            MenuEditSelectAll.Size = new System.Drawing.Size(338, 22);
            MenuEditSelectAll.Text = "&Select all";
            MenuEditSelectAll.Click += MenuEditSelectAll_Click;
            // 
            // MenuEditSelectRelevant
            // 
            MenuEditSelectRelevant.Name = "MenuEditSelectRelevant";
            MenuEditSelectRelevant.Size = new System.Drawing.Size(338, 22);
            MenuEditSelectRelevant.Text = "Select &top level packages";
            MenuEditSelectRelevant.Click += MenuEditSelectRelevant_Click;
            // 
            // MenuEditDeselectAll
            // 
            MenuEditDeselectAll.Name = "MenuEditDeselectAll";
            MenuEditDeselectAll.Size = new System.Drawing.Size(338, 22);
            MenuEditDeselectAll.Text = "&Deselect all";
            MenuEditDeselectAll.Click += MenuEditDeselectAll_Click;
            // 
            // MenuExtras
            // 
            MenuExtras.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { MenuExtrasCreateTask, toolStripSeparator1, MenuExtrasOpenLogs, MenuExtrasOpenCandyShopConfig });
            MenuExtras.Name = "MenuExtras";
            MenuExtras.Size = new System.Drawing.Size(50, 20);
            MenuExtras.Text = "&Extras";
            // 
            // MenuExtrasCreateTask
            // 
            MenuExtrasCreateTask.Name = "MenuExtrasCreateTask";
            MenuExtrasCreateTask.Size = new System.Drawing.Size(344, 22);
            MenuExtrasCreateTask.Text = "&Display notification for outdated packages on login";
            MenuExtrasCreateTask.Click += MenuExtrasCreateTask_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(341, 6);
            // 
            // MenuExtrasOpenLogs
            // 
            MenuExtrasOpenLogs.Name = "MenuExtrasOpenLogs";
            MenuExtrasOpenLogs.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L;
            MenuExtrasOpenLogs.Size = new System.Drawing.Size(344, 22);
            MenuExtrasOpenLogs.Text = "Open &Chocolatey logs";
            MenuExtrasOpenLogs.Click += MenuExtrasOpenLogs_Click;
            // 
            // MenuExtrasOpenCandyShopConfig
            // 
            MenuExtrasOpenCandyShopConfig.Name = "MenuExtrasOpenCandyShopConfig";
            MenuExtrasOpenCandyShopConfig.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P;
            MenuExtrasOpenCandyShopConfig.Size = new System.Drawing.Size(344, 22);
            MenuExtrasOpenCandyShopConfig.Text = "Open Candy&Shop configuration folder";
            MenuExtrasOpenCandyShopConfig.Click += MenuExtrasOpenCandyShopConfig_Click;
            // 
            // MenuHelp
            // 
            MenuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { MenuHelpGithub, MenuHelpLicense, MenuHelpMetaPackages });
            MenuHelp.Name = "MenuHelp";
            MenuHelp.Size = new System.Drawing.Size(44, 20);
            MenuHelp.Text = "&Help";
            // 
            // MenuHelpGithub
            // 
            MenuHelpGithub.Name = "MenuHelpGithub";
            MenuHelpGithub.Size = new System.Drawing.Size(153, 22);
            MenuHelpGithub.Text = "&Github";
            MenuHelpGithub.Click += MenuHelpGithub_Click;
            // 
            // MenuHelpLicense
            // 
            MenuHelpLicense.Name = "MenuHelpLicense";
            MenuHelpLicense.Size = new System.Drawing.Size(153, 22);
            MenuHelpLicense.Text = "&License";
            MenuHelpLicense.Click += MenuHelpLicense_Click;
            // 
            // MenuHelpMetaPackages
            // 
            MenuHelpMetaPackages.Name = "MenuHelpMetaPackages";
            MenuHelpMetaPackages.Size = new System.Drawing.Size(153, 22);
            MenuHelpMetaPackages.Text = "&Meta packages";
            MenuHelpMetaPackages.Click += MenuHelpMetaPackages_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(730, 534);
            Controls.Add(MainPanel);
            Controls.Add(CandyShopMenu);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = CandyShopMenu;
            Name = "MainWindow";
            Text = "ChocoAutoUpdateForm";
            Load += ChocoAutoUpdateForm_Load;
            MainPanel.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            TabUpgrade.ResumeLayout(false);
            TabInstalled.ResumeLayout(false);
            CandyShopMenu.ResumeLayout(false);
            CandyShopMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private UpgradePage UpgradePage;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabUpgrade;
        private System.Windows.Forms.TabPage TabInstalled;
        private InstalledPage InstalledPage;
        private System.Windows.Forms.MenuStrip CandyShopMenu;
        private System.Windows.Forms.ToolStripMenuItem MenuExtras;
        private System.Windows.Forms.ToolStripMenuItem MenuExtrasCreateTask;
        private System.Windows.Forms.ToolStripMenuItem MenuHelp;
        private System.Windows.Forms.ToolStripMenuItem MenuHelpGithub;
        private System.Windows.Forms.ToolStripMenuItem MenuHelpLicense;
        private System.Windows.Forms.ToolStripMenuItem MenuEdit;
        private System.Windows.Forms.ToolStripMenuItem MenuEditSelectAll;
        private System.Windows.Forms.ToolStripMenuItem MenuEditDeselectAll;
        private System.Windows.Forms.ToolStripMenuItem MenuEditSelectRelevant;
        private System.Windows.Forms.ToolStripMenuItem MenuHelpMetaPackages;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MenuExtrasOpenLogs;
        private System.Windows.Forms.ToolStripMenuItem MenuExtrasOpenCandyShopConfig;
        private System.Windows.Forms.ToolStripMenuItem MenuEditRefresh;
        private System.Windows.Forms.ToolStripMenuItem MenuEditRefreshInstalled;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}