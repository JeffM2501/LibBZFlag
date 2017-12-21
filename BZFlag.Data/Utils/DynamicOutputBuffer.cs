using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.LinearMath;
using BZFlag.Data.Flags;

namespace BZFlag.Data.Utils
{
    public class DynamicOutputBuffer
    {
        public static DynamicOutputBuffer Get( int code )
        {
            lock(BufferCache)
            {
                foreach(var db in BufferCache)
                {
                    if (!db.Used())
                    {
                        db.Reset(code);
                        return db;
                    }
                }

                DynamicOutputBuffer b = new DynamicOutputBuffer(code);
                b.Reset(code);
                BufferCache.Add(b);
                return b;
            }
        }

        public static DynamicOutputBuffer GetTempBuffer( int size)
        {
            return new DynamicOutputBuffer(false, size);
        }

        private static List<DynamicOutputBuffer> BufferCache = new List<DynamicOutputBuffer>();

        protected UInt16 Code = 0;

        private byte[] GlobalBuffer = new byte[2048];
        protected int BytesUsed = 0;

        protected byte[] Buffer = null;


        protected bool InUse = false;
        public object Locker = new object();


        public int GetBytesUsed()
        {
            return BytesUsed;
        }

        public byte[] GetRawBuffer()
        {
            return Buffer;
        }

        public bool Used ()
        {
            lock (Locker)
                return InUse;
        }

        protected void SetUnused()
        {
            lock (Locker)
                InUse = false;
        }

        protected void Reset(int code)
        {
            lock (Locker)
                InUse = true;

            BytesUsed = 4;
            Buffer = GlobalBuffer;
            Code = (UInt16)code;
            WriteUInt16(0, 0);
            WriteUInt16(Code, 2);
        }

        protected DynamicOutputBuffer()
        {
            BytesUsed = 4;
            Buffer = GlobalBuffer;
        }

        protected DynamicOutputBuffer(int code)
        {
            BytesUsed = 4;
            Buffer = GlobalBuffer;
            Code = (UInt16)code;
            WriteUInt16(0, 0);
            WriteUInt16(Code, 2);
        }

         protected DynamicOutputBuffer(bool useGlobal, int size)
         {
             BytesUsed = 0;
 
             if (useGlobal)
                 Buffer = GlobalBuffer;
             else
                 Buffer = new byte[size];
         }


        public void SetCode(int code)
        {
            Code = (UInt16)code;
            WriteUInt16(Code, 2);
        }

        public byte[] GetMessageBuffer()
        {
            WriteUInt16((UInt16)(BytesUsed - 4), 0);
            byte[] outbuffer = new byte[BytesUsed];
            Array.Copy(Buffer, outbuffer, BytesUsed);

            SetUnused();
            return outbuffer;
        }

        public byte[] GetFinalBuffer()
        {
            byte[] outbuffer = new byte[BytesUsed];
            Array.Copy(Buffer, outbuffer, BytesUsed);

            SetUnused();
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

        public void WriteUInt16(int value)
        {
            WriteUInt16((UInt16)value);
        }

        protected void WriteUInt16(UInt16 value, int offset)
        {
            CheckBuffer(2);

            var b = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);

            Array.Copy(b, 0, Buffer, offset, 2);
        }

        public void WriteInt16(int value)
        {
            WriteInt16((Int16)value);
        }

        public void WriteInt16(Int16 value)
        {
            CheckBuffer(2);

            var b = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);

            Array.Copy(b, 0, Buffer, BytesUsed, 2);
            BytesUsed += 2;
        }

        public void WriteUInt32(int value)
        {
            WriteUInt32((UInt32)value);
        }

        public void WriteUInt32(UInt32 value)
        {
            CheckBuffer(4);

            var b = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);

            Array.Copy(b, 0, Buffer, BytesUsed, 4);
            BytesUsed += 4;
        }

        public void WriteInt32(Int32 value)
        {
            CheckBuffer(4);

            var b = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);

            Array.Copy(b, 0, Buffer, BytesUsed, 4);
            BytesUsed += 4;
        }

        public void WriteUInt64(UInt64 value)
        {
            CheckBuffer(8);

            var b = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);

            Array.Copy(b, 0, Buffer, BytesUsed, 4);
            BytesUsed += 8;
        }

        public void WriteInt64(Int64 value)
        {
            CheckBuffer(8);

            var b = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);

            Array.Copy(b, 0, Buffer, BytesUsed, 4);
            BytesUsed += 8;
        }

        public void WriteFloat(float value)
        {
            CheckBuffer(4);

            var b = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);

            Array.Copy(b, 0, Buffer, BytesUsed, 4);
            BytesUsed += 4;
        }

        public void WriteVector2F(Vector2F value)
        {
            WriteFloat(value.X);
            WriteFloat(value.Y);
        }

        public void WriteVector3F(Vector3F value)
        {
            WriteFloat(value.X);
            WriteFloat(value.Y);
            WriteFloat(value.Z);
        }

        public void WriteVector4F(Vector4F value)
        {
            WriteFloat(value.A);
            WriteFloat(value.X);
            WriteFloat(value.Y);
            WriteFloat(value.Z);
        }

        public void WriteColor4F(Color4F value)
        {
            WriteFloat(value.R);
            WriteFloat(value.G);
            WriteFloat(value.B);
            WriteFloat(value.A);
        }

        public void WriteSmallVector3F(Vector3F value)
        {
            WriteSmallDist(value.X);
            WriteSmallDist(value.Y);
            WriteSmallDist(value.Z);
        }

        public void WriteSmallVelVector3F(Vector3F value)
        {
            WriteSmallVel(value.X);
            WriteSmallVel(value.Y);
            WriteSmallVel(value.Z);
        }

        public void WriteDouble(double value)
        {
            CheckBuffer(8);

            var b = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);

            Array.Copy(b, 0, Buffer, BytesUsed, 8);
            BytesUsed += 4;
        }

        public void WriteFixedSizeString(string value, int size)
        {
            CheckBuffer(size);

            int i = value.Length;
            if (i > size)
                i = size;

            Encoding.UTF8.GetBytes(value, 0, i, Buffer, BytesUsed);

            if (i < size)
            {
                while (i < size)
                {
                    Buffer[BytesUsed + i] = 0;
                    i++;
                }
            }
            BytesUsed += size;
        }

        public void WritePascalString(string value)
        {
            int actualSize = value.Length;
            if (actualSize > Byte.MaxValue)
                actualSize = Byte.MaxValue;

            CheckBuffer(actualSize + 1);

            Buffer[BytesUsed] = (byte)actualSize;

            Encoding.UTF8.GetBytes(value, 0, actualSize, Buffer, BytesUsed + 1);
            BytesUsed += actualSize + 1;
        }

        public void WriteUShortPascalString(string value)
        {
            int actualSize = value.Length;
            if (actualSize > UInt16.MaxValue)
                actualSize = UInt16.MaxValue;

            CheckBuffer(actualSize + 2);

            WriteUInt16((UInt16)actualSize, BytesUsed);

            Encoding.UTF8.GetBytes(value, 0, actualSize, Buffer, BytesUsed + 2);
            BytesUsed += actualSize + 2;
        }

        public void WriteULongPascalString(string value)
        {
            int actualSize = value.Length;
            if (actualSize > int.MaxValue)
                actualSize = int.MaxValue;

            CheckBuffer(actualSize + 4);

            WriteUInt32((UInt32)actualSize);

            Encoding.UTF8.GetBytes(value, 0, actualSize, Buffer, BytesUsed);
            BytesUsed += actualSize;
        }
    
        public void WriteNullTermString(string value)
        {
            CheckBuffer(value.Length + 1);
            Encoding.UTF8.GetBytes(value, 0, value.Length, Buffer, BytesUsed);
            BytesUsed += value.Length;
            WriteByte(byte.MinValue);
        }

        public void WriteFlagUpdateData(FlagUpdateData flag)
        {
            WriteInt16(flag.FlagID);
            WriteFixedSizeString(flag.Abreviation, 2);
            WriteUInt16((UInt16)flag.Status);
            WriteUInt16((UInt16)flag.Endurance);
            WriteByte(flag.Owner);
            WriteVector3F(flag.Postion);
            WriteVector3F(flag.LaunchPosition);
            WriteVector3F(flag.LandingPostion);
            WriteFloat(flag.FlightTime);
            WriteFloat(flag.FlightEnd);
            WriteFloat(flag.InitalVelocity);
        }

        public void WriteSmallDist(float val)
        {
            WriteInt16((Int16)((val * Constants.SmallScale) / Constants.SmallMaxDist));
        }

        public void WriteSmallAngle(float val)
        {
            WriteInt16((Int16)((val * Constants.SmallScale) / System.Math.PI));
        }

        public void WriteSmallScale(float val)
        {
            WriteInt16((Int16)(val * Constants.SmallScale));
        }


        public void WriteSmallVel(float val)
        {
            WriteSmallScale(val / Constants.SmallMaxVel);
        }

        public void WriteSmallAngVel(float val)
        {
            WriteSmallScale(val / Constants.SmallMaxAngVel);
        }
    }
}
