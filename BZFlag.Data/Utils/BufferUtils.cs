using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Data.Utils
{
	public static class BufferUtils
	{
		// TODO, make some faster unpack code that doesn't use arrays for common values

		private static byte[] shortBuffer = new byte[] { 0, 0 };
		private static byte[] longBuffer = new byte[] { 0, 0,0,0 };

		public static ushort ReadUInt16(byte[] fromBuffer, int readOffset)
		{
			Array.Copy(fromBuffer, readOffset, shortBuffer, 0, 2);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(shortBuffer);

			return BitConverter.ToUInt16(shortBuffer,0);
		}

		public static uint ReadUInt32(byte[] fromBuffer, int readOffset)
		{
			Array.Copy(fromBuffer, readOffset, longBuffer, 0, 4);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(longBuffer);

			return BitConverter.ToUInt32(longBuffer, 0);
		}

		public static UInt64 ReadUInt64(byte[] fromBuffer, int readOffset)
		{
			Array.Copy(fromBuffer, readOffset, longBuffer, 0, 8);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(longBuffer);

			return BitConverter.ToUInt64(longBuffer, 0);
		}

		public static short ReadInt16(byte[] fromBuffer, int readOffset)
		{
			Array.Copy(fromBuffer, readOffset, shortBuffer, 0, 2);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(shortBuffer);

			return BitConverter.ToInt16(shortBuffer, 0);
		}

		public static Int32 ReadInt32(byte[] fromBuffer, int readOffset)
		{
			Array.Copy(fromBuffer, readOffset, longBuffer, 0, 4);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(longBuffer);

			return BitConverter.ToInt32(longBuffer, 0);
		}

		public static Int64 ReadInt64(byte[] fromBuffer, int readOffset)
		{
			Array.Copy(fromBuffer, readOffset, longBuffer, 0, 8);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(longBuffer);

			return BitConverter.ToInt64(longBuffer, 0);
		}

		private static byte[] singleBuffer = new byte[4] { 0, 0, 0, 0 };
		public static float ReadSingle(byte[] fromBuffer, int readOffset)
		{
			Array.Copy(fromBuffer, readOffset, singleBuffer, 0, 4);
 			if(BitConverter.IsLittleEndian)
 				Array.Reverse(singleBuffer);

			return BitConverter.ToSingle(singleBuffer, 0);
		}

		private static byte[] doubleBuffer = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
		public static double ReadDouble(byte[] fromBuffer, int readOffset)
		{
			Array.Copy(fromBuffer, readOffset, doubleBuffer, 0, 8);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(doubleBuffer);
			return BitConverter.ToDouble(doubleBuffer, 0);
		}

		public static string ReadFixedSizeString(byte[] fromBuffer, int readOffset, int size)
		{
			byte[] b = new byte[size];
			Array.Copy(fromBuffer, readOffset, b, 0, size);

			return Encoding.UTF8.GetString(b);
		}

		public static int ReadPascalString(byte[] fromBuffer, int readOffset, ref string output)
		{
			int size = (int)fromBuffer[readOffset];
			byte[] b = new byte[size];
			Array.Copy(fromBuffer, readOffset+1, b, 0, size);
			output = Encoding.UTF8.GetString(b);
			return size + 1;
		}

		public static int ReadUShortPascalString(byte[] fromBuffer, int readOffset, ref string output)
		{
			int size = ReadUInt16(fromBuffer, readOffset);
			byte[] b = new byte[size];
			Array.Copy(fromBuffer, readOffset+2, b, 0, size);

			output = Encoding.UTF8.GetString(b);
			return size + 2;
		}
	}
}
