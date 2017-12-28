using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

using BZFlag.Networking;
using BZFlag.Networking.Messages;
using BZFlag.Game.Host.Players;
using BZFlag.Networking.Messages.BZFS.UDP;
using BZFlag.Data.Utils;

namespace BZFlag.Game.Host
{
    public class UDPConnectionManager
    {
        protected Dictionary<IPAddress, List<ServerPlayer>> AcceptableClients = new Dictionary<IPAddress, List<ServerPlayer>>();
        public bool AllowAll = false;

        public class OutOfBandUDPEventArgs : EventArgs
        {
            public byte[] DataBuffer = null;
            public IPEndPoint Source = null;
        }

        public event EventHandler<OutOfBandUDPEventArgs> OutOfBandUDPMessage = null;

        protected UdpClient UDPSocketV4 = null;
        protected UdpClient UDPSocketV6 = null;
        protected int UDPInPort = 5154;

        protected MessageManager AcceptableMessages = null;

        public UDPConnectionManager(MessageManager unpacker)
        {
            AcceptableMessages = unpacker;
        }

        public void AddAcceptalbePlayer(IPAddress address, ServerPlayer player)
        {
            lock (AcceptableClients)
            {
                if (!AcceptableClients.ContainsKey(address))
                    AcceptableClients.Add(address, new List<ServerPlayer>());
                AcceptableClients[address].Add(player);
            }
        }

        public void RemoveAcceptablePlayer(IPAddress address, ServerPlayer player)
        {
            lock (AcceptableClients)
            {
                if (AcceptableClients.ContainsKey(address))
                {
                    AcceptableClients[address].Remove(player);
                    if (AcceptableClients[address].Count == 0 )
                        AcceptableClients.Remove(address);
                }
            }
        }

        public void Listen(int port)
        {
            UDPInPort = port;
            UDPSocketV4 = new UdpClient(UDPInPort,AddressFamily.InterNetwork);
            UDPReceiveThreadV4 = new Thread(new ThreadStart(ReceiveV4));
            UDPReceiveThreadV4.Start();

            try
            {
                UDPSocketV6 = new UdpClient(UDPInPort, AddressFamily.InterNetworkV6);
                UDPReceiveThreadV6 = new Thread(new ThreadStart(ReceiveV6));
                UDPReceiveThreadV6.Start();
            }
            catch (Exception)
            {

                UDPSocketV6 = null;
                UDPReceiveThreadV6 = null;
            }
        }

        private Thread UDPReceiveThreadV4 = null;
        private Thread UDPReceiveThreadV6 = null;

        protected void ReceiveV4()
        {
            while(true)
            {
                IPEndPoint source = null;

                byte[] data = UDPSocketV4.Receive(ref source);

                if (data != null && data.Length > 0 && source != null)
                    ProcessUDPPackets(source, data);
            }
        }

        protected void ReceiveV6()
        {
            while (true)
            {
                IPEndPoint source = null;

                byte[] data = UDPSocketV6.Receive(ref source);

                if (data != null && data.Length > 0 && source != null)
                    ProcessUDPPackets(source, data);
            }
        }

        public void Shutdown()
        {
            try
            {
                if (UDPReceiveThreadV4 != null)
                    UDPReceiveThreadV4.Abort();
                UDPReceiveThreadV4 = null;

                if (UDPReceiveThreadV6 != null)
                    UDPReceiveThreadV6.Abort();
                UDPReceiveThreadV6 = null;

                if (UDPSocketV4 != null)
                {
                    UDPSocketV4.Close();
                    UDPSocketV4.Dispose();
                }

                if (UDPSocketV6 != null)
                {
                    UDPSocketV6.Close();
                    UDPSocketV6.Dispose();
                }
            }
            catch (Exception)
            {
            }

            UDPSocketV4 = null;
            UDPSocketV6 = null;
        }

        protected void ProcessUDPPackets(IPEndPoint ep, byte[] data)
        {
            if (data.Length < 4)
                return;

            int len = BufferUtils.ReadUInt16(data, 0);
            int code = BufferUtils.ReadUInt16(data, 2);

            InboundMessageBuffer.CompletedMessage msg = new InboundMessageBuffer.CompletedMessage();
            msg.ID = code;
            msg.Size = len;

            if (data.Length < len + 4)
                return;

            msg.Tag = ep;
            msg.Data = new byte[len];
            Array.Copy(data, 4, msg.Data, 0, len);

            string msgCode = Encoding.ASCII.GetString(data, 2, 2);
        
            if (AcceptableClients.ContainsKey(ep.Address))
                CompleteMessageRecived(msg);
            else if (AllowAll && OutOfBandUDPMessage != null)
            {
                OutOfBandUDPEventArgs args = new OutOfBandUDPEventArgs();
                args.DataBuffer = data;
                args.Source = ep;
                OutOfBandUDPMessage.Invoke(this, args);
            }
        }

        private ServerPlayer GetPlayerForAddress(MsgUDPLinkRequest req, IPEndPoint ep)
        {
            if (!AcceptableClients.ContainsKey(ep.Address))
                return null;

            foreach (var p in AcceptableClients[ep.Address])
            {
                if (req != null)
                {
                    if (p.PlayerID == req.PlayerID)
                    {
                        p.UDPEndpoint = ep;
                        return p;
                    }
                }
                else if (p.UDPEndpoint.Port == ep.Port)
                    return p;
              
            }
            return null;
        }

        public void WriteUDP(byte[] buffer, IPEndPoint address)
        {
            if (UDPSocketV4 != null && address.AddressFamily == AddressFamily.InterNetwork)
                UDPSocketV4.Send(buffer, buffer.Length, address);
            else if (UDPSocketV6 != null && address.AddressFamily == AddressFamily.InterNetworkV6)
                UDPSocketV6.Send(buffer, buffer.Length, address);
        }

        private void CompleteMessageRecived(InboundMessageBuffer.CompletedMessage msg)
        {
           msg.UDP = true;

            IPEndPoint clientAddress = msg.Tag as IPEndPoint;

            NetworkMessage unpacked = null;

            if (AcceptableMessages != null)
                unpacked = AcceptableMessages.Unpack(msg.ID, msg.Data, true);

            if (unpacked == null)
            {
                Logger.Log3("Unknown UDP Packet " + Encoding.ASCII.GetString(msg.Data));
                return;
            }

            ServerPlayer player = GetPlayerForAddress(unpacked as MsgUDPLinkRequest, clientAddress);
            if (player != null)
                player.ProcessUDPMessage(unpacked);
            else
                Logger.Log3("Unknown UDP Player Msg" + unpacked.CodeAbreviation);
        }
    }
}
