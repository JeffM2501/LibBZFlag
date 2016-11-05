using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages
{
	public abstract class NetworkMessage
	{
		public int Code = int.MinValue;

		public abstract void Unpack(byte[] data);
		public abstract byte[] Pack();

		public static int CodeFromChars(string msgCode)
		{
			if(msgCode.Length < 2)
				msgCode = " " + msgCode;

			byte[] b = Encoding.ASCII.GetBytes(msgCode.Substring(0, 2));
			return (int)BitConverter.ToUInt16(b,0);
		}

		private int BufferOffset = 0;
		protected void ResetOffset()
		{
			BufferOffset = 0;
		}

		protected UInt16 ReadUInt16(byte[] b)
		{
			if(b.Length < BufferOffset + 2)
				return 0;

			BufferOffset += 2;
			return BufferUtils.ReadUInt16(b, BufferOffset-2);
		}

		protected Int16 ReadInt16(byte[] b)
		{
			if(b.Length < BufferOffset + 2)
				return 0;

			BufferOffset += 2;
			return BufferUtils.ReadInt16(b, BufferOffset - 2);
		}

		protected UInt32 ReadUInt32(byte[] b)
		{
			if(b.Length < BufferOffset + 4)
				return 0;

			BufferOffset += 4;
			return BufferUtils.ReadUInt32(b, BufferOffset - 4);
		}

		protected Int32 ReadInt32(byte[] b)
		{
			if(b.Length < BufferOffset + 4)
				return 0;

			BufferOffset += 4;
			return BufferUtils.ReadInt32(b, BufferOffset - 4);
		}
	}

	public class UnknownMessage : NetworkMessage
	{
		public byte[] DataBuffer = null;

		public UnknownMessage(int code, byte[] b)
		{
			DataBuffer = b;
			Code = code;
		}

		public override byte[] Pack()
		{
			throw new NotImplementedException();
		}

		public override void Unpack(byte[] data)
		{
			DataBuffer = data;
		}
	}
}
