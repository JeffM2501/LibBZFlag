using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.Data.Teams;
using BZFlag.Data.Utils;
using BZFlag.LinearMath;

namespace BZFlag.Networking.Messages.BZFS.Shots
{
    public class MsgShotBegin : NetworkMessage
    {
        public float TimeSent = 0;
        public int PlayerID = -1;
        public int ShotID = -1;

        public Vector3F Position = Vector3F.Zero;
        public Vector3F Velocity = Vector3F.Zero;

        public float DeltaTime = 0;

        public TeamColors Team = TeamColors.NoTeam;
        public string Flag = string.Empty;

        public float Lifetime = float.MinValue;

        public MsgShotBegin()
        {
            Code = CodeFromChars("sb");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);
            buffer.WriteFloat(TimeSent);
            buffer.WriteByte(PlayerID);
            buffer.WriteInt16(ShotID);
            buffer.WriteVector3F(Position);
            buffer.WriteVector3F(Velocity);
            buffer.WriteFloat(DeltaTime);

            buffer.WriteInt16((UInt16)Team);

            buffer.WriteFixedSizeString(Flag, 2);
            buffer.WriteFloat(Lifetime);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);

            TimeSent = ReadFloat();
            PlayerID = ReadByte();
            ShotID = ReadUInt16();

            Position = ReadVector3F();
            Velocity = ReadVector3F();

            DeltaTime = ReadFloat();
            Team = (TeamColors)ReadInt16();

            Flag = ReadFixedSizeString(2);
            Lifetime = ReadFloat();
        }
    }
}
