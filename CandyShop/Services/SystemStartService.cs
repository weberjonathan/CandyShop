using IWshRuntimeLibrary;
using Microsoft.Win32.TaskScheduler;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;

namespace CandyShop.Services
{
    /// <summary>
    /// This service provides method to enable or disable launch of Candy Shop on system boot
    /// </summary>
    internal class SystemStartService
    {
        private const string LEGACY_TASK_NAME = "CandyShopLaunch";
        private readonly string CurrentExe = Process.GetCurrentProcess().MainModule.FileName;
        private readonly string CurrentWorkingDir = Directory.GetParent(Process.GetCurrentProcess().MainModule.FileName).FullName;

        private readonly string SHORTCUT_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "CandyShop.lnk");

        public void RegisterOnStartup()
        {
            WshShell shell = new();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(SHORTCUT_PATH);
            shortcut.Description = "Silently launches Candy Shop";
            shortcut.TargetPath = Path.GetFullPath(CurrentExe);
            shortcut.Arguments = "--background";
            shortcut.Save();
        }

        public void UnregisterOnStartup()
        {
            if (System.IO.File.Exists(SHORTCUT_PATH))
            {
                System.IO.File.Delete(SHORTCUT_PATH);
            }
        }

        public bool IsLaunchOnStartup()
        {
            return System.IO.File.Exists(SHORTCUT_PATH);
        }

        public bool LaunchTaskExists()
        {
            var task = TaskService.Instance.GetTask(LEGACY_TASK_NAME);
            return task != null;
        }

        /// <exception cref="CandyShopException"></exception>
        public void RemoveLegacyTask()
        {
            try
            {
                TaskService.Instance.RootFolder.DeleteTask(LEGACY_TASK_NAME, true);
            }
            catch (Exception e)
            {
                Log.Error("Failed to delete task: {0}", e.Message);
                throw new CandyShopException($"Could not delete Windows task with name '{LEGACY_TASK_NAME}'. Please launch Candy Shop with admin privileges and try again.", e);
            }
        }
    }
}
