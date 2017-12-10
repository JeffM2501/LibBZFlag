using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using BZFlag.Networking.Messages;
using BZFlag.Networking;


using BZFlag.Game.Host.Players;
using BZFlag.Networking.Messages.BZFS.Player;

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
        }

        private void Player_Disconnected(object sender, Networking.Common.Peer e)
        {
            lock (Players)
                Players.Remove(e as ServerPlayer);
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
    }
}
