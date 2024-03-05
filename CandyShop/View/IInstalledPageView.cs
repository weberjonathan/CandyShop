using System;
using System.Collections.Generic;

namespace CandyShop.View
{
    interface IInstalledPageView
    {
        event EventHandler ShowTopLevelOnlyChanged;
        event EventHandler FilterTextChanged;
        event EventHandler SelectedItemChanged;

        List<string> Items { get; }
        string SelectedItem { get; }
        string FilterText { get; }
        bool ShowTopLevelOnly { get; } // a toggle that defines if only top level packages are shown in the list view
        public bool EnableTopLevelToggle { get; set; } // whether above mentioned toggle is displayed, or not
        public bool Loading { get; set; }
        void AppendItem(string name, string version);
        void InsertItem(int index, string name, string version);
        void RemoveItem(string name);
        void UpdateDetails(string details);
    }
}
