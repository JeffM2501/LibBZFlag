using BZFlag.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using BZFlag.Services;


namespace BZFlag.Game.Host
{
    public class Server
    {
		public TCPConnectionManager TCPConnections = null;
        public UDPConnectionManager UDPConnections = new UDPConnectionManager();

        protected List<PlayerProcessor> ConnectionHandlers = new List<PlayerProcessor>();

        public ServerConfig ConfigData = new ServerConfig();


		public PublicServer PubServer = new PublicServer();

        public virtual void AcceptTCPConnection (TcpClient client)
        {
            ServerPlayer p = NewPlayerRecord(client);
        }

        public virtual ServerPlayer NewPlayerRecord(TcpClient client) {  return new ServerPlayer(client);}


        public Server( ServerConfig cfg )
        {
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
        }

		private void PubServer_RequestErrored(object sender, EventArgs e)
		{
			Logger.Log1("Public List Failed: " + PubServer.LastError);
		}

		private void PubServer_RequestCompleted(object sender, EventArgs e)
		{
			Logger.Log3("Public List Update Complete");
		}

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


                System.Threading.Thread.Sleep(20);
            }
        }

        protected virtual void BZFSProtocolConnectionAccepted(object sender, TCPConnectionManager.PendingClient e)
        {
           
        }
    }
}
