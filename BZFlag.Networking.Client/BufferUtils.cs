using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking
{
	public static class BufferUtils
	{
		public static ushort ReadUInt16(byte[] fromBuffer, int readBitOffset)
		{
			return ReadUInt16(fromBuffer, 16, readBitOffset);
		}

		public static uint ReadUInt32(byte[] fromBuffer, int readBitOffset)
		{
			return ReadUInt32(fromBuffer, 32, readBitOffset);
		}

		public static short ReadInt16(byte[] fromBuffer, int readBitOffset)
		{
			return (short)ReadUInt16(fromBuffer, 16, readBitOffset);
		}

		public static Int32 ReadInt32(byte[] fromBuffer, int readBitOffset)
		{
			return (Int32)ReadUInt32(fromBuffer, 16, readBitOffset);
		}

		private static byte[] singleBuffer = new byte[4] { 0, 0, 0, 0 };
		public static float ReadSingle(byte[] buffer, int offset)
		{
			ReadBytes(buffer, 4, offset, singleBuffer, 0);
			return BitConverter.ToSingle(singleBuffer, 0);
		}

		private static byte[] doubleBuffer = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
		public static double ReadDouble(byte[] buffer, int offset)
		{
			ReadBytes(buffer, 4, offset, doubleBuffer, 0);
			return BitConverter.ToDouble(doubleBuffer, 0);
		}

		public static string ReadFixedSizeString(byte[] buffer, int offset, int size)
		{
			byte[] b = new byte[size];
			ReadBytes(buffer, size, offset, b, 0);
			return Encoding.UTF8.GetString(b);
		}

		public static int ReadPascalString(byte[] buffer, int offset, ref string output)
		{
			int size = ReadByte(buffer, 8, offset);
			byte[] b = new byte[size];
			ReadBytes(buffer, size, offset + 2, b, 0);
			output = Encoding.UTF8.GetString(b);
			return size + 1;
		}

		public static int ReadUShortPascalString(byte[] buffer, int offset, ref string output)
		{
			int size = ReadUInt16(buffer, offset);
			byte[] b = new byte[size];
			ReadBytes(buffer, size, offset+2, b, 0);
			output = Encoding.UTF8.GetString(b);
			return size + 2;
		}

		// code below reused from lidgrin.network
		// https://github.com/lidgren/lidgren-network-gen3
		/* Copyright (c) 2010 Michael Lidgren

		Permission is hereby granted, free of charge, to any person obtaining a copy of this software
		and associated documentation files (the "Software"), to deal in the Software without
		restriction, including without limitation the rights to use, copy, modify, merge, publish,
		distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom
		the Software is furnished to do so, subject to the following conditions:

		The above copyright notice and this permission notice shall be included in all copies or
		substantial portions of the Software.

		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
		INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
		PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
		LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
		TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
		USE OR OTHER DEALINGS IN THE SOFTWARE.

		*/
		public static byte ReadByte(byte[] fromBuffer, int numberOfBits, int readBitOffset)
		{
			int bytePtr = readBitOffset >> 3;
			int startReadAtIndex = readBitOffset - (bytePtr * 8); // (readBitOffset % 8);

			if(startReadAtIndex == 0 && numberOfBits == 8)
				return fromBuffer[bytePtr];

			// mask away unused bits lower than (right of) relevant bits in first byte
			byte returnValue = (byte)(fromBuffer[bytePtr] >> startReadAtIndex);

			int numberOfBitsInSecondByte = numberOfBits - (8 - startReadAtIndex);

			if(numberOfBitsInSecondByte < 1)
			{
				// we don't need to read from the second byte, but we DO need
				// to mask away unused bits higher than (left of) relevant bits
				return (byte)(returnValue & (255 >> (8 - numberOfBits)));
			}

			byte second = fromBuffer[bytePtr + 1];

			// mask away unused bits higher than (left of) relevant bits in second byte
			second &= (byte)(255 >> (8 - numberOfBitsInSecondByte));

			return (byte)(returnValue | (byte)(second << (numberOfBits - numberOfBitsInSecondByte)));
		}

		public static void ReadBytes(byte[] fromBuffer, int numberOfBytes, int readBitOffset, byte[] destination, int destinationByteOffset)
		{
			int readPtr = readBitOffset >> 3;
			int startReadAtIndex = readBitOffset - (readPtr * 8); // (readBitOffset % 8);

			if(startReadAtIndex == 0)
			{
				Buffer.BlockCopy(fromBuffer, readPtr, destination, destinationByteOffset, numberOfBytes);
				return;
			}

			int secondPartLen = 8 - startReadAtIndex;
			int secondMask = 255 >> secondPartLen;

			for(int i = 0; i < numberOfBytes; i++)
			{
				// mask away unused bits lower than (right of) relevant bits in byte
				int b = fromBuffer[readPtr] >> startReadAtIndex;

				readPtr++;

				// mask away unused bits higher than (left of) relevant bits in second byte
				int second = fromBuffer[readPtr] & secondMask;

				destination[destinationByteOffset++] = (byte)(b | (second << secondPartLen));
			}

			return;
		}

		public static ushort ReadUInt16(byte[] fromBuffer, int numberOfBits, int readBitOffset)
		{
			ushort returnValue;
			if(numberOfBits <= 8)
			{
				returnValue = ReadByte(fromBuffer, numberOfBits, readBitOffset);
				return returnValue;
			}
			returnValue = ReadByte(fromBuffer, 8, readBitOffset);
			numberOfBits -= 8;
			readBitOffset += 8;

			if(numberOfBits <= 8)
			{
				returnValue |= (ushort)(ReadByte(fromBuffer, numberOfBits, readBitOffset) << 8);
			}

			if(!BitConverter.IsLittleEndian)
			{
				// reorder bytes
				uint retVal = returnValue;
				retVal = ((retVal & 0x0000ff00) >> 8) | ((retVal & 0x000000ff) << 8);
				return (ushort)retVal;
			}
			return returnValue;
		}

		public static uint ReadUInt32(byte[] fromBuffer, int numberOfBits, int readBitOffset)
		{
			uint returnValue;
			if(numberOfBits <= 8)
			{
				returnValue = ReadByte(fromBuffer, numberOfBits, readBitOffset);
				return returnValue;
			}
			returnValue = ReadByte(fromBuffer, 8, readBitOffset);
			numberOfBits -= 8;
			readBitOffset += 8;

			if(numberOfBits <= 8)
			{
				returnValue |= (uint)(ReadByte(fromBuffer, numberOfBits, readBitOffset) << 8);
				return returnValue;
			}
			returnValue |= (uint)(ReadByte(fromBuffer, 8, readBitOffset) << 8);
			numberOfBits -= 8;
			readBitOffset += 8;

			if(numberOfBits <= 8)
			{
				uint r = ReadByte(fromBuffer, numberOfBits, readBitOffset);
				r <<= 16;
				returnValue |= r;
				return returnValue;
			}
			returnValue |= (uint)(ReadByte(fromBuffer, 8, readBitOffset) << 16);
			numberOfBits -= 8;
			readBitOffset += 8;

			returnValue |= (uint)(ReadByte(fromBuffer, numberOfBits, readBitOffset) << 24);

			if(!BitConverter.IsLittleEndian)
			{
				// reorder bytes
				return
				((returnValue & 0xff000000) >> 24) |
				((returnValue & 0x00ff0000) >> 8) |
				((returnValue & 0x0000ff00) << 8) |
				((returnValue & 0x000000ff) << 24);
			}
			return returnValue;
		}

		/// <summary>
		/// Reads the specified number of bytes
		/// </summary>
		public static byte[] ReadBytes(byte[] buffer, int offset, int numberOfBytes)
		{
			byte[] retval = new byte[numberOfBytes];
			ReadBytes(buffer, numberOfBytes, offset, retval, 0);
			return retval;
		}
	}
}
