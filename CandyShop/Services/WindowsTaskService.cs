using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;
using System.IO;

namespace CandyShop.Services
{
    /// <summary>
    /// This service class allows access to the Windows Task that can be used to launch CandyShop on system start
    /// </summary>
    internal class WindowsTaskService
    {
        private const string TASKNAME = "CandyShopLaunch";

        public void CreateTask()
        {
            string executable = Process.GetCurrentProcess().MainModule.FileName;
            string dir = Directory.GetParent(executable).FullName;

            TaskDefinition definition = TaskService.Instance.NewTask();
            definition.RegistrationInfo.Description = "Launch CandyShop with elevated privileges to display outdated packages on login.";
            definition.Principal.LogonType = TaskLogonType.InteractiveToken;

            LogonTrigger trigger = new LogonTrigger();

            definition.Triggers.Add(trigger);
            definition.Actions.Add(executable, "--background", dir);
            definition.Principal.RunLevel = TaskRunLevel.Highest;

            TaskService.Instance.RootFolder.RegisterTaskDefinition(TASKNAME, definition);
        }

        public void RemoveTask()
        {
            using (TaskService ts = new TaskService())
            {
                Task task = ts.GetTask(TASKNAME);
                if (task != null)
                {
                    ts.RootFolder.DeleteTask(TASKNAME);
                }
            }
        }

        public bool TaskExists()
        {
            using TaskService ts = new TaskService();
            return ts.GetTask(TASKNAME) != null;
        }

        public void ToggleTask()
        {
            if (TaskExists()) RemoveTask();
            if (!TaskExists()) CreateTask();
        }
    }
}
