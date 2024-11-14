using CandyShop.Controls;
using System;
using System.Windows.Forms;

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
            MainPanel = new Panel();
            tabControl1 = new TabControl();
            TabUpgrade = new TabPage();
            UpgradePage = new UpgradePage();
            TabInstalled = new TabPage();
            InstalledPage = new InstalledPage();
            AdminBanner = new Banner();
            columnHeader1 = new ColumnHeader();
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
            MainPanel.Controls.Add(AdminBanner);
            MainPanel.Dock = DockStyle.Fill;
            MainPanel.Location = new System.Drawing.Point(0, 0);
            MainPanel.Name = "MainPanel";
            MainPanel.Size = new System.Drawing.Size(730, 534);
            MainPanel.TabIndex = 1;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(TabUpgrade);
            tabControl1.Controls.Add(TabInstalled);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 64);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(730, 470);
            tabControl1.TabIndex = 3;
            // 
            // TabUpgrade
            // 
            TabUpgrade.Controls.Add(UpgradePage);
            TabUpgrade.Location = new System.Drawing.Point(4, 24);
            TabUpgrade.Name = "TabUpgrade";
            TabUpgrade.Padding = new Padding(3);
            TabUpgrade.Size = new System.Drawing.Size(722, 442);
            TabUpgrade.TabIndex = 0;
            TabUpgrade.Text = "Upgrade";
            TabUpgrade.UseVisualStyleBackColor = true;
            // 
            // UpgradePage
            // 
            UpgradePage.AllowPinnedUacIon = false;
            UpgradePage.CleanShortcuts = false;
            UpgradePage.CloseAfterUpgrade = false;
            UpgradePage.Dock = DockStyle.Fill;
            UpgradePage.Loading = true;
            UpgradePage.Location = new System.Drawing.Point(3, 3);
            UpgradePage.Name = "UpgradePage";
            UpgradePage.ShowUacIcons = false;
            UpgradePage.Size = new System.Drawing.Size(716, 436);
            UpgradePage.TabIndex = 2;
            // 
            // TabInstalled
            // 
            TabInstalled.Controls.Add(InstalledPage);
            TabInstalled.Location = new System.Drawing.Point(4, 24);
            TabInstalled.Name = "TabInstalled";
            TabInstalled.Padding = new Padding(3);
            TabInstalled.Size = new System.Drawing.Size(722, 435);
            TabInstalled.TabIndex = 1;
            TabInstalled.Text = "Installed";
            TabInstalled.UseVisualStyleBackColor = true;
            // 
            // InstalledPage
            // 
            InstalledPage.Dock = DockStyle.Fill;
            InstalledPage.LoadingDetails = false;
            InstalledPage.Loading = false;
            InstalledPage.Location = new System.Drawing.Point(3, 3);
            InstalledPage.Name = "InstalledPage";
            InstalledPage.Size = new System.Drawing.Size(716, 429);
            InstalledPage.TabIndex = 0;
            // 
            // AdminBanner
            // 
            AdminBanner.Dock = DockStyle.Top;
            AdminBanner.Location = new System.Drawing.Point(0, 0);
            AdminBanner.Name = "AdminBanner";
            AdminBanner.Padding = new Padding(0, 0, 0, 10);
            AdminBanner.Size = new System.Drawing.Size(730, 64);
            AdminBanner.TabIndex = 4;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(730, 534);
            Controls.Add(MainPanel);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "MainWindow";
            Text = "ChocoAutoUpdateForm";
            MainPanel.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            TabUpgrade.ResumeLayout(false);
            TabInstalled.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Panel MainPanel;
        private Banner AdminBanner;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private UpgradePage UpgradePage;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabUpgrade;
        private System.Windows.Forms.TabPage TabInstalled;
        private InstalledPage InstalledPage;
    }
}