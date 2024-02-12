using System;
using System.Collections.Generic;

namespace CandyShop.View
{
    internal class PinnedChangedArgs : EventArgs
    {
        public string Name;
        public bool Pinned;
    }

    interface IUpgradePageView
    {
        event EventHandler UpgradeAllClick;
        event EventHandler UpgradeSelectedClick;
        event EventHandler CancelClick;
        event EventHandler CleanShortcutsChanged;
        public event EventHandler CheckTopLevelClicked;
        public event EventHandler AlwaysHideAdminWarningClicked;

        public event EventHandler<PinnedChangedArgs> PinnedChanged;

        string[] Items { get; }
        string[] SelectedItems { get; }
        bool ShowAdminWarning { get; set; }
        bool CleanShortcuts { get; set; }
        bool Loading { get; set; }

        void AddItem(string[] data);
        void CheckAllItems();
        void SetItemCheckState(string name, bool state);
        void UncheckAllItems();
        void SetPinned(string name, bool pinned);
    }
}
