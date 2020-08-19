using System;
using System.Threading;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;

namespace AccountSync
{
    public class Synchronize : IServerEntryPoint
    {
        private static IUserDataManager UserDataManager { get; set; }
        private static ILogger Log { get; set; }

        // ReSharper disable once TooManyDependencies
        public Synchronize(IUserDataManager userDataManager, ILogManager logMan)
        {
            UserDataManager = userDataManager;
            Log = logMan.GetLogger(Plugin.Instance.Name);
        }

        // ReSharper disable once TooManyArguments
        public static void SynchronizePlayState(User syncToUser, User syncFromUser, BaseItem item, long? playbackPositionTicks)
        {
            var syncToItemData   = UserDataManager.GetUserData(syncToUser, item); //Sync To
            var syncFromItemData = UserDataManager.GetUserData(syncFromUser, item); //Sync From

            //If the long? playbackPositionTicks is null, it's the scheduled task requesting the sync operation
            //If the long? playbackPositionTicks has Value, then it is being used by the "PlaybackStopEventArgs" which would send the Progress Tick along with the Void request.
            if (playbackPositionTicks is null) 
            {
                //Only update the progress to the Sync'd account if the SyncFrom account progress is further along in the media stream.
                if (syncToItemData.PlaybackPositionTicks < syncFromItemData.PlaybackPositionTicks) 
                {
                    syncToItemData.PlaybackPositionTicks = syncFromItemData.PlaybackPositionTicks;
                    return;
                }
            }

            syncToItemData.PlaybackPositionTicks = playbackPositionTicks.Value;

            UserDataManager.SaveUserData(syncToUser, item, syncToItemData, UserDataSaveReason.PlaybackProgress, CancellationToken.None);

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        // ReSharper disable once MethodNameNotMeaningful
        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}
