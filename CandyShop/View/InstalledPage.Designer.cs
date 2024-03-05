namespace CandyShop.View
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
            LstPackages = new System.Windows.Forms.ListView();
            colName = new System.Windows.Forms.ColumnHeader();
            colVer = new System.Windows.Forms.ColumnHeader();
            PanelTop = new System.Windows.Forms.Panel();
            TextSearch = new System.Windows.Forms.TextBox();
            CheckHideSuffixed = new System.Windows.Forms.CheckBox();
            SplitContainer = new System.Windows.Forms.SplitContainer();
            TxtDetails = new System.Windows.Forms.TextBox();
            SpinnerDetails = new Spinner();
            PanelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SplitContainer).BeginInit();
            SplitContainer.Panel1.SuspendLayout();
            SplitContainer.Panel2.SuspendLayout();
            SplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // LstPackages
            // 
            LstPackages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colName, colVer });
            LstPackages.Dock = System.Windows.Forms.DockStyle.Fill;
            LstPackages.FullRowSelect = true;
            LstPackages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LstPackages.Location = new System.Drawing.Point(0, 0);
            LstPackages.Name = "LstPackages";
            LstPackages.Size = new System.Drawing.Size(600, 250);
            LstPackages.TabIndex = 0;
            LstPackages.UseCompatibleStateImageBehavior = false;
            LstPackages.View = System.Windows.Forms.View.Details;
            LstPackages.SelectedIndexChanged += LstPackages_SelectedIndexChanged;
            // 
            // colName
            // 
            colName.Text = "Name";
            // 
            // colVer
            // 
            colVer.Text = "Version";
            // 
            // PanelTop
            // 
            PanelTop.Controls.Add(TextSearch);
            PanelTop.Controls.Add(CheckHideSuffixed);
            PanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            PanelTop.Location = new System.Drawing.Point(0, 0);
            PanelTop.Name = "PanelTop";
            PanelTop.Size = new System.Drawing.Size(600, 44);
            PanelTop.TabIndex = 1;
            // 
            // TextSearch
            // 
            TextSearch.Location = new System.Drawing.Point(0, 13);
            TextSearch.Name = "TextSearch";
            TextSearch.PlaceholderText = "Search";
            TextSearch.Size = new System.Drawing.Size(430, 23);
            TextSearch.TabIndex = 1;
            TextSearch.TextChanged += TextSearch_TextChanged;
            TextSearch.KeyDown += TextSearch_KeyDown;
            // 
            // CheckHideSuffixed
            // 
            CheckHideSuffixed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            CheckHideSuffixed.AutoSize = true;
            CheckHideSuffixed.Checked = true;
            CheckHideSuffixed.CheckState = System.Windows.Forms.CheckState.Checked;
            CheckHideSuffixed.Location = new System.Drawing.Point(452, 15);
            CheckHideSuffixed.Name = "CheckHideSuffixed";
            CheckHideSuffixed.Size = new System.Drawing.Size(136, 19);
            CheckHideSuffixed.TabIndex = 0;
            CheckHideSuffixed.Text = "&Show top plevel only";
            CheckHideSuffixed.UseVisualStyleBackColor = true;
            CheckHideSuffixed.CheckedChanged += CheckHideSuffixed_CheckedChanged;
            // 
            // SplitContainer
            // 
            SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            SplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            SplitContainer.Location = new System.Drawing.Point(0, 44);
            SplitContainer.Name = "SplitContainer";
            SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer.Panel1
            // 
            SplitContainer.Panel1.Controls.Add(LstPackages);
            // 
            // SplitContainer.Panel2
            // 
            SplitContainer.Panel2.Controls.Add(SpinnerDetails);
            SplitContainer.Panel2.Controls.Add(TxtDetails);
            SplitContainer.Size = new System.Drawing.Size(600, 408);
            SplitContainer.SplitterDistance = 250;
            SplitContainer.TabIndex = 2;
            SplitContainer.Text = "splitContainer1";
            // 
            // TxtDetails
            // 
            TxtDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            TxtDetails.Location = new System.Drawing.Point(0, 0);
            TxtDetails.Multiline = true;
            TxtDetails.Name = "TxtDetails";
            TxtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            TxtDetails.Size = new System.Drawing.Size(600, 154);
            TxtDetails.TabIndex = 0;
            TxtDetails.WordWrap = false;
            // 
            // SpinnerDetails
            // 
            SpinnerDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            SpinnerDetails.Location = new System.Drawing.Point(0, 0);
            SpinnerDetails.Margin = new System.Windows.Forms.Padding(20);
            SpinnerDetails.Name = "SpinnerDetails";
            SpinnerDetails.Size = new System.Drawing.Size(600, 154);
            SpinnerDetails.TabIndex = 1;
            // 
            // InstalledPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(SplitContainer);
            Controls.Add(PanelTop);
            Name = "InstalledPage";
            Size = new System.Drawing.Size(600, 452);
            PanelTop.ResumeLayout(false);
            PanelTop.PerformLayout();
            SplitContainer.Panel1.ResumeLayout(false);
            SplitContainer.Panel2.ResumeLayout(false);
            SplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SplitContainer).EndInit();
            SplitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ListView LstPackages;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colVer;
        private System.Windows.Forms.Panel PanelTop;
        private System.Windows.Forms.SplitContainer SplitContainer;
        private System.Windows.Forms.TextBox TxtDetails;
        private System.Windows.Forms.CheckBox CheckHideSuffixed;
        private System.Windows.Forms.TextBox TextSearch;
        private Spinner SpinnerDetails;
    }
}
