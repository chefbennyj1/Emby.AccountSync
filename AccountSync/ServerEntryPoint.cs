using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Logging;

namespace AccountSync
{
    public class ServerEntryPoint : IServerEntryPoint
    {
        public static ServerEntryPoint Instance { get; set; }
        private ISessionManager SessionManager { get; set; }
        private IUserManager UserManager { get; set; }
        private ILogger Log { get; set; }
        public ServerEntryPoint(ISessionManager sesMan, IUserManager userMan, ILogManager logManager)
        {
            Instance = this;
            SessionManager = sesMan;
            UserManager = userMan;
            Log = logManager.GetLogger(Plugin.Instance.Name);
            SessionManager.PlaybackStopped += SessionManager_PlaybackStopped;

        }

        private void SessionManager_PlaybackStopped(object sender, PlaybackStopEventArgs e)
        {
           
            if (!Plugin.Instance.Configuration.SyncList.Exists(user => user.SyncFromAccount == e.Session.UserId)) return;
            
            var sync = Plugin.Instance.Configuration.SyncList.FirstOrDefault(user => user.SyncFromAccount == e.Session.UserId);

            var syncToUser   = UserManager.GetUserById(sync.SyncToAccount);
            
            Synchronize.SynchronizePlayState(syncToUser, e.Item, e.PlaybackPositionTicks.Value);

        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            Plugin.Instance.UpdateConfiguration(Plugin.Instance.Configuration);
        }
    }
}
