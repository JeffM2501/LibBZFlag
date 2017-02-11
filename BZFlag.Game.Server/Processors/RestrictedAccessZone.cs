using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BZFlag.Networking.Messages;

using BZFlag.Game.Host.Players;
using BZFlag.Networking;
using BZFlag.Networking.Messages.BZFS.World;
using BZFlag.Networking.Messages.BZFS.Player;

namespace BZFlag.Game.Host.Processors
{
    public class RestrictedAccessZone : PlayerProcessor
    {
        public event EventHandler<ServerPlayer> PromotePlayer = null;

        protected ServerMessageDispatcher MessageDispatch = new ServerMessageDispatcher();

        public RestrictedAccessZone()
        {
            MessageProcessor = SecurityJailMessageFacotry.Factory;

            MessageDispatch.Add(new MsgWantWHash(), HandleWantWorldHash);
            MessageDispatch.Add(new MsgEnter(), HandleEnter);
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
                PromotePlayer.Invoke(this,sp);
        }
        private void HandleEnter(ServerPlayer player, NetworkMessage msg)
        {
            MsgEnter enter = msg as MsgEnter;
            if (enter == null)
                return;

            hash.WorldHash = "NOPE!";
            player.SendMessage(hash);
        }

        private void HandleWantWorldHash(ServerPlayer player, NetworkMessage msg)
        {
            MsgWantWHash hash = msg as MsgWantWHash;
            if (hash == null)
                return;

            hash.WorldHash = "NOPE!";
            player.SendMessage(hash);
        }
    }
}
