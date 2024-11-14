using System;

namespace CandyShop.View
{
    partial class FirstStartForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirstStartForm));
            lblChooseSource = new System.Windows.Forms.Label();
            cmbSource = new System.Windows.Forms.ComboBox();
            checkCacheAdmin = new System.Windows.Forms.CheckBox();
            checkRequireAdmin = new System.Windows.Forms.CheckBox();
            lblSecurity1 = new System.Windows.Forms.Label();
            lblSecurity2 = new System.Windows.Forms.Label();
            btnContinue = new System.Windows.Forms.Button();
            lblHint = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // lblChooseSource
            // 
            lblChooseSource.AutoSize = true;
            lblChooseSource.Location = new System.Drawing.Point(18, 23);
            lblChooseSource.Name = "lblChooseSource";
            lblChooseSource.Size = new System.Drawing.Size(208, 15);
            lblChooseSource.TabIndex = 0;
            lblChooseSource.Text = "Please choose your package manager.";
            // 
            // cmbSource
            // 
            cmbSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbSource.FormattingEnabled = true;
            cmbSource.Items.AddRange(new object[] { "Winget (Default)", "Chocolatey" });
            cmbSource.Location = new System.Drawing.Point(18, 50);
            cmbSource.Name = "cmbSource";
            cmbSource.Size = new System.Drawing.Size(276, 23);
            cmbSource.TabIndex = 1;
            // 
            // checkCacheAdmin
            // 
            checkCacheAdmin.AutoSize = true;
            checkCacheAdmin.Location = new System.Drawing.Point(18, 132);
            checkCacheAdmin.Name = "checkCacheAdmin";
            checkCacheAdmin.Size = new System.Drawing.Size(276, 19);
            checkCacheAdmin.TabIndex = 2;
            checkCacheAdmin.Text = "Cache administrator privileges during upgrades";
            checkCacheAdmin.UseVisualStyleBackColor = true;
            // 
            // checkRequireAdmin
            // 
            checkRequireAdmin.AutoSize = true;
            checkRequireAdmin.Location = new System.Drawing.Point(18, 107);
            checkRequireAdmin.Name = "checkRequireAdmin";
            checkRequireAdmin.Size = new System.Drawing.Size(263, 19);
            checkRequireAdmin.TabIndex = 3;
            checkRequireAdmin.Text = "Require administrator privileges for upgrades";
            checkRequireAdmin.UseVisualStyleBackColor = true;
            // 
            // lblSecurity1
            // 
            lblSecurity1.BackColor = System.Drawing.SystemColors.Control;
            lblSecurity1.Location = new System.Drawing.Point(338, 23);
            lblSecurity1.Name = "lblSecurity1";
            lblSecurity1.Size = new System.Drawing.Size(276, 97);
            lblSecurity1.TabIndex = 4;
            lblSecurity1.Text = resources.GetString("lblSecurity1.Text");
            // 
            // LblSecurity2
            // 
            lblSecurity2.BackColor = System.Drawing.SystemColors.Control;
            lblSecurity2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblSecurity2.Location = new System.Drawing.Point(338, 120);
            lblSecurity2.Name = "LblSecurity2";
            lblSecurity2.Size = new System.Drawing.Size(276, 59);
            lblSecurity2.TabIndex = 5;
            lblSecurity2.Text = "If you do not understand the security implications of this feature, it is strongly recommended to leave this option unchecked.";
            // 
            // btnContinue
            // 
            btnContinue.Location = new System.Drawing.Point(539, 263);
            btnContinue.Name = "btnContinue";
            btnContinue.Size = new System.Drawing.Size(75, 23);
            btnContinue.TabIndex = 6;
            btnContinue.Text = "&Continue";
            btnContinue.UseVisualStyleBackColor = true;
            // 
            // lblHint
            // 
            lblHint.BackColor = System.Drawing.SystemColors.Control;
            lblHint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            lblHint.Location = new System.Drawing.Point(12, 212);
            lblHint.Name = "lblHint";
            lblHint.Size = new System.Drawing.Size(602, 30);
            lblHint.TabIndex = 7;
            lblHint.Text = "Settings may be changed directly in the configuration file, which can be opened using the \"Extras\" menu. A proper configuration page is planned for later.";
            // 
            // FirstStartForm
            // 
            AcceptButton = btnContinue;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(627, 298);
            Controls.Add(lblHint);
            Controls.Add(btnContinue);
            Controls.Add(lblSecurity2);
            Controls.Add(lblSecurity1);
            Controls.Add(checkRequireAdmin);
            Controls.Add(checkCacheAdmin);
            Controls.Add(cmbSource);
            Controls.Add(lblChooseSource);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FirstStartForm";
            Text = "Candy Shop - First start";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblChooseSource;
        private System.Windows.Forms.ComboBox cmbSource;
        private System.Windows.Forms.CheckBox checkCacheAdmin;
        private System.Windows.Forms.CheckBox checkRequireAdmin;
        private System.Windows.Forms.Label lblSecurity1;
        private System.Windows.Forms.Label lblSecurity2;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Label lblHint;
    }
}