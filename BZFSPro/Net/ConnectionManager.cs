using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using BZFlag.Game.Host;
using BZFlag.Networking;

namespace BZFSPro.Net
{
    internal partial class ConnectionManager
    {
        public int Port = 5154;
        public TcpListener ListenerV4 = null;
        public TcpListener ListenerV6 = null;

        public class Connection : EventArgs
        {
            public TcpClient ClientConnection = null;
            public NetworkStream NetStream = null;

            public bool Active = true;

            public bool DataRecieved = false;
            public bool ProtcolPassed = false;

            public byte[] PendingData = new byte[0];

            public string GetIPAsString()
            {
                if (ClientConnection == null)
                    return string.Empty;

                return ((IPEndPoint)ClientConnection.Client.RemoteEndPoint).Address.ToString();
            }

            public IPAddress GetIPAddress()
            {
                if (ClientConnection == null)
                    return IPAddress.None;

                return ((IPEndPoint)ClientConnection.Client.RemoteEndPoint).Address;
            }
        }

        protected List<Connection> PendingConnecitons = new List<Connection>();


        public event EventHandler<Connection> RawTCPConnected = null;
        public event EventHandler<Connection> RawTCPDisconnected = null;

        public event EventHandler<Connection> BZFSProtocolConnectionAccepted = null;

        public void Startup(int port)
        {
            Port = port;
            ListenerV4 = new TcpListener(IPAddress.Any, port);

            ListenerV4.Start();
            ListenerV4.BeginAcceptTcpClient(TCPClientAcceptedV4, null);

            try
            {
                ListenerV6 = new TcpListener(IPAddress.IPv6Any, port);

                ListenerV6.Start();
                ListenerV6.BeginAcceptTcpClient(TCPClientAcceptedV6, null);
            }
            catch (Exception)
            {
                ListenerV6 = null;
            }
        }

        public void Shutdown()
        {

        }

        protected void TCPClientAcceptedV4(IAsyncResult ar)
        {
            Connection c = new Connection();
            c.ClientConnection = ListenerV4.EndAcceptTcpClient(ar);
            c.NetStream = c.ClientConnection.GetStream();

            Logger.Log2("IPV4 Connection accepted from " + c.ClientConnection.Client.RemoteEndPoint.ToString());
            AcceptClient(c);

            ListenerV4.BeginAcceptTcpClient(TCPClientAcceptedV4, null);
        }

        protected void TCPClientAcceptedV6(IAsyncResult ar)
        {
            if (ListenerV6 == null)
                return;

            Connection c = new Connection();
            c.ClientConnection = ListenerV6.EndAcceptTcpClient(ar);
            c.NetStream = c.ClientConnection.GetStream();

            Logger.Log2("IPV6 Connection accepted from " + c.ClientConnection.Client.RemoteEndPoint.ToString());
            AcceptClient(c);

            ListenerV6.BeginAcceptTcpClient(TCPClientAcceptedV4, null);
        }

        protected void AcceptClient(Connection c)
        {
            Logger.Log2("TCP Connection pending from " + c.ClientConnection.Client.RemoteEndPoint.ToString());

            lock (PendingConnecitons)
                PendingConnecitons.Add(c);

            RawTCPConnected?.Invoke(this, c);
            ProcessPendingClients(); // try one cycle they often have data
        }

        protected void RemovePendingClient(Connection pc)
        {
            lock (PendingConnecitons)
                PendingConnecitons.Remove(pc);
        }

        protected void DisconnectPendingClient(Connection pc)
        {
            pc.Active = false;
            pc.ClientConnection.Close();
            RemovePendingClient(pc);

            RawTCPDisconnected?.Invoke(this,pc);
        }

        protected void HandleDataReceiveD(Connection client, byte[] buffer, int read)
        {
            if (read != buffer.Length || BZFSProtocolConnectionAccepted == null)
            {
                client.ProtcolPassed = false;
                DisconnectPendingClient(client);
                Logger.Log4("Disconnecting abnormal connection from " + client.ClientConnection.Client.RemoteEndPoint.ToString());
            }
            else
            {
                if (Encoding.ASCII.GetString(buffer) == Protocol.BZFSHailString)
                {
                    client.ProtcolPassed = true;
                    client.NetStream.Write(Protocol.DefaultBZFSVersion, 0, Protocol.DefaultBZFSVersion.Length);
                    client.NetStream.Flush();
                    Logger.Log4("BZFS header from " + client.ClientConnection.Client.RemoteEndPoint.ToString());

                    BZFSProtocolConnectionAccepted?.Invoke(this, client);
                }
            }
        }

        public void Update()
        {
            ProcessPendingClients();
        }

        protected void ProcessPendingClients()
        {
            Connection[] clients = null;

            lock (PendingConnecitons)
                clients = PendingConnecitons.ToArray();

            foreach (Connection c in clients)
            {
                int available = c.ClientConnection.Available;

                if (c.ClientConnection.Available > 0)
                {
                    if (!c.ProtcolPassed && c.ClientConnection.Available >= Protocol.BZFSHail.Length)
                    {
                        c.DataRecieved = true;
                        byte[] buffer = new byte[Protocol.BZFSHail.Length];

                        HandleDataReceiveD(c, buffer, c.NetStream.Read(buffer, 0, buffer.Length));
                    }
                }
            }
        }
    }
}
