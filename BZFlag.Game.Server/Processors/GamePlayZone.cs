using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BZFlag.Networking.Messages;

using BZFlag.Game.Host.Players;
using BZFlag.Networking.Messages.BZFS;
using BZFlag.Data.Teams;
using BZFlag.Networking.Messages.BZFS.Control;

namespace BZFlag.Game.Host.Processors
{
    internal class GamePlayZone : PlayerProcessor
    {
        public Server ServerHost = null;

        public Dictionary<TeamColors, List<ServerPlayer>> Teams = new Dictionary<TeamColors, List<ServerPlayer>>();

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

            lock (Teams)
            {
                if (Teams.ContainsKey(player.ActualTeam))
                    Teams[player.ActualTeam].Remove(player);
            }

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

            ServerHost.PreAddPlayer(player);

            if (player.ActualTeam == TeamColors.NoTeam)
            {
                SendReject(player, MsgReject.RejectionCodes.RejectTeamFull, "The team " + player.DesiredTeam.ToString() + " is full");
                return;
            }

            Logger.Log2("Player " + player.Callsign + " assigned to team " + player.ActualTeam.ToString());

            lock(Teams)
            {
                if (!Teams.ContainsKey(player.ActualTeam))
                    Teams.Add(player.ActualTeam, new List<ServerPlayer>());

                Teams[player.ActualTeam].Add(player);
            }
       
            // tell everyone they joined

            player.NeedStartupInfo = false;

            ServerHost.PostAddPlayer(player);
            UpdatePublicListServer?.Invoke(this, EventArgs.Empty);
        }

        private void HandleChatMessage(ServerPlayer player, NetworkMessage msg)
        {
       
        }

        private void BZDatabase_ValueChanged(object sender, Data.BZDB.Database.DatabaseItem e)
        {
  
        }

        public int GetTeamPlayerCount(TeamColors team)
        {
            lock (Teams)
            {
                if (Teams.ContainsKey(team))
                    return Teams[team].Count;
                return 0;
            }
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
