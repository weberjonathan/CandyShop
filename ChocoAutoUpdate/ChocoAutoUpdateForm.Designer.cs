namespace ChocoAutoUpdate
{
    partial class ChocoAutoUpdateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChocoAutoUpdateForm));
            this.BottomPanel = new System.Windows.Forms.Panel();
            this.BtnUpgradeChecked = new System.Windows.Forms.Button();
            this.LinkGithub = new System.Windows.Forms.LinkLabel();
            this.BtnUpgradeAll = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.TxtLoading = new System.Windows.Forms.Label();
            this.LstPackages = new System.Windows.Forms.ListView();
            this.ColName = new System.Windows.Forms.ColumnHeader();
            this.ColCurr = new System.Windows.Forms.ColumnHeader();
            this.ColAvail = new System.Windows.Forms.ColumnHeader();
            this.ColPinned = new System.Windows.Forms.ColumnHeader();
            this.TopPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.BottomPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TopPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BottomPanel
            // 
            this.BottomPanel.Controls.Add(this.BtnUpgradeChecked);
            this.BottomPanel.Controls.Add(this.LinkGithub);
            this.BottomPanel.Controls.Add(this.BtnUpgradeAll);
            this.BottomPanel.Controls.Add(this.BtnCancel);
            this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
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
            this.BtnUpgradeChecked.Click += new System.EventHandler(this.BtnUpgradeChecked_Click);
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
            this.BtnUpgradeAll.Enabled = false;
            this.BtnUpgradeAll.Location = new System.Drawing.Point(310, 15);
            this.BtnUpgradeAll.Name = "BtnUpgradeAll";
            this.BtnUpgradeAll.Size = new System.Drawing.Size(104, 23);
            this.BtnUpgradeAll.TabIndex = 2;
            this.BtnUpgradeAll.Text = "Upgrade &All";
            this.BtnUpgradeAll.UseVisualStyleBackColor = true;
            this.BtnUpgradeAll.Click += new System.EventHandler(this.BtnUpgradeAll_Click);
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
            this.MainPanel.Controls.Add(this.TxtLoading);
            this.MainPanel.Controls.Add(this.LstPackages);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 47);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(604, 309);
            this.MainPanel.TabIndex = 1;
            // 
            // TxtLoading
            // 
            this.TxtLoading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TxtLoading.Location = new System.Drawing.Point(250, 117);
            this.TxtLoading.Name = "TxtLoading";
            this.TxtLoading.Size = new System.Drawing.Size(100, 15);
            this.TxtLoading.TabIndex = 1;
            this.TxtLoading.Text = "Loading...";
            this.TxtLoading.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LstPackages
            // 
            this.LstPackages.CheckBoxes = true;
            this.LstPackages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColName,
            this.ColCurr,
            this.ColAvail,
            this.ColPinned});
            this.LstPackages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LstPackages.FullRowSelect = true;
            this.LstPackages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LstPackages.HideSelection = false;
            this.LstPackages.Location = new System.Drawing.Point(0, 0);
            this.LstPackages.MultiSelect = false;
            this.LstPackages.Name = "LstPackages";
            this.LstPackages.ShowItemToolTips = true;
            this.LstPackages.Size = new System.Drawing.Size(604, 309);
            this.LstPackages.TabIndex = 0;
            this.LstPackages.UseCompatibleStateImageBehavior = false;
            this.LstPackages.View = System.Windows.Forms.View.Details;
            this.LstPackages.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.LstPackages_ItemChecked);
            // 
            // ColName
            // 
            this.ColName.Name = "ColName";
            this.ColName.Text = "Name";
            this.ColName.Width = 200;
            // 
            // ColCurr
            // 
            this.ColCurr.Name = "ColCurr";
            this.ColCurr.Text = "Current";
            this.ColCurr.Width = 170;
            // 
            // ColAvail
            // 
            this.ColAvail.Name = "ColAvail";
            this.ColAvail.Text = "Available";
            this.ColAvail.Width = 170;
            // 
            // ColPinned
            // 
            this.ColPinned.Name = "ColPinned";
            this.ColPinned.Text = "Pinned";
            // 
            // TopPanel
            // 
            this.TopPanel.BackColor = System.Drawing.SystemColors.Window;
            this.TopPanel.Controls.Add(this.label1);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
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
            this.ClientSize = new System.Drawing.Size(604, 411);
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
        private System.Windows.Forms.ListView LstPackages;
        private System.Windows.Forms.ColumnHeader ColName;
        private System.Windows.Forms.ColumnHeader ColCurr;
        private System.Windows.Forms.ColumnHeader ColAvail;
        private System.Windows.Forms.ColumnHeader ColPinned;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label TxtLoading;
        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnUpgradeChecked;
    }
}