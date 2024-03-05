namespace CandyShop.View
{
    partial class Spinner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Spinner));
            SpinnerImage = new System.Windows.Forms.PictureBox();
            TablePanel = new System.Windows.Forms.TableLayoutPanel();
            LabelDescription = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)SpinnerImage).BeginInit();
            TablePanel.SuspendLayout();
            SuspendLayout();
            // 
            // SpinnerImage
            // 
            SpinnerImage.BackColor = System.Drawing.Color.White;
            SpinnerImage.Dock = System.Windows.Forms.DockStyle.Bottom;
            SpinnerImage.Image = (System.Drawing.Image)resources.GetObject("SpinnerImage.Image");
            SpinnerImage.InitialImage = (System.Drawing.Image)resources.GetObject("SpinnerImage.InitialImage");
            SpinnerImage.Location = new System.Drawing.Point(3, 111);
            SpinnerImage.Name = "SpinnerImage";
            SpinnerImage.Size = new System.Drawing.Size(530, 20);
            SpinnerImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            SpinnerImage.TabIndex = 1;
            SpinnerImage.TabStop = false;
            // 
            // TablePanel
            // 
            TablePanel.BackColor = System.Drawing.Color.White;
            TablePanel.ColumnCount = 1;
            TablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TablePanel.Controls.Add(SpinnerImage, 0, 0);
            TablePanel.Controls.Add(LabelDescription, 0, 2);
            TablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            TablePanel.Location = new System.Drawing.Point(0, 0);
            TablePanel.Name = "TablePanel";
            TablePanel.RowCount = 3;
            TablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            TablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            TablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TablePanel.Size = new System.Drawing.Size(536, 335);
            TablePanel.TabIndex = 2;
            // 
            // LabelDescription
            // 
            LabelDescription.AutoSize = true;
            LabelDescription.Dock = System.Windows.Forms.DockStyle.Top;
            LabelDescription.ForeColor = System.Drawing.SystemColors.GrayText;
            LabelDescription.Location = new System.Drawing.Point(3, 167);
            LabelDescription.Name = "LabelDescription";
            LabelDescription.Size = new System.Drawing.Size(530, 15);
            LabelDescription.TabIndex = 2;
            LabelDescription.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Spinner
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(TablePanel);
            Name = "Spinner";
            Size = new System.Drawing.Size(536, 335);
            ((System.ComponentModel.ISupportInitialize)SpinnerImage).EndInit();
            TablePanel.ResumeLayout(false);
            TablePanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.PictureBox SpinnerImage;
        private System.Windows.Forms.TableLayoutPanel TablePanel;
        private System.Windows.Forms.Label LabelDescription;
    }
}
