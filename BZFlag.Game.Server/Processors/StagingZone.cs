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

namespace BZFlag.Game.Host.Processors
{
    public class StagingZone : PlayerProcessor
    {
        public GameWorld World = new GameWorld();

        public StagingZone(ServerConfig cfg) : base(cfg)
        {
            MessageProcessor = SecurityJailMessageFacotry.Factory;

            RegisterCommonHandlers();
        }

        protected override void PlayerAdded(ServerPlayer player)
        {

        }
    }
}
