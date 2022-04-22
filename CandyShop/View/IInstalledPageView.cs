using System;
using System.Collections.Generic;

namespace CandyShop.View
{
    interface IInstalledPageView
    {
        event EventHandler HideDependenciesChanged;
        event EventHandler FilterTextChanged;
        event EventHandler SelectedItemChanged;

        List<string> Items { get; }
        string SelectedItem { get; }
        string FilterText { get; }
        bool HideDependencies { get; }

        void AppendItem(string name, string version);
        void InsertItem(int index, string name, string version);
        void RemoveItem(string name);
        void UpdateDetails(string details);
    }
}
