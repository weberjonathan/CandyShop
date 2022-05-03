using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CandyShop.Services
{
    internal class ShortcutService
    {
        private class ShortcutsFileContainer
        {
            public ShortcutsFileContainer()
            {
                Id = Guid.NewGuid().ToString();
            }

            public string Id { get; private set; }
            public List<string> Shortcuts { get; set; } = new List<string>();
        }

        private FileSystemWatcher DesktopWatcher;
        private FileSystemWatcher CommonDesktopWatcher;

        private readonly string ShortcutsInfoDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CandyShop/");

        public List<string> GetCurrentDesktopShortcuts()
        {
            var desktopShortcuts = Directory.GetFiles(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "*.lnk",
                SearchOption.TopDirectoryOnly).ToList();

            var commonDesktopShortcuts = Directory.GetFiles(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                "*.lnk",
                SearchOption.TopDirectoryOnly).ToList();

            return desktopShortcuts.Union(commonDesktopShortcuts).ToList();
        }

        public void WatchDesktops(Action<string> onShortcutAdded)
        {
            // setup watcher for desktop shortcuts
            DesktopWatcher = InitWatcher(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), onShortcutAdded);
            CommonDesktopWatcher = InitWatcher(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), onShortcutAdded);
        }

        public void DisposeWatchers()
        {
            DesktopWatcher?.Dispose();
            CommonDesktopWatcher?.Dispose();
        }

        public void DeleteShortcuts(List<string> filepaths)
        {
            foreach (string path in filepaths.Where(path => File.Exists(path)))
            {
                Log.Information($"Deleted desktop shortcut {Path.GetFileNameWithoutExtension(path)}.");
                try
                {
                    File.Delete(path);
                }
                catch (IOException e)
                {
                    Log.Error($"Failed to delete desktop shortcut at \'{path}\': {e.Message}");
                }
            }
        }

        private FileSystemWatcher InitWatcher(string path, Action<string> onShortcutAdded)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.BeginInit();
            watcher.Path = path;
            watcher.Filter = "*.lnk";
            watcher.IncludeSubdirectories = false;
            watcher.NotifyFilter = NotifyFilters.FileName;
            watcher.EnableRaisingEvents = true;
            watcher.Created += new FileSystemEventHandler((sender, e) => onShortcutAdded(e.FullPath));
            watcher.EndInit();
            return watcher;
        }
    }
}
