using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using BZFlag.Networking.Messages;
using BZFlag.Networking;


using BZFlag.Game.Host.Players;
using BZFlag.Networking.Messages.BZFS.UDP;
using System.Text;
using BZFlag.Data.Utils;
using static BZFlag.Networking.InboundMessageBuffer;

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

        protected UdpClient UDPSocket = null;
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
            UDPSocket = new UdpClient(UDPInPort);

            UDPReceiveThread = new Thread(new ThreadStart(Receive));
            UDPReceiveThread.Start();
        }

        private Thread UDPReceiveThread = null;

        protected void Receive()
        {
            while(true)
            {
                IPEndPoint source = null;

                byte[] data = UDPSocket.Receive(ref source);

                if (data != null && data.Length > 0 && source != null)
                    ProcessUDPPackets(source, data);
            }
        }

        public void Shutdown()
        {
            try
            {
                if (UDPReceiveThread != null)
                    UDPReceiveThread.Abort();
                UDPReceiveThread = null;

                if (UDPSocket != null)
                {
                    UDPSocket.Close();
                    UDPSocket.Dispose();
                }
            }
            catch (Exception)
            {
            }
            UDPSocket = null;
        }

        protected void ProcessUDPPackets(IPEndPoint ep, byte[] data)
        {

            if (data.Length < 4)
                return;

            int len = BufferUtils.ReadUInt16(data, 0);
            int code = BufferUtils.ReadUInt16(data, 2);

            CompletedMessage msg = new CompletedMessage();
            msg.ID = code;
            msg.Size = len;

            if (data.Length < len + 4)
                return;

            msg.Tag = ep;
            msg.Data = new byte[len];
            Array.Copy(data, 4, msg.Data, 0, len);

            string msgCode = Encoding.ASCII.GetString(data, 2, 2);
        
            if (AcceptableClients.ContainsKey(ep.Address))
            {
                CompleteMessageRecived(msg);
            }
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
            if (UDPSocket != null)
            {
            //    UDPSocket.Connect(address);
              int sent = UDPSocket.Send(buffer, buffer.Length, address);
            }
        }

        private void CompleteMessageRecived(CompletedMessage msg)
        {
           // while (msg != null || msg.Tag as IPEndPoint == null)
            {
                msg.UDP = true;

                IPEndPoint clientAddress = msg.Tag as IPEndPoint;

                NetworkMessage unpacked = null;

                if(AcceptableMessages != null)
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
}
