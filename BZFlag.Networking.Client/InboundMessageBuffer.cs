using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking
{
	public class InboundMessageBuffer
	{
		public class CompletedMessage
		{
			public int ID = -1;
			public int Size = -1;
			public byte[] Data = new byte[0];
		}

		protected List<CompletedMessage> CompletedMessages = new List<CompletedMessage>();

		protected byte[] PartialMessage = null;

		public event EventHandler CompleteMessageRecived = null;

		public void Clear()
		{
			lock(CompletedMessages)
				CompletedMessages.Clear();

			PartialMessage = null;
		}

		public CompletedMessage GetMessage()
		{
			CompletedMessage msg = null;
			lock(CompletedMessages)
			{
				if (CompletedMessages.Count != 0)
				{
					msg = CompletedMessages[0];
					CompletedMessages.RemoveAt(0);
				}
			}
			return msg;
		}

		public CompletedMessage[] GetMessages()
		{
			CompletedMessage[] msgs = null;
			lock(CompletedMessages)
			{
				if(CompletedMessages.Count != 0)
				{
					msgs = CompletedMessages.ToArray();
					CompletedMessages.Clear();
				}
			}
			return msgs;
		}

		protected void PushMessage(CompletedMessage msg)
		{
			lock(CompletedMessages)
			{
				CompletedMessages.Add(msg);
			}

			if(CompleteMessageRecived != null)
				CompleteMessageRecived.Invoke(this, EventArgs.Empty);
		}

		// TODO, do some pooling here, for performance
		public void AddData(byte[] buffer)
		{
			if (PartialMessage == null)
			{
				PartialMessage = buffer;
			}
			else
			{
				int copyStart = PartialMessage.Length;
				Array.Resize(ref PartialMessage, copyStart + buffer.Length);
				Array.Copy(buffer, 0, PartialMessage, copyStart, buffer.Length);
			}

			if (PartialMessage.Length < 4)
			{

				int len  = BufferUtils.ReadUInt16(PartialMessage, 0);
				int code = BufferUtils.ReadUInt16(PartialMessage, 2);

				if (PartialMessage.Length >= (len + 4))
				{
					// message is long enough, parse it
					CompletedMessage msg = new CompletedMessage();
					msg.ID = code;
					msg.Size = len;
					msg.Data = new byte[len];
					Array.Copy(buffer, 4, msg.Data, 0, len);

					PushMessage(msg);

					if(buffer.Length == len + 4)
						buffer = null;
					else
					{
						byte[] remanats = new byte[buffer.Length - (len + 4)];
						Array.Copy(buffer, (len + 4), remanats, 0, remanats.Length);
						buffer = remanats;
					}
				}
			}	
		}
	}
}
