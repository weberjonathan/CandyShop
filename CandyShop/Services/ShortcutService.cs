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

        // --------------------------------

        ///// <summary>
        ///// Writes current desktop shortcuts to temporary a file.
        ///// </summary>
        ///// <returns>The name of the newly created file</returns>
        //public string WriteCurrentShortcuts()
        //{
        //    List<string> shortcuts = GetCurrentDesktopShortcuts();
        //    return WriteShourtcutsToFile(shortcuts);
        //}

        ///// <summary>
        ///// Compares old shortcuts with Id to current desktop
        ///// shortcuts.
        ///// </summary>
        ///// <param name="comparerId">Id of old shortcuts file</param>
        ///// <returns>List of new desktop shortcuts</returns>
        //public List<string> GetNewShortcuts(string comparerId)
        //{
        //    var oldShortcuts = ReadShortcutsFromFile(comparerId);
        //    return GetDifferenceOfShortcuts(oldShortcuts);
        //}

        ///// <summary>
        ///// Deletes new shortcuts that were created between the
        ///// snapshot identified by the comparerId and now.
        ///// Also deletes the shortcuts file corresponding to the id
        ///// </summary>
        ///// <param name="comparerId"></param>
        //public void DeleteShortcuts(string comparerId)
        //{
        //    List<string> shortcuts = GetNewShortcuts(comparerId);
        //    foreach (string filepath in shortcuts)
        //    {
        //        if (File.Exists(filepath)) File.Delete(filepath);
        //    }

        //    string shortcutsFile = Path.Combine(ShortcutsInfoDirectory, comparerId);
        //    if (File.Exists(shortcutsFile))
        //    {
        //        File.Delete(Path.Combine(ShortcutsInfoDirectory, comparerId));
        //    }
        //}
        
        //private List<string> GetCurrentDesktopShortcuts()
        //{
        //    return Directory.GetFiles(
        //        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        //        "*.lnk",
        //        SearchOption.TopDirectoryOnly).ToList();
        //}

        //private List<string> ReadShortcutsFromFile(string filename)
        //{
        //    List<string> rtn = new List<string>();
        //    string filepath = Path.Combine(ShortcutsInfoDirectory, filename);

        //    try
        //    {
        //        string content = File.ReadAllText(filepath, Encoding.Default); // TODO try catch
        //        ShortcutsFileContainer container = JsonSerializer.Deserialize<ShortcutsFileContainer>(content);
        //        rtn = container.Shortcuts;
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error("An unknown error occurred: {}", e.Message);
        //        return rtn;
        //    }

            
        //    return rtn;
        //}

        //private string WriteShourtcutsToFile(List<string> shortcuts)
        //{
        //    var data = new ShortcutsFileContainer();
        //    data.Shortcuts = shortcuts;
            
        //    string json = JsonSerializer.Serialize(data);
        //    try
        //    {
        //        Directory.CreateDirectory(ShortcutsInfoDirectory);
        //        File.WriteAllText(Path.Combine(ShortcutsInfoDirectory, data.Id), json, Encoding.Default);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error("An unknown error occurred: {}", e.Message);
        //        return "";
        //    }

        //    return data.Id;
        //}

        //private List<string> GetDifferenceOfShortcuts(List<string> oldShortcuts)
        //{
        //    List<string> newShortcuts = GetCurrentDesktopShortcuts();
        //    return newShortcuts.Except(oldShortcuts).ToList();
        //}
    }
}
