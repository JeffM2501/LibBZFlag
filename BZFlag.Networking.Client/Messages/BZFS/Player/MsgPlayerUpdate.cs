using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Players;
using BZFlag.Data.Types;

namespace BZFlag.Networking.Messages.BZFS.Player
{

    public class MsgPlayerUpdateBase : NetworkMessage
    {
        public float TimeStamp = float.MinValue;
        public int PlayerID = -1;
        public int Order = -1;
        public PlayerStatuses Status = PlayerStatuses.DeadStatus;

        public Vector3F Position = Vector3F.Zero;
        public Vector3F Velocity = Vector3F.Zero;
        public float Azimuth = 0;
        public float AngularVelocity = 0;

        public float JumpSquish = 0;
        public int OnDriver = -1;
        public float UserSpeed = 0;
        public float UserAngVel = 0;

        public PlayerStatusSounds Sounds = PlayerStatusSounds.NoSounds;

        protected void UnpackHeader(byte[] data)
        {
            TimeStamp = ReadFloat(data);
            PlayerID = ReadByte(data);

            Order = ReadInt32(data);
            Status = (PlayerStatuses)ReadInt16(data);
        }

        protected void UnpackFooter(byte[] data)
        {
            if (Status == PlayerStatuses.JumpJets)
                JumpSquish = ReadSmallScale(data);

            if (Status == PlayerStatuses.OnDriver)
                OnDriver = ReadInt32(data);

            if (Status == PlayerStatuses.UserInputs)
            {
                UserSpeed = ReadSmallVel(data);
                UserAngVel = ReadSmallAngVel(data);
            }

            if (Status == PlayerStatuses.PlaySound)
                Sounds = (PlayerStatusSounds)ReadByte(data);
        }

        protected void PackHeader(DynamicOutputBuffer buffer)
        {
            buffer.WriteFloat(TimeStamp);
            buffer.WriteByte((byte)PlayerID);

            buffer.WriteInt32(Order);
            buffer.WriteInt16((Int16)Status);
        }

        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            throw new NotImplementedException();
        }
    }

    public class MsgPlayerUpdate : MsgPlayerUpdateBase
    {
        public MsgPlayerUpdate()
        {
            Code = CodeFromChars("pu");
        }

        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();
            UnpackHeader(data);

            Position = ReadVector3F(data);
            Velocity = ReadVector3F(data);
            Azimuth = ReadFloat(data);
            AngularVelocity = ReadFloat(data);

            UnpackFooter(data);
        }
    }

    public class MsgPlayerUpdateSmall : MsgPlayerUpdateBase
    {
        public MsgPlayerUpdateSmall()
        {
            Code = CodeFromChars("ps");
        }

        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();
            UnpackHeader(data);

            Position = ReadSmallVector3F(data);
            Velocity = ReadSmallVelVector3F(data);
            Azimuth = ReadSmallAngle(data);
            AngularVelocity = ReadSmallAngVel(data);

            UnpackFooter(data);
        }
    }
}
