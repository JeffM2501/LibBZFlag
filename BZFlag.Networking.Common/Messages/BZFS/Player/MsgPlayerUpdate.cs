using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Players;
using BZFlag.Data.Types;
using BZFlag.Data.Utils;

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

        protected void UnpackHeader()
        {
            TimeStamp = ReadFloat();
            PlayerID = ReadByte();

            Order = ReadInt32();
            Status = (PlayerStatuses)ReadInt16();
        }

        protected void UnpackFooter()
        {
            if (Status == PlayerStatuses.JumpJets)
                JumpSquish = ReadSmallScale();

            if (Status == PlayerStatuses.OnDriver)
                OnDriver = ReadInt32();

            if (Status == PlayerStatuses.UserInputs)
            {
                UserSpeed = ReadSmallVel();
                UserAngVel = ReadSmallAngVel();
            }

            if (Status == PlayerStatuses.PlaySound)
                Sounds = (PlayerStatusSounds)ReadByte();
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
            Reset(data);
            UnpackHeader();

            Position = ReadVector3F();
            Velocity = ReadVector3F();
            Azimuth = ReadFloat();
            AngularVelocity = ReadFloat();

            UnpackFooter();
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
            Reset(data);
            UnpackHeader();

            Position = ReadSmallVector3F();
            Velocity = ReadSmallVelVector3F();
            Azimuth = ReadSmallAngle();
            AngularVelocity = ReadSmallAngVel();

            UnpackFooter();
        }
    }
}
