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

        public static void SynchronizePlayState(User syncToUser, User syncFromUser, BaseItem item)
        {
            var syncToItemData = UserDataManager.GetUserData(syncToUser, item); //Sync To
            var syncFromItemData = UserDataManager.GetUserData(syncFromUser, item); //Sync From
            
            if (syncToItemData.PlaybackPositionTicks < syncFromItemData.PlaybackPositionTicks)
            {
                syncToItemData.PlaybackPositionTicks = syncFromItemData.PlaybackPositionTicks;
                UserDataManager.SaveUserData(syncToUser, item, syncToItemData, UserDataSaveReason.PlaybackProgress, CancellationToken.None);
            }

        }

        // ReSharper disable once TooManyArguments
        public static void SynchronizePlayState(User syncToUser, BaseItem item, long? playbackPositionTicks)
        {
            var syncToUserItemData   = UserDataManager.GetUserData(syncToUser, item); //Sync To
            
            syncToUserItemData.PlaybackPositionTicks = playbackPositionTicks.Value;

            UserDataManager.SaveUserData(syncToUser, item, syncToUserItemData, UserDataSaveReason.PlaybackProgress, CancellationToken.None);

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
