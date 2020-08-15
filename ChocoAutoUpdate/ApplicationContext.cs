using ChocoAutoUpdate.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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

        public bool IsElevated { get; set; }

        public ApplicationContext()
        {
            // create context menu
            ToolStripItem item = new ToolStripMenuItem
            {
                Text = "Exit",
            };
            item.Click += TrayIcon_Rightclick;

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(item);

            // Initialize Tray Icon
            _TrayIcon = new NotifyIcon()
            {
                // Icon = Resources.AppIcon,
                Icon = Resources.IconNew,
                Visible = true,
                ContextMenuStrip = contextMenu
            };

            // check outdated
            try
            {
                _Choco = new Choco();
            }
            catch (ChocolateyException e)
            {
                MessageBox.Show(
                    $"An error occurred while executing Chocolatey: \"{e.Message}\"",
                    $"{Application.ProductName} Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Exit();
            }
            catch (ChocoAutoUpdateException e)
            {
                MessageBox.Show(
                    e.Message,
                    $"{Application.ProductName} Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Exit();
            }

            int count = _Choco.Outdated.Count;

#if DEBUG
            // count = 3;
#endif

            // prepare balloon and click handlers
            if (count > 0)
            {
                _TrayIcon.BalloonTipClicked += TrayIcon_BalloonTipClicked;
                _TrayIcon.MouseClick += TrayIcon_Click;
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

        private void TrayIcon_Rightclick(object sender, EventArgs e)
        {
            Exit();
        }

        private void TrayIcon_Click(object sender, MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Left)) Upgrade();
        }

        private void TrayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            Upgrade();
        }

        private void Exit()
        {
            _TrayIcon.Visible = false;
            Environment.Exit(0);
        }

        private void Upgrade()
        {
            ChocoAutoUpdateForm form = new ChocoAutoUpdateForm(_Choco)
            {
                IsElevated = this.IsElevated
            };
            
            if (form.ShowDialog().Equals(DialogResult.OK))
            {
                // upgrade
                AllocConsole();
                Console.WriteLine($"> choco upgrade {_Choco.Outdated.MarkedPackages.GetPackagesAsString()} -y");

                try
                {
                    _Choco.Upgrade();
                }
                catch (ChocolateyException e)
                {
                    MessageBox.Show(
                        $"An error occurred while executing Chocolatey: \"{e.Message}\"",
                        $"{Application.ProductName} Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    Exit();
                }

                // remove shortcuts
                if (_Choco.NewShortcuts.Length > 0)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append($"\n\nCreated {_Choco.NewShortcuts.Length} new desktop shortcut(s) during the upgrade process:\n");
                    foreach (string shortcut in _Choco.NewShortcuts)
                    {
                        msg.Append($"- {Path.GetFileNameWithoutExtension(shortcut)}");
                    }
                    msg.Append("Do you wish to delete them all?");

                    DialogResult result = MessageBox.Show(
                        msg.ToString(),
                        Application.ProductName,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1);

                    if (result.Equals(DialogResult.Yes))
                    {
                        Queue<string> shortcuts = new Queue<string>(_Choco.NewShortcuts);
                        while (shortcuts.Count > 0)
                        {
                            string shortcut = shortcuts.Dequeue();
                            try
                            {
                                File.Delete(shortcut);
                            }
                            catch (IOException)
                            {
                                // TODO
                            }
                        }
                    }
                }

                // exit
                Console.CursorVisible = false;
                Console.Write("\nPress any key to terminate... ");
                Console.ReadKey();
                Exit();
            }
        }
    }
}
