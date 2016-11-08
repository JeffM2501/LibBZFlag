using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.World
{
	public class MsgGetWorld : NetworkMessage
	{
		public UInt32 Offset = 0;
		public byte[] Data = new byte[0];

		public MsgGetWorld()
		{
			Code = CodeFromChars("gw");
		}

		public MsgGetWorld(UInt32 offset)
		{
			Code = CodeFromChars("gw");
			Offset = offset;
		}

		public override byte[] Pack()
		{
			DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);
			buffer.WriteUInt32(Offset);
			return buffer.GetMessageBuffer();
		}

		public override void Unpack(byte[] data)
		{
			ResetOffset();
			Offset = ReadUInt32(data);
			Data = ReadRestOfBytes(data);
		}
	}
}
