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
            LstPackages = new Controls.PackageListBox();
            SplitContainer = new System.Windows.Forms.SplitContainer();
            packageInfoBox1 = new Controls.PackageInfoBox();
            ((System.ComponentModel.ISupportInitialize)SplitContainer).BeginInit();
            SplitContainer.Panel1.SuspendLayout();
            SplitContainer.Panel2.SuspendLayout();
            SplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // LstPackages
            // 
            LstPackages.ColumnHeaders = null;
            LstPackages.Dock = System.Windows.Forms.DockStyle.Fill;
            LstPackages.Hint = "";
            LstPackages.Loading = false;
            LstPackages.Location = new System.Drawing.Point(0, 0);
            LstPackages.Name = "LstPackages";
            LstPackages.NoPackages = false;
            LstPackages.Size = new System.Drawing.Size(600, 290);
            LstPackages.TabIndex = 0;
            // 
            // SplitContainer
            // 
            SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            SplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            SplitContainer.Location = new System.Drawing.Point(0, 0);
            SplitContainer.Name = "SplitContainer";
            SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer.Panel1
            // 
            SplitContainer.Panel1.Controls.Add(LstPackages);
            // 
            // SplitContainer.Panel2
            // 
            SplitContainer.Panel2.Controls.Add(packageInfoBox1);
            SplitContainer.Size = new System.Drawing.Size(600, 452);
            SplitContainer.SplitterDistance = 290;
            SplitContainer.TabIndex = 2;
            SplitContainer.Text = "splitContainer1";
            // 
            // packageInfoBox1
            // 
            packageInfoBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            packageInfoBox1.Loading = false;
            packageInfoBox1.Location = new System.Drawing.Point(0, 0);
            packageInfoBox1.Name = "packageInfoBox1";
            packageInfoBox1.Size = new System.Drawing.Size(600, 158);
            packageInfoBox1.TabIndex = 0;
            // 
            // InstalledPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(SplitContainer);
            Name = "InstalledPage";
            Size = new System.Drawing.Size(600, 452);
            SplitContainer.Panel1.ResumeLayout(false);
            SplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SplitContainer).EndInit();
            SplitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Controls.PackageListBox LstPackages;
        private System.Windows.Forms.SplitContainer SplitContainer;
        private Controls.PackageInfoBox packageInfoBox1;
    }
}
