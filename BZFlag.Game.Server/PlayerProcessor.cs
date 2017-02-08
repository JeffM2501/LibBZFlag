using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using BZFlag.Networking.Messages;
using BZFlag.Networking;


using BZFlag.Game.Host.Players;

namespace BZFlag.Game.Host
{
	public class PlayerProcessor
	{
		protected Thread WorkerThread = null;
		private List<ServerPlayer> Players = new List<ServerPlayer>();

        protected MessageManager MessageProcessor = null;


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

            player.Disconnected += Player_Disconnected;

			if(WorkerThread == null)
			{
				WorkerThread = new Thread(new ThreadStart(ProcessPendingPlayers));
				WorkerThread.Start();
			}
		}

        protected void RemovePlayer(ServerPlayer sp)
        {
            sp.Disconnected -= Player_Disconnected;
        }

        private void Player_Disconnected(object sender, Networking.Common.Peer e)
        {
            lock (Players)
                Players.Remove(e as ServerPlayer);
        }

        public virtual void ProcessClientMessage(ServerPlayer player, NetworkMessage msg)
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
					int count = 0; 
					while (count < MaxMessagesPerClientCycle)
					{
						InboundMessageBuffer.CompletedMessage buffer = player.InboundTCP.GetMessage();
						if(buffer == null)
							break;
		
						NetworkMessage msg = MessageProcessor.Unpack(buffer.ID, buffer.Data);
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
