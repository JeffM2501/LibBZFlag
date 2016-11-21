using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using BZFlag.Networking.Messages;

namespace BZFlag.Networking
{
	public class PendingPlayerSecurityLockdown
	{
		protected Thread WorkerThread = null;

		private List<ServerPlayer> PendingPlayers = new List<ServerPlayer>();

		private InboundMessageCallbackProcessor Handler = new InboundMessageCallbackProcessor();

		public class PlayerConnectionState
		{
			public bool SentEnter = false;
			public bool SentWorld = false;

			Handler.Add(new MsgQueryGame(),HandleQueryGame);
		}

		public static int MaxMessagesPerClientCycle = 10;

		public void Shutdown()
		{
			if(WorkerThread != null)
				WorkerThread.Abort();

			WorkerThread = null;
		}

		public void AddPendingConnection(object sender, TCPConnectionManager.PendingClient client)
		{
			lock(PendingPlayers)
				PendingPlayers.Add(new ServerPlayer(client.ClientConnection));

			if(WorkerThread == null)
			{
				WorkerThread = new Thread(new ThreadStart(ProcessPendingPlayers));
				WorkerThread.Start();
			}
		}

		protected void ProcessPendingPlayers()
		{
			ServerPlayer[] prisoners = null;

			lock(PendingPlayers)
				prisoners = PendingPlayers.ToArray();

			while(prisoners.Length > 0)
			{
				foreach(ServerPlayer suplicant in prisoners)
				{
					suplicant.ProcessTCP();

					int count = 0; 
					while (count < MaxMessagesPerClientCycle)
					{
						InboundMessageBuffer.CompletedMessage buffer = suplicant.InboundTCP.GetMessage();
						if(buffer == null)
							break;
		
						NetworkMessage msg = SecurityJailMessageFacotry.Factory.Unpack(buffer.ID, buffer.Data);
						msg.Tag = suplicant;
						msg.FromUDP = false;

						if(!Handler.DispatchMessage(msg))
							suplicant.PushInboundMessage(msg);

						count++;
					}
					
				}
				Thread.Sleep(100);
				lock(PendingPlayers)
					prisoners = PendingPlayers.ToArray();
			}

			WorkerThread = null;
		}
	}
}
