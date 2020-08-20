using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Tasks;

namespace AccountSync
{
    public class AccountSyncScheduledTask : IScheduledTask, IConfigurableScheduledTask
    {
        private IUserManager UserManager { get; }
        private IUserDataManager UserDataManager { get; }
        private ILibraryManager LibraryManager { get; }
        private ILogger Log { get; }

        public AccountSyncScheduledTask(IUserManager userManager, ILibraryManager libraryManager, ILogManager logManager, IUserDataManager userDataManager)
        {
            UserManager = userManager;
            LibraryManager = libraryManager;
            UserDataManager = userDataManager;
            Log = logManager.GetLogger(Plugin.Instance.Name);
        }
        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            try
            {
                foreach (var syncProfile in Plugin.Instance.Configuration.SyncList)
                {
                    var syncToUser = UserManager.GetUserById(syncProfile.SyncToAccount); //Sync To
                    var syncFromUser = UserManager.GetUserById(syncProfile.SyncFromAccount); //Sync From

                    var queryResultIds = LibraryManager.GetInternalItemIds(new InternalItemsQuery()
                        {IncludeItemTypes = new[] {"Movie", "Episode"}});

                    for (var i = 0; i <= queryResultIds.Count() - 1; i++)
                    {
                        var item = LibraryManager.GetItemById(queryResultIds[i]);

                        Synchronize.SynchronizePlayState(syncToUser, syncFromUser, item);

                        progress.Report(queryResultIds.Count() - (queryResultIds.Count() - i) / 100);
                    }

                }
            }
            catch
            {
            }

            progress.Report(100.0);
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[]
            {
                new TaskTriggerInfo
                {
                    Type          = TaskTriggerInfo.TriggerInterval,
                    IntervalTicks = TimeSpan.FromHours(1).Ticks
                },
                new TaskTriggerInfo()
                {
                    SystemEvent = SystemEvent.WakeFromSleep,
                    Type        = TaskTriggerInfo.TriggerSystemEvent
                }
            };
        }

        public string Name => "Account Sync Notification";
        public string Key => "Account Sync";
        public string Description => "Sync watched states for media items between two accounts.";
        public string Category => "Accounts";
        public bool IsHidden => false;
        public bool IsEnabled => true;
        public bool IsLogged => true;
    }
}
