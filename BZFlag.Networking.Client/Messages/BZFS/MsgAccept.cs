using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS
{
	public class MsgAccept : NetworkMessage
	{
		public int PlayerID = -1;
		public MsgAccept()
		{
			Code = CodeFromChars("ac");
		}

		public override byte[] Pack()
		{
			DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);
			buffer.WriteByte((byte)PlayerID);
			return buffer.GetMessageBuffer();
		}

		public override void Unpack(byte[] data)
		{
			ResetOffset();
			PlayerID = ReadByte(data);
		}
	}
}
