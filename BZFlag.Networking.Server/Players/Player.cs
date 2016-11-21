
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using BZFlag.Networking.Messages;

namespace BZFlag.Networking
{
	public class ServerPlayer
	{
		public int PlayerID = 0;
		public object Tag = 0;

		public TcpClient ClientConnection = null;

		protected NetworkStream NetStream = null;

		private List<NetworkMessage> InboundMessges = new List<NetworkMessage>();
		private List<NetworkMessage> OutboundMessages = new List<NetworkMessage>();

		public InboundMessageBuffer InboundTCP = new InboundMessageBuffer(false);
		public InboundMessageBuffer InboundUDP = new InboundMessageBuffer(true);


		public ServerPlayer(TcpClient client)
		{
			ClientConnection = client;
		}

		public void ProcessTCP()
		{
			if(ClientConnection == null)
				return;

			if(NetStream == null)
				NetStream = ClientConnection.GetStream();

			if(ClientConnection.Available == 0)
				return;

			byte[] b = new byte[ClientConnection.Available];
			NetStream.Read(b, 0, b.Length);

			InboundTCP.AddData(b);
		}

		public void PushInboundMessage(NetworkMessage msg)
		{
			lock(InboundMessges)
				InboundMessges.Add(msg);
		}
	}
}
