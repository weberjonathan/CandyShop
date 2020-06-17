using System;
using System.Windows.Forms;

namespace ChocoAutoUpdate
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // TODO
            // - do not require admin; show warning instead and implement --hide-admin-warn

            bool hideAdminWarn = false;

            foreach (string arg in args)
            {
                if (arg.Equals("--hide-admin-warn"))
                {
                    hideAdminWarn = true;
                }
            }
            
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApplicationContext appContext = new ApplicationContext();
            appContext.HideAdminWarn = hideAdminWarn;
            Application.Run(appContext);
        }
    }
}
