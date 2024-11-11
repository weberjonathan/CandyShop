using System;

namespace CandyShop.View
{
    internal class PinnedChangedArgs : EventArgs
    {
        public string Name;
    }

    interface IUpgradePageView : ITabPage
    {
        event EventHandler UpgradeAllClick;
        event EventHandler UpgradeSelectedClick;
        event EventHandler CleanShortcutsChanged;
        event EventHandler CloseAfterUpgradeChanged;
        public event EventHandler CheckTopLevelClicked;
        event EventHandler RefreshClicked;

        public event EventHandler<PinnedChangedArgs> PinnedChanged;

        string[] Items { get; }
        string[] SelectedItems { get; }
        bool CleanShortcuts { get; set; }
        bool CloseAfterUpgrade { get; set; }
        bool Loading { get; set; }
        bool ShowUacIcons { get; set; }
        bool AllowPinnedUacIon { get; set; }
        void AddItem(object[] data);
        void ClearItems();
        void CheckAllItems();
        void SetItemCheckState(string name, bool state);
        void UncheckAllItems();
        void SetPinned(string name, bool pinned);
        void DisplayEmpty();
    }
}
