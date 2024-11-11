﻿using System;
using System.Windows.Forms;

namespace CandyShop.View
{
    interface IMainWindowView : ITabPage
    {
        IInstalledPageView InstalledPackagesPage { get; }
        IUpgradePageView UpgradePackagesPage { get; }
        bool LaunchOnSystemStartEnabled { get; set; }
        bool ShowAdminWarning {  get; set; }

        event EventHandler RefreshClicked;
        event EventHandler HideAdminWarningClicked;

        void DisplayError(string msg, params string[] args);

        Form ToForm()
        {
            return (Form) this;
        }

        T ToForm<T>() where T : Form
        {
            return (T) this;
        }
    }
}
