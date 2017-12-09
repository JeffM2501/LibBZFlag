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
    public class MsgGMUpdate : NetworkMessage
    {
        public int PlayerID = -1;
        public int ShotID = -1;
        public Vector3F Position = Vector3F.Zero;
        public Vector3F Velocity = Vector3F.Zero;
        public float DeltaTime = 0;
        public TeamColors Team = TeamColors.NoTeam;

        public int TargetID = -1;

        public MsgGMUpdate()
        {
            Code = CodeFromChars("gm");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);

            buffer.WriteByte(PlayerID);
            buffer.WriteUInt16((UInt16)ShotID);
            buffer.WriteVector3F(Position);
            buffer.WriteVector3F(Velocity);
            buffer.WriteFloat(DeltaTime);
            buffer.WriteInt16((Int16)Team);
            buffer.WriteByte(TargetID);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            PlayerID = ReadByte();
            ShotID = ReadUInt16();
            Position = ReadVector3F();
            Velocity = ReadVector3F();
            DeltaTime = ReadFloat();
            Team = (TeamColors)ReadInt16();
            TargetID = ReadByte();
        }
    }
}
