using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Players;
using BZFlag.Data.Types;
using BZFlag.Data.Utils;
using BZFlag.LinearMath;

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
            TimeStamp = ReadFloat();						//4
            PlayerID = ReadByte();							// 5

            Order = ReadInt32();							// 9
            Status = (PlayerStatuses)ReadInt16();			// 11
        }

        protected void UnpackFooter()
        {
            if (Status == PlayerStatuses.JumpJets)
                JumpSquish = ReadSmallScale();				//2

            if (Status == PlayerStatuses.OnDriver)
                OnDriver = ReadInt32();						//4

            if (Status == PlayerStatuses.UserInputs)
            {
                UserSpeed = ReadSmallVel();					//6
                UserAngVel = ReadSmallAngVel();				//8
            }

            if (Status == PlayerStatuses.PlaySound)
                Sounds = (PlayerStatusSounds)ReadByte();	//9
        }

        protected void PackHeader(DynamicOutputBuffer buffer)
        {
            buffer.WriteFloat(TimeStamp);
            buffer.WriteByte((byte)PlayerID);

            buffer.WriteInt32(Order);
            buffer.WriteInt16((Int16)Status);
        }

        protected void PackFooter(DynamicOutputBuffer buffer)
        {
            if (Status == PlayerStatuses.JumpJets)
                buffer.WriteSmallScale(JumpSquish);				//2

            if (Status == PlayerStatuses.OnDriver)
               buffer.WriteInt32(OnDriver);						//4

            if (Status == PlayerStatuses.UserInputs)
            {
                buffer.WriteSmallVel(UserSpeed);					//6
                buffer.WriteSmallAngVel(UserAngVel);				//8
            }

            if (Status == PlayerStatuses.PlaySound)
                buffer.WriteByte((byte)Sounds);	                //9
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
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);

            PackHeader(buffer);

            buffer.WriteVector3F(Position);
            buffer.WriteVector3F(Velocity);

            buffer.WriteFloat(Azimuth);
            buffer.WriteFloat(AngularVelocity);

            PackFooter(buffer);
            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            UnpackHeader();							//11

            Position = ReadVector3F();				// 23
            Velocity = ReadVector3F();				// 35
            Azimuth = ReadFloat();					// 39
            AngularVelocity = ReadFloat();			// 43

            UnpackFooter();							// 52
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
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);

            PackHeader(buffer);

            buffer.WriteSmallVector3F(Position);
            buffer.WriteSmallVelVector3F(Velocity);

            buffer.WriteSmallAngle(Azimuth);
            buffer.WriteSmallAngVel(AngularVelocity);

            PackFooter(buffer);
            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            UnpackHeader();							// 11

            Position = ReadSmallVector3F();			// 17
            Velocity = ReadSmallVelVector3F();		// 23
            Azimuth = ReadSmallAngle();				// 25
            AngularVelocity = ReadSmallAngVel();	// 27

            UnpackFooter();							// 38
        }
    }
}
