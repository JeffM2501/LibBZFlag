using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.Data.Flags;
using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages
{
    public abstract class NetworkMessage : DynamicBufferReader
    {
        public static bool IsOnServer = false;

        public bool IsServer()
        {
            return IsOnServer;
        }

        public int Code = int.MinValue;
        public string CodeAbreviation = string.Empty;

        public bool FromUDP = false;
        public object Tag = null;

        public abstract void Unpack(byte[] data);
        public abstract byte[] Pack();

        public int CodeFromChars(string msgCode)
        {
            if (msgCode.Length < 2)
                msgCode = " " + msgCode;

            CodeAbreviation = msgCode;

            byte[] b = Encoding.ASCII.GetBytes(msgCode.Substring(0, 2));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return (int)BitConverter.ToUInt16(b, 0);
        }

        protected float ReadSmallDist()
        {
            return ((float)ReadInt16() * Constants.SmallMaxDist) / Constants.SmallScale;
        }

        protected float ReadSmallAngle()
        {
            return ((float)(ReadInt16() * Math.PI)) / Constants.SmallScale;
        }

        protected float ReadSmallScale()
        {
            return ((float)ReadInt16()) / Constants.SmallScale;
        }

        protected float ReadSmallVel()
        {
            return ((float)ReadUInt16() * Constants.SmallMaxVel) / Constants.SmallScale;
        }

        protected float ReadSmallAngVel()
        {
            return ((float)ReadUInt16() * Constants.SmallMaxAngVel) / Constants.SmallScale;
        }

        protected Vector3F ReadSmallVector3F()
        {
            return new Vector3F(ReadSmallDist(), ReadSmallDist(), ReadSmallDist());
        }
        protected Vector3F ReadSmallVelVector3F()
        {
            return new Vector3F(ReadSmallVel(), ReadSmallVel(), ReadSmallVel());
        }

        protected FlagUpdateData ReadFlagUpdateData()
        {
            FlagUpdateData flag = new FlagUpdateData();
            flag.FlagID = ReadUInt16();
            flag.Abreviation = ReadFixedSizeString(2);
            flag.Status = (FlagStatuses)ReadUInt16();
            flag.Endurance = (FlagEndurances)ReadUInt16();
            flag.Owner = ReadByte();
            flag.Postion = ReadVector3F();
            flag.LaunchPosition = ReadVector3F();
            flag.LandingPostion = ReadVector3F();
            flag.FlightTime = ReadFloat();
            flag.FlightEnd = ReadFloat();
            flag.InitalVelocity = ReadFloat();

            return flag;
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