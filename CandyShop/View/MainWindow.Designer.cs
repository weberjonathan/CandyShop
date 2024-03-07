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
            MainPanel.SuspendLayout();
            tabControl1.SuspendLayout();
            TabUpgrade.SuspendLayout();
            TabInstalled.SuspendLayout();
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
            InstalledPage.LoadingDetails = false;
            InstalledPage.LoadingPackages = false;
            InstalledPage.Location = new System.Drawing.Point(3, 3);
            InstalledPage.Name = "InstalledPage";
            InstalledPage.Size = new System.Drawing.Size(716, 476);
            InstalledPage.TabIndex = 0;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(730, 534);
            Controls.Add(MainPanel);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "MainWindow";
            Text = "ChocoAutoUpdateForm";
            Load += ChocoAutoUpdateForm_Load;
            MainPanel.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            TabUpgrade.ResumeLayout(false);
            TabInstalled.ResumeLayout(false);
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
    }
}