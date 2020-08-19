using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace AccountSync.Configuration
{
    public class AccountSync
    {
        //Sync To User (Admin)
        public string SyncToAccount { get; set; }

        //Sync From User (Sync'd to Admin)
        public string SyncFromAccount { get; set; }
    }

    public class PluginConfiguration : BasePluginConfiguration
    {
        public List<AccountSync> SyncList { get; set; }
    }
}
