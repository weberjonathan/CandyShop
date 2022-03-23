using System;
using System.Collections.Generic;

namespace CandyShop.View
{
    interface IUpgradePageView
    {
        public event EventHandler UpgradeAllClick;
        public event EventHandler UpgradeSelectedClick;
        public event EventHandler CancelClick;

        string[] Items { get; }
        string[] SelectedItems { get; }
        bool ShowAdminWarning { get; set; }

        void AddItem(string[] data);
        void CheckAllItems();
        void CheckItemsByText(List<string> names);
        void UncheckAllItems();
    }
}
