using System;
using System.Collections.Generic;

namespace CandyShop.View
{
    interface IUpgradePageView
    {
        event EventHandler UpgradeAllClick;
        event EventHandler UpgradeSelectedClick;
        event EventHandler CancelClick;
        event EventHandler CleanShortcutsChanged;

        string[] Items { get; }
        string[] SelectedItems { get; }
        bool ShowAdminWarning { get; set; }
        bool CleanShortcuts { get; set; }

        void AddItem(string[] data);
        void CheckAllItems();
        void CheckItemsByText(List<string> names);
        void UncheckAllItems();
    }
}
