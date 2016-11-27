using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using BZFlag.Networking;
using BZFlag.Game.Security;

namespace BZFlag.Game.Host
{
	public class TCPConnectionManager
	{
		public int Port = 5154;
		public TcpListener Listener = null;

		public BanList Bans = new BanList();

		public class PendingClient : EventArgs
		{
			public TcpClient ClientConnection = null;

			public bool Active = false;

			public bool DNSStarted = false;
			public IPHostEntry HostEntry = null;

			public bool DNSPassed = false;

			public bool DataRecieved = false;
			public bool ProtcolPassed = false;

			public byte[] PendingData = new byte[0];
		}
		protected List<PendingClient> PendingClients = new List<PendingClient>();

		public event EventHandler<PendingClient> BZFSProtocolConnectionAccepted = null;

		protected Thread WorkerThread = null;

        protected Server Host = null;

		public TCPConnectionManager(int port, Server server)
		{
            Host = server;
            Port = port;
			Listener = new TcpListener(IPAddress.Any, port);
		}

		public void StartUp()
		{
			Listener.BeginAcceptTcpClient(TCPClientAccepted, null);
		}

		public void Shutdown()
		{
			if(WorkerThread != null)
				WorkerThread.Abort();

			WorkerThread = null;
		}

		protected void TCPClientAccepted(IAsyncResult ar)
		{
			PendingClient c = new PendingClient();
			c.ClientConnection = Listener.EndAcceptTcpClient(ar);
			Listener.BeginAcceptTcpClient(TCPClientAccepted, null);

			var address = ((IPEndPoint)c.ClientConnection.Client.RemoteEndPoint).Address.ToString();

			var ban = Bans.FindIPBan(address);
			if (ban != null)
			{
				c.ClientConnection.Close();
				return;
			}

			lock(PendingClients)
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
			lock(PendingClients)
				PendingClients.Remove(pc);
		}

		protected void ProcessPendingClients()
		{
			PendingClient[] clients = null;

			lock(PendingClients)
				clients = PendingClients.ToArray();

			while (clients.Length > 0)
			{
				foreach(PendingClient c in clients )
				{
					if (!c.DNSStarted)
					{
						var address = ((IPEndPoint)c.ClientConnection.Client.RemoteEndPoint).Address.ToString();
						Dns.BeginGetHostEntry(address, DNSLookupCompleted, c);
						c.DNSStarted = true;
					}
					else if (!c.DNSPassed)
					{
						if(c.HostEntry != null)
						{
							// lookup the host in the ban list
							var ban = Bans.FindHostBan(c.HostEntry.HostName);
							if(ban != null)
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

					if (c.ClientConnection.Available >= Protocol.BZFSHail.Length)
					{
						if (!c.ProtcolPassed)
						{
							c.DataRecieved = true;
							byte[] buffer = new byte[Protocol.BZFSHail.Length];

							var stream = c.ClientConnection.GetStream();

							int read = stream.Read(buffer,0, buffer.Length);
							if (read != buffer.Length)
							{
								c.ProtcolPassed = false;
								DisconnectPendingClient(c);
							}
							else
							{
								if (Encoding.ASCII.GetString(buffer) == Protocol.BZFSHailString)
								{
									c.ProtcolPassed = true;
									stream.Write(Protocol.DefaultBZFSVersion, 0, Protocol.DefaultBZFSVersion.Length);
								}
							}
						}
					}

					if (c.Active)
					{
						if (c.DNSStarted && c.DNSPassed && c.DataRecieved && c.ProtcolPassed)
						{
							RemovePendingClient(c);

							// send them off to the next step
							if(BZFSProtocolConnectionAccepted != null)
								BZFSProtocolConnectionAccepted.Invoke(this, c);
						}
					}
				}

				Thread.Sleep(100);
				lock(PendingClients)
					clients = PendingClients.ToArray();
			}

			WorkerThread = null;
		}

		protected void DNSLookupCompleted(IAsyncResult ar)
		{
			var results = Dns.EndGetHostEntry(ar);
			PendingClient c = ar.AsyncState as PendingClient;
			if(c == null)
				return;

			c.HostEntry = results;
		}

	}
}
