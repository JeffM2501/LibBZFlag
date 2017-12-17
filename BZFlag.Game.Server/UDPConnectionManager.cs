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

        protected UdpClient UDPHost = null;

        protected InboundMessageBuffer MsgBuffer = new InboundMessageBuffer(true);  // if we ever have to buffer across packets, then this is one per endpoint

        protected MessageManager AcceptableMessages = null;

        public UDPConnectionManager(MessageManager unpacker)
        {
            MsgBuffer.CompleteMessageRecived += MsgBuffer_CompleteMessageRecived;
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
            UDPHost = new UdpClient(port);

            StartUDPListen();
        }

        protected Thread UDPListenThred = null;

        protected void StartUDPListen()
        {
            UDPListenThred = new Thread(new ThreadStart(Receive));
            UDPListenThred.Start();
        }

        protected void Receive()
        {
            while (true)
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

                ProcessUDPPackets(source, data);
            }
            UDPListenThred = null;

        }

        public void Shutdown()
        {
            if (UDPListenThred != null)
                UDPListenThred.Abort();

            UDPListenThred = null;

            if (UDPHost != null)
                UDPHost.Close();
        }

        protected void ProcessUDPPackets(IPEndPoint ep, byte[] data)
        {
            if (AcceptableClients.ContainsKey(ep.Address))
                MsgBuffer.AddData(data, ep);
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
                else if (p.UDPEndpoint == ep)
                    return p;
              
            }
            return null;
        }

        private void MsgBuffer_CompleteMessageRecived(object sender, EventArgs e)
        {
            InboundMessageBuffer.CompletedMessage msg = MsgBuffer.GetMessage();

            while (msg != null || msg.Tag as IPEndPoint == null)
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
