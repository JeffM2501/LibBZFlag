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

        public MessageManager MessageFactory = ServerMessageFactory.Factory;

        public OutboundMessageBuffer OutboundTCP = new OutboundMessageBuffer();
        public OutboundMessageBuffer OutboundUDP = new OutboundMessageBuffer();

        public string ConnectionError = string.Empty;

        protected TcpClient TCP = null;

        public IPEndPoint UDPEndpoint = null;

        public delegate void WriteUDPFunction(byte[] buffer, IPEndPoint address);

        public WriteUDPFunction WriteUDP = null;

        private static readonly bool RaiseDataMessages = false;

        public int MaxMessagesPerCycle = 20;

        public bool ReturnPingsEarly = false;

        protected string HostName = string.Empty;
        protected int HostPort = -1;

        public event EventHandler<Peer> Disconnected = null;

        public bool Active { get; private set; }

        public void Link(TcpClient client)
        {
            Active = true;
            TCP = client;
            Connected = true;

            if (InboundTCP == null)
            {
                InboundTCP = new InboundMessageBuffer(false);
                InboundUDP = new InboundMessageBuffer(true);
            }
        }

        public string GetTCPRemoteAddresString()
        {
            return TCP != null ? TCP.Client.RemoteEndPoint.ToString() : string.Empty;
        }

        IPEndPoint RemoteUDPEndpoint = null;

        public void Shutdown()
        {
            HostName = string.Empty;
            HostPort = -1;
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

        /// <summary>
        /// sends a message RIGHT NOW
        /// </summary>
        /// <param name="viaTCP"></param>
        /// <param name="msg"></param>
        public void SendDirectMessage(bool viaTCP, byte[] msg)
        {
            if (viaTCP)
            {
                var st = TCP.GetStream();
                if (st != null && st.CanWrite)
                {
                    st.Write(msg, 0, msg.Length);
                    st.Flush();
                }
                else
                    OutboundTCP.PushDirectMessage(msg);
            }
            else
                WriteUDP?.Invoke(msg, UDPEndpoint);
        }

        public event EventHandler TCPConnected = null;
        public event EventHandler HostHasData = null;
        public event EventHandler TCPHostDisconnect = null;
        public event EventHandler HostIsNotBZFS = null;

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

        public void Update()
        {
            PackAll();

            ProcessSockets();

            for (int i = 0; i < MaxMessagesPerCycle; i++)
            {
                var buffer = InboundTCP.GetMessage();
                if (buffer == null)
                    break;
                ProcessMessage(buffer);
            }

            for (int i = 0; i < MaxMessagesPerCycle; i++)
            {
                var buffer = InboundUDP.GetMessage();
                if (buffer == null)
                    break;
                ProcessMessage(buffer);
            }
        }

        protected void PackAll()
        {
            OutboundTCP.Process();
            OutboundUDP.Process();
        }

        protected void ProcessMessage(InboundMessageBuffer.CompletedMessage buffer)
        {
            NetworkMessage msg = MessageFactory.Unpack(buffer.ID, buffer.Data);
            if (msg != null)
            {
                msg.FromUDP = buffer.UDP;

                if (MessageReceived != null)
                    MessageReceived.Invoke(this, new MessageReceivedEventArgs(this, msg));
            }
        }

        // TCP polling local data
        protected bool Connected = false;
        private string HostProtoVersion = string.Empty;

        public void FlushTCP()
        {
            if (TCP == null)
                return;
            OutboundTCP.Process();

            var stream = TCP.GetStream();
            if (stream.CanWrite)
            {
                lock (OutboundTCP)
                {
                    byte[] outbound = OutboundTCP.Pop();
                    while (stream.CanWrite && outbound != null)
                    {
                        try
                        {
                            stream.Write(outbound, 0, outbound.Length);
                            stream.Flush();
                        }
                        catch (Exception)
                        {
                            outbound = null;
                        }
                        outbound = OutboundTCP.Pop();
                    }
                }
            }
        }

        protected virtual void ProcessSockets()
        {
            var stream = TCP.GetStream();
            if (stream.CanWrite)
            {
                byte[] outbound = null;

                lock (OutboundTCP)
                    outbound = OutboundTCP.Pop();

                for(int i = 0; i < 10; i++)
                {
                    try
                    {
                        if (outbound.Length < 4)
                            stream.Write(outbound, 0, outbound.Length);
                        else
                            stream.Write(outbound, 0, outbound.Length);
                        lock (OutboundTCP)
                            outbound = OutboundTCP.Pop();
                    }
                    catch (Exception)
                    {

                        outbound = null;
                    }

                }
                stream.Flush();
            }

            int udpCount = 0;
            while (udpCount < 10)
            {
                byte[] outbound = OutboundUDP.Pop();
                if (outbound == null)
                    break;

                if (WriteUDP != null)
                    WriteUDP(outbound, UDPEndpoint);
            }

            if (!Connected)
            {
                if (TCP.Available >= 9)
                {
                    byte[] header = new byte[8];
                    if (stream.Read(header, 0, 8) != 8)
                    {
                        HostIsNotBZFS?.Invoke(this, EventArgs.Empty);
                        Disconnect();
                        return;
                    }
                    HostProtoVersion = Encoding.ASCII.GetString(header);
                    if (HostProtoVersion.Substring(0, 4) != "BZFS")
                    {
                        HostIsNotBZFS?.Invoke(this, EventArgs.Empty);
                        Disconnect();
                        return;
                    }

                    Connected = true;
                    TCPConnected?.Invoke(this, EventArgs.Empty);
                    int b = stream.ReadByte();

                }
            }

            if (Connected && TCP.Available > 0)
            {
                byte[] data = new byte[TCP.Available];
                int read = stream.Read(data, 0, data.Length);

                InboundTCP.AddData(data);
            }
            else if (Connected)
            {
                if (!TCP.Connected)
                {
                    Connected = false;
                    TCPHostDisconnect?.Invoke(this, EventArgs.Empty);
                    {
                        Disconnect();
                        Active = false;
                        return;
                    }
                }
            }

        }

        public virtual void Disconnect()
        {
            Disconnected?.Invoke(this, this);

            Shutdown();
        }
    }
}