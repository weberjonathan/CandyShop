namespace CandyShop.Controls
{
    partial class Banner
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
            ColoredContainer = new System.Windows.Forms.Panel();
            HorizontalStack = new System.Windows.Forms.TableLayoutPanel();
            Icon = new System.Windows.Forms.Panel();
            LblAdmin = new System.Windows.Forms.Label();
            BtnHideWarning = new System.Windows.Forms.LinkLabel();
            LowerBorder = new System.Windows.Forms.Panel();
            UpperBorder = new System.Windows.Forms.Panel();
            HorizontalStack.SuspendLayout();
            SuspendLayout();
            //
            // ColoredContainer
            //
            ColoredContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            ColoredContainer.Controls.Add(HorizontalStack);
            ColoredContainer.Controls.Add(LowerBorder);
            ColoredContainer.Controls.Add(UpperBorder);
            // 
            // HorizontalStack
            // 
            HorizontalStack.ColumnCount = 3;
            HorizontalStack.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            HorizontalStack.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            HorizontalStack.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            HorizontalStack.Controls.Add(Icon, 0, 0);
            HorizontalStack.Controls.Add(LblAdmin, 1, 0);
            HorizontalStack.Controls.Add(BtnHideWarning, 2, 0);
            HorizontalStack.Dock = System.Windows.Forms.DockStyle.Fill;
            HorizontalStack.Location = new System.Drawing.Point(0, 0);
            HorizontalStack.Name = "HorizontalStack";
            HorizontalStack.RowCount = 1;
            HorizontalStack.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            HorizontalStack.Size = new System.Drawing.Size(1045, 42);
            HorizontalStack.TabIndex = 4;
            // 
            // Icon
            // 
            Icon.BackgroundImage = Properties.Resources.ic_warn;
            Icon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            Icon.Dock = System.Windows.Forms.DockStyle.Fill;
            Icon.Location = new System.Drawing.Point(0, 0);
            Icon.Margin = new System.Windows.Forms.Padding(0);
            Icon.Name = "Icon";
            Icon.Size = new System.Drawing.Size(32, 42);
            Icon.TabIndex = 1;
            // 
            // LblAdmin
            // 
            LblAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
            LblAdmin.Location = new System.Drawing.Point(35, 0);
            LblAdmin.Name = "LblAdmin";
            LblAdmin.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            LblAdmin.Size = new System.Drawing.Size(947, 42);
            LblAdmin.TabIndex = 0;
            LblAdmin.Text = "Candy Shop does not have administrator privileges. Proceed with caution!";
            LblAdmin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BtnHideWarning
            // 
            BtnHideWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnHideWarning.Location = new System.Drawing.Point(988, 0);
            BtnHideWarning.Name = "BtnHideWarning";
            BtnHideWarning.Size = new System.Drawing.Size(54, 42);
            BtnHideWarning.TabIndex = 2;
            BtnHideWarning.TabStop = true;
            BtnHideWarning.Text = "&Hide";
            BtnHideWarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LowerBorder
            // 
            LowerBorder.BackColor = System.Drawing.SystemColors.ControlDark;
            LowerBorder.Dock = System.Windows.Forms.DockStyle.Bottom;
            LowerBorder.Location = new System.Drawing.Point(0, 42);
            LowerBorder.Name = "LowerBorder";
            LowerBorder.Size = new System.Drawing.Size(1045, 1);
            LowerBorder.TabIndex = 5;
            // 
            // UpperBorder
            // 
            UpperBorder.BackColor = System.Drawing.SystemColors.ControlDark;
            UpperBorder.Dock = System.Windows.Forms.DockStyle.Top;
            LowerBorder.Location = new System.Drawing.Point(0, 42);
            UpperBorder.Name = "UpperBorder";
            UpperBorder.Size = new System.Drawing.Size(1045, 1);
            UpperBorder.TabIndex = 5;
            // 
            // Banner
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(ColoredContainer);
            Name = "Banner";
            Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            Size = new System.Drawing.Size(1045, 53);
            HorizontalStack.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel ColoredContainer;
        private System.Windows.Forms.TableLayoutPanel HorizontalStack;
        private System.Windows.Forms.Panel LowerBorder;
        private System.Windows.Forms.Panel UpperBorder;
        private System.Windows.Forms.Panel Icon;
        private System.Windows.Forms.LinkLabel BtnHideWarning;
        private System.Windows.Forms.Label LblAdmin;
    }
}
