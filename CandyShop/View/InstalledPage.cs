using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using Serilog;

namespace CandyShop.View
{
    partial class InstalledPage : UserControl, IInstalledPageView
    {
        public InstalledPage()
        {
            InitializeComponent();
            Resize += new EventHandler((sender, e) => ResizeSearchbar());
            LstPackages.Other.SelectedIndexChanged += new EventHandler((sender, e) => SelectedItemChanged?.Invoke(this, EventArgs.Empty));
            SplitContainer.Panel2Collapsed = true;
        }

        public event EventHandler ShowTopLevelOnlyChanged;
        public event EventHandler FilterTextChanged;
        public event EventHandler SelectedItemChanged;

        private bool _EnableTopLevelToggle = true;
        public bool EnableTopLevelToggle
        {
            get { return _EnableTopLevelToggle; }
            set {
                CheckHideSuffixed.Visible = value;
                _EnableTopLevelToggle = value;
                ResizeSearchbar();
            }
        }

        public string SelectedItem
        {
            get
            {
                if (LstPackages.Other.SelectedItems.Count > 0)
                    return LstPackages.Other.SelectedItems?[0].Text;
                else
                    return null;
            }
        }

        public bool ShowTopLevelOnly => CheckHideSuffixed.Checked;

        public bool LoadingPackages
        {
            get
            {
                return LstPackages.Loading;
            }
            set
            {
                LstPackages.Loading = value;
                SplitContainer.Panel2Collapsed = true;
                packageInfoBox1.Text = string.Empty;
            }
        }

        public bool LoadingDetails
        {
            get
            {
                return packageInfoBox1.Loading;
            }
            set
            {
                packageInfoBox1.Loading = value;
                if (value)
                {
                    SplitContainer.Panel2Collapsed = false;
                }
            }
        }

        public List<string> Items => LstPackages.Other.Items
            .Cast<ListViewItem>()
            .Select(item => item.Text)
            .ToList();

        public string FilterText => TextSearch.Text;

        public void AppendItem(string name, string version)
        {
            LstPackages.Other.Items.Add(new ListViewItem(new string[] { name, version }));
            var test = LstPackages.Other.Columns;
            LoadingPackages = false;
        }

        public void InsertItem(int index, string name, string version)
        {
            LstPackages.Other.Items.Insert(index, new ListViewItem(new string[] { name, version }));
        }

        public void ClearItems()
        {
            LstPackages.Other.Items.Clear();
        }

        public void UpdateDetails(string details)
        {
            packageInfoBox1.Text = details;
            SplitContainer.Panel2Collapsed = false;
            LoadingDetails = false;
        }

        public void RemoveItem(string name)
        {
            LstPackages.Other.Items.Remove(LstPackages.Other.FindItemWithText(name));
        }

        private void CheckHideSuffixed_CheckedChanged(object sender, EventArgs e)
        {
            ShowTopLevelOnlyChanged?.Invoke(this, EventArgs.Empty);
        }

        private void TextSearch_TextChanged(object sender, EventArgs e)
        {
            FilterTextChanged?.Invoke(this, EventArgs.Empty);
        }

        private void TextSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter) && LstPackages.Other.Items.Count > 0)
            {
                LstPackages.Other.Items[0].Selected = true;
            }
        }

        private void ResizeSearchbar()
        {
            int width = EnableTopLevelToggle ? CheckHideSuffixed.Location.X - 20 : LstPackages.Other.Width;
            TextSearch.Size = new System.Drawing.Size(width, TextSearch.Height);
        }
    }
}
