using System;
using System.Windows.Forms;

namespace CandyShop.View
{
    interface IMainWindow
    {
        event EventHandler CancelPressed;

        IInstalledPage InstalledPackagesPage { get; }
        IUpgradePage UpgradePackagesPage { get; }
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
