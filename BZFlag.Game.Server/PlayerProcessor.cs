using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using BZFlag.Networking.Messages;
using BZFlag.Networking;


using BZFlag.Game.Host.Players;
using BZFlag.Networking.Messages.BZFS.Player;
using BZFlag.Networking.Messages.BZFS.UDP;

namespace BZFlag.Game.Host
{
    public class PlayerProcessor
    {
        protected Thread WorkerThread = null;
        private List<ServerPlayer> Players = new List<ServerPlayer>();

        protected MessageManager MessageProcessor = null;

        public int SleepTime = 100;
        public static int MaxMessagesPerClientCycle = 10;

        protected ServerConfig Config = null;

        public PlayerProcessor(ServerConfig cfg)
        {
            Config = cfg;
        }

        public event EventHandler<ServerPlayer> PromotePlayer = null;
        protected void Promote(ServerPlayer sp)
        {
            RemovePlayer(sp);
            if (PromotePlayer != null)
                PromotePlayer.Invoke(this, sp);
        }

        public virtual void Setup()
        {

        }

        public void Shutdown()
        {
            if (WorkerThread != null)
                WorkerThread.Abort();

            WorkerThread = null;
        }

        protected virtual void PlayerAdded(ServerPlayer player)
        {

        }

        protected virtual void PlayerRemoved(ServerPlayer player)
        {

        }

        public void AddPendingConnection(ServerPlayer player)
        {
            lock (Players)
                Players.Add(player);

            player.Disconnected += Player_Disconnected;

            PlayerAdded(player);

            if (WorkerThread == null)
            {
                WorkerThread = new Thread(new ThreadStart(ProcessPendingPlayers));
                WorkerThread.Start();
            }
        }

        protected void RemovePlayer(ServerPlayer sp)
        {
            sp.Disconnected -= Player_Disconnected;
            PlayerRemoved(sp);
        }

        private void Player_Disconnected(object sender, Networking.Common.Peer e)
        {
            lock (Players)
                Players.Remove(e as ServerPlayer);
        }

        protected virtual void UpdatePlayer(ServerPlayer player)
        {

        }

        protected void ProcessPendingPlayers()
        {
            ServerPlayer[] locals = null;

            lock (Players)
                locals = Players.ToArray();

            while (locals.Length > 0)
            {
                foreach (ServerPlayer player in locals)
                {
                    int count = 0;
                    while (count < MaxMessagesPerClientCycle)
                    {
                        InboundMessageBuffer.CompletedMessage buffer = player.InboundTCP.GetMessage();
                        if (buffer == null)
                            break;

                        NetworkMessage msg = MessageProcessor.Unpack(buffer.ID, buffer.Data);
                        msg.Tag = player;
                        msg.FromUDP = false;

                        ProcessClientMessage(player, msg);

                        count++;
                    }

                    count = 0;
                    while (count < MaxMessagesPerClientCycle)
                    {
                        var msg = player.InboundMessageProcessor.Pop();
                        if (msg == null)
                            break;

                        ProcessClientMessage(player, msg);

                        count++;
                    }

                    UpdatePlayer(player);
                }
                Thread.Sleep(SleepTime);
                lock (Players)
                    locals = Players.ToArray();
            }

            WorkerThread = null;
        }

        // basic message dispatch
        protected ServerMessageDispatcher MessageDispatch = new ServerMessageDispatcher();

        protected void RegisterCommonHandlers()
        {
            MessageDispatch.Add(new MsgExit(), HandleExit);

            MessageDispatch.Add(new MsgUDPLinkRequest(), HandleUDPLinkRequest);
            MessageDispatch.Add(new MsgUDPLinkEstablished(), HandleUDPLinkEstablished);
        }

        public virtual void ProcessClientMessage(ServerPlayer player, NetworkMessage msg)
        {
            if (!MessageDispatch.DispatchMessage(player, msg))
                HandleUnknownMessage(player, msg);
        }

        protected virtual void HandleUnknownMessage(ServerPlayer player, NetworkMessage msg)
        {
            Logger.Log1(this.GetType().Name + " unhandled message " + msg.CodeAbreviation);
        }

        // common message handlers

        protected void HandleExit(ServerPlayer player, NetworkMessage msg)
        {
            MsgExit enter = msg as MsgExit;
            if (enter == null)
                return;

            RemovePlayer(player);
            player.Disconnect();
        }

        protected void HandleUDPLinkRequest(ServerPlayer player, NetworkMessage msg)
        {
            MsgUDPLinkRequest udp = msg as MsgUDPLinkRequest;
            if (udp == null || player.UDPStatus != ServerPlayer.UDPConenctionStatuses.Unknown)
                return;

            Logger.Log3("Player:" + player.PlayerID.ToString() + " Sent UDP Link Request ");

            player.SendMessage(true,new MsgUDPLinkEstablished());

            player.UDPStatus = ServerPlayer.UDPConenctionStatuses.RequestSent;
            player.SendMessage(false, udp);
        }

        protected void HandleUDPLinkEstablished(ServerPlayer player, NetworkMessage msg)
        {
            MsgUDPLinkEstablished udp = msg as MsgUDPLinkEstablished;
            if (udp == null)
                return;

            if (udp.FromUDP)
                Logger.Log3("Player:" + player.PlayerID.ToString() + " Sent UDP Link Established from UDP ");
            else
                Logger.Log3("Player:" + player.PlayerID.ToString() + " Sent UDP Link Established from TCP ");

            player.UDPStatus = ServerPlayer.UDPConenctionStatuses.Connected;
        }
    }
}
