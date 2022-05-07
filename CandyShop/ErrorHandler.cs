using System;
using System.Windows.Forms;

namespace CandyShop
{
    class ErrorHandler
    {
        public static void ShowError(string msg, params string[] args)
        {
            if (args != null && args.Length > 0) msg = String.Format(msg, args);
            MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
