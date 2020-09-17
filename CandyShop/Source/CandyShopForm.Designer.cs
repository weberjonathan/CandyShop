namespace CandyShop
{
    partial class CandyShopForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CandyShopForm));
            this.MainPanel = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabUpgrade = new System.Windows.Forms.TabPage();
            this.UpgradePage = new CandyShop.UpgradePage();
            this.TabInstalled = new System.Windows.Forms.TabPage();
            this.InstalledPage = new CandyShop.InstalledPage();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.MainPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.TabUpgrade.SuspendLayout();
            this.TabInstalled.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.SystemColors.Window;
            this.MainPanel.Controls.Add(this.tabControl1);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(730, 534);
            this.MainPanel.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TabUpgrade);
            this.tabControl1.Controls.Add(this.TabInstalled);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(730, 534);
            this.tabControl1.TabIndex = 3;
            // 
            // TabUpgrade
            // 
            this.TabUpgrade.Controls.Add(this.UpgradePage);
            this.TabUpgrade.Location = new System.Drawing.Point(4, 24);
            this.TabUpgrade.Name = "TabUpgrade";
            this.TabUpgrade.Padding = new System.Windows.Forms.Padding(3);
            this.TabUpgrade.Size = new System.Drawing.Size(722, 506);
            this.TabUpgrade.TabIndex = 0;
            this.TabUpgrade.Text = "Upgrade";
            this.TabUpgrade.UseVisualStyleBackColor = true;
            // 
            // UpgradePage
            // 
            this.UpgradePage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UpgradePage.Location = new System.Drawing.Point(3, 3);
            this.UpgradePage.Name = "UpgradePage";
            this.UpgradePage.Size = new System.Drawing.Size(716, 500);
            this.UpgradePage.TabIndex = 2;
            // 
            // TabInstalled
            // 
            this.TabInstalled.Controls.Add(this.InstalledPage);
            this.TabInstalled.Location = new System.Drawing.Point(4, 24);
            this.TabInstalled.Name = "TabInstalled";
            this.TabInstalled.Padding = new System.Windows.Forms.Padding(3);
            this.TabInstalled.Size = new System.Drawing.Size(722, 506);
            this.TabInstalled.TabIndex = 1;
            this.TabInstalled.Text = "Installed";
            this.TabInstalled.UseVisualStyleBackColor = true;
            // 
            // InstalledPage
            // 
            this.InstalledPage.DetailsText = null;
            this.InstalledPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InstalledPage.Location = new System.Drawing.Point(3, 3);
            this.InstalledPage.Name = "InstalledPage";
            this.InstalledPage.Size = new System.Drawing.Size(716, 500);
            this.InstalledPage.TabIndex = 0;
            // 
            // CandyShopForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 534);
            this.Controls.Add(this.MainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "CandyShopForm";
            this.Text = "ChocoAutoUpdateForm";
            this.Load += new System.EventHandler(this.ChocoAutoUpdateForm_Load);
            this.MainPanel.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.TabUpgrade.ResumeLayout(false);
            this.TabInstalled.ResumeLayout(false);
            this.ResumeLayout(false);

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