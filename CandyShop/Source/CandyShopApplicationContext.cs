using CandyShop.Chocolatey;
using CandyShop.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CandyShop
{
    public class CandyShopApplicationContext : ApplicationContext
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

        public CandyShopApplicationContext()
        {
            Application.ApplicationExit += new EventHandler((sender, e) =>
            {
                if (_TrayIcon != null) _TrayIcon.Visible = false;
            });

            List<string> args = new List<string>(Environment.GetCommandLineArgs());
            Silent = args.Find(s => s.Equals("--silent") || s.Equals("-s")) != null;

            if (Silent)
            {
                DisplayNotifyIcon();
            }
            else
            {
                DisplayForm();
            }
        }

        public bool Silent { get; }

        private void DisplayNotifyIcon()
        {
            // create context menu
            ToolStripItem exitItem = new ToolStripMenuItem
            {
                Text = "Exit",
            };
            exitItem.Click += new EventHandler((sender, e) => { ExitThread(); });

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
            int count = 0;
            try
            {
                count = ChocolateyWrapper.CheckOutdated().Count;
            }
            catch (ChocolateyException)
            {
                _TrayIcon.BalloonTipIcon = ToolTipIcon.Error;
                _TrayIcon.Text = Application.ProductName;
                _TrayIcon.BalloonTipTitle = $"Candy Shop";
                _TrayIcon.BalloonTipText = $"Could not check for outdated packages.";
                _TrayIcon.ShowBalloonTip(2000);
                Environment.Exit(0);
            }

            // prepare balloon and click handlers
            if (count > 0)
            {
                _TrayIcon.BalloonTipClicked += new EventHandler((sender, e) => { DisplayForm(); });
                _TrayIcon.MouseClick += new MouseEventHandler((sender, e) =>
                {
                    if (e.Button.Equals(MouseButtons.Left)) DisplayForm();
                });

                _TrayIcon.BalloonTipIcon = ToolTipIcon.Info;
                _TrayIcon.Text = Application.ProductName;
                _TrayIcon.BalloonTipTitle = $"{count} package{(count == 1 ? " is" : "s are")} outdated.";
                _TrayIcon.BalloonTipText = $"To upgrade click here or the tray icon later.";
                _TrayIcon.ShowBalloonTip(2000);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void DisplayForm()
        {
            /* form.Show() has to be used (not ShowDialog()),
             * else Application.Run() in Program.cs may not have run
             * which causes ExitThread() to not function.
             */
            
            CandyShopForm form = new CandyShopForm();
            form.FormClosed += new FormClosedEventHandler((sender, e) =>
            {
                form.ShowInTaskbar = false;
                form.Hide();

                if (form.DialogResult.Equals(DialogResult.OK))
                {
                    DisplayUpgradeConsole(form.SelectedPackages);
                }
                else
                {
                    if (!Silent)
                    {
                        ExitThread();
                    }
                }
            });

            form.Show();
        }

        private void DisplayUpgradeConsole(List<ChocolateyPackage> packages)
        {
            // setup watcher for desktop shortcuts
            Queue<string> shortcuts = new Queue<string>();
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                watcher.Filter = "*.lnk";
                watcher.IncludeSubdirectories = false;
                watcher.NotifyFilter = NotifyFilters.FileName;
                watcher.InternalBufferSize = 65536; // TODO test; incurs performance penalty so remove if not useful
                watcher.Created += new FileSystemEventHandler((sender, e) =>
                {
                    shortcuts.Enqueue(e.FullPath);
                });
                watcher.EnableRaisingEvents = true;

                // upgrade
                AllocConsole();
                Console.CursorVisible = false;

                try
                {
                    ChocolateyWrapper.Upgrade(packages);
                }
                catch (ChocolateyException e)
                {
                    MessageBox.Show(
                        $"An error occurred while executing Chocolatey: \"{e.Message}\"",
                        $"{Application.ProductName} Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    ExitThread();
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
                SetForegroundWindow(handle);
            }
            Console.CursorVisible = false;
            Console.Write("\nPress any key to terminate... ");
            Console.ReadKey();
            ExitThread();
        }
    }
}
