using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace BZFlag.Networking
{
	public class TCPConnectionManager
	{
		public int Port = 5154;
		public TcpListener Listener = null;

		public class PendingClient
		{
			public TcpClient ClientConnection = null;

			public bool DNSStarted = false;
			public IPHostEntry HostEntry = null;

			public bool DNSPassed = false;

			public bool IPStarted = false;
			public bool IPPassed = false;

			public bool DataRecieved = false;
			public bool ProtcolPassed = false;

			public byte[] PendingData = new byte[0];
		}
		protected List<PendingClient> PendingClients = new List<PendingClient>();

		protected Thread WorkerThread = null;

		public TCPConnectionManager(int port)
		{
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

			lock(PendingClients)
				PendingClients.Add(c);

			if (WorkerThread == null)
			{
				WorkerThread = new Thread(new ThreadStart(ProcessPendingClients));
				WorkerThread.Start();
			}
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
