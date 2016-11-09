
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BZFlag.Data.Teams;

namespace BZFlag.Networking.Messages.BZFS.Flags
{
    public class MsgCaptureFlag : NetworkMessage
    {
        public int PlayerID = -1;
        public int FlagID = -1;
        public TeamColors Team = TeamColors.NoTeam;

        public MsgCaptureFlag()
        {
            Code = CodeFromChars("cf");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);

            buffer.WriteByte(PlayerID);
            buffer.WriteUInt16((UInt16)FlagID);
            buffer.WriteInt16((Int16)Team);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();

            PlayerID = ReadByte(data);
            FlagID = ReadUInt16(data);
            Team = (TeamColors)ReadInt16(data);
        }
    }
}
