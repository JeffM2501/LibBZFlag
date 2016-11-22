using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using BZFlag.Networking.Messages;

namespace BZFlag.Networking
{
	public class PlayerProcessor
	{
		protected Thread WorkerThread = null;
		private List<ServerPlayer> Players = new List<ServerPlayer>();

        public int SleepTime = 100;
		public static int MaxMessagesPerClientCycle = 10;

		public void Shutdown()
		{
			if(WorkerThread != null)
				WorkerThread.Abort();

			WorkerThread = null;
		}

		public void AddPendingConnection(ServerPlayer player)
		{
			lock(Players)
                Players.Add(player);

			if(WorkerThread == null)
			{
				WorkerThread = new Thread(new ThreadStart(ProcessPendingPlayers));
				WorkerThread.Start();
			}
		}

        protected virtual void ProcessClientMessage(ServerPlayer player, NetworkMessage msg)
        {

        }

		protected void ProcessPendingPlayers()
		{
			ServerPlayer[] locals = null;

			lock(Players)
                locals = Players.ToArray();

			while(locals.Length > 0)
			{
				foreach(ServerPlayer player in locals)
				{
                    player.ProcessTCP();

					int count = 0; 
					while (count < MaxMessagesPerClientCycle)
					{
						InboundMessageBuffer.CompletedMessage buffer = player.InboundTCP.GetMessage();
						if(buffer == null)
							break;
		
						NetworkMessage msg = SecurityJailMessageFacotry.Factory.Unpack(buffer.ID, buffer.Data);
						msg.Tag = player;
						msg.FromUDP = false;

                        ProcessClientMessage(player, msg);

						count++;
					}
					
				}
				Thread.Sleep(SleepTime);
				lock(Players)
                    locals = Players.ToArray();
			}

			WorkerThread = null;
		}
	}
}
