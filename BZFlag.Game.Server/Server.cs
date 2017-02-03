using BZFlag.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;


namespace BZFlag.Game.Host
{
    public class Server
    {
		public TCPConnectionManager TCPConnections = null;
        public UDPConnectionManager UDPConnections = new UDPConnectionManager();

        protected List<PlayerProcessor> ConnectionHandlers = new List<PlayerProcessor>();

        public ServerConfig ConfigData = new ServerConfig();

        public virtual void AcceptTCPConnection (TcpClient client)
        {
            ServerPlayer p = NewPlayerRecord(client);
        }

        public virtual ServerPlayer NewPlayerRecord(TcpClient client) {  return new ServerPlayer(client);}


        public Server( ServerConfig cfg )
        {
            ConfigData = cfg;
        }

        public void Listen()
        {
            int port = ConfigData.Port;

            TCPConnections = new TCPConnectionManager(port, this);
            TCPConnections.BZFSProtocolConnectionAccepted += BZFSProtocolConnectionAccepted;

            UDPConnections = new UDPConnectionManager();
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
