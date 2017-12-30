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
using BZFlag.Networking.Messages.BZFS.Shots;
using System.Diagnostics;

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
            MessageProcessor = GameServerMessageFacotry.Factory;

            ServerHost.State.BZDatabase.ValueChanged += this.BZDatabase_ValueChanged;

            ServerHost.State.Flags.FlagAdded += Flags_FlagAdded;
            ServerHost.State.Flags.FlagRemoved += Flags_FlagRemoved;

            RegisterCommonHandlers();

            MessageDispatch.Add(new MsgMessage(), HandleChatMessage);
            MessageDispatch.Add(new MsgAlive(), HandleAlive);
            MessageDispatch.Add(new MsgPlayerUpdateSmall(), HandlePlayerUpdate);
            MessageDispatch.Add(new MsgPlayerUpdate(), HandlePlayerUpdate);

            MessageDispatch.Add(new MsgShotBegin(), HandleShotBegin);
            MessageDispatch.Add(new MsgShotEnd(), HandleShotEnd);
            MessageDispatch.Add(new MsgKilled(), HandleKilled);
        }

        protected override void HandleUnknownMessage(ServerPlayer player, NetworkMessage msg)
        {
            byte[] b = BitConverter.GetBytes((UInt16)msg.Code);
            Array.Reverse(b);
            string code = Encoding.ASCII.GetString(b);

            Logger.Log2("Unknown message in gameplay handler, " + code);
            base.HandleUnknownMessage(player, msg);
        }

        private void Flags_FlagRemoved(object sender, World.FlagManager.FlagInstance e)
        {
            throw new NotImplementedException();
        }

        private void Flags_FlagAdded(object sender, World.FlagManager.FlagInstance e)
        {
            throw new NotImplementedException();
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

        protected override void Update()
        {
            ServerHost.State.GameTime.Update();
            ServerHost.State.Flags.Update(ServerHost.State.GameTime);
            ServerHost.State.Shots.Update(ServerHost.State.GameTime);
            ServerHost.State.Players.Update(ServerHost.State.GameTime);
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

            ServerHost.State.Flags.SendInitialFlagUpdate(player);

            player.NeedStartupInfo = false;

            UpdatePublicListServer?.Invoke(this, EventArgs.Empty);
        }

        private void HandleShotBegin(ServerPlayer player, NetworkMessage msg)
        {
            ServerHost.State.Shots.HandleShotBegin(player, msg as MsgShotBegin);
        }

        private void HandleShotEnd(ServerPlayer player, NetworkMessage msg)
        {
            ServerHost.State.Shots.HandleShotEnd(player, msg as MsgShotEnd);
        }

        private void HandleKilled(ServerPlayer player, NetworkMessage msg)
        {
            // TODO, tell shots and players that someone died
            SendToAll(msg, msg.FromUDP);
        }

        private void HandleChatMessage(ServerPlayer player, NetworkMessage msg)
        {
            ServerHost.State.Chat.HandleChatMessage(player, msg as MsgMessage);
        }

        private void HandleAlive(ServerPlayer player, NetworkMessage msg)
        {
            if (!player.Allowances.AllowPlay)
                return;

            ServerHost.State.Players.StartSpawn(player, msg as MsgAlive);
        }

        private void HandlePlayerUpdate(ServerPlayer player, NetworkMessage msg)
        {
            ServerHost.State.Players.PlayerUpdate(player, msg as MsgPlayerUpdateBase);
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
