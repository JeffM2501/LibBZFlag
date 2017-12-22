using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BZFlag.Game.Host;
using BZFlag.Game.Host.API;
using static BZFlag.Game.Host.ChatManager;

namespace BadWordFilter
{
    public class Filter : PlugIn
    {
        public string Name => "BadWords";

        public string Description => "Default bad word filter";

        public void Shutdown()
        {
        }

        public void Startup(Server serverInstance)
        {
            LoadDatabase(serverInstance.ConfigData.GetCustomConfigData("BadWordsFile"));
            serverInstance.State.Chat.DefaultFilter = FilterChat;
        }

        public void LoadDatabase(string path)
        {
            if (path == null || path == string.Empty)
                return;

        }

        protected virtual bool FilterChat(ChatMessageEventArgs message)
        {

            return false;
        }
    }
}
