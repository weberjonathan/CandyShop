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
            this.LstPackages.Size = new System.Drawing.Size(405, 339);
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
            // InstalledPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LstPackages);
            this.Name = "InstalledPage";
            this.Size = new System.Drawing.Size(405, 339);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView LstPackages;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colVer;
    }
}
