using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.UDP
{
	public class MsgUDPLinkRequest : NetworkMessage
	{
		int PlayerID = -1;
		public MsgUDPLinkRequest()
		{
			Code = CodeFromChars("of");
		}

		public override byte[] Pack()
		{
			DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);
			buffer.WriteByte((byte)PlayerID);
		}

		public override void Unpack(byte[] data)
		{
			ResetOffset();
			PlayerID = ReadByte(data);
		}
	}
}
