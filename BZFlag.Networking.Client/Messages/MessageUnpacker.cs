using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace BZFlag.Networking.Messages
{
	public class MessageUnpacker
	{
		protected List<NetworkMessage> ParsedMessages = new List<NetworkMessage>();
		protected List<InboundMessageBuffer.CompletedMessage> InboudMessages = new List<InboundMessageBuffer.CompletedMessage>();

		private Thread Worker = null;

		public void Push(InboundMessageBuffer.CompletedMessage msg)
		{
			lock(InboudMessages)
				InboudMessages.Add(msg);
		}

		public NetworkMessage Pop ()
		{
			NetworkMessage msg = null;
			lock(ParsedMessages)
			{
				if (ParsedMessages.Count > 0)
				{
					msg = ParsedMessages[0];
					ParsedMessages.RemoveAt(0);
				}
			}
			return msg;
		}


		protected InboundMessageBuffer.CompletedMessage GetNextInbound()
		{
			InboundMessageBuffer.CompletedMessage msg = null;
			lock(InboudMessages)
			{
				if(InboudMessages.Count > 0)
				{
					msg = InboudMessages[0];
					InboudMessages.RemoveAt(0);
				}
			}
			return msg;
		}

		public void CompleteMessage(NetworkMessage msg)
		{
			lock(ParsedMessages)
				ParsedMessages.Add(msg);
		}

		public void Start()
		{
			Stop();

			Worker = new Thread(new ThreadStart(Process));
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
				InboundMessageBuffer.CompletedMessage buffer = GetNextInbound();
				while (buffer != null)
				{
					// lookup message class
					buffer = GetNextInbound();

					NetworkMessage msg = MessageFactory.Unpack(buffer.ID, buffer.Data);
					if(msg != null)
						CompleteMessage(msg);

				}
				Thread.Sleep(10);

			}
		}
	}
}
