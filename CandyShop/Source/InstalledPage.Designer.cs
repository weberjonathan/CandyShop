namespace CandyShop
{
    partial class InstalledPage
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
            this.LstPackages = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colVer = new System.Windows.Forms.ColumnHeader();
            this.PanelTop = new System.Windows.Forms.Panel();
            this.SplitContainer = new System.Windows.Forms.SplitContainer();
            this.TxtDetails = new System.Windows.Forms.TextBox();
            this.CheckHideMeta = new System.Windows.Forms.CheckBox();
            this.PanelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
            this.SplitContainer.Panel1.SuspendLayout();
            this.SplitContainer.Panel2.SuspendLayout();
            this.SplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // LstPackages
            // 
            this.LstPackages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colVer});
            this.LstPackages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LstPackages.FullRowSelect = true;
            this.LstPackages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LstPackages.HideSelection = false;
            this.LstPackages.Location = new System.Drawing.Point(0, 0);
            this.LstPackages.Name = "LstPackages";
            this.LstPackages.Size = new System.Drawing.Size(600, 250);
            this.LstPackages.TabIndex = 0;
            this.LstPackages.UseCompatibleStateImageBehavior = false;
            this.LstPackages.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.Text = "Name";
            // 
            // colVer
            // 
            this.colVer.Text = "Version";
            // 
            // PanelTop
            // 
            this.PanelTop.Controls.Add(this.CheckHideMeta);
            this.PanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelTop.Location = new System.Drawing.Point(0, 0);
            this.PanelTop.Name = "PanelTop";
            this.PanelTop.Size = new System.Drawing.Size(600, 44);
            this.PanelTop.TabIndex = 1;
            // 
            // SplitContainer
            // 
            this.SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.SplitContainer.Location = new System.Drawing.Point(0, 44);
            this.SplitContainer.Name = "SplitContainer";
            this.SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer.Panel1
            // 
            this.SplitContainer.Panel1.Controls.Add(this.LstPackages);
            // 
            // SplitContainer.Panel2
            // 
            this.SplitContainer.Panel2.Controls.Add(this.TxtDetails);
            this.SplitContainer.Size = new System.Drawing.Size(600, 408);
            this.SplitContainer.SplitterDistance = 250;
            this.SplitContainer.TabIndex = 2;
            this.SplitContainer.Text = "splitContainer1";
            // 
            // TxtDetails
            // 
            this.TxtDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtDetails.Location = new System.Drawing.Point(0, 0);
            this.TxtDetails.Multiline = true;
            this.TxtDetails.Name = "TxtDetails";
            this.TxtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtDetails.Size = new System.Drawing.Size(600, 154);
            this.TxtDetails.TabIndex = 0;
            this.TxtDetails.WordWrap = false;
            // 
            // CheckHideMeta
            // 
            this.CheckHideMeta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckHideMeta.AutoSize = true;
            this.CheckHideMeta.Checked = true;
            this.CheckHideMeta.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckHideMeta.Location = new System.Drawing.Point(455, 15);
            this.CheckHideMeta.Name = "CheckHideMeta";
            this.CheckHideMeta.Size = new System.Drawing.Size(133, 19);
            this.CheckHideMeta.TabIndex = 0;
            this.CheckHideMeta.Text = "Hide &meta packages";
            this.CheckHideMeta.UseVisualStyleBackColor = true;
            this.CheckHideMeta.CheckedChanged += new System.EventHandler(this.CheckHideMeta_CheckedChanged);
            // 
            // InstalledPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SplitContainer);
            this.Controls.Add(this.PanelTop);
            this.Name = "InstalledPage";
            this.Size = new System.Drawing.Size(600, 452);
            this.PanelTop.ResumeLayout(false);
            this.PanelTop.PerformLayout();
            this.SplitContainer.Panel1.ResumeLayout(false);
            this.SplitContainer.Panel2.ResumeLayout(false);
            this.SplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
            this.SplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView LstPackages;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colVer;
        private System.Windows.Forms.Panel PanelTop;
        private System.Windows.Forms.SplitContainer SplitContainer;
        private System.Windows.Forms.TextBox TxtDetails;
        private System.Windows.Forms.CheckBox CheckHideMeta;
    }
}
