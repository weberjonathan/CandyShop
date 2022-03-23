using System;
using System.Windows.Forms;

namespace CandyShop.View
{
    interface IMainWindowView
    {
        event EventHandler CancelPressed;

        IInstalledPageView InstalledPackagesPage { get; }
        IUpgradePageView UpgradePackagesPage { get; }
        bool CreateTaskEnabled { get; set; }

        void DisplayError(string msg);
        void ClearAdminHints();
        void ShowAdminHints();

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
