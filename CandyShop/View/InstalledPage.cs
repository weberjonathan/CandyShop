using CandyShop.Chocolatey;
using CandyShop.Controller;
using CandyShop.Properties;
using System;
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

        public List<string> Items
        {
            get
            {
                var listViewItems = (IEnumerable<ListViewItem>) LstPackages.Items;
                return listViewItems.Select(item => item.Text).ToList();
            }
        }

        public string FilterText => throw new NotImplementedException();

        public event EventHandler HideDependenciesChanged;
        public event EventHandler FilterTextChanged;
        public event EventHandler SelectedItemChanged;

        public void AppendItem(string name, string version)
        {
            LstPackages.Items.Add(new ListViewItem(new string[] { name, version }));
        }

        public void InsertItem(string name, string version)
        {
            int latestPossibleIndex = LstPackages.FindItemWithText(name).Index;
            ListViewItem lastVisibilePackage = null;

            // find package that is supposed to be directly above it
            for (int j = 0; j < latestPossibleIndex; j++)
            {
                ListViewItem previousPackage = LstPackages.FindItemWithText(Items[j]);
                if (previousPackage != null)
                {
                    lastVisibilePackage = previousPackage;
                }
            }

            // insert
            int index = 0;
            if (lastVisibilePackage != null)
            {
                index = LstPackages.Items.IndexOf(lastVisibilePackage) + 1;
            }

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
