namespace CandyShop.Controls
{
    partial class SearchBar
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
            SplitContainer = new System.Windows.Forms.SplitContainer();
            TextSearch = new System.Windows.Forms.TextBox();
            checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)SplitContainer).BeginInit();
            SplitContainer.Panel1.SuspendLayout();
            SplitContainer.Panel2.SuspendLayout();
            SplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // SplitContainer
            // 
            SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            SplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            SplitContainer.IsSplitterFixed = true;
            SplitContainer.Location = new System.Drawing.Point(0, 0);
            SplitContainer.Name = "SplitContainer";
            // 
            // SplitContainer.Panel1
            // 
            SplitContainer.Panel1.Controls.Add(TextSearch);
            SplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 5, 5, 5);
            // 
            // SplitContainer.Panel2
            // 
            SplitContainer.Panel2.Controls.Add(checkBox1);
            SplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(5);
            SplitContainer.Size = new System.Drawing.Size(589, 34);
            SplitContainer.SplitterDistance = 433;
            SplitContainer.TabIndex = 0;
            // 
            // TextSearch
            // 
            TextSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            TextSearch.Location = new System.Drawing.Point(0, 5);
            TextSearch.Name = "TextSearch";
            TextSearch.PlaceholderText = "Search";
            TextSearch.Size = new System.Drawing.Size(428, 23);
            TextSearch.TabIndex = 4;
            // 
            // checkBox1
            // 
            checkBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox1.Location = new System.Drawing.Point(8, 7);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(136, 19);
            checkBox1.TabIndex = 4;
            checkBox1.Text = "&Show top plevel only";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // SearchBar
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(SplitContainer);
            Name = "SearchBar";
            Size = new System.Drawing.Size(589, 34);
            SplitContainer.Panel1.ResumeLayout(false);
            SplitContainer.Panel1.PerformLayout();
            SplitContainer.Panel2.ResumeLayout(false);
            SplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SplitContainer).EndInit();
            SplitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer SplitContainer;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox TextSearch;
    }
}
