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

		public MessageManager Factory = ClientMessageFactory.Factory;

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
				InboundMessageBuffer.CompletedMessage buffer = GetNextInbound();
				while (buffer != null)
				{
					NetworkMessage msg = Factory.Unpack(buffer.ID, buffer.Data);
					if(msg != null)
					{
						msg.FromUDP = buffer.UDP;
						CompleteMessage(msg);
					}

					buffer = GetNextInbound();

				}
				Thread.Sleep(10);

			}
		}
	}
}
