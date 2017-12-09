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

namespace BZFlag.Game.Host.Processors
{
    public class StagingZone : PlayerProcessor
    {
       


        public StagingZone(ServerConfig cfg) : base(cfg)
        {
            MessageProcessor = SecurityJailMessageFacotry.Factory;

            RegisterCommonHandlers();

            MessageDispatch.Add(new MsgWantWHash(), HandleWantWorldHash);
            MessageDispatch.Add(new MsgNegotiateFlags(), HandleNegotiateFlags);
        }

        protected override void PlayerAdded(ServerPlayer player)
        {
            if (player.ClientFlagList != null)
                HandleNegotiateFlags(player, player.ClientFlagList);
        }

        private void HandleWantWorldHash(ServerPlayer player, NetworkMessage msg)
        {
            MsgWantWHash hash = msg as MsgWantWHash;
            if (hash == null)
                return;

            hash.WorldHash = "NOPE!";
            player.SendMessage(hash);
        }

        private void HandleNegotiateFlags(ServerPlayer player, NetworkMessage msg)
        {
            MsgNegotiateFlags flags = msg as MsgNegotiateFlags;
            if (flags == null)
                return;

            // just hang onto this data, we don't want to bother processing it until they have cleared the security jail
            player.ClientFlagList = flags;
        }
    }
}
