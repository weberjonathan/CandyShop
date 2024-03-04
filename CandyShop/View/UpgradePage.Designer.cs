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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpgradePage));
            LblAdmin = new System.Windows.Forms.Label();
            PanelTop = new System.Windows.Forms.Panel();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            panel1 = new System.Windows.Forms.Panel();
            BtnHideWarning = new System.Windows.Forms.LinkLabel();
            BtnUpgradeSelected = new System.Windows.Forms.Button();
            BtnUpgradeAll = new System.Windows.Forms.Button();
            BtnCancel = new System.Windows.Forms.Button();
            panel2 = new System.Windows.Forms.Panel();
            CheckCloseAfterUpgrade = new System.Windows.Forms.CheckBox();
            LblSelected = new System.Windows.Forms.Label();
            CheckDeleteShortcuts = new System.Windows.Forms.CheckBox();
            LstPackages = new System.Windows.Forms.ListView();
            colName = new System.Windows.Forms.ColumnHeader();
            colCurrent = new System.Windows.Forms.ColumnHeader();
            colAvail = new System.Windows.Forms.ColumnHeader();
            colPinned = new System.Windows.Forms.ColumnHeader();
            LblLoading = new System.Windows.Forms.Label();
            PanelTop.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // LblAdmin
            // 
            LblAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
            LblAdmin.Location = new System.Drawing.Point(19, 0);
            LblAdmin.Name = "LblAdmin";
            LblAdmin.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            LblAdmin.Size = new System.Drawing.Size(575, 68);
            LblAdmin.TabIndex = 0;
            LblAdmin.Text = "Candy Shop does not have administrator privileges. Proceed with caution!";
            LblAdmin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PanelTop
            // 
            PanelTop.BackColor = System.Drawing.SystemColors.Info;
            PanelTop.Controls.Add(tableLayoutPanel1);
            PanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            PanelTop.Location = new System.Drawing.Point(0, 0);
            PanelTop.Name = "PanelTop";
            PanelTop.Size = new System.Drawing.Size(637, 68);
            PanelTop.TabIndex = 2;
            PanelTop.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            tableLayoutPanel1.Controls.Add(panel1, 0, 0);
            tableLayoutPanel1.Controls.Add(BtnHideWarning, 2, 0);
            tableLayoutPanel1.Controls.Add(LblAdmin, 1, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(637, 68);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.SystemColors.Info;
            panel1.BackgroundImage = (System.Drawing.Image)resources.GetObject("panel1.BackgroundImage");
            panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new System.Windows.Forms.Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(16, 68);
            panel1.TabIndex = 1;
            // 
            // BtnHideWarning
            // 
            BtnHideWarning.Anchor = System.Windows.Forms.AnchorStyles.Right;
            BtnHideWarning.AutoSize = true;
            BtnHideWarning.Location = new System.Drawing.Point(602, 26);
            BtnHideWarning.Name = "BtnHideWarning";
            BtnHideWarning.Size = new System.Drawing.Size(32, 15);
            BtnHideWarning.TabIndex = 2;
            BtnHideWarning.TabStop = true;
            BtnHideWarning.Text = "&Hide";
            BtnHideWarning.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // BtnUpgradeSelected
            // 
            BtnUpgradeSelected.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BtnUpgradeSelected.Enabled = false;
            BtnUpgradeSelected.Location = new System.Drawing.Point(446, 39);
            BtnUpgradeSelected.Name = "BtnUpgradeSelected";
            BtnUpgradeSelected.Size = new System.Drawing.Size(91, 23);
            BtnUpgradeSelected.TabIndex = 3;
            BtnUpgradeSelected.Text = "&Upgrade";
            BtnUpgradeSelected.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            BtnUpgradeSelected.UseVisualStyleBackColor = true;
            // 
            // BtnUpgradeAll
            // 
            BtnUpgradeAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BtnUpgradeAll.Enabled = false;
            BtnUpgradeAll.Location = new System.Drawing.Point(336, 39);
            BtnUpgradeAll.Name = "BtnUpgradeAll";
            BtnUpgradeAll.Size = new System.Drawing.Size(104, 23);
            BtnUpgradeAll.TabIndex = 2;
            BtnUpgradeAll.Text = "Upgrade &All";
            BtnUpgradeAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            BtnUpgradeAll.UseVisualStyleBackColor = true;
            // 
            // BtnCancel
            // 
            BtnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BtnCancel.Location = new System.Drawing.Point(543, 39);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new System.Drawing.Size(75, 23);
            BtnCancel.TabIndex = 4;
            BtnCancel.Text = "&Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Controls.Add(CheckCloseAfterUpgrade);
            panel2.Controls.Add(LblSelected);
            panel2.Controls.Add(CheckDeleteShortcuts);
            panel2.Controls.Add(BtnUpgradeSelected);
            panel2.Controls.Add(BtnUpgradeAll);
            panel2.Controls.Add(BtnCancel);
            panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel2.Location = new System.Drawing.Point(0, 454);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(637, 79);
            panel2.TabIndex = 0;
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
            // LstPackages
            // 
            LstPackages.CheckBoxes = true;
            LstPackages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colName, colCurrent, colAvail, colPinned });
            LstPackages.Dock = System.Windows.Forms.DockStyle.Fill;
            LstPackages.FullRowSelect = true;
            LstPackages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LstPackages.Location = new System.Drawing.Point(0, 68);
            LstPackages.MultiSelect = false;
            LstPackages.Name = "LstPackages";
            LstPackages.ShowItemToolTips = true;
            LstPackages.Size = new System.Drawing.Size(637, 386);
            LstPackages.TabIndex = 3;
            LstPackages.UseCompatibleStateImageBehavior = false;
            LstPackages.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            colName.Text = "Name";
            // 
            // colCurrent
            // 
            colCurrent.Text = "Current";
            // 
            // colAvail
            // 
            colAvail.Text = "Available";
            // 
            // colPinned
            // 
            colPinned.Text = "Pinned";
            // 
            // LblLoading
            // 
            LblLoading.BackColor = System.Drawing.Color.Transparent;
            LblLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            LblLoading.Location = new System.Drawing.Point(0, 68);
            LblLoading.Name = "LblLoading";
            LblLoading.Size = new System.Drawing.Size(637, 386);
            LblLoading.TabIndex = 1;
            LblLoading.Text = "Loading ...";
            LblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UpgradePage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(LblLoading);
            Controls.Add(LstPackages);
            Controls.Add(PanelTop);
            Controls.Add(panel2);
            Name = "UpgradePage";
            Size = new System.Drawing.Size(637, 533);
            PanelTop.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label LblAdmin;
        private System.Windows.Forms.Panel PanelTop;
        private System.Windows.Forms.Button BtnUpgradeSelected;
        private System.Windows.Forms.Button BtnUpgradeAll;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView LstPackages;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colCurrent;
        private System.Windows.Forms.ColumnHeader colAvail;
        private System.Windows.Forms.ColumnHeader colPinned;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label LblLoading;
        private System.Windows.Forms.CheckBox CheckDeleteShortcuts;
        private System.Windows.Forms.Label LblSelected;
        private System.Windows.Forms.LinkLabel BtnHideWarning;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox CheckCloseAfterUpgrade;
    }
}
