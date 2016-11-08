using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking
{
	public class DynamicOutputBuffer
	{
		protected UInt16 Code = 0;

		private static byte[] Buffer = new byte[2048];
		private static int BytesUsed = 0;

		public DynamicOutputBuffer()
		{
			BytesUsed = 4;
		}

		public DynamicOutputBuffer( int code)
		{
			BytesUsed = 4;
			Code = (UInt16)code;
			WriteUInt16(0, 0);
			WriteUInt16(Code, 2);
		}

		public void SetCode(int code)
		{
			Code = (UInt16)code;
			WriteUInt16(Code, 2);
		}

		public byte[] GetMessageBuffer()
		{
			WriteUInt16((UInt16)(BytesUsed-4), 0);
			byte[] outbuffer = new byte[BytesUsed];
			Array.Copy(Buffer, outbuffer, BytesUsed);
			return outbuffer;
		}

		private void CheckBuffer(int toAdd)
		{
			if (BytesUsed + toAdd > Buffer.Length)
				Array.Resize(ref Buffer, Buffer.Length + 1024);
		}

		public void WriteByte(byte b)
		{
			CheckBuffer(1);

			Buffer[BytesUsed] = b;
			BytesUsed++;
		}

        public void WriteByte(int b)
        {
            WriteByte((byte)b);
        }

        public void WriteBytes(byte[] bytes)
		{
			CheckBuffer(bytes.Length);

			Array.Copy(bytes, 0, Buffer, BytesUsed, bytes.Length);
			BytesUsed += bytes.Length;
		}

		public void WriteUInt16(UInt16 value)
		{
			WriteUInt16(value, BytesUsed);
			BytesUsed += 2;
		}

		protected void WriteUInt16(UInt16 value, int offset)
		{
			CheckBuffer(2);

			var b = BitConverter.GetBytes(value);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(b);

			Array.Copy(b, 0, Buffer, offset, 2);
		}

		public void WriteInt16(Int16 value)
		{
			CheckBuffer(2);

			var b = BitConverter.GetBytes(value);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(b);

			Array.Copy(b, 0, Buffer, BytesUsed, 2);
			BytesUsed += 2;
		}

		public void WriteUInt32(UInt32 value)
		{
			CheckBuffer(4);

			var b = BitConverter.GetBytes(value);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(b);

			Array.Copy(b, 0, Buffer, BytesUsed, 4);
			BytesUsed += 4;
		}

		public void WriteInt32(Int32 value)
		{
			CheckBuffer(4);

			var b = BitConverter.GetBytes(value);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(b);

			Array.Copy(b, 0, Buffer, BytesUsed, 4);
			BytesUsed += 4;
		}

		public void WriteUInt64(UInt64 value)
		{
			CheckBuffer(8);

			var b = BitConverter.GetBytes(value);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(b);

			Array.Copy(b, 0, Buffer, BytesUsed, 4);
			BytesUsed += 8;
		}

		public void WriteInt64(Int32 value)
		{
			CheckBuffer(8);

			var b = BitConverter.GetBytes(value);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(b);

			Array.Copy(b, 0, Buffer, BytesUsed, 4);
			BytesUsed += 8;
		}

		public void WriteFloat(float value)
		{
			CheckBuffer(4);

			var b = BitConverter.GetBytes(value);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(b);

			Array.Copy(b, 0, Buffer, BytesUsed, 4);
			BytesUsed += 4;
		}

		public void WriteDouble(double value)
		{
			CheckBuffer(8);

			var b = BitConverter.GetBytes(value);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(b);

			Array.Copy(b, 0, Buffer, BytesUsed, 8);
			BytesUsed += 4;
		}

		public void WriteFixedSizeString(string value, int size)
		{
			CheckBuffer(size);

			int i = value.Length;
			if(i > size)
				i = size;

			Encoding.UTF8.GetBytes(value, 0, i, Buffer, BytesUsed);

			if (i < size)
			{
				while (i < size)
				{
					Buffer[BytesUsed+i] = 0;
					i++;
				}
			}
			BytesUsed += size;
		}

		public void WritePascalString(string value)
		{
			int actualSize = value.Length;
			if(actualSize > Byte.MaxValue)
				actualSize = Byte.MaxValue;

			CheckBuffer(actualSize + 1);

			Buffer[BytesUsed] = (byte)actualSize;

			Encoding.UTF8.GetBytes(value, 0, actualSize, Buffer, BytesUsed+1);
			BytesUsed += actualSize + 1;
		}

		public void WriteUShortPascalString(string value)
		{
			int actualSize = value.Length;
			if(actualSize > UInt16.MaxValue)
				actualSize = UInt16.MaxValue;

			CheckBuffer(actualSize + 2);

			WriteUInt16((UInt16)actualSize, BytesUsed);

			Encoding.UTF8.GetBytes(value, 0, actualSize, Buffer, BytesUsed + 2);
			BytesUsed += actualSize + 2;
		}

		public void WriteNullTermString(string value)
		{
			CheckBuffer(value.Length + 1);
			Encoding.UTF8.GetBytes(value, 0, value.Length, Buffer, BytesUsed);
			BytesUsed += value.Length;
			WriteByte(byte.MinValue);
		}
	}
}
