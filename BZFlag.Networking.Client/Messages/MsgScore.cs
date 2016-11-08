using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages
{
    public class MsgScore : NetworkMessage
    {
        public int PlayerID = -1;
        public int Wins = 0;
        public int Losses = 0;
        public int TeamKills = 0;

        public MsgScore()
        {
            Code = CodeFromChars("sc");
        }
        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();
            PlayerID = ReadByte(data);
            Wins = ReadUInt16(data);
            Losses = ReadUInt16(data);
            TeamKills = ReadUInt16(data);
        }
    }
}
