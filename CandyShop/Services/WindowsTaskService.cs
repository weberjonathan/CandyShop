﻿using Microsoft.Win32.TaskScheduler;
using System;
using System.Diagnostics;
using System.IO;

namespace CandyShop.Services
{
    /// <summary>
    /// This service class allows access to the Windows Task that can be used to launch CandyShop on system start
    /// </summary>
    internal class WindowsTaskService
    {
        private const string LAUNCH_TASK_NAME = "CandyShopLaunch";
        private readonly string CurrentExe = Process.GetCurrentProcess().MainModule.FileName;
        private readonly string CurrentWorkingDir = Directory.GetParent(Process.GetCurrentProcess().MainModule.FileName).FullName;

        public void CreateLaunchTask()
        {
            TaskDefinition definition = TaskService.Instance.NewTask();
            definition.RegistrationInfo.Description = "Launch CandyShop with elevated privileges to display outdated packages on login.";
            definition.Principal.LogonType = TaskLogonType.InteractiveToken;
            definition.Principal.RunLevel = TaskRunLevel.Highest;
            definition.Actions.Add(CurrentExe, "--background", CurrentWorkingDir);

            LogonTrigger trigger = new LogonTrigger();
            definition.Triggers.Add(trigger);

            TaskService.Instance.RootFolder.RegisterTaskDefinition(LAUNCH_TASK_NAME, definition);
        }

        public void RemoveLaunchTask()
        {
            RemoveTask(LAUNCH_TASK_NAME);
        }

        public bool LaunchTaskExists()
        {
            using TaskService ts = new TaskService();
            return ts.GetTask(LAUNCH_TASK_NAME) != null;
        }

        private void RemoveTask(string name)
        {
            using (TaskService ts = new TaskService())
            {
                Task task = ts.GetTask(name);
                if (task != null)
                {
                    ts.RootFolder.DeleteTask(name);
                }
            }
        }
    }
}