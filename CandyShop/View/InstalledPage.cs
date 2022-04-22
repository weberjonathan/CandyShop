﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace CandyShop.View
{
    partial class InstalledPage : UserControl, IInstalledPageView
    {
        public InstalledPage()
        {
            InitializeComponent();

            this.Resize += new EventHandler((sender, e) =>
            {
                TextSearch.Size = new System.Drawing.Size(CheckHideSuffixed.Location.X - 20, TextSearch.Height);
            });

            LstPackages.Resize += new EventHandler((sender, e) =>
            {
                int availWidth = LstPackages.Width - LstPackages.Margin.Left - LstPackages.Margin.Right - SystemInformation.VerticalScrollBarWidth;

                LstPackages.Columns[0].Width = (int)Math.Floor(availWidth * .6);
                LstPackages.Columns[1].Width = (int)Math.Floor(availWidth * .4);
            });
        }

        public event EventHandler HideDependenciesChanged;
        public event EventHandler FilterTextChanged;
        public event EventHandler SelectedItemChanged;

        public string SelectedItem
        {
            get
            {
                if (LstPackages.SelectedItems.Count > 0)
                    return LstPackages.SelectedItems?[0].Text;
                else
                    return null;
            }
        }

        public bool HideDependencies => CheckHideSuffixed.Checked;

        public List<string> Items => LstPackages.Items
            .Cast<ListViewItem>()
            .Select(item => item.Text)
            .ToList();

        public string FilterText => TextSearch.Text;

        public void AppendItem(string name, string version)
        {
            LstPackages.Items.Add(new ListViewItem(new string[] { name, version }));
        }

        public void InsertItem(int index, string name, string version)
        {
            LstPackages.Items.Insert(index, new ListViewItem(new string[] { name, version }));
        }

        public void UpdateDetails(string details)
        {
            TxtDetails.Text = details;
        }

        public void RemoveItem(string name)
        {
            LstPackages.Items.Remove(LstPackages.FindItemWithText(name));
        }

        private void LstPackages_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CheckHideSuffixed_CheckedChanged(object sender, EventArgs e)
        {
            HideDependenciesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void TextSearch_TextChanged(object sender, EventArgs e)
        {
            FilterTextChanged?.Invoke(this, EventArgs.Empty);
        }

        private void TextSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter) && LstPackages.Items.Count > 0)
            {
                LstPackages.Items[0].Selected = true;
            }
        }
    }
}