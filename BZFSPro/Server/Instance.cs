using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using BZFlag.Game.Host;
using BZFlag.Game.Host.Players;
using BZFSPro.Net;

namespace BZFSPro.Server
{
    internal class Instance
    {
        private bool IsDone = false;
        public bool Done()
        {
            lock (this)
                return IsDone;
        }

        private int ActiveConenctions = 0;
        public void AddActiveConnection()
        {
            lock (this)
                ActiveConenctions++;
        }

        protected void RemoveActiveConnection()
        {
            lock (this)
            {
                ActiveConenctions--;
                if (ActiveConenctions < 0)
                    ActiveConenctions = 0;
            }
        }

        public bool HasActiveConnections()
        {
            lock (this)
                return ActiveConenctions > 0;
        }

        protected Dictionary<int, ConnectionManager> ListeningPorts = new Dictionary<int, ConnectionManager>();

        public GameState State = new GameState();

        public UntrustedArea Jail = new UntrustedArea();

        public void Run( ServerConfig config )
        {
            // startup
            State.Init(config);
            Startup();

            // run
            while (!Done())
            {
                CheckNewConnections();

                if (HasActiveConnections())
                    Thread.Sleep(25);
                else
                    Thread.Sleep(1000);
            }

            // cleanup
        }

        protected void Startup()
        {
            SetupListeners();
            Jail.ReleasePlayer += Jail_ReleasePlayer;
            Jail.Setup(State);
        }

        private void Jail_ReleasePlayer(object sender, BZFlag.Game.Host.Players.ServerPlayer e)
        {
            throw new NotImplementedException();
        }

        private void ListenPort_BZFSProtocolConnectionAccepted(object sender, ConnectionManager.Connection e)
        {
            ConnectionManager port = sender as ConnectionManager;
            if (port == null)
                return;

            ServerPlayer player = new ServerPlayer(e.ClientConnection);
        }

        private void ListenPort_RawTCPConnected(object sender, ConnectionManager.Connection e)
        {
            AddActiveConnection();
        }

        private void ListenPort_RawTCPDisconnected(object sender, ConnectionManager.Connection e)
        {
            RemoveActiveConnection();
        }

        protected void SetupListeners()
        {
            ConnectionManager listenPort = new ConnectionManager();
            listenPort.BZFSProtocolConnectionAccepted += ListenPort_BZFSProtocolConnectionAccepted;
            listenPort.RawTCPConnected += ListenPort_RawTCPConnected;
            listenPort.RawTCPDisconnected += ListenPort_RawTCPDisconnected;
            listenPort.Startup(State.ConfigData.Port);
            ListeningPorts.Add(listenPort.Port, listenPort);
        }

        protected void CheckNewConnections()
        {
            foreach (ConnectionManager port in ListeningPorts.Values)
                port.Update(); // checks for new connections and dispatches them
        }
    }
}
