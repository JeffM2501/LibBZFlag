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

namespace BZFlag.Game.Host.Processors
{
    public class RestrictedAccessZone : PlayerProcessor
    {
        public event EventHandler<ServerPlayer> PromotePlayer = null;

        protected ServerMessageDispatcher MessageDispatch = new ServerMessageDispatcher();

        public RestrictedAccessZone(ServerConfig cfg) : base(cfg)
        {
            MessageProcessor = SecurityJailMessageFacotry.Factory;

            MessageDispatch.Add(new MsgWantWHash(), HandleWantWorldHash);
            MessageDispatch.Add(new MsgEnter(), HandleEnter);
            MessageDispatch.Add(new MsgNegotiateFlags(), HandleNegotiateFlags);
        }

        public override void ProcessClientMessage(ServerPlayer player, NetworkMessage msg)
        {
            if (!MessageDispatch.DispatchMessage(player, msg))
            {
                Logger.Log1("Restricted Access Zone unhandled message " + msg.Code);
            }
        }

        protected void Promote(ServerPlayer sp)
        {
            RemovePlayer(sp);
            if (PromotePlayer != null)
                PromotePlayer.Invoke(this, sp);
        }

        private void HandleEnter(ServerPlayer player, NetworkMessage msg)
        {
            MsgEnter enter = msg as MsgEnter;
            if (enter == null)
                return;

            if (enter.Callsign == string.Empty || enter.Callsign.Length < 3)
            {
                player.SendMessage(new MsgReject(MsgReject.RejectionCodes.RejectBadCallsign, "Invalid callsign"));
                return;
            }


            player.Callsign = enter.Callsign;
            player.Motto = enter.Motto;
            player.Token = enter.Token;

            MsgAccept accept = new MsgAccept();
            accept.PlayerID = player.PlayerID;

            player.SendMessage(accept);
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
