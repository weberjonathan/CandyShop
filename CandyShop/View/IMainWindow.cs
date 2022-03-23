using System.Windows.Forms;

namespace CandyShop.View
{
    interface IMainWindow
    {
        IInstalledPage InstalledPackagesPage { get; }
        IUpgradePage UpgradePackagesPage { get; }
        bool CreateTaskEnabled { get; }

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
