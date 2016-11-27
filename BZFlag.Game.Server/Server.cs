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


        public virtual void AcceptTCPConnection (TcpClient client)
        {
            ServerPlayer p = NewPlayerRecord(client);
        }

        public virtual ServerPlayer NewPlayerRecord(TcpClient client) {  return new ServerPlayer(client);}


        public Server()
        {

        }

        public void Listen( int port )
        {
            TCPConnections = new TCPConnectionManager(port, this);
            TCPConnections.BZFSProtocolConnectionAccepted += BZFSProtocolConnectionAccepted;

            UDPConnections = new UDPConnectionManager();
        }

        protected virtual void BZFSProtocolConnectionAccepted(object sender, TCPConnectionManager.PendingClient e)
        {
           
        }
    }
}
