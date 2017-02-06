using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using BZFlag.Networking.Messages;
using BZFlag.Networking;

namespace BZFlag.Game.Host
{
    public class UDPConnectionManager
    {
        public delegate void ProcessUDPMessage(NetworkMessage msg);

        protected class HandlerData
        {
            public ServerPlayer Player = null;
            public ProcessUDPMessage Handler = null;
            public MessageManager Unpacker = null;
        }

        protected Dictionary<IPAddress, HandlerData> AcceptableClients = new Dictionary<IPAddress, HandlerData>();
		public bool AllowAll = false;

		public class OutOfBandUDPEventArgs : EventArgs
		{
			public byte[] DataBuffer = null;
			public IPEndPoint Source = null;
		}

		public event EventHandler<OutOfBandUDPEventArgs> OutOfBandUDPMessage = null;

		protected UdpClient UDPHost = null;

        protected InboundMessageBuffer MsgBuffer = new InboundMessageBuffer(true);  // if we ever have to buffer across packets, then this is one per endpoint

        public UDPConnectionManager()
        {
            MsgBuffer.CompleteMessageRecived += MsgBuffer_CompleteMessageRecived;
        }

        public void AcceptMessages(IPAddress address, ServerPlayer player, ProcessUDPMessage handler, MessageManager unpacker)
        {
            HandlerData data = new HandlerData();
            data.Player = player;
            data.Handler = handler;
            data.Unpacker = unpacker;

            lock (AcceptableClients)
            {
                if (AcceptableClients.ContainsKey(address))
                    AcceptableClients[address] = data;
                else
                    AcceptableClients.Add(address, data);
            }
        }

        public void ClearMessageHandler(IPAddress address)
        {
            lock(AcceptableClients)
            {
                if (AcceptableClients.ContainsKey(address))
                    AcceptableClients.Remove(address);
            }
        }

        public void Listen(int port)
        {
			UDPHost = new UdpClient(port);
		//	UDPHost.Connect(port);


			StartUDPListen();

		}

		protected void StartUDPListen()
		{
			new Thread(new ThreadStart(ReceiveOne)).Start();
		}

		protected void ReceiveOne()
		{
			byte[] data = null;
			IPEndPoint source = new IPEndPoint(IPAddress.Any, 5154);
			try
			{
			
				data = UDPHost.Receive(ref source);
			}
			catch (System.Exception /*ex*/)
			{
				
			}
			
			StartUDPListen();
			ProcessUDPPackets(source,data);
		}

        public void Shutdown()
        {
            if (UDPHost != null)
                UDPHost.Close();
        }

        protected void ProcessUDPPackets(IPEndPoint ep, byte[] data)
        {
            if (AcceptableClients.ContainsKey(ep.Address))
                MsgBuffer.AddData(data, AcceptableClients[ep.Address]);
			else if (AllowAll && OutOfBandUDPMessage != null)
			{
				OutOfBandUDPEventArgs args = new OutOfBandUDPEventArgs();
				args.DataBuffer = data;
				args.Source = ep;
				OutOfBandUDPMessage.Invoke(this, args);
			}
        }

        private void MsgBuffer_CompleteMessageRecived(object sender, EventArgs e)
        {
            InboundMessageBuffer.CompletedMessage msg = MsgBuffer.GetMessage();

            while (msg != null)
            {
                msg.UDP = true;
                HandlerData data = msg.Tag as HandlerData;
                if (data == null)
                    continue;

                var unpacked = data.Unpacker.Unpack(msg.ID, msg.Data, true);
                unpacked.Tag = data.Player;
                data.Handler(unpacked);
            }
        }
    }
}
