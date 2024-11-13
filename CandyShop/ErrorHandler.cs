using System.Windows.Forms;

namespace CandyShop
{
    class ErrorHandler
    {
        public static void ShowError(string msg, params string[] args)
        {
            if (args != null && args.Length > 0) msg = string.Format(msg, args);
            MessageBox.Show(msg, MetaInfo.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void NotifyError(NotifyIcon icon, string msg, params string[] args)
        {
            if (args != null && args.Length > 0) msg = string.Format(msg, args);

            icon.BalloonTipIcon = ToolTipIcon.Error;
            icon.Text = MetaInfo.Name;
            icon.BalloonTipTitle = MetaInfo.WindowTitle;
            icon.BalloonTipText = msg;
            icon.ShowBalloonTip(2000);
        }
    }
}
