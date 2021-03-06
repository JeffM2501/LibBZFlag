using System;
using System.Collections.Generic;
using System.IO;
using BZFlag.Game.Host;
using BZFlag.Game.Host.API;
using static BZFlag.Game.Host.Players.ChatManager;

namespace BadWordFilter
{
    public class Filter : PlugIn
    {
        public override string Name => "BadWords";

        public override string Description => "Default bad word filter";

        protected List<string> BadWords = new List<string>();

        public override void Shutdown(Server serverInstance)
        {
            State.Chat.DefaultFilter = null;
        }

        public override void Startup(Server serverInstance)
        {
            LoadDatabase(serverInstance.ConfigData.GetCustomConfigData("BadWordsFile"));
            State.Chat.DefaultFilter = FilterChat;
        }

        public void LoadDatabase(string path)
        {
            if (path == null || path == string.Empty)
                return;

            if (File.Exists(path))
                BadWords.AddRange(File.ReadLines(path));
        }

        protected virtual bool FilterChat(ChatMessageEventArgs message)
        {
            string upperMSG = message.MessageText.ToUpperInvariant();
            foreach(string word in BadWords)
            {
                if (upperMSG.Contains(word.ToUpperInvariant()))
                {
                    // TODO, handle case incentive replenishment
                    word.Replace(word, string.Empty);

                    message.Filtered = true;
                }
            }

            return message.Filtered;
        }
    }
}
