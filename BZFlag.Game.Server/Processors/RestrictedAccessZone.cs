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
    public class RestrictedAccessZone : PlayerProcessor
    {
        public BanList Bans = new BanList();

        public RestrictedAccessZone(ServerConfig cfg) : base(cfg)
        {
            MessageProcessor = SecurityJailMessageFacotry.Factory;

            RegisterCommonHandlers();

            MessageDispatch.Add(new MsgWantWHash(), HandleWantWorldHash);
            MessageDispatch.Add(new MsgEnter(), HandleEnter);
            MessageDispatch.Add(new MsgNegotiateFlags(), HandleNegotiateFlags);
        }

        protected override void HandleUnknownMessage(ServerPlayer player, NetworkMessage msg)
        {
            Logger.Log1("Restricted Access Zone unhandled message " + msg.Code);
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

            if (player.Token == string.Empty)
            {
                player.AuthStatus = ServerPlayer.AuthStatuses.NoneProvided;
                if (!Config.AllowAnonUsers)
                    player.SendMessage(new MsgReject(MsgReject.RejectionCodes.RejectBadCallsign, "Registered Users Only"));
                else
                    SendAccept(player);
            }
            else
            {
                player.AuthStatus = ServerPlayer.AuthStatuses.InProgress;

                ClientTokenCheck checker = new ClientTokenCheck();
                checker.Tag = player;
                checker.RequestCompleted += this.Checker_RequestCompleted;
                checker.RequestErrored += this.Checker_RequestErrored;

                checker.CheckToken(player.Callsign, player.Token, player.ConnectionData.GetIPAsString(), Config.SecurityGroups);
            }
        }

        private void Checker_RequestErrored(object sender, EventArgs e)
        {
            ClientTokenCheck checker = sender as ClientTokenCheck;
            if (checker == null || checker.Tag as ServerPlayer == null)
                return;

            ServerPlayer player = checker.Tag as ServerPlayer;

            player.AuthStatus = ServerPlayer.AuthStatuses.Failed;
            if (!Config.AllowAnonUsers)
                player.SendMessage(new MsgReject(MsgReject.RejectionCodes.RejectBadCallsign, "Authentication Failed"));
            else
                SendAccept(player);
        }

        private void Checker_RequestCompleted(object sender, EventArgs e)
        {
            ClientTokenCheck checker = sender as ClientTokenCheck;
            if (checker == null || checker.Tag as ServerPlayer == null)
                return;

            ServerPlayer player = checker.Tag as ServerPlayer;

            player.AuthStatus = ServerPlayer.AuthStatuses.Valid;
            player.BZID = checker.BZID;
            player.GroupMemberships = checker.Groups;

            var ban = Bans.FindIDBan(checker.BZID);

            if (ban != null)
                player.SendMessage(new MsgReject(MsgReject.RejectionCodes.RejectIDBanned, ban.Reason));
            else
                SendAccept(player);
        }

        private void SendAccept(ServerPlayer player)
        {
            MsgAccept accept = new MsgAccept();
            accept.PlayerID = player.PlayerID;

            player.SendMessage(accept);

            Promote(player);
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
