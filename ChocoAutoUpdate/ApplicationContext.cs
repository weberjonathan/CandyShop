using ChocoHelpers;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ChocoAutoUpdate
{
    public class ApplicationContext : System.Windows.Forms.ApplicationContext
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private NotifyIcon _TrayIcon;
        private Choco _Choco;

        public ApplicationContext()
        {
            // Initialize Tray Icon
            _TrayIcon = new NotifyIcon()
            {
                // Icon = Resources.AppIcon,
                Icon = new System.Drawing.Icon(@"C:\Users\Jonathan\Desktop\Untitled.ico"),
                Visible = true
            };

            _TrayIcon.BalloonTipClicked += TrayIcon_BalloonTipClicked;
            _TrayIcon.Click += TrayIcon_Click;
            _TrayIcon.BalloonTipIcon = ToolTipIcon.Info;
            _TrayIcon.Text = "Chocolatey Auto-Updater";

            // check outdated
            try
            {
                _Choco = new Choco();
            }
            catch (Exception)
            {
                // TODO
                throw;
            }

            int count = _Choco.OutdatedCount;

            // prepare Balloon
            if (count > 0)
            {
                _TrayIcon.BalloonTipTitle = $"{count} package{(count == 1 ? " is" : "s are")} outdated.";
                _TrayIcon.BalloonTipText = $"To upgrade click here or the tray icon later.";
                _TrayIcon.ShowBalloonTip(2000);
            }
            else
            {
                Exit();
            }
        }

        private void TrayIcon_Click(object sender, EventArgs e)
        {
            Upgrade();
        }

        private void TrayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            Upgrade();
        }

        private void Exit()
        {
            _TrayIcon.Visible = false;
            Application.Exit();
        }

        private void Upgrade()
        {
            DialogResult result = MessageBox.Show(
                "Do you wish to upgrade all outdated packages?\n\nDo not change or rename shortcuts on your desktop while the installation is ongoing!",
                Application.ProductName,
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1
            );

            if (result.Equals(DialogResult.Yes))
            {
                AllocConsole();
                Console.WriteLine("> choco upgrade all -y");

                _Choco.Out = Console.Out;
                try
                {
                    _Choco.Upgrade();
                }
                catch (ChocolateyException)
                {
                    // TODO
                    throw;
                }
                _Choco.RemoveShortcuts();

                Console.Write("Press any key to terminate... ");
                Console.ReadKey();
                Exit();
            }
            else if (result.Equals(DialogResult.No))
            {
                Exit();
            }
        }
    }
}
