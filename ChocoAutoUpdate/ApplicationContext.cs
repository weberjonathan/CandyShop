using ChocoAutoUpdate.Properties;
using ChocoHelpers;
using System;
using System.IO;
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

        public bool HideAdminWarn { get; set; } // TODO implement

        // TODO exit button
        
        public ApplicationContext()
        {
            // Initialize Tray Icon
            _TrayIcon = new NotifyIcon()
            {
                // Icon = Resources.AppIcon,
                Icon = Resources.Icon,
                Visible = true
            };

            // check outdated
            try
            {
                _Choco = new Choco();
            }
            catch (ChocolateyException)
            {
                // TODO
                throw;
            }
            catch (ChocoHelpersException)
            {
                // TODO
                throw;
            }

            int count = _Choco.OutdatedCount;

            // prepare balloon and click handlers
            if (count > 0)
            {
                _TrayIcon.BalloonTipClicked += TrayIcon_BalloonTipClicked;
                _TrayIcon.Click += TrayIcon_Click;
                _TrayIcon.BalloonTipIcon = ToolTipIcon.Info;
                _TrayIcon.Text = Application.ProductName;

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
                // check if admin
                // TODO actually check privileges
                if (!HideAdminWarn)
                {
                    result = MessageBox.Show(
                        $"{Application.ProductName} does not have administrator privileges. Do you wish to continue?",
                        Application.ProductName,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    );

                    if (result.Equals(DialogResult.No))
                    {
                        Exit();
                    }
                }

                // upgrade
                AllocConsole();
                Console.WriteLine("> choco upgrade all -y");

                try
                {
                    _Choco.Upgrade();
                }
                catch (ChocolateyException)
                {
                    // TODO
                    throw;
                }

                // remove shortcuts
                Console.WriteLine($"Created {_Choco.NewShortcuts.Length} new desktop shortcut(s) during the upgrade process:");
                foreach (string shortcut in _Choco.NewShortcuts)
                {
                    Console.WriteLine($"- {Path.GetFileNameWithoutExtension(shortcut)}");
                }
                Console.Write("Do you wish to delete them all? [y] ");
                
                if (Console.ReadLine().ToLower().Equals("y"))
                {
                    _Choco.RemoveShortcuts();
                    if (_Choco.NewShortcuts.Length > 0)
                    {
                        Console.WriteLine("Could not delete one or more shortcuts:");
                        foreach (string shortcut in _Choco.NewShortcuts)
                        {
                            Console.WriteLine($"- {Path.GetFileNameWithoutExtension(shortcut)}");
                        }
                    }
                }

                // exit
                Console.CursorVisible = false;
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
