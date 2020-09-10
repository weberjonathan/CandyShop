using CandyShop.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CandyShop
{
    public class ChocoAutoUpdateTray : System.Windows.Forms.ApplicationContext
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private NotifyIcon _TrayIcon;

        public ChocoAutoUpdateTray()
        {
            // create context menu
            ToolStripItem exitItem = new ToolStripMenuItem
            {
                Text = "Exit",
            };
            exitItem.Click += new EventHandler((sender, e) => { Exit(); });

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(exitItem);

            // Initialize Tray Icon
            _TrayIcon = new NotifyIcon()
            {
                // Icon = Resources.AppIcon,
                Icon = Resources.IconNew,
                Visible = true,
                ContextMenuStrip = contextMenu
            };

            // check outdated
            int count = GetOutdatedCount();

            #if DEBUG
            count = 3;
            #endif

            // prepare balloon and click handlers
            if (count > 0)
            {
                _TrayIcon.BalloonTipClicked += new EventHandler((sender, e) => { InitiateUpgrade(); });
                _TrayIcon.MouseClick += new MouseEventHandler((sender, e) =>
                {
                    if (e.Button.Equals(MouseButtons.Left)) InitiateUpgrade();
                });
                
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

        private int GetOutdatedCount()
        {
            List<ChocolateyPackage> outdatedPckgs = new List<ChocolateyPackage>();
            try
            {
                outdatedPckgs = ChocolateyWrapper.CheckOutdated();
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
            catch (CandyShopException e)
            {
                MessageBox.Show(
                    e.Message,
                    $"{Application.ProductName} Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Exit();
            }

            return outdatedPckgs.Count;
        }

        private void InitiateUpgrade()
        {
            CandyShopForm form = new CandyShopForm();
            if (form.ShowDialog().Equals(DialogResult.OK))
            {
                // setup watcher for desktop shortcuts
                Queue<string> shortcuts = new Queue<string>();
                using (FileSystemWatcher watcher = new FileSystemWatcher())
                {
                    watcher.Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    watcher.Filter = "*.lnk";
                    watcher.IncludeSubdirectories = false;
                    watcher.NotifyFilter = NotifyFilters.FileName;
                    watcher.Created += new FileSystemEventHandler((sender, e) =>
                    {
                        shortcuts.Enqueue(e.FullPath);
                    });
                    watcher.EnableRaisingEvents = true;

                    // upgrade
                    AllocConsole();
                    Console.CursorVisible = false;
                    // Console.WriteLine($"> choco upgrade {_Choco.Outdated.MarkedPackages.GetPackagesAsString()} -y"); // TODO

                    try
                    {
                        ChocolateyWrapper.Upgrade(form.SelectedPackages);
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
                }

                // remove shortcuts
                if (shortcuts.Count > 0)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append($"During the upgrade process {shortcuts.Count} new desktop shortcut(s) were created:\n\n");
                    foreach (string shortcut in shortcuts)
                    {
                        msg.Append($"- {Path.GetFileNameWithoutExtension(shortcut)}\n");
                    }
                    msg.Append($"\nDo you want to delete all {shortcuts.Count} shortcut(s)?");

                    DialogResult result = MessageBox.Show(
                        msg.ToString(),
                        Application.ProductName,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1);

                    if (result.Equals(DialogResult.Yes))
                    {
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
                IntPtr handle = GetConsoleWindow();
                if (!IntPtr.Zero.Equals(handle))
                {
                    SetForegroundWindow(handle); // TODO test
                }
                Console.CursorVisible = false;
                Console.Write("\nPress any key to terminate... ");
                Console.ReadKey();
                Exit();
            }
        }

        private void Exit()
        {
            _TrayIcon.Visible = false;
            Environment.Exit(0);
        }
    }
}
