using System;
using System.Collections.Generic;
using System.IO;
using AccountSync.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace AccountSync
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasThumbImage, IHasWebPages
    {

        public static Plugin Instance { get; private set; }
        
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public override Guid Id            => new Guid("AFEE16BE-0273-455B-89DA-8AECE378094E");
        public override string Name        => "Account Sync";
        public override string Description => "Sync watched status between two Emby user account profiles";

        public IEnumerable<PluginPageInfo> GetPages() => new[]
        {
            new PluginPageInfo
            {
                Name                 = "AccountSyncPluginConfigurationPage",
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.AccountSyncPluginConfigurationPage.html"
            },
            new PluginPageInfo
            {
                Name                 = "AccountSyncPluginConfigurationPageJS",
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.AccountSyncPluginConfigurationPage.js"
            }

        };

        public Stream GetThumbImage()
        {
            var type = GetType();
            return type.Assembly.GetManifestResourceStream(type.Namespace + ".thumb.jpg");
        }

        public ImageFormat ThumbImageFormat => ImageFormat.Jpg;

    }
}
