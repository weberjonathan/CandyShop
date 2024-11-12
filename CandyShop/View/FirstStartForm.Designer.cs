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
            label1 = new System.Windows.Forms.Label();
            cmbSource = new System.Windows.Forms.ComboBox();
            checkCacheAdmin = new System.Windows.Forms.CheckBox();
            checkRequireAdmin = new System.Windows.Forms.CheckBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            btnContinue = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(18, 23);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(208, 15);
            label1.TabIndex = 0;
            label1.Text = "Please choose your package manager.";
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
            // label2
            // 
            label2.BackColor = System.Drawing.SystemColors.Control;
            label2.Location = new System.Drawing.Point(338, 23);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(276, 97);
            label2.TabIndex = 4;
            label2.Text = resources.GetString("label2.Text");
            // 
            // label3
            // 
            label3.BackColor = System.Drawing.SystemColors.Control;
            label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label3.Location = new System.Drawing.Point(338, 120);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(276, 59);
            label3.TabIndex = 5;
            label3.Text = "If you do not understand the security implications of this feature, it is strongly recommended to leave this option unchecked.";
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
            // label4
            // 
            label4.BackColor = System.Drawing.SystemColors.Control;
            label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label4.Location = new System.Drawing.Point(12, 212);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(602, 30);
            label4.TabIndex = 7;
            label4.Text = "Settings may be changed directly in the configuration file, which can be opened using the \"Extras\" menu. A proper configuration page is planned for later.";
            // 
            // FirstStartForm
            // 
            AcceptButton = btnContinue;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(627, 298);
            Controls.Add(label4);
            Controls.Add(btnContinue);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(checkRequireAdmin);
            Controls.Add(checkCacheAdmin);
            Controls.Add(cmbSource);
            Controls.Add(label1);
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

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSource;
        private System.Windows.Forms.CheckBox checkCacheAdmin;
        private System.Windows.Forms.CheckBox checkRequireAdmin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Label label4;
    }
}