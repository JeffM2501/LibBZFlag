using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BZFlag.Networking.Messages;

using BZFlag.Game.Host.Players;
using BZFlag.Networking.Messages.BZFS;
using BZFlag.Networking.Messages.BZFS.Player;
using BZFlag.Data.Teams;
using BZFlag.Networking.Messages.BZFS.Control;

namespace BZFlag.Game.Host.Processors
{
    internal class GamePlayZone : PlayerProcessor
    {
        public Server ServerHost = null;

        public event EventHandler UpdatePublicListServer = null;
        public event EventHandler<ServerPlayer> PlayerRejected;

        public GamePlayZone(Server server) : base(server.ConfigData)
        {
            ServerHost = server;
            MessageProcessor = SecurityJailMessageFacotry.Factory;

            ServerHost.State.BZDatabase.ValueChanged += this.BZDatabase_ValueChanged;

            RegisterCommonHandlers();

            MessageDispatch.Add(new MsgMessage(), HandleChatMessage);
        }

        protected override void PlayerAdded(ServerPlayer player)
        {
            base.PlayerAdded(player);
        }

        protected override void PlayerRemoved(ServerPlayer player)
        {
            base.PlayerRemoved(player);

            ServerHost.State.Players.RemovePlayer(player);

            // tell everyone they went away

            ServerHost.RemovedPlayer(player);
            UpdatePublicListServer?.Invoke(this, EventArgs.Empty);
        }

        protected override void UpdatePlayer(ServerPlayer player)
        {
            base.UpdatePlayer(player);

            if (player.NeedStartupInfo)
                DoPlayerAdd(player);
        }

        protected void DoPlayerAdd(ServerPlayer player)
        {
            player.ActualTeam = ServerHost.GetPlayerTeam(player);

            if (!ServerHost.State.Players.AddPlayer(player))
            {
                SendReject(player, MsgReject.RejectionCodes.RejectTeamFull, "The team " + player.DesiredTeam.ToString() + " is full");
                return;
            }

            UpdatePublicListServer?.Invoke(this, EventArgs.Empty);
        }

        private void HandleChatMessage(ServerPlayer player, NetworkMessage msg)
        {
       
        }

        private void BZDatabase_ValueChanged(object sender, Data.BZDB.Database.DatabaseItem e)
        {
  
        }

        private void SendReject(ServerPlayer player, MsgReject.RejectionCodes code, string reason)
        {
            player.Accepted = false;
            player.RejectionReason = code.ToString() + " :" + reason;

            Logger.Log1("Reject Player " + player.PlayerID + " " + player.RejectionReason);
            player.SendMessage(new MsgReject(code, reason));

            PlayerRejected?.Invoke(this, player);

            player.Disconnect();
            RemovePlayer(player);
        }
    }
}
