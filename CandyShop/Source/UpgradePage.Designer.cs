using CandyShop.Properties;

namespace CandyShop
{
    partial class UpgradePage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpgradePage));
            this.LblAdmin = new System.Windows.Forms.Label();
            this.PanelTop = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnUpgradeSelected = new System.Windows.Forms.Button();
            this.BtnUpgradeAll = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.PanelBottom = new System.Windows.Forms.Panel();
            this.LblLoading = new System.Windows.Forms.Label();
            this.LstPackages = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colCurrent = new System.Windows.Forms.ColumnHeader();
            this.colAvail = new System.Windows.Forms.ColumnHeader();
            this.colPinned = new System.Windows.Forms.ColumnHeader();
            this.PanelProgress = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.ProgressBarPackage = new System.Windows.Forms.ProgressBar();
            this.lblTotalProgress = new System.Windows.Forms.Label();
            this.ProgressBarTotal = new System.Windows.Forms.ProgressBar();
            this.PanelTop.SuspendLayout();
            this.PanelBottom.SuspendLayout();
            this.PanelProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // LblAdmin
            // 
            this.LblAdmin.AutoSize = true;
            this.LblAdmin.Location = new System.Drawing.Point(38, 17);
            this.LblAdmin.Name = "LblAdmin";
            this.LblAdmin.Size = new System.Drawing.Size(396, 15);
            this.LblAdmin.TabIndex = 0;
            this.LblAdmin.Text = "Candy Shop does not have administrator privileges. Proceed with caution!";
            // 
            // PanelTop
            // 
            this.PanelTop.BackColor = System.Drawing.SystemColors.Info;
            this.PanelTop.Controls.Add(this.panel1);
            this.PanelTop.Controls.Add(this.LblAdmin);
            this.PanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelTop.Location = new System.Drawing.Point(0, 0);
            this.PanelTop.Name = "PanelTop";
            this.PanelTop.Size = new System.Drawing.Size(637, 47);
            this.PanelTop.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Info;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(16, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(16, 16);
            this.panel1.TabIndex = 1;
            // 
            // BtnUpgradeSelected
            // 
            this.BtnUpgradeSelected.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnUpgradeSelected.Enabled = false;
            this.BtnUpgradeSelected.Location = new System.Drawing.Point(449, 15);
            this.BtnUpgradeSelected.Name = "BtnUpgradeSelected";
            this.BtnUpgradeSelected.Size = new System.Drawing.Size(91, 23);
            this.BtnUpgradeSelected.TabIndex = 3;
            this.BtnUpgradeSelected.Text = "&Upgrade";
            this.BtnUpgradeSelected.UseVisualStyleBackColor = true;
            // 
            // BtnUpgradeAll
            // 
            this.BtnUpgradeAll.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnUpgradeAll.Enabled = false;
            this.BtnUpgradeAll.Location = new System.Drawing.Point(339, 15);
            this.BtnUpgradeAll.Name = "BtnUpgradeAll";
            this.BtnUpgradeAll.Size = new System.Drawing.Size(104, 23);
            this.BtnUpgradeAll.TabIndex = 2;
            this.BtnUpgradeAll.Text = "Upgrade &All";
            this.BtnUpgradeAll.UseVisualStyleBackColor = true;
            this.BtnUpgradeAll.Visible = false;
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnCancel.Location = new System.Drawing.Point(546, 15);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 4;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // PanelBottom
            // 
            this.PanelBottom.Controls.Add(this.BtnUpgradeSelected);
            this.PanelBottom.Controls.Add(this.BtnUpgradeAll);
            this.PanelBottom.Controls.Add(this.BtnCancel);
            this.PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PanelBottom.Location = new System.Drawing.Point(0, 478);
            this.PanelBottom.Name = "PanelBottom";
            this.PanelBottom.Size = new System.Drawing.Size(637, 55);
            this.PanelBottom.TabIndex = 0;
            // 
            // LblLoading
            // 
            this.LblLoading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblLoading.AutoSize = true;
            this.LblLoading.BackColor = System.Drawing.Color.Transparent;
            this.LblLoading.Location = new System.Drawing.Point(265, 263);
            this.LblLoading.Name = "LblLoading";
            this.LblLoading.Size = new System.Drawing.Size(62, 15);
            this.LblLoading.TabIndex = 1;
            this.LblLoading.Text = "Loading ...";
            // 
            // LstPackages
            // 
            this.LstPackages.CheckBoxes = true;
            this.LstPackages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colCurrent,
            this.colAvail,
            this.colPinned});
            this.LstPackages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LstPackages.FullRowSelect = true;
            this.LstPackages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LstPackages.HideSelection = false;
            this.LstPackages.Location = new System.Drawing.Point(0, 47);
            this.LstPackages.MultiSelect = false;
            this.LstPackages.Name = "LstPackages";
            this.LstPackages.ShowItemToolTips = true;
            this.LstPackages.Size = new System.Drawing.Size(637, 431);
            this.LstPackages.TabIndex = 3;
            this.LstPackages.UseCompatibleStateImageBehavior = false;
            this.LstPackages.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.Text = "Name";
            // 
            // colCurrent
            // 
            this.colCurrent.Text = "Current";
            // 
            // colAvail
            // 
            this.colAvail.Text = "Available";
            // 
            // colPinned
            // 
            this.colPinned.Text = "Pinned";
            // 
            // PanelProgress
            // 
            this.PanelProgress.Controls.Add(this.label1);
            this.PanelProgress.Controls.Add(this.ProgressBarPackage);
            this.PanelProgress.Controls.Add(this.lblTotalProgress);
            this.PanelProgress.Controls.Add(this.ProgressBarTotal);
            this.PanelProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PanelProgress.Location = new System.Drawing.Point(0, 332);
            this.PanelProgress.Name = "PanelProgress";
            this.PanelProgress.Size = new System.Drawing.Size(637, 146);
            this.PanelProgress.TabIndex = 5;
            this.PanelProgress.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Package progress";
            // 
            // ProgressBarPackage
            // 
            this.ProgressBarPackage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBarPackage.Location = new System.Drawing.Point(16, 39);
            this.ProgressBarPackage.Name = "ProgressBarPackage";
            this.ProgressBarPackage.Size = new System.Drawing.Size(605, 23);
            this.ProgressBarPackage.Step = 1;
            this.ProgressBarPackage.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressBarPackage.TabIndex = 2;
            // 
            // lblTotalProgress
            // 
            this.lblTotalProgress.AutoSize = true;
            this.lblTotalProgress.Location = new System.Drawing.Point(16, 83);
            this.lblTotalProgress.Name = "lblTotalProgress";
            this.lblTotalProgress.Size = new System.Drawing.Size(80, 15);
            this.lblTotalProgress.TabIndex = 1;
            this.lblTotalProgress.Text = "Total progress";
            // 
            // ProgressBarTotal
            // 
            this.ProgressBarTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBarTotal.Location = new System.Drawing.Point(16, 103);
            this.ProgressBarTotal.Name = "ProgressBarTotal";
            this.ProgressBarTotal.Size = new System.Drawing.Size(605, 23);
            this.ProgressBarTotal.Step = 1;
            this.ProgressBarTotal.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.ProgressBarTotal.TabIndex = 0;
            // 
            // UpgradePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PanelProgress);
            this.Controls.Add(this.LblLoading);
            this.Controls.Add(this.LstPackages);
            this.Controls.Add(this.PanelBottom);
            this.Controls.Add(this.PanelTop);
            this.Name = "UpgradePage";
            this.Size = new System.Drawing.Size(637, 533);
            this.PanelTop.ResumeLayout(false);
            this.PanelTop.PerformLayout();
            this.PanelBottom.ResumeLayout(false);
            this.PanelProgress.ResumeLayout(false);
            this.PanelProgress.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblAdmin;
        private System.Windows.Forms.Panel PanelTop;
        private System.Windows.Forms.Button BtnUpgradeSelected;
        private System.Windows.Forms.Button BtnUpgradeAll;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Panel PanelBottom;
        private System.Windows.Forms.Label LblLoading;
        private System.Windows.Forms.ListView LstPackages;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colCurrent;
        private System.Windows.Forms.ColumnHeader colAvail;
        private System.Windows.Forms.ColumnHeader colPinned;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel PanelProgress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar ProgressBarPackage;
        private System.Windows.Forms.Label lblTotalProgress;
        private System.Windows.Forms.ProgressBar ProgressBarTotal;
    }
}
