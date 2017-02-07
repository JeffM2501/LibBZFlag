using BZFlag.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using BZFlag.Services;
using BZFlag.Game.Host.Processors;
using BZFlag.Game.Host.Players;
using BZFlag.Data.Players;

namespace BZFlag.Game.Host
{
    public class Server
    {
		public TCPConnectionManager TCPConnections = null;
        public UDPConnectionManager UDPConnections = new UDPConnectionManager();

        protected RestrictedAccessZone SecurityArea = new RestrictedAccessZone();
     //   protected PlayerProcessor AcceptedGamePlayers = new PlayerProcessor();

        public ServerConfig ConfigData = new ServerConfig();

        protected Dictionary<int,ServerPlayer> ConnectedPlayers = new Dictionary<int, ServerPlayer>();
        protected int LastPlayerID = 0;

		public PublicServer PubServer = new PublicServer();

        public Server( ServerConfig cfg )
        {
            Networking.Messages.NetworkMessage.IsOnServer = true;

            Logger.SetLogFilePath(cfg.LogFile);
			Logger.LogLevel = cfg.LogLevel;

            ConfigData = cfg;

			if (ConfigData.ListPublicly)
			{
				PubServer.Address = ConfigData.PublicHost;
				PubServer.Description = ConfigData.PublicTitle;
				PubServer.Name = ConfigData.PublicHost;
				PubServer.Port = ConfigData.Port;
				PubServer.Key = ConfigData.PublicListKey;
				PubServer.AdvertGroups = string.Join(",", ConfigData.PublicAdvertizeGroups.ToArray());

				PubServer.RequestCompleted += PubServer_RequestCompleted;
				PubServer.RequestErrored += PubServer_RequestErrored;
			}

            SecurityArea.PromotePlayer += SecurityArea_PromotePlayer;
        }

        private void PubServer_RequestErrored(object sender, EventArgs e)
		{
			Logger.Log1("Public List Failed: " + PubServer.LastError);
		}

		private void PubServer_RequestCompleted(object sender, EventArgs e)
		{
			Logger.Log3("Public List Update Complete");
		}

        protected int FindPlayerID()
        {
            LastPlayerID++;
            if (LastPlayerID > PlayerConstants.MaxUseablePlayerID)
                LastPlayerID = PlayerConstants.MinimumPlayerID;

            lock (ConnectedPlayers)
            {
                if (!ConnectedPlayers.ContainsKey(LastPlayerID))
                    return LastPlayerID;
                else
                {
                    for (int i = PlayerConstants.MinimumPlayerID; i <= PlayerConstants.MaxUseablePlayerID; i++)
                    {
                        if (!ConnectedPlayers.ContainsKey(i))
                        {
                            LastPlayerID = i;
                            return LastPlayerID;
                        }
                    }
                }
            }

            // we are full up
            LastPlayerID = 0;
            return -1;
        }

        protected virtual ServerPlayer AcceptTCPConnection(TCPConnectionManager.PendingClient client)
        {
            ServerPlayer p = NewPlayerRecord(client);
            p.PlayerID = FindPlayerID();
            if (p.PlayerID < 0)
                return null;

            lock (ConnectedPlayers)
                ConnectedPlayers.Add(p.PlayerID, p);

            return p;
        }

        protected virtual ServerPlayer NewPlayerRecord(TCPConnectionManager.PendingClient client) { return new ServerPlayer(client); }

        public void Listen()
        {
            int port = ConfigData.Port;

			Logger.Log1("Listening on port " + port.ToString());

            TCPConnections = new TCPConnectionManager(port, this);
            TCPConnections.BZFSProtocolConnectionAccepted += BZFSProtocolConnectionAccepted;

            UDPConnections = new UDPConnectionManager();

			if(ConfigData.ListPublicly)
			{
				PubServer.UpdateMasterServer();
				Logger.Log1("Listening on port " + port.ToString());
			}
        }

        public void Run()
        {
            Listen();

            while(true)
            {
                // do any monitoring we need done here....

                ServerPlayer[] sp = null;
                lock (ConnectedPlayers)
                    sp = ConnectedPlayers.Values.ToArray();

                // remove any deaded connections
                foreach(var p in sp)
                {
                    if (!p.Active)
                    {
                        lock (ConnectedPlayers)
                            ConnectedPlayers.Remove(p.PlayerID);
                    }
                }


                System.Threading.Thread.Sleep(100);
            }
        }

        protected virtual void BZFSProtocolConnectionAccepted(object sender, TCPConnectionManager.PendingClient e)
        {
            var player = AcceptTCPConnection(e);
            if (player == null) // could not make a player for some reason
            {
                e.ClientConnection.Client.Disconnect(false);
                return;
            }

            // send them into the restricted zone until they validate

            SecurityArea.AddPendingConnection(player);
        }

        private void SecurityArea_PromotePlayer(object sender, ServerPlayer e)
        {
            // they passed muster
        }
    }
}
