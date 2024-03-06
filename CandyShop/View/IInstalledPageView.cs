using CandyShop.Controls;
using System;
using System.Collections.Generic;

namespace CandyShop.View
{
    interface IInstalledPageView : ITabPage
    {
        event EventHandler SelectedItemChanged; 

        List<string> Items { get; }
        string SelectedItem { get; }
        CommonSearchBar SearchBar { get; }
        public bool LoadingPackages { get; set; }
        public bool LoadingDetails { get; set; }
        void AppendItem(string[] data);
        void InsertItem(int index, string[] data);
        void ClearItems();
        void RemoveItem(string name);
        void UpdateDetails(string details);
    }
}
