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
            this.BottomPanel = new System.Windows.Forms.Panel();
            this.BtnUpgradeChecked = new System.Windows.Forms.Button();
            this.LinkGithub = new System.Windows.Forms.LinkLabel();
            this.BtnUpgradeAll = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabUpgrade = new System.Windows.Forms.TabPage();
            this.UpgradePage = new CandyShop.UpgradePage();
            this.TabInstalled = new System.Windows.Forms.TabPage();
            this.InstalledPage = new CandyShop.InstalledPage();
            this.TopPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.BottomPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.TabUpgrade.SuspendLayout();
            this.TabInstalled.SuspendLayout();
            this.TopPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BottomPanel
            // 
            this.BottomPanel.Controls.Add(this.BtnUpgradeChecked);
            this.BottomPanel.Controls.Add(this.LinkGithub);
            this.BottomPanel.Controls.Add(this.BtnUpgradeAll);
            this.BottomPanel.Controls.Add(this.BtnCancel);
            this.BottomPanel.Location = new System.Drawing.Point(0, 356);
            this.BottomPanel.Name = "BottomPanel";
            this.BottomPanel.Size = new System.Drawing.Size(604, 55);
            this.BottomPanel.TabIndex = 0;
            // 
            // BtnUpgradeChecked
            // 
            this.BtnUpgradeChecked.Enabled = false;
            this.BtnUpgradeChecked.Location = new System.Drawing.Point(420, 15);
            this.BtnUpgradeChecked.Name = "BtnUpgradeChecked";
            this.BtnUpgradeChecked.Size = new System.Drawing.Size(91, 23);
            this.BtnUpgradeChecked.TabIndex = 3;
            this.BtnUpgradeChecked.Text = "&Upgrade";
            this.BtnUpgradeChecked.UseVisualStyleBackColor = true;
            this.BtnUpgradeChecked.Click += new System.EventHandler(this.UpgradePage_UpgradeSelectedClick);
            // 
            // LinkGithub
            // 
            this.LinkGithub.AutoSize = true;
            this.LinkGithub.Location = new System.Drawing.Point(12, 19);
            this.LinkGithub.Name = "LinkGithub";
            this.LinkGithub.Size = new System.Drawing.Size(45, 15);
            this.LinkGithub.TabIndex = 1;
            this.LinkGithub.TabStop = true;
            this.LinkGithub.Text = "GitHub";
            // 
            // BtnUpgradeAll
            // 
            this.BtnUpgradeAll.Enabled = false;
            this.BtnUpgradeAll.Location = new System.Drawing.Point(310, 15);
            this.BtnUpgradeAll.Name = "BtnUpgradeAll";
            this.BtnUpgradeAll.Size = new System.Drawing.Size(104, 23);
            this.BtnUpgradeAll.TabIndex = 2;
            this.BtnUpgradeAll.Text = "Upgrade &All";
            this.BtnUpgradeAll.UseVisualStyleBackColor = true;
            this.BtnUpgradeAll.Click += new System.EventHandler(this.UpgradePage_UpgradeAllClick);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Enabled = false;
            this.BtnCancel.Location = new System.Drawing.Point(517, 15);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 4;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
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
            this.InstalledPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InstalledPage.Location = new System.Drawing.Point(3, 3);
            this.InstalledPage.Name = "InstalledPage";
            this.InstalledPage.Size = new System.Drawing.Size(716, 500);
            this.InstalledPage.TabIndex = 0;
            // 
            // TopPanel
            // 
            this.TopPanel.BackColor = System.Drawing.SystemColors.Window;
            this.TopPanel.Controls.Add(this.label1);
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(604, 47);
            this.TopPanel.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(446, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "!!! ChocoAuto Update does not have administrator privileges. Proceed with caution" +
    "!";
            // 
            // ChocoAutoUpdateForm
            // 
            this.AcceptButton = this.BtnUpgradeChecked;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(730, 534);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.BottomPanel);
            this.Controls.Add(this.TopPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ChocoAutoUpdateForm";
            this.Text = "ChocoAutoUpdateForm";
            this.Load += new System.EventHandler(this.ChocoAutoUpdateForm_Load);
            this.BottomPanel.ResumeLayout(false);
            this.BottomPanel.PerformLayout();
            this.MainPanel.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.TabUpgrade.ResumeLayout(false);
            this.TabInstalled.ResumeLayout(false);
            this.TopPanel.ResumeLayout(false);
            this.TopPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel BottomPanel;
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.Button BtnUpgradeAll;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.LinkLabel LinkGithub;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnUpgradeChecked;
        private UpgradePage UpgradePage;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabUpgrade;
        private System.Windows.Forms.TabPage TabInstalled;
        private InstalledPage InstalledPage;
    }
}