using CandyShop.Controls;
using System;

namespace CandyShop.View
{
    internal interface IPinSupport
    {
        public event EventHandler<PinnedChangedArgs> PinnedChanged;

        void UpdatePinnedState(string name, bool pinned);
    }
}
