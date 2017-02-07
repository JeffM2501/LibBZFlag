
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using BZFlag.Networking.Messages;
using BZFlag.Networking.Common;

namespace BZFlag.Game.Host.Players
{
	public class ServerPlayer : Peer
	{
		public int PlayerID = 0;
		public object Tag = 0;

		protected NetworkStream NetStream = null;

        public TCPConnectionManager.PendingClient ConnectionData = null;

        public ServerPlayer(TCPConnectionManager.PendingClient pc)
		{
            ConnectionData = pc;
            Link(ConnectionData.ClientConnection);
		}
	}
}
