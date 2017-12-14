using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BZFlag.Networking.Messages;

using BZFlag.Game.Host.Players;
using BZFlag.Networking;
using BZFlag.Networking.Messages.BZFS.Player;
using BZFlag.Networking.Messages.BZFS;
using BZFlag.Services;
using BZFlag.Game.Security;
using BZFlag.Game.Host.World;
using BZFlag.Data.Game;
using System.Security.Cryptography;


namespace BZFlag.Game.Host.Processors
{
    public class GamePlayZone : PlayerProcessor
    {
        public Server.GameState State = null;

        public GamePlayZone(ServerConfig cfg, Server.GameState state) : base(cfg)
        {
            State = state;
            MessageProcessor = SecurityJailMessageFacotry.Factory;

            state.BZDatabase.ValueChanged += this.BZDatabase_ValueChanged;

            RegisterCommonHandlers();

            MessageDispatch.Add(new MsgMessage(), HandleChatMessage);
        }

        protected override void PlayerAdded(ServerPlayer player)
        {
            base.PlayerAdded(player);
        }

        protected override void UpdatePlayer(ServerPlayer player)
        {
            base.UpdatePlayer(player);

            if (player.NeedStartupInfo)
            {

            }
        }

        private void HandleChatMessage(ServerPlayer player, NetworkMessage msg)
        {
       
        }

        private void BZDatabase_ValueChanged(object sender, Data.BZDB.Database.DatabaseItem e)
        {
  
        }
    }
}
