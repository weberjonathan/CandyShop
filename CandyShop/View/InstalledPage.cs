using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using Serilog;
using CandyShop.Controls;
using CandyShop.Properties;

namespace CandyShop.View
{
    partial class InstalledPage : UserControl, IInstalledPageView
    {
        private CommonSearchBar SearchBar;

        public InstalledPage()
        {
            InitializeComponent();
            LstPackages.Hint = LocaleEN.TEXT_LOADING_INSTALLED;
            LstPackages.Other.SelectedIndexChanged += new EventHandler((sender, e) => SelectedItemChanged?.Invoke(this, EventArgs.Empty));
            SplitContainer.Panel2Collapsed = true;
        }

        public event EventHandler ShowTopLevelOnlyChanged;
        public event EventHandler FilterTextChanged;
        public event EventHandler SelectedItemChanged;

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

        public bool ShowTopLevelOnly => SearchBar.FilterTopLevelOnly;

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

        public string FilterText => SearchBar.Text;

        public void BuildControls(ICommon provider)
        {
            LstPackages.Columns = provider.GetInstalledColumns();

            SearchBar = provider.GetSearchBar();
            SearchBar.Dock = DockStyle.Top;
            SearchBar.FilterTopLevelOnlyChanged += new EventHandler((sender, e) => ShowTopLevelOnlyChanged?.Invoke(sender, e));
            SearchBar.SearchChanged += new EventHandler((sender, e) => FilterTextChanged?.Invoke(sender, e));
            SearchBar.SearchEnterPressed += new EventHandler((sender, e) =>
            {
                if (LstPackages.Other.Items.Count > 0)
                    LstPackages.Other.Items[0].Selected = true;
            });
            Controls.Add(SearchBar);
        }

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
            LoadingDetails = false;
            SplitContainer.Panel2Collapsed = false;
        }

        public void RemoveItem(string name)
        {
            LstPackages.Other.Items.Remove(LstPackages.Other.FindItemWithText(name));
        }
    }
}
