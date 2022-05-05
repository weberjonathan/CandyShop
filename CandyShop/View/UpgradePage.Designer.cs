using CandyShop.Properties;

namespace CandyShop.View
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.LblSelected = new System.Windows.Forms.Label();
            this.CheckDeleteShortcuts = new System.Windows.Forms.CheckBox();
            this.LstPackages = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colCurrent = new System.Windows.Forms.ColumnHeader();
            this.colAvail = new System.Windows.Forms.ColumnHeader();
            this.colPinned = new System.Windows.Forms.ColumnHeader();
            this.LblLoading = new System.Windows.Forms.Label();
            this.PanelTop.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.PanelTop.Visible = false;
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
            this.BtnUpgradeSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnUpgradeSelected.Enabled = false;
            this.BtnUpgradeSelected.Location = new System.Drawing.Point(446, 39);
            this.BtnUpgradeSelected.Name = "BtnUpgradeSelected";
            this.BtnUpgradeSelected.Size = new System.Drawing.Size(91, 23);
            this.BtnUpgradeSelected.TabIndex = 3;
            this.BtnUpgradeSelected.Text = "&Upgrade";
            this.BtnUpgradeSelected.UseVisualStyleBackColor = true;
            // 
            // BtnUpgradeAll
            // 
            this.BtnUpgradeAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnUpgradeAll.Enabled = false;
            this.BtnUpgradeAll.Location = new System.Drawing.Point(336, 39);
            this.BtnUpgradeAll.Name = "BtnUpgradeAll";
            this.BtnUpgradeAll.Size = new System.Drawing.Size(104, 23);
            this.BtnUpgradeAll.TabIndex = 2;
            this.BtnUpgradeAll.Text = "Upgrade &All";
            this.BtnUpgradeAll.UseVisualStyleBackColor = true;
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.Location = new System.Drawing.Point(543, 39);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 4;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.LblSelected);
            this.panel2.Controls.Add(this.CheckDeleteShortcuts);
            this.panel2.Controls.Add(this.BtnUpgradeSelected);
            this.panel2.Controls.Add(this.BtnUpgradeAll);
            this.panel2.Controls.Add(this.BtnCancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 454);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(637, 79);
            this.panel2.TabIndex = 0;
            // 
            // LblSelected
            // 
            this.LblSelected.AutoSize = true;
            this.LblSelected.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.LblSelected.Location = new System.Drawing.Point(3, 11);
            this.LblSelected.Name = "LblSelected";
            this.LblSelected.Size = new System.Drawing.Size(132, 15);
            this.LblSelected.TabIndex = 6;
            this.LblSelected.Text = "Selected package count";
            // 
            // CheckDeleteShortcuts
            // 
            this.CheckDeleteShortcuts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckDeleteShortcuts.AutoSize = true;
            this.CheckDeleteShortcuts.Location = new System.Drawing.Point(507, 10);
            this.CheckDeleteShortcuts.Name = "CheckDeleteShortcuts";
            this.CheckDeleteShortcuts.Size = new System.Drawing.Size(111, 19);
            this.CheckDeleteShortcuts.TabIndex = 5;
            this.CheckDeleteShortcuts.Text = "Delete &shortcuts";
            this.CheckDeleteShortcuts.UseVisualStyleBackColor = true;
            this.CheckDeleteShortcuts.CheckedChanged += new System.EventHandler(this.CheckDeleteShortcuts_CheckedChanged);
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
            this.LstPackages.Size = new System.Drawing.Size(637, 407);
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
            // LblLoading
            // 
            this.LblLoading.BackColor = System.Drawing.Color.Transparent;
            this.LblLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblLoading.Location = new System.Drawing.Point(0, 47);
            this.LblLoading.Name = "LblLoading";
            this.LblLoading.Size = new System.Drawing.Size(637, 407);
            this.LblLoading.TabIndex = 1;
            this.LblLoading.Text = "Loading ...";
            this.LblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UpgradePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LblLoading);
            this.Controls.Add(this.LstPackages);
            this.Controls.Add(this.PanelTop);
            this.Controls.Add(this.panel2);
            this.Name = "UpgradePage";
            this.Size = new System.Drawing.Size(637, 533);
            this.PanelTop.ResumeLayout(false);
            this.PanelTop.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LblAdmin;
        private System.Windows.Forms.Panel PanelTop;
        private System.Windows.Forms.Button BtnUpgradeSelected;
        private System.Windows.Forms.Button BtnUpgradeAll;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView LstPackages;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colCurrent;
        private System.Windows.Forms.ColumnHeader colAvail;
        private System.Windows.Forms.ColumnHeader colPinned;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label LblLoading;
        private System.Windows.Forms.CheckBox CheckDeleteShortcuts;
        private System.Windows.Forms.Label LblSelected;
    }
}
