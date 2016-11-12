using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;

namespace BZFlag.Data.Utils
{
    public abstract class DynamicBufferReader
    {
        protected byte[] Buffer = new byte[0];
        protected int BufferOffset = 0;

        public DynamicBufferReader()
        {
            BufferOffset = 0;
        }

        public DynamicBufferReader(byte[] data)
        {
            Buffer = data;
            BufferOffset = 0;
        }

        public void Reset(byte[] data)
        {
            Buffer = data;
            BufferOffset = 0;
        }

        public byte ReadByte()
        {
            if (Buffer.Length < BufferOffset + 1)
                return 0;

            BufferOffset += 1;
            return Buffer[BufferOffset - 1];
        }

        public int Size()
        {
            return Buffer.Length;
        }

        public byte[] ReadBytes( int size)
        {
            if (Buffer.Length < BufferOffset + size)
                return new byte[0];

            byte[] d = new byte[size];
            Array.Copy(Buffer, BufferOffset, d, 0, size);
            BufferOffset += size;
            return d;
        }

        public byte[] ReadRestOfBytes()
        {
            byte[] d = new byte[Buffer.Length - BufferOffset];
            Array.Copy(Buffer, BufferOffset, d, 0, d.Length);
            BufferOffset = Buffer.Length;
            return d;
        }

        public UInt16 ReadUInt16()
        {
            if (Buffer.Length < BufferOffset + 2)
                return 0;

            BufferOffset += 2;
            return BufferUtils.ReadUInt16(Buffer, BufferOffset - 2);
        }

        public Int16 ReadInt16()
        {
            if (Buffer.Length < BufferOffset + 2)
                return 0;

            BufferOffset += 2;
            return BufferUtils.ReadInt16(Buffer, BufferOffset - 2);
        }

        public UInt32 ReadUInt32()
        {
            if (Buffer.Length < BufferOffset + 4)
                return 0;

            BufferOffset += 4;
            return BufferUtils.ReadUInt32(Buffer, BufferOffset - 4);
        }

        public Int32 ReadInt32()
        {
            if (Buffer.Length < BufferOffset + 4)
                return 0;

            BufferOffset += 4;
            return BufferUtils.ReadInt32(Buffer, BufferOffset - 4);
        }

        public UInt64 ReadUInt64()
        {
            if (Buffer.Length < BufferOffset + 4)
                return 0;

            BufferOffset += 8;
            return BufferUtils.ReadUInt64(Buffer, BufferOffset - 8);
        }

        public Int64 ReadInt64()
        {
            if (Buffer.Length < BufferOffset + 8)
                return 0;

            BufferOffset += 8;
            return BufferUtils.ReadInt64(Buffer, BufferOffset - 8);
        }

        public float ReadFloat()
        {
            if (Buffer.Length < BufferOffset + 4)
                return 0;

            BufferOffset += 4;
            return BufferUtils.ReadSingle(Buffer, BufferOffset - 4);
        }

        public double ReadDouble()
        {
            if (Buffer.Length < BufferOffset + 8)
                return 0;

            BufferOffset += 8;
            return BufferUtils.ReadDouble(Buffer, BufferOffset - 8);
        }
		public Color4F ReadColor4F()
		{
			return new Color4F(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
		}

		public Vector4F ReadVector4F()
        {
            return new Vector4F(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Vector3F ReadVector3F()
        {
            return new Vector3F(ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Vector2F ReadVector2F()
        {
            return new Vector2F(ReadFloat(), ReadFloat());
        }


        public string ReadFixedSizeString(int size)
        {
            if (Buffer.Length < BufferOffset + size)
                return string.Empty;

            string s = Encoding.UTF8.GetString(Buffer, BufferOffset, size);
            BufferOffset += size;
            return s.TrimEnd(new char[] { '\0' });
        }

        public string ReadPascalString()
        {
            if (Buffer.Length < BufferOffset + 1)
                return string.Empty;

            byte len = Buffer[BufferOffset];

            if (Buffer.Length < BufferOffset + 1 + len)
                return string.Empty;

            string s = Encoding.UTF8.GetString(Buffer, BufferOffset + 1, len);
            BufferOffset += len + 1;
            return s;
        }

        public string ReadUShortPascalString()
        {
            if (Buffer.Length < BufferOffset + 2)
                return string.Empty;

            int len = BufferUtils.ReadUInt16(Buffer, BufferOffset);

            if (Buffer.Length < BufferOffset + 2 + len)
                return string.Empty;

            string s = Encoding.UTF8.GetString(Buffer, BufferOffset + 2, len);
            BufferOffset += len + 2;
            return s;
        }

        public string ReadULongPascalString()
        {
            if (Buffer.Length < BufferOffset + 4)
                return string.Empty;

            int len = (int)BufferUtils.ReadUInt32(Buffer, BufferOffset);

            if (Buffer.Length < BufferOffset + 4 + len)
                return string.Empty;

            string s = Encoding.UTF8.GetString(Buffer, BufferOffset + 4, len);
            BufferOffset += len + 4;
            return s;
        }

        public string ReadNullTermString(bool readToEnd)
        {
            if (readToEnd)
            {
                int start = BufferOffset;
                BufferOffset = Buffer.Length;
                return Encoding.UTF8.GetString(Buffer, start, BufferOffset - start - 1);
            }
            else
            {
                int end = Array.FindIndex(Buffer, BufferOffset, x => x == byte.MinValue);
                if (end == -1)
                    return string.Empty;
                string ret = Encoding.UTF8.GetString(Buffer, BufferOffset, BufferOffset - end);
                BufferOffset = end + 1;
                return ret;
            }
        }
    }
}
