using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using BZFlag.Networking.Messages;

namespace BZFlag.Networking
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

        protected UdpClient UDPHost = new UdpClient();

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
            UDPHost.Connect(IPAddress.Any, port);

            UDPHost.BeginReceive(ProcessUDPPackets, null);
        }

        public void Shutdown()
        {
            if (UDPHost != null)
                UDPHost.Close();
        }

        protected void ProcessUDPPackets(IAsyncResult result)
        {
            IPEndPoint ep = null;
            byte[] data = UDPHost.EndReceive(result, ref ep);

            UDPHost.BeginReceive(ProcessUDPPackets, null);

            if (AcceptableClients.ContainsKey(ep.Address))
                MsgBuffer.AddData(data, AcceptableClients[ep.Address]);
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
