using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Info
{
    public class MsgScore : NetworkMessage
    {
        public int PlayerID = 0;
        public int Wins = 0;
        public int Losses = 0;
        public int TeamKills = 0;

        public MsgScore()
        {
            Code = CodeFromChars("sc");
        }

        public override byte[] Pack()
        {
            var buffer = Data.Utils.DynamicOutputBuffer.Get(Code);

            buffer.WriteByte(1);
            buffer.WriteByte(PlayerID);
            buffer.WriteUInt16(Wins);
            buffer.WriteUInt16(Losses);
            buffer.WriteUInt16(TeamKills);

            return buffer.GetMessageBuffer();

        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            int count = ReadByte();

            PlayerID = ReadByte();
            Wins = ReadUInt16();
            Losses = ReadUInt16();
            TeamKills = ReadUInt16();
        }
    }
}
