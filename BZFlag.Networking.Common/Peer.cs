using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;

using BZFlag.Networking.Messages;

namespace BZFlag.Networking.Common
{
    public class Peer : EventArgs
    {
        public InboundMessageBuffer InboundTCP = null;
        public InboundMessageBuffer InboundUDP = null;

        public MessageUnpacker InboundMessageProcessor = new MessageUnpacker();

        public OutboundMessageBuffer OutboundTCP = new OutboundMessageBuffer();
        public OutboundMessageBuffer OutboundUDP = new OutboundMessageBuffer();

        public string ConnectionError = string.Empty;

        protected TcpClient TCP = null;

        public delegate void WriteUDPFunction(byte[] buffer, IPEndPoint address);

        public WriteUDPFunction WriteUDP = null;

        /// <summary>
        /// Only used by client connections, server connections have to share a single UDP with all users and do prefi
        /// </summary>
        protected UdpClient UDP = null;

        protected bool SendUDPInTCPThread = false;
        public IPEndPoint UDPEndpoint = null;

        protected Thread TCPNetworkPollThread = null;
        protected Thread UDPNetworkPollThread = null;

        private static readonly bool RaiseDataMessages = false;

        public int MaxMessagesPerCycle = 20;

        public bool ReturnPingsEarly = false;

        protected string HostName = string.Empty;
        protected int HostPort = -1;

        public event EventHandler<Peer> Disconnected = null;

        public bool Active { get; private set; }

        public class MessageReceivedEventArgs : EventArgs
        {
            public Peer Recipient = null;
            public NetworkMessage Message = null;

            public MessageReceivedEventArgs(Peer p, NetworkMessage msg)
            {
                Recipient = p;
                Message = msg;
            }
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived = null;

        protected enum NetworkPushMessages
        {
            None,
            ConnectedTCP,
            ConnectedUDP,
            HostHasData,
            TCPHostDisconnect,
            HostIsNotBZFS,
        }

        private List<NetworkPushMessages> PendingNetworkNotifications = new List<NetworkPushMessages>();

        private void PushNetworkNotificatioin(NetworkPushMessages msg)
        {
            if (msg == NetworkPushMessages.None)
                return;

            lock (PendingNetworkNotifications)
                PendingNetworkNotifications.Add(msg);
        }

        protected NetworkPushMessages PopNetworkNotification()
        {
            NetworkPushMessages n = NetworkPushMessages.None;
            lock (PendingNetworkNotifications)
            {
                if (PendingNetworkNotifications.Count > 0)
                {
                    n = PendingNetworkNotifications[0];
                    PendingNetworkNotifications.RemoveAt(0);
                }
            }
            return n;
        }

        public virtual void AddInboundMessage(InboundMessageBuffer.CompletedMessage msg)
        {
            InboundMessageProcessor.Push(msg);
        }

        private void Inbound_CompleteMessageRecived(object sender, EventArgs e)
        {
            InboundMessageBuffer buffer = sender as InboundMessageBuffer;
            if (buffer == null)
                return;

            foreach (var m in buffer.GetMessages())
                AddInboundMessage(m);
        }

        public void Connect(string server, int port)
        {
            Shutdown();

            if (InboundTCP == null)
            {
                InboundTCP = new InboundMessageBuffer(false);
                InboundUDP = new InboundMessageBuffer(true);

                InboundTCP.CompleteMessageRecived += Inbound_CompleteMessageRecived;
                InboundUDP.CompleteMessageRecived += Inbound_CompleteMessageRecived;
            }

            OutboundTCP.Start();
            OutboundUDP.Start();

            OutboundTCP.PushDirectMessage(Protocol.BZFSHail);

            HostName = server;
            HostPort = port;

            ConnectionError = string.Empty;
            try
            {
                TCP = new TcpClient(server, port);
            }
            catch (Exception ex)
            {

                TCP = null;
                ConnectionError = ex.ToString();
                return;

            }

            InboundMessageProcessor.Start();

            TCPNetworkPollThread = new Thread(new ThreadStart(PollTCP));
            TCPNetworkPollThread.Start();
        }

        public void Link(TcpClient client)
        {
            Active = true;
            TCP = client;
            Connected = true;
            OutboundTCP.Start();

            SendUDPInTCPThread = true;

            OutboundUDP.Start();

            if (InboundTCP == null)
            {
                InboundTCP = new InboundMessageBuffer(false);
                InboundUDP = new InboundMessageBuffer(true);
            }

            InboundMessageProcessor.Start();

            TCPNetworkPollThread = new Thread(new ThreadStart(PollTCP));
            TCPNetworkPollThread.Start();
        }

        IPEndPoint RemoteUDPEndpoint = null;

        public void ConnectToUDP()
        {
            if (HostName == string.Empty || HostPort < 0)
                return;

            UDP = new UdpClient(HostName, HostPort);


            RemoteUDPEndpoint = TCP.Client.RemoteEndPoint as IPEndPoint;

            UDPNetworkPollThread = new Thread(new ThreadStart(PollUDP));
            UDPNetworkPollThread.Start();
        }

        public void Shutdown()
        {
            OutboundTCP.Stop();
            OutboundUDP.Stop();
            InboundMessageProcessor.Stop();

            if (TCPNetworkPollThread != null && TCPNetworkPollThread.IsAlive)
                TCPNetworkPollThread.Abort();
            if (UDPNetworkPollThread != null && UDPNetworkPollThread.IsAlive)
                UDPNetworkPollThread.Abort();

            TCPNetworkPollThread = null;
            UDPNetworkPollThread = null;

            if (UDP != null)
                UDP.Close();

            if (TCP != null)
                TCP.Close();
            TCP = null;
            UDP = null;

            HostName = string.Empty;
            HostPort = -1;

            PendingNetworkNotifications.Clear();
            if (InboundTCP != null)
                InboundTCP.Clear();

            if (InboundUDP != null)
                InboundUDP.Clear();

            OutboundTCP.Clear();
            OutboundUDP.Clear();

            Active = false;

        }

        public void SendMessage(NetworkMessage msg)
        {
            SendMessage(true, msg);
        }

        public void SendMessage(bool viaTCP, NetworkMessage msg)
        {
            if (viaTCP)
                OutboundTCP.Push(msg);
            else
                OutboundUDP.Push(msg);
        }

        public void SendDirectMessage(bool viaTCP, byte[] msg)
        {
            if (viaTCP)
                OutboundTCP.PushDirectMessage(msg);
            else
                OutboundUDP.PushDirectMessage(msg);
        }

        public event EventHandler TCPConnected = null;
        public event EventHandler HostHasData = null;
        public event EventHandler TCPHostDisconnect = null;
        public event EventHandler HostIsNotBZFS = null;

        public void Update()
        {
            NetworkPushMessages evtMsg = PopNetworkNotification();
            while (evtMsg != NetworkPushMessages.None)
            {
                switch (evtMsg)
                {
                    case NetworkPushMessages.ConnectedTCP:
                        if (TCPConnected != null)
                            TCPConnected.Invoke(this, EventArgs.Empty);
                        break;

                    case NetworkPushMessages.HostHasData:
                        if (HostHasData != null)
                            HostHasData.Invoke(this, EventArgs.Empty);
                        break;

                    case NetworkPushMessages.TCPHostDisconnect:
                        if (TCPHostDisconnect != null)
                            TCPHostDisconnect.Invoke(this, EventArgs.Empty);
                        break;

                    case NetworkPushMessages.HostIsNotBZFS:
                        if (HostIsNotBZFS != null)
                            HostIsNotBZFS.Invoke(this, EventArgs.Empty);
                        break;
                }
                evtMsg = PopNetworkNotification();
            }

            int count = 0;
            var msg = InboundMessageProcessor.Pop();
            while (msg != null)
            {
                if (MessageReceived != null)
                    MessageReceived.Invoke(this, new MessageReceivedEventArgs(this, msg));
                count++;
                if (count >= MaxMessagesPerCycle)
                    msg = null;
                else
                    msg = InboundMessageProcessor.Pop();
            }

        }

        // TCP polling local data
        protected bool Connected = false;
        private string HostProtoVersion = string.Empty;

        protected virtual void PollTCP()
        {
            var stream = TCP.GetStream();
            while (true)
            {
                if (stream.CanWrite && Connected)
                {
                    byte[] outbound = OutboundTCP.Pop();
                    while (stream.CanWrite && outbound != null)
                    {
                        try
                        {
                            stream.Write(outbound, 0, outbound.Length);
                            outbound = OutboundTCP.Pop();
                        }
                        catch (Exception)
                        {

                            outbound = null;
                        }
                        
                    }
                    stream.Flush();
                }

                if (SendUDPInTCPThread)
                {
                    int udpCount = 0;
                    while (udpCount < 10)
                    {
                        byte[] outbound = OutboundUDP.Pop();
                        if (outbound == null)
                            break;

                        if (WriteUDP != null)
                            WriteUDP(outbound, UDPEndpoint);
                        else
                            UDPSendingsocket.SendTo(outbound, UDPEndpoint);
                    }
                }
                
                if (!Connected)
                {
                    if (TCP.Available >= 9)
                    {
                        byte[] header = new byte[8];
                        if (stream.Read(header, 0, 8) != 8)
                        {
                            PushNetworkNotificatioin(NetworkPushMessages.HostIsNotBZFS);
                            return;
                        }
                        HostProtoVersion = Encoding.ASCII.GetString(header);
                        if (HostProtoVersion.Substring(0, 4) != "BZFS")
                        {
                            PushNetworkNotificatioin(NetworkPushMessages.HostIsNotBZFS);
                            return;
                        }

                        Connected = true;
                        PushNetworkNotificatioin(NetworkPushMessages.ConnectedTCP);
                        int b = stream.ReadByte();

                    }
                }

                if (Connected && TCP.Available > 0)
                {
                    byte[] data = new byte[TCP.Available];
                    int read = stream.Read(data, 0, data.Length);
                    InboundTCP.AddData(data);
                    if (RaiseDataMessages)
                        PushNetworkNotificatioin(NetworkPushMessages.HostHasData);
                }
                Thread.Sleep(10);
            }
        }


        protected Socket UDPSendingsocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        protected virtual void PollUDP()
        {
            while (true)
            {
                byte[] outbound = OutboundUDP.Pop();
                while (outbound != null)
                {

                    UDPSendingsocket.SendTo(outbound, RemoteUDPEndpoint);
                   // UDP.Send(outbound, outbound.Length);
                    outbound = OutboundUDP.Pop();
                }

                if (!Connected)
                {
                    if (UDP.Available >= 9)
                    {
                        IPEndPoint from = new IPEndPoint(IPAddress.Any, 0);
                        byte[] data = UDP.Receive(ref from);

                        InboundUDP.AddData(data);
                        if (RaiseDataMessages)
                            PushNetworkNotificatioin(NetworkPushMessages.HostHasData);
                    }
                }
                Thread.Sleep(10);
            }
        }

        public virtual void Disconnect()
        {
            if (Disconnected != null)
                Disconnected.Invoke(this, this);

            Shutdown();
        }
    }
}