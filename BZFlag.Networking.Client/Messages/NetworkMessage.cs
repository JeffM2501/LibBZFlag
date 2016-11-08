using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;

namespace BZFlag.Networking.Messages
{
	public abstract class NetworkMessage
	{
		public int Code = int.MinValue;
		public bool FromUDP = false;

		public abstract void Unpack(byte[] data);
		public abstract byte[] Pack();

		public static int CodeFromChars(string msgCode)
		{
			if(msgCode.Length < 2)
				msgCode = " " + msgCode;

			byte[] b = Encoding.ASCII.GetBytes(msgCode.Substring(0, 2));
			if(BitConverter.IsLittleEndian)
				Array.Reverse(b);
			return (int)BitConverter.ToUInt16(b,0);
		}

		private int BufferOffset = 0;
		protected void ResetOffset()
		{
			BufferOffset = 0;
		}

		protected byte ReadByte(byte[] b)
		{
			if(b.Length < BufferOffset + 1)
				return 0;

			BufferOffset += 1;
			return b[BufferOffset-1];
		}

		protected byte[] ReadBytes(byte[] b, int size)
		{
			if(b.Length < BufferOffset + size)
				return new byte[0];

			byte[] d = new byte[size];
			Array.Copy(b, BufferOffset, d, 0, size);
			BufferOffset += size;
			return d;
		}

		protected byte[] ReadRestOfBytes(byte[] b)
		{
			byte[] d = new byte[b.Length-BufferOffset];
			Array.Copy(b, BufferOffset, d, 0, d.Length);
			BufferOffset = b.Length;
			return d;
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

		protected UInt64 ReadUInt64(byte[] b)
		{
			if(b.Length < BufferOffset + 4)
				return 0;

			BufferOffset += 8;
			return BufferUtils.ReadUInt64(b, BufferOffset - 8);
		}

		protected Int64 ReadInt64(byte[] b)
		{
			if(b.Length < BufferOffset + 8)
				return 0;

			BufferOffset += 8;
			return BufferUtils.ReadInt64(b, BufferOffset - 8);
		}

        protected float ReadFloat(byte[] b)
        {
            if (b.Length < BufferOffset + 4)
                return 0;

            BufferOffset += 4;
            return BufferUtils.ReadSingle(b, BufferOffset - 4);
        }

        protected double ReadDouble(byte[] b)
        {
            if (b.Length < BufferOffset + 8)
                return 0;

            BufferOffset += 8;
            return BufferUtils.ReadDouble(b, BufferOffset - 8);
        }

        protected Vector4F ReadVector4F(byte[] b)
        {
            return new Vector4F(ReadFloat(b), ReadFloat(b), ReadFloat(b), ReadFloat(b));
        }

        protected Vector3F ReadVector3F(byte[] b)
        {
            return new Vector3F(ReadFloat(b), ReadFloat(b), ReadFloat(b));
        }

        protected Vector2F ReadVector2f(byte[] b)
        {
            return new Vector2F(ReadFloat(b), ReadFloat(b));
        }

        protected string ReadFixedSizeString(byte[] b, int size)
		{
			if(b.Length < BufferOffset + size)
				return string.Empty;

			string s = Encoding.UTF8.GetString(b, BufferOffset, size);
			BufferOffset += size;
			return s.TrimEnd(new char[] { '\0' });
		}

		protected string ReadPascalString(byte[] b)
		{
			if(b.Length < BufferOffset + 1)
				return string.Empty;

			byte len = b[BufferOffset];

			if(b.Length < BufferOffset + 1 + len)
				return string.Empty;

			string s = Encoding.UTF8.GetString(b, BufferOffset+1, len);
			BufferOffset += len+1;
			return s;
		}

		protected string ReadUShortPascalString(byte[] b)
		{
			if(b.Length < BufferOffset + 2)
				return string.Empty;

			int len = BufferUtils.ReadUInt16(b, BufferOffset);

			if(b.Length < BufferOffset + 2 + len)
				return string.Empty;

			string s = Encoding.UTF8.GetString(b, BufferOffset + 2, len);
			BufferOffset += len + 2;
			return s;
		}


		protected string ReadNullTermString(byte[] b, bool readToEnd)
		{
			if (readToEnd)
			{
				int start = BufferOffset;
				BufferOffset = b.Length;
				return Encoding.UTF8.GetString(b, start, BufferOffset - start - 1);
			}
			else
			{
				int end = Array.FindIndex(b, BufferOffset, x => x == byte.MinValue);
				if(end == -1)
					return string.Empty;
				string ret = Encoding.UTF8.GetString(b, BufferOffset, BufferOffset - end);
				BufferOffset = end + 1;
				return ret;
			}
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
