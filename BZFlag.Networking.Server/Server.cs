using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;


namespace BZFlag.Networking
{
    public class Server
    {
		public TCPConnectionManager InboundConnections = null;


        public virtual void AcceptTCPConnection (TcpClient client)
        {
            ServerPlayer p = NewPlayerRecord(client);
        }

        public virtual ServerPlayer NewPlayerRecord(TcpClient client) {  return new ServerPlayer(client);}
	}
}
