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
            this.LblAdmin = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnUpgradeSelected = new System.Windows.Forms.Button();
            this.LinkGithub = new System.Windows.Forms.LinkLabel();
            this.BtnUpgradeAll = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.LblLoading = new System.Windows.Forms.Label();
            this.LstPackages = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colCurrent = new System.Windows.Forms.ColumnHeader();
            this.colAvail = new System.Windows.Forms.ColumnHeader();
            this.colPinned = new System.Windows.Forms.ColumnHeader();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // LblAdmin
            // 
            this.LblAdmin.AutoSize = true;
            this.LblAdmin.Location = new System.Drawing.Point(12, 18);
            this.LblAdmin.Name = "LblAdmin";
            this.LblAdmin.Size = new System.Drawing.Size(446, 15);
            this.LblAdmin.TabIndex = 0;
            this.LblAdmin.Text = "!!! ChocoAuto Update does not have administrator privileges. Proceed with caution" +
    "!";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.LblAdmin);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(637, 47);
            this.panel1.TabIndex = 2;
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
            // LinkGithub
            // 
            this.LinkGithub.AutoSize = true;
            this.LinkGithub.Location = new System.Drawing.Point(12, 19);
            this.LinkGithub.Name = "LinkGithub";
            this.LinkGithub.Size = new System.Drawing.Size(45, 15);
            this.LinkGithub.TabIndex = 1;
            this.LinkGithub.TabStop = true;
            this.LinkGithub.Text = "GitHub";
            this.LinkGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkGithub_LinkClicked);
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
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnCancel.Enabled = false;
            this.BtnCancel.Location = new System.Drawing.Point(546, 15);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 4;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.BtnUpgradeSelected);
            this.panel2.Controls.Add(this.LinkGithub);
            this.panel2.Controls.Add(this.BtnUpgradeAll);
            this.panel2.Controls.Add(this.BtnCancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 478);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(637, 55);
            this.panel2.TabIndex = 0;
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
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LblLoading);
            this.Controls.Add(this.LstPackages);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(637, 533);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblAdmin;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BtnUpgradeSelected;
        private System.Windows.Forms.LinkLabel LinkGithub;
        private System.Windows.Forms.Button BtnUpgradeAll;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label LblLoading;
        private System.Windows.Forms.ListView LstPackages;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colCurrent;
        private System.Windows.Forms.ColumnHeader colAvail;
        private System.Windows.Forms.ColumnHeader colPinned;
    }
}
