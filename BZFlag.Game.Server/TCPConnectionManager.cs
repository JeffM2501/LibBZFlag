using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using BZFlag.Networking;

namespace BZFlag.Game.Host
{
    public class TCPConnectionManager
    {
        public int Port = 5154;
        public TcpListener ListenerV4 = null;
        public TcpListener ListenerV6 = null;


        public delegate bool BanCallback(PendingClient player, ref string reason);

        public BanCallback CheckIPBan;
        public BanCallback CheckHostBan;

        public class PendingClient : EventArgs
        {
            public TcpClient ClientConnection = null;
            public NetworkStream NetStream = null;

            public bool Active = true;

            public bool DNSStarted = false;
            public IPHostEntry HostEntry = null;

            public bool DNSPassed = false;

            public bool DataRecieved = false;
            public bool ProtcolPassed = false;
            public bool VersionPassed = false;

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

        protected List<PendingClient> PendingClients = new List<PendingClient>();

        public event EventHandler<PendingClient> BZFSProtocolConnectionAccepted = null;

        protected Thread WorkerThread = null;

        protected Server Host = null;

        public TCPConnectionManager(int port, Server server)
        {
            Host = server;
            Port = port;
            ListenerV4 = new TcpListener(IPAddress.Any, port);
            try
            {
                ListenerV6 = new TcpListener(IPAddress.IPv6Any, port);
            }
            catch (Exception)
            {
                ListenerV6 = null;
            }
        }

        public void StartUp()
        {
            ListenerV4.Start();
            ListenerV4.BeginAcceptTcpClient(TCPClientAcceptedV4, null);

            try
            {
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
            if (WorkerThread != null)
                WorkerThread.Abort();

            WorkerThread = null;

            ListenerV4.Stop();
            if (ListenerV6 != null)
                ListenerV6.Stop();
        }

        protected void TCPClientAcceptedV4(IAsyncResult ar)
        {
            PendingClient c = new PendingClient();
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

            PendingClient c = new PendingClient();
            c.ClientConnection = ListenerV6.EndAcceptTcpClient(ar);
            c.NetStream = c.ClientConnection.GetStream();

            Logger.Log2("IPV6 Connection accepted from " + c.ClientConnection.Client.RemoteEndPoint.ToString());
            AcceptClient(c);

            ListenerV6.BeginAcceptTcpClient(TCPClientAcceptedV4, null);
        }

        protected void AcceptClient(PendingClient c)
        {
            Logger.Log2("TCP Connection accepted from " + c.ClientConnection.Client.RemoteEndPoint.ToString());

            bool ban = false;
            string reason = string.Empty;
            if (CheckIPBan != null)
                ban = CheckIPBan(c, ref reason);

            if (ban)
            {
                c.ClientConnection.Close();
                return;
            }

            lock (PendingClients)
                PendingClients.Add(c);

            if (WorkerThread == null)
            {
                WorkerThread = new Thread(new ThreadStart(ProcessPendingClients));
                WorkerThread.Start();
            }
        }

        protected void DisconnectPendingClient(PendingClient pc)
        {
            pc.Active = false;
            pc.ClientConnection.Close();
            RemovePendingClient(pc);
        }

        protected void RemovePendingClient(PendingClient pc)
        {
            lock (PendingClients)
                PendingClients.Remove(pc);
        }

        protected void ProcessPendingClients()
        {
            PendingClient[] clients = null;

            lock (PendingClients)
                clients = PendingClients.ToArray();

            while (clients.Length > 0)
            {
                foreach (PendingClient c in clients)
                {
                    if (!c.DNSStarted)
                    {
                        var address = ((IPEndPoint)c.ClientConnection.Client.RemoteEndPoint).Address.ToString();
                        Logger.Log3("DNS Lookup started for " + address);
                        Dns.BeginGetHostEntry(address, DNSLookupCompleted, c);
                        c.DNSStarted = true;
                    }
                    else if (!c.DNSPassed)
                    {
                        if (c.HostEntry != null)
                        {
                            Logger.Log3("Ban check started for " + c.HostEntry.HostName);
                            // lookup the host in the ban list

                            bool ban = false;
                            string reason = string.Empty;
                            if (CheckHostBan != null)
                                ban = CheckHostBan(c, ref reason);

                            if (!ban)
                                c.DNSPassed = true;
                            else
                            {
                                c.HostEntry = null;
                                DisconnectPendingClient(c);
                            }
                        }
                    }

                    if (!c.Active)
                        continue;

                    int available = c.ClientConnection.Available;

                    if (c.ClientConnection.Available > 0)
                    {
                        if (!c.ProtcolPassed && c.ClientConnection.Available >= Protocol.BZFSHail.Length)
                        {
                            c.DataRecieved = true;
                            byte[] buffer = new byte[Protocol.BZFSHail.Length];

                            int read = c.NetStream.Read(buffer, 0, buffer.Length);
                            if (read != buffer.Length)
                            {
                                c.ProtcolPassed = false;
                                DisconnectPendingClient(c);
                                Logger.Log4("Disconnecting abnormal connection from " + c.ClientConnection.Client.RemoteEndPoint.ToString());
                            }
                            else
                            {
                                if (Encoding.ASCII.GetString(buffer) == Protocol.BZFSHailString)
                                {
                                    c.ProtcolPassed = true;
                                    c.NetStream.Write(Protocol.DefaultBZFSVersion, 0, Protocol.DefaultBZFSVersion.Length);
                                    c.NetStream.Flush();
                                    c.VersionPassed = true;
                                    Logger.Log4("BZFS header from " + c.ClientConnection.Client.RemoteEndPoint.ToString());
                                }
                            }
                        }
                    }

                    if (c.Active)
                    {
                        if (c.DNSStarted && c.DNSPassed && c.DataRecieved && c.ProtcolPassed && c.VersionPassed)
                        {
                            RemovePendingClient(c);

                            Logger.Log4("Accepted BZFS connection from " + c.ClientConnection.Client.RemoteEndPoint.ToString());
                            // send them off to the next step
                            if (BZFSProtocolConnectionAccepted != null)
                                BZFSProtocolConnectionAccepted.Invoke(this, c);
                        }
                    }
                }

                Thread.Sleep(100);
                lock (PendingClients)
                    clients = PendingClients.ToArray();
            }

            WorkerThread = null;
        }

        protected void DNSLookupCompleted(IAsyncResult ar)
        {
            PendingClient c = ar.AsyncState as PendingClient;
            if (c == null)
                return;

            try
            {
                var results = Dns.EndGetHostEntry(ar);
 
                c.HostEntry = results;

                Logger.Log3("DNS Lookup completed for " + c.HostEntry.HostName);
            }
            catch (Exception)
            {
                c.HostEntry = new IPHostEntry();
            }
        }
    }
}
