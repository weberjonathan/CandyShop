using System;
using System.Collections.Generic;

namespace CandyShop.View
{
    interface IPackageViewer
    {
        event EventHandler PackagesAdded;
        event EventHandler RefreshClicked;

        bool Loading { get; set; }

        void ClearPackages();
        void AddPackages(List<object[]> item);
    }
}
