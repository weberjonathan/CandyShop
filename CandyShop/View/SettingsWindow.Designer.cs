namespace CandyShop.View
{
    partial class SettingsWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsWindow));
            panel2 = new System.Windows.Forms.Panel();
            lblChooseSource = new System.Windows.Forms.Label();
            panel3 = new System.Windows.Forms.Panel();
            btnOk = new System.Windows.Forms.Button();
            btnApply = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            checkEnableGsudo = new System.Windows.Forms.CheckBox();
            lblSecurity2 = new System.Windows.Forms.Label();
            lblSecurity1 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            checkRequireAdmin = new System.Windows.Forms.CheckBox();
            checkCacheAdmin = new System.Windows.Forms.CheckBox();
            cmbSource = new System.Windows.Forms.ComboBox();
            lblGSudoStatus = new System.Windows.Forms.Label();
            btnGSudoBinary = new System.Windows.Forms.Button();
            label6 = new System.Windows.Forms.Label();
            txtGSudoBinary = new System.Windows.Forms.TextBox();
            lblChocoStatus = new System.Windows.Forms.Label();
            btnChocoBinary = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            txtChocoBinary = new System.Windows.Forms.TextBox();
            lblWingetStatus = new System.Windows.Forms.Label();
            btnWingetBinary = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            txtWingetBinary = new System.Windows.Forms.TextBox();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            panel2.Controls.Add(lblChooseSource);
            panel2.Dock = System.Windows.Forms.DockStyle.Top;
            panel2.Location = new System.Drawing.Point(0, 0);
            panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(741, 63);
            panel2.TabIndex = 2;
            panel2.Visible = false;
            // 
            // lblChooseSource
            // 
            lblChooseSource.AutoSize = true;
            lblChooseSource.Location = new System.Drawing.Point(10, 21);
            lblChooseSource.Name = "lblChooseSource";
            lblChooseSource.Size = new System.Drawing.Size(709, 20);
            lblChooseSource.TabIndex = 29;
            lblChooseSource.Text = "Welcome to Candy Shop. Please review the settings below and change them as needed before continuing.";
            // 
            // panel3
            // 
            panel3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            panel3.Controls.Add(btnOk);
            panel3.Controls.Add(btnApply);
            panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel3.Location = new System.Drawing.Point(0, 446);
            panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(741, 71);
            panel3.TabIndex = 36;
            // 
            // btnOk
            // 
            btnOk.Location = new System.Drawing.Point(634, 20);
            btnOk.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(86, 31);
            btnOk.TabIndex = 10;
            btnOk.Text = "&OK";
            btnOk.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            btnApply.Location = new System.Drawing.Point(542, 20);
            btnApply.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnApply.Name = "btnApply";
            btnApply.Size = new System.Drawing.Size(86, 31);
            btnApply.TabIndex = 9;
            btnApply.Text = "&Apply";
            btnApply.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(checkEnableGsudo);
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(lblSecurity2);
            panel1.Controls.Add(lblSecurity1);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(checkRequireAdmin);
            panel1.Controls.Add(checkCacheAdmin);
            panel1.Controls.Add(cmbSource);
            panel1.Controls.Add(lblGSudoStatus);
            panel1.Controls.Add(btnGSudoBinary);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(txtGSudoBinary);
            panel1.Controls.Add(lblChocoStatus);
            panel1.Controls.Add(btnChocoBinary);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(txtChocoBinary);
            panel1.Controls.Add(lblWingetStatus);
            panel1.Controls.Add(btnWingetBinary);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(txtWingetBinary);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 63);
            panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(741, 517);
            panel1.TabIndex = 3;
            // 
            // checkEnableGsudo
            // 
            checkEnableGsudo.AutoSize = true;
            checkEnableGsudo.Location = new System.Drawing.Point(21, 276);
            checkEnableGsudo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            checkEnableGsudo.Name = "checkEnableGsudo";
            checkEnableGsudo.Size = new System.Drawing.Size(121, 24);
            checkEnableGsudo.TabIndex = 37;
            checkEnableGsudo.Text = "Enable gsudo";
            checkEnableGsudo.UseVisualStyleBackColor = true;
            // 
            // lblSecurity2
            // 
            lblSecurity2.BackColor = System.Drawing.SystemColors.Control;
            lblSecurity2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblSecurity2.Location = new System.Drawing.Point(519, 252);
            lblSecurity2.Name = "lblSecurity2";
            lblSecurity2.Size = new System.Drawing.Size(201, 160);
            lblSecurity2.TabIndex = 7;
            lblSecurity2.Text = "If you do not understand the security implications of this feature, it is strongly recommended to leave the option to cache administrator privileges unchecked.";
            // 
            // lblSecurity1
            // 
            lblSecurity1.BackColor = System.Drawing.SystemColors.Control;
            lblSecurity1.Location = new System.Drawing.Point(519, 20);
            lblSecurity1.Name = "lblSecurity1";
            lblSecurity1.Size = new System.Drawing.Size(201, 236);
            lblSecurity1.TabIndex = 6;
            lblSecurity1.Text = resources.GetString("lblSecurity1.Text");
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(21, 24);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(110, 20);
            label7.TabIndex = 33;
            label7.Text = "Package source";
            // 
            // checkRequireAdmin
            // 
            checkRequireAdmin.AutoSize = true;
            checkRequireAdmin.Location = new System.Drawing.Point(21, 243);
            checkRequireAdmin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            checkRequireAdmin.Name = "checkRequireAdmin";
            checkRequireAdmin.Size = new System.Drawing.Size(282, 24);
            checkRequireAdmin.TabIndex = 1;
            checkRequireAdmin.Text = "Upgrade with administrator privileges";
            checkRequireAdmin.UseVisualStyleBackColor = true;
            // 
            // checkCacheAdmin
            // 
            checkCacheAdmin.AutoSize = true;
            checkCacheAdmin.Location = new System.Drawing.Point(21, 309);
            checkCacheAdmin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            checkCacheAdmin.Name = "checkCacheAdmin";
            checkCacheAdmin.Size = new System.Drawing.Size(345, 24);
            checkCacheAdmin.TabIndex = 2;
            checkCacheAdmin.Text = "Cache administrator privileges during upgrades";
            checkCacheAdmin.UseVisualStyleBackColor = true;
            // 
            // cmbSource
            // 
            cmbSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbSource.FormattingEnabled = true;
            cmbSource.Items.AddRange(new object[] { "Winget", "Chocolatey" });
            cmbSource.Location = new System.Drawing.Point(144, 20);
            cmbSource.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cmbSource.Name = "cmbSource";
            cmbSource.Size = new System.Drawing.Size(329, 28);
            cmbSource.TabIndex = 0;
            // 
            // lblGSudoStatus
            // 
            lblGSudoStatus.AutoSize = true;
            lblGSudoStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            lblGSudoStatus.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            lblGSudoStatus.Location = new System.Drawing.Point(104, 387);
            lblGSudoStatus.Name = "lblGSudoStatus";
            lblGSudoStatus.Size = new System.Drawing.Size(217, 20);
            lblGSudoStatus.TabIndex = 28;
            lblGSudoStatus.Text = "Status (exe not found, or version)";
            // 
            // btnGSudoBinary
            // 
            btnGSudoBinary.Location = new System.Drawing.Point(387, 352);
            btnGSudoBinary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnGSudoBinary.Name = "btnGSudoBinary";
            btnGSudoBinary.Size = new System.Drawing.Size(86, 31);
            btnGSudoBinary.TabIndex = 8;
            btnGSudoBinary.Text = "Browse";
            btnGSudoBinary.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(21, 356);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(50, 20);
            label6.TabIndex = 26;
            label6.Text = "gsudo";
            // 
            // txtGSudoBinary
            // 
            txtGSudoBinary.Location = new System.Drawing.Point(104, 352);
            txtGSudoBinary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtGSudoBinary.Name = "txtGSudoBinary";
            txtGSudoBinary.Size = new System.Drawing.Size(276, 27);
            txtGSudoBinary.TabIndex = 7;
            // 
            // lblChocoStatus
            // 
            lblChocoStatus.AutoSize = true;
            lblChocoStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            lblChocoStatus.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            lblChocoStatus.Location = new System.Drawing.Point(104, 189);
            lblChocoStatus.Name = "lblChocoStatus";
            lblChocoStatus.Size = new System.Drawing.Size(217, 20);
            lblChocoStatus.TabIndex = 24;
            lblChocoStatus.Text = "Status (exe not found, or version)";
            // 
            // btnChocoBinary
            // 
            btnChocoBinary.Location = new System.Drawing.Point(387, 155);
            btnChocoBinary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnChocoBinary.Name = "btnChocoBinary";
            btnChocoBinary.Size = new System.Drawing.Size(86, 31);
            btnChocoBinary.TabIndex = 6;
            btnChocoBinary.Text = "Browse";
            btnChocoBinary.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(21, 159);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(83, 20);
            label4.TabIndex = 22;
            label4.Text = "Chocolatey";
            // 
            // txtChocoBinary
            // 
            txtChocoBinary.Location = new System.Drawing.Point(104, 155);
            txtChocoBinary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtChocoBinary.Name = "txtChocoBinary";
            txtChocoBinary.Size = new System.Drawing.Size(276, 27);
            txtChocoBinary.TabIndex = 5;
            // 
            // lblWingetStatus
            // 
            lblWingetStatus.AutoSize = true;
            lblWingetStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            lblWingetStatus.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            lblWingetStatus.Location = new System.Drawing.Point(104, 109);
            lblWingetStatus.Name = "lblWingetStatus";
            lblWingetStatus.Size = new System.Drawing.Size(217, 20);
            lblWingetStatus.TabIndex = 20;
            lblWingetStatus.Text = "Status (exe not found, or version)";
            // 
            // btnWingetBinary
            // 
            btnWingetBinary.Location = new System.Drawing.Point(387, 75);
            btnWingetBinary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnWingetBinary.Name = "btnWingetBinary";
            btnWingetBinary.Size = new System.Drawing.Size(86, 31);
            btnWingetBinary.TabIndex = 4;
            btnWingetBinary.Text = "Browse";
            btnWingetBinary.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(21, 79);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(57, 20);
            label1.TabIndex = 18;
            label1.Text = "Winget";
            // 
            // txtWingetBinary
            // 
            txtWingetBinary.Location = new System.Drawing.Point(104, 75);
            txtWingetBinary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtWingetBinary.Name = "txtWingetBinary";
            txtWingetBinary.Size = new System.Drawing.Size(276, 27);
            txtWingetBinary.TabIndex = 3;
            // 
            // SettingsWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(741, 580);
            Controls.Add(panel1);
            Controls.Add(panel2);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "SettingsWindow";
            Text = "SettingsWindow";
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblChooseSource;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblSecurity2;
        private System.Windows.Forms.Label lblSecurity1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkRequireAdmin;
        private System.Windows.Forms.CheckBox checkCacheAdmin;
        private System.Windows.Forms.ComboBox cmbSource;
        private System.Windows.Forms.Label lblGSudoStatus;
        private System.Windows.Forms.Button btnGSudoBinary;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtGSudoBinary;
        private System.Windows.Forms.Label lblChocoStatus;
        private System.Windows.Forms.Button btnChocoBinary;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtChocoBinary;
        private System.Windows.Forms.Label lblWingetStatus;
        private System.Windows.Forms.Button btnWingetBinary;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtWingetBinary;
        private System.Windows.Forms.CheckBox checkEnableGsudo;
    }
}