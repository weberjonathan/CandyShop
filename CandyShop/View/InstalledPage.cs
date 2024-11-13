using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using CandyShop.Properties;
using CandyShop.Controls.PackageManager;
using CandyShop.ControlsFactory;

namespace CandyShop.View
{
    partial class InstalledPage : UserControl, IInstalledPageView
    {
        private const int COL_INDEX_NAME = 1;

        public InstalledPage()
        {
            InitializeComponent();
            LstPackages.Hint = LocaleEN.TEXT_LOADING_INSTALLED;
            LstPackages.Other.SelectionChanged += new EventHandler((sender, e) => SelectedItemChanged?.Invoke(this, EventArgs.Empty));
            SplitContainer.Panel2Collapsed = true;
        }

        public event EventHandler SelectedItemChanged;

        public string SelectedItem
        {
            get
            {
                if (LstPackages.Other.SelectedRows.Count > 0)
                    return LstPackages.Other.SelectedRows[0].Cells[COL_INDEX_NAME].Value.ToString();
                else
                    return null;
            }
        }

        public CommonSearchBar SearchBar { get; private set; }

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

        public List<string> Items => LstPackages.Other.Rows
            .Cast<DataGridViewRow>()
            .Select(item => (string)item.Cells[COL_INDEX_NAME].Value)
            .ToList();

        public void BuildControls(AbstractCommon provider)
        {
            LstPackages.ColumnHeaders = provider.GetInstalledColumns();
            LstPackages.CheckBoxes = false;

            SearchBar = provider.GetSearchBar();
            SearchBar.Dock = DockStyle.Top;
            SearchBar.SearchEnterPressed += new EventHandler((sender, e) =>
            {
                if (LstPackages.Other.RowCount > 0)
                    LstPackages.Other.Rows[0].Selected = true;
            });
            Controls.Add(SearchBar);
        }

        public void AppendItem(List<string> data)
        {
            data.Insert(0, "false");
            LstPackages.Other.Rows.Add(data.ToArray());
            LoadingPackages = false;
        }

        public void InsertItem(int index, List<string> data)
        {
            data.Insert(0, "false");
            LstPackages.Other.Rows.Insert(index, data.ToArray());
        }

        public void ClearItems()
        {
            LstPackages.Other.Rows.Clear();
        }

        public void UpdateDetails(string details)
        {
            packageInfoBox1.Text = details;
            LoadingDetails = false;
            SplitContainer.Panel2Collapsed = false;
        }

        public void RemoveItem(string name)
        {
            DataGridViewRow item = null;
            foreach (DataGridViewRow row in LstPackages.Other.Rows)
            {
                if (name.Equals(row.Cells[COL_INDEX_NAME].Value))
                {
                    item = row;
                    break;
                }
            }

            if (item != null)
                LstPackages.Other.Rows.Remove(item);
        }
    }
}
