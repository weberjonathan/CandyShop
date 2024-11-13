using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using CandyShop.Properties;
using CandyShop.Controls;
using CandyShop.Controls.Factory;

namespace CandyShop.View
{
    partial class InstalledPage : UserControl, ITabPage, IPinSupport
    {
        public InstalledPage()
        {
            InitializeComponent();
            LstPackages.Hint = LocaleEN.TEXT_LOADING_INSTALLED;
            LstPackages.Other.SelectionChanged += new EventHandler((sender, e) => SelectedItemChanged?.Invoke(this, EventArgs.Empty));
            SplitContainer.Panel2Collapsed = true;

            LstPackages.PinChangeRequest += new EventHandler<PinnedChangedArgs>((sender, e) => PinnedChanged?.Invoke(this, e));
        }

        public event EventHandler<PinnedChangedArgs> PinnedChanged;
        public event EventHandler SelectedItemChanged;

        public string SelectedItem
        {
            get
            {
                if (LstPackages.Other.SelectedRows.Count > 0)
                    return LstPackages.Other.SelectedRows[0].Cells[LstPackages.NameCol.Index].Value.ToString();
                else
                    return null;
            }
        }

        public SearchBar SearchBar { get; private set; }

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
            .Select(item => (string)item.Cells[LstPackages.NameCol.Index].Value)
            .ToList();

        public void BuildControls(IControlsFactory provider)
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

        public void AppendItem(object[] data)
        {
            if (LoadingPackages)
                LoadingPackages = false;

            LstPackages.AddItem(data);
        }

        public void InsertItem(int index, object[] data)
        {
            LstPackages.Other.Rows.Insert(index, data);
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
                if (name.Equals(row.Cells[LstPackages.NameCol.Index].Value))
                {
                    item = row;
                    break;
                }
            }

            if (item != null)
                LstPackages.Other.Rows.Remove(item);
        }

        public void UpdatePinnedState(string name, bool pinned)
        {
            LstPackages.SetPinned(name, pinned);
        }
    }
}
