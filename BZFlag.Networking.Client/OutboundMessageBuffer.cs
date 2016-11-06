using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

using BZFlag.Networking.Messages;

namespace BZFlag.Networking
{
	public class OutboundMessageBuffer
	{
		private List<byte[]> Outbound = new List<byte[]>();

		private List<NetworkMessage> UnprocessedOutbound = new List<NetworkMessage>();

		private Thread Worker = null;

		public void Start()
		{
			Stop();

			Worker = new Thread(new ThreadStart(Process));
			Worker.Start();
		}

		public void Stop()
		{
			if(Worker != null && Worker.IsAlive)
				Worker.Abort();

			Worker = null;
		}

		protected virtual void Process()
		{
			while(true)
			{
				NetworkMessage msg = GetNextUnprocessedMessage();
				while(msg != null)
				{
					byte[] buffer = msg.Pack();
					if(buffer != null)
						PushDirectMessage(buffer);

					msg = GetNextUnprocessedMessage();

				}
				Thread.Sleep(10);

			}
		}


		public byte[] Pop()
		{
			lock(Outbound)
			{
				if(Outbound.Count == 0)
					return null;

				byte[] b = Outbound[0];
				Outbound.RemoveAt(0);
				return b;
			}
		}

		public void PushDirectMessage(byte[] buffer)
		{
			lock(Outbound)
				Outbound.Add(buffer);
		}

		public void Push(NetworkMessage msg)
		{
			lock(UnprocessedOutbound)
				UnprocessedOutbound.Add(msg);
		}

		protected NetworkMessage GetNextUnprocessedMessage()
		{
			lock(UnprocessedOutbound)
			{
				if(UnprocessedOutbound.Count == 0)
					return null;

				NetworkMessage b = UnprocessedOutbound[0];
				UnprocessedOutbound.RemoveAt(0);
				return b;
			}
		}

		public void Clear()
		{
			lock(Outbound)
				Outbound.Clear();
		}
	}
}
