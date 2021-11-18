using CandyShop.Chocolatey;
using CandyShop.Controls;
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

        private readonly CandyShopForm _MainForm;

        private readonly ChocolateyService _ChocolateyService;

        public CandyShopApplicationContext()
        {
            _ChocolateyService = new ChocolateyService();

            // determine silent mode
            List<string> args = new List<string>(Environment.GetCommandLineArgs());
            bool launchMinimized = args.Find(s => s.Equals("--background") ||
                                                s.Equals("-b")) != null;

            // create form; performs package upgrade onFormClosed and exits afterwards
            _MainForm = new CandyShopForm(_ChocolateyService);
            _MainForm.FormClosing += new FormClosingEventHandler((sender, e) =>
            {
                _MainForm.Hide();

                if (launchMinimized)
                {
                    // perform upgrade and exit; or exit if closed by user; else keep running in tray
                    if (_MainForm.DialogResult == DialogResult.OK && _MainForm.HasSelectedPackages)
                    {
                        PerformPackageUpgrade(_MainForm.SelectedPackages);
                        Environment.Exit(0);
                    }
                    else if (_MainForm.DialogResult == DialogResult.None)
                    {
                        // closed using 'X' in upper right corner
                        Environment.Exit(0);
                    }
                    else
                    {
                        _MainForm.DialogResult = DialogResult.None;
                        e.Cancel = true;
                    }
                }
                else
                {
                    // perform upgrade if needed; always exit
                    if (_MainForm.DialogResult == DialogResult.OK && _MainForm.HasSelectedPackages)
                    {
                        PerformPackageUpgrade(_MainForm.SelectedPackages);
                    }
                    Environment.Exit(0);
                }
                
                
            });

            // launch with form or in tray
            if (launchMinimized)
            {
                // creates a tray icon, displays a notification if outdated packages
                // are found and opens the upgrade UI on click
                RunInBackground();
            }
            else
            {
                // intialize loading of packages and show form
                _MainForm.LoadPackages();
                _MainForm.Show();
            }
        }

        private async void RunInBackground()
        {
            List<ChocolateyPackage> packages = null;

            // create tray icon
            NotifyIcon icon = InitTrayIcon();

            // obtain outdated packages
            try
            {
                packages = await _ChocolateyService.FetchOutdatedAsync();
            }
            catch (ChocolateyException)
            {
                icon.BalloonTipIcon = ToolTipIcon.Error;
                icon.Text = Application.ProductName;
                icon.BalloonTipTitle = $"Candy Shop";
                icon.BalloonTipText = Properties.strings.Err_CheckOutdated;
                icon.ShowBalloonTip(2000);
                Environment.Exit(0);
            }

            _MainForm.SetOutdatedPackages(packages);

            // create click handlers
            icon.BalloonTipClicked += new EventHandler((sender, e) =>
            {
                _MainForm.Show();
            });

            icon.MouseClick += new MouseEventHandler((sender, e) =>
            {
                _MainForm.Show();
            });


            if (packages.Count > 0)
            {
                ShowNotification(packages.Count, icon);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private NotifyIcon InitTrayIcon()
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
            NotifyIcon rtn = new NotifyIcon()
            {
                // Icon = Resources.AppIcon,
                Icon = Resources.IconNew,
                Visible = true,
                ContextMenuStrip = contextMenu
            };

            // make sure tray icon is removed on application exit
            Application.ApplicationExit += new EventHandler((sender, e) =>
            {
                if (rtn != null) rtn.Visible = false;
            });

            return rtn;
        }

        private void ShowNotification(int packageCount, NotifyIcon icon)
        {
            icon.BalloonTipIcon = ToolTipIcon.Info;
            icon.Text = Application.ProductName;
            icon.BalloonTipTitle = $"{packageCount} package{(packageCount == 1 ? " is" : "s are")} outdated.";
            icon.BalloonTipText = $"To upgrade click here or the tray icon later.";
            icon.ShowBalloonTip(2000);
        }

        private void PerformPackageUpgrade(List<ChocolateyPackage> packages)
        {
            // setup watcher for desktop shortcuts
            Queue<string> shortcuts = new Queue<string>();
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.BeginInit();

                watcher.Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                watcher.Filter = "*.lnk";
                watcher.IncludeSubdirectories = false;
                watcher.NotifyFilter = NotifyFilters.FileName;
                watcher.InternalBufferSize = 65536; // TODO test; incurs performance penalty so remove if not useful
                watcher.EnableRaisingEvents = true;
                watcher.Created += new FileSystemEventHandler((sender, e) =>
                {
                    shortcuts.Enqueue(e.FullPath);
                });

                watcher.EndInit();

                // upgrade
                AllocConsole();
                Console.CursorVisible = false;

                try
                {
                    _ChocolateyService.Upgrade(packages);
                }
                catch (ChocolateyException e)
                {
                    MessageBox.Show(
                        $"An error occurred while executing Chocolatey: \"{e.Message}\"",
                        $"{Application.ProductName} Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    return;
                }
            }

            // display results
            IntPtr handle = GetConsoleWindow();
            if (!IntPtr.Zero.Equals(handle))
            {
                SetForegroundWindow(handle);
            }
            Console.CursorVisible = false;
            Console.Write("\nPress any key to continue... ");
            Console.ReadKey();

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
        }
    }
}
