using Microsoft.Windows.AppNotifications;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CandyShop
{
    public enum NotificationResult { Show, Ignore, UpgradeAll, None };


    internal class NotificationShowHandler
    {
        private SemaphoreSlim Semaphore = new(1);

        private NotificationResult ResultState = NotificationResult.None;

        /// <summary>
        /// Wraps incoming notification events and offers access through an asynchronous method
        /// </summary>
        public NotificationShowHandler()
        {
            if (!AppNotificationManager.IsSupported())
            {
                return;
            }

            Semaphore.Wait();

            AppNotificationManager.Default.NotificationInvoked += (sender, e) =>
            {
                string arg = e.Argument;
                if ("".Equals(e.Argument) || (HasAction(e.Arguments, "show")))
                    ResultState = NotificationResult.Show;
                else if (HasAction(e.Arguments, "ignore"))
                    ResultState = NotificationResult.Ignore;
                else if (HasAction(e.Arguments, "upgrade"))
                    ResultState = NotificationResult.UpgradeAll;

                Semaphore.Release();
            };
            AppNotificationManager.Default.Register();
        }

        public async Task<NotificationResult> AwaitResult()
        {
            await Semaphore.WaitAsync().ConfigureAwait(false);
            Semaphore.Release();
            return ResultState;
        }

        private bool HasAction(IDictionary<string, string> store, string name)
        {
            return store.TryGetValue("action", out string value) && value.Equals(name);
        }
    }
}
