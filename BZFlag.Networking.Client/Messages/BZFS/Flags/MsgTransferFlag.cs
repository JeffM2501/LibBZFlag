using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Flags
{
	public class MsgTransferFlag : NetworkMessage
	{
		public int FromID = -1;
		public int ToID = -1;
		public int FlagID = -1;

		public MsgTransferFlag()
		{
			Code = CodeFromChars("tf");
		}

		public override byte[] Pack()
		{
			DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);

			buffer.WriteByte(FromID);
			buffer.WriteByte(ToID);
			buffer.WriteUInt16((UInt16)FlagID);

			return buffer.GetMessageBuffer();
		}

		public override void Unpack(byte[] data)
		{
			ResetOffset();
			FromID = ReadByte(data);
			ToID = ReadByte(data);
			FlagID = ReadUInt16(data);
		}
	}
}
