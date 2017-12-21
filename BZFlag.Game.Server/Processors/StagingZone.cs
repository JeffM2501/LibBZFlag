using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BZFlag.Networking.Messages;

using BZFlag.Game.Host.Players;
using BZFlag.Networking;
using BZFlag.Networking.Messages.BZFS.World;
using BZFlag.Networking.Messages.BZFS.Player;
using BZFlag.Networking.Messages.BZFS.Control;
using BZFlag.Networking.Messages.BZFS.Info;
using BZFlag.Services;
using BZFlag.Game.Security;
using BZFlag.Game.Host.World;
using BZFlag.Networking.Messages.BZFS.BZDB;

namespace BZFlag.Game.Host.Processors
{
    internal class StagingZone : PlayerProcessor
    {
        public GameWorld World = new GameWorld();
        public BZFlag.Data.BZDB.Database DB = null;

        public StagingZone(ServerConfig cfg) : base(cfg)
        {
            MessageProcessor = SecurityJailMessageFacotry.Factory;

            RegisterCommonHandlers();
        }

        protected override void UpdatePlayer(ServerPlayer player)
        {
            PackInitalBZDB(player);
            Promote(player);
        }

        protected void PackInitalBZDB(ServerPlayer player)
        {
            int size = 0;

            bool sentOne = false;

            Dictionary<string, string> currentList = new Dictionary<string, string>();
            foreach (var item in DB.GetVars())
            {
                int thisSize = item.Key.Length + item.RawValue.Length + 2;
                if ( thisSize + size >= 512)
                {
                    sentOne = true;
                    player.SendMessage(new MsgSetVars(currentList));
                    currentList.Clear();
                    size = 0;
                }
                currentList.Add(item.Key, item.RawValue);
                size += thisSize;
            }

            if (!sentOne || currentList.Count > 0)
                player.SendMessage(new MsgSetVars(currentList));
        }
    }
}
