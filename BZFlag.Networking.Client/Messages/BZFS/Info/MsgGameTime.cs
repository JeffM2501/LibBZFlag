using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Info
{
	public class MsgGameTime : NetworkMessage
	{
		public UInt64 NetTime = UInt64.MinValue;

		public MsgGameTime()
		{
			Code = CodeFromChars("gt");
		}

		public override byte[] Pack()
		{
			DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);
			buffer.WriteUInt64(NetTime);
			return buffer.GetMessageBuffer();
		}

		public override void Unpack(byte[] data)
		{
			ResetOffset();
			NetTime = ReadUInt32(data);
		}
	}
}
