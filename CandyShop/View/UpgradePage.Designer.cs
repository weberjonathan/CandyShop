using CandyShop.Controls;
using CandyShop.Properties;

namespace CandyShop.View
{
    partial class UpgradePage
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            LstPackages = new PackageListBox();
            BtnUpgradeAll = new System.Windows.Forms.Button();
            BtnUpgradeSelected = new System.Windows.Forms.Button();
            CheckDeleteShortcuts = new System.Windows.Forms.CheckBox();
            LblSelected = new System.Windows.Forms.Label();
            CheckCloseAfterUpgrade = new System.Windows.Forms.CheckBox();
            panel2 = new System.Windows.Forms.Panel();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // LstPackages
            // 
            LstPackages.CheckBoxes = true;
            LstPackages.ColumnHeaders = null;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            LstPackages.DefaultStyle = dataGridViewCellStyle1;
            LstPackages.Dock = System.Windows.Forms.DockStyle.Fill;
            LstPackages.Hint = "";
            LstPackages.Loading = false;
            LstPackages.Location = new System.Drawing.Point(0, 0);
            LstPackages.Name = "LstPackages";
            LstPackages.NoPackages = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            LstPackages.PinnedStyle = dataGridViewCellStyle2;
            LstPackages.Size = new System.Drawing.Size(637, 454);
            LstPackages.TabIndex = 3;
            // 
            // BtnUpgradeAll
            // 
            BtnUpgradeAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BtnUpgradeAll.Enabled = false;
            BtnUpgradeAll.Location = new System.Drawing.Point(417, 39);
            BtnUpgradeAll.Name = "BtnUpgradeAll";
            BtnUpgradeAll.Size = new System.Drawing.Size(104, 23);
            BtnUpgradeAll.TabIndex = 2;
            BtnUpgradeAll.Text = "Upgrade &All";
            BtnUpgradeAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            BtnUpgradeAll.UseVisualStyleBackColor = true;
            // 
            // BtnUpgradeSelected
            // 
            BtnUpgradeSelected.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BtnUpgradeSelected.Enabled = false;
            BtnUpgradeSelected.Location = new System.Drawing.Point(527, 39);
            BtnUpgradeSelected.Name = "BtnUpgradeSelected";
            BtnUpgradeSelected.Size = new System.Drawing.Size(91, 23);
            BtnUpgradeSelected.TabIndex = 3;
            BtnUpgradeSelected.Text = "&Upgrade";
            BtnUpgradeSelected.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            BtnUpgradeSelected.UseVisualStyleBackColor = true;
            // 
            // CheckDeleteShortcuts
            // 
            CheckDeleteShortcuts.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            CheckDeleteShortcuts.AutoSize = true;
            CheckDeleteShortcuts.Location = new System.Drawing.Point(507, 10);
            CheckDeleteShortcuts.Name = "CheckDeleteShortcuts";
            CheckDeleteShortcuts.Size = new System.Drawing.Size(111, 19);
            CheckDeleteShortcuts.TabIndex = 5;
            CheckDeleteShortcuts.Text = "Delete &shortcuts";
            CheckDeleteShortcuts.UseVisualStyleBackColor = true;
            CheckDeleteShortcuts.CheckedChanged += CheckDeleteShortcuts_CheckedChanged;
            // 
            // LblSelected
            // 
            LblSelected.AutoSize = true;
            LblSelected.ForeColor = System.Drawing.SystemColors.ControlDark;
            LblSelected.Location = new System.Drawing.Point(3, 11);
            LblSelected.Name = "LblSelected";
            LblSelected.Size = new System.Drawing.Size(132, 15);
            LblSelected.TabIndex = 6;
            LblSelected.Text = "Selected package count";
            // 
            // CheckCloseAfterUpgrade
            // 
            CheckCloseAfterUpgrade.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            CheckCloseAfterUpgrade.AutoSize = true;
            CheckCloseAfterUpgrade.Location = new System.Drawing.Point(363, 10);
            CheckCloseAfterUpgrade.Name = "CheckCloseAfterUpgrade";
            CheckCloseAfterUpgrade.Size = new System.Drawing.Size(129, 19);
            CheckCloseAfterUpgrade.TabIndex = 7;
            CheckCloseAfterUpgrade.Text = "&Close after upgrade";
            CheckCloseAfterUpgrade.UseVisualStyleBackColor = true;
            CheckCloseAfterUpgrade.CheckedChanged += CheckCloseAfterUpgrade_CheckedChanged;
            // 
            // panel2
            // 
            panel2.Controls.Add(CheckCloseAfterUpgrade);
            panel2.Controls.Add(LblSelected);
            panel2.Controls.Add(CheckDeleteShortcuts);
            panel2.Controls.Add(BtnUpgradeSelected);
            panel2.Controls.Add(BtnUpgradeAll);
            panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel2.Location = new System.Drawing.Point(0, 454);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(637, 79);
            panel2.TabIndex = 0;
            // 
            // UpgradePage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(LstPackages);
            Controls.Add(panel2);
            Name = "UpgradePage";
            Size = new System.Drawing.Size(637, 533);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private PackageListBox LstPackages;
        private System.Windows.Forms.Button BtnUpgradeAll;
        private System.Windows.Forms.Button BtnUpgradeSelected;
        private System.Windows.Forms.CheckBox CheckDeleteShortcuts;
        private System.Windows.Forms.Label LblSelected;
        private System.Windows.Forms.CheckBox CheckCloseAfterUpgrade;
        private System.Windows.Forms.Panel panel2;
    }
}
