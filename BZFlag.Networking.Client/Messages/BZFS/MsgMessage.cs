using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS
{
	public class MsgMessage : NetworkMessage
	{
		public enum MessageTypes
		{
			ChatMessage = 0,
			ActionMessage
		};

		public int From = 0;
		public int To = 0;
		public MessageTypes MessageType = MessageTypes.ChatMessage;
		public string MessageText = string.Empty;

		public MsgMessage()
		{
			Code = CodeFromChars("mg");
		}

		public override byte[] Pack()
		{
			DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);
			buffer.WriteByte((byte)From);
			buffer.WriteByte((byte)To);
			buffer.WriteByte((byte)MessageType);
			buffer.WriteNullTermString(MessageText);

			return buffer.GetMessageBuffer();
		}

		public override void Unpack(byte[] data)
		{
			ResetOffset();
			From = ReadByte(data);
			To = ReadByte(data);
			MessageType = (MessageTypes)ReadByte(data);

			MessageText = ReadNullTermString(data, true);
		}
	}
}
