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
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            panel1 = new System.Windows.Forms.Panel();
            panel3 = new System.Windows.Forms.Panel();
            btnOk = new System.Windows.Forms.Button();
            btnApply = new System.Windows.Forms.Button();
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
            panel2 = new System.Windows.Forms.Panel();
            lblChooseSource = new System.Windows.Forms.Label();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(panel1, 0, 1);
            tableLayoutPanel1.Controls.Add(panel2, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(648, 435);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
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
            panel1.Location = new System.Drawing.Point(3, 53);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(642, 379);
            panel1.TabIndex = 0;
            // 
            // panel3
            // 
            panel3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            panel3.Controls.Add(btnOk);
            panel3.Controls.Add(btnApply);
            panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel3.Location = new System.Drawing.Point(0, 318);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(642, 61);
            panel3.TabIndex = 36;
            // 
            // btnOk
            // 
            btnOk.Location = new System.Drawing.Point(555, 19);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(75, 23);
            btnOk.TabIndex = 10;
            btnOk.Text = "&OK";
            btnOk.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            btnApply.Location = new System.Drawing.Point(474, 19);
            btnApply.Name = "btnApply";
            btnApply.Size = new System.Drawing.Size(75, 23);
            btnApply.TabIndex = 9;
            btnApply.Text = "&Apply";
            btnApply.UseVisualStyleBackColor = true;
            // 
            // lblSecurity2
            // 
            lblSecurity2.BackColor = System.Drawing.SystemColors.Control;
            lblSecurity2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblSecurity2.Location = new System.Drawing.Point(454, 178);
            lblSecurity2.Name = "lblSecurity2";
            lblSecurity2.Size = new System.Drawing.Size(176, 128);
            lblSecurity2.TabIndex = 7;
            lblSecurity2.Text = "If you do not understand the security implications of this feature, it is strongly recommended to leave the option to cache administrator privileges unchecked.";
            // 
            // lblSecurity1
            // 
            lblSecurity1.BackColor = System.Drawing.SystemColors.Control;
            lblSecurity1.Location = new System.Drawing.Point(454, 15);
            lblSecurity1.Name = "lblSecurity1";
            lblSecurity1.Size = new System.Drawing.Size(176, 162);
            lblSecurity1.TabIndex = 6;
            lblSecurity1.Text = resources.GetString("lblSecurity1.Text");
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(18, 18);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(89, 15);
            label7.TabIndex = 33;
            label7.Text = "Package source";
            // 
            // checkRequireAdmin
            // 
            checkRequireAdmin.AutoSize = true;
            checkRequireAdmin.Location = new System.Drawing.Point(18, 199);
            checkRequireAdmin.Name = "checkRequireAdmin";
            checkRequireAdmin.Size = new System.Drawing.Size(313, 19);
            checkRequireAdmin.TabIndex = 1;
            checkRequireAdmin.Text = "Upgrade with administrator privileges (requires gsudo)";
            checkRequireAdmin.UseVisualStyleBackColor = true;
            // 
            // checkCacheAdmin
            // 
            checkCacheAdmin.AutoSize = true;
            checkCacheAdmin.Location = new System.Drawing.Point(18, 224);
            checkCacheAdmin.Name = "checkCacheAdmin";
            checkCacheAdmin.Size = new System.Drawing.Size(276, 19);
            checkCacheAdmin.TabIndex = 2;
            checkCacheAdmin.Text = "Cache administrator privileges during upgrades";
            checkCacheAdmin.UseVisualStyleBackColor = true;
            // 
            // cmbSource
            // 
            cmbSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbSource.FormattingEnabled = true;
            cmbSource.Items.AddRange(new object[] { "Winget", "Chocolatey" });
            cmbSource.Location = new System.Drawing.Point(126, 15);
            cmbSource.Name = "cmbSource";
            cmbSource.Size = new System.Drawing.Size(288, 23);
            cmbSource.TabIndex = 0;
            // 
            // lblGSudoStatus
            // 
            lblGSudoStatus.AutoSize = true;
            lblGSudoStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            lblGSudoStatus.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            lblGSudoStatus.Location = new System.Drawing.Point(91, 282);
            lblGSudoStatus.Name = "lblGSudoStatus";
            lblGSudoStatus.Size = new System.Drawing.Size(178, 15);
            lblGSudoStatus.TabIndex = 28;
            lblGSudoStatus.Text = "Status (exe not found, or version)";
            // 
            // btnGSudoBinary
            // 
            btnGSudoBinary.Location = new System.Drawing.Point(339, 256);
            btnGSudoBinary.Name = "btnGSudoBinary";
            btnGSudoBinary.Size = new System.Drawing.Size(75, 23);
            btnGSudoBinary.TabIndex = 8;
            btnGSudoBinary.Text = "Browse";
            btnGSudoBinary.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(18, 259);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(40, 15);
            label6.TabIndex = 26;
            label6.Text = "gsudo";
            // 
            // txtGSudoBinary
            // 
            txtGSudoBinary.Location = new System.Drawing.Point(91, 256);
            txtGSudoBinary.Name = "txtGSudoBinary";
            txtGSudoBinary.Size = new System.Drawing.Size(242, 23);
            txtGSudoBinary.TabIndex = 7;
            // 
            // lblChocoStatus
            // 
            lblChocoStatus.AutoSize = true;
            lblChocoStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            lblChocoStatus.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            lblChocoStatus.Location = new System.Drawing.Point(91, 142);
            lblChocoStatus.Name = "lblChocoStatus";
            lblChocoStatus.Size = new System.Drawing.Size(178, 15);
            lblChocoStatus.TabIndex = 24;
            lblChocoStatus.Text = "Status (exe not found, or version)";
            // 
            // btnChocoBinary
            // 
            btnChocoBinary.Location = new System.Drawing.Point(339, 116);
            btnChocoBinary.Name = "btnChocoBinary";
            btnChocoBinary.Size = new System.Drawing.Size(75, 23);
            btnChocoBinary.TabIndex = 6;
            btnChocoBinary.Text = "Browse";
            btnChocoBinary.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(18, 119);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(67, 15);
            label4.TabIndex = 22;
            label4.Text = "Chocolatey";
            // 
            // txtChocoBinary
            // 
            txtChocoBinary.Location = new System.Drawing.Point(91, 116);
            txtChocoBinary.Name = "txtChocoBinary";
            txtChocoBinary.Size = new System.Drawing.Size(242, 23);
            txtChocoBinary.TabIndex = 5;
            // 
            // lblWingetStatus
            // 
            lblWingetStatus.AutoSize = true;
            lblWingetStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            lblWingetStatus.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            lblWingetStatus.Location = new System.Drawing.Point(91, 82);
            lblWingetStatus.Name = "lblWingetStatus";
            lblWingetStatus.Size = new System.Drawing.Size(178, 15);
            lblWingetStatus.TabIndex = 20;
            lblWingetStatus.Text = "Status (exe not found, or version)";
            // 
            // btnWingetBinary
            // 
            btnWingetBinary.Location = new System.Drawing.Point(339, 56);
            btnWingetBinary.Name = "btnWingetBinary";
            btnWingetBinary.Size = new System.Drawing.Size(75, 23);
            btnWingetBinary.TabIndex = 4;
            btnWingetBinary.Text = "Browse";
            btnWingetBinary.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(18, 59);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(45, 15);
            label1.TabIndex = 18;
            label1.Text = "Winget";
            // 
            // txtWingetBinary
            // 
            txtWingetBinary.Location = new System.Drawing.Point(91, 56);
            txtWingetBinary.Name = "txtWingetBinary";
            txtWingetBinary.Size = new System.Drawing.Size(242, 23);
            txtWingetBinary.TabIndex = 3;
            // 
            // panel2
            // 
            panel2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            panel2.Controls.Add(lblChooseSource);
            panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(642, 44);
            panel2.TabIndex = 1;
            // 
            // lblChooseSource
            // 
            lblChooseSource.AutoSize = true;
            lblChooseSource.Location = new System.Drawing.Point(9, 16);
            lblChooseSource.Name = "lblChooseSource";
            lblChooseSource.Size = new System.Drawing.Size(566, 15);
            lblChooseSource.TabIndex = 29;
            lblChooseSource.Text = "Welcome to Candy Shop. Please review the settings below and change them as needed before continuing.";
            // 
            // SettingsWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(648, 435);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "SettingsWindow";
            Text = "SettingsWindow";
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel3.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
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
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblChooseSource;
        private System.Windows.Forms.Label lblSecurity2;
        private System.Windows.Forms.Label lblSecurity1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnApply;
    }
}