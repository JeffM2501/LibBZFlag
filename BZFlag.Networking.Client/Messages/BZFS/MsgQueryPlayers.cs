using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS
{
    public class MsgQueryPlayers : NetworkMessage
    {
        public int NumTeams = 0;
        public int NumPlayers = 0;

        public MsgQueryPlayers()
        {
            Code = CodeFromChars("qp");
        }

        public override byte[] Pack()
        {
            return new DynamicOutputBuffer(Code).GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();
            NumTeams = ReadUInt16(data);
            NumPlayers = ReadUInt16(data);
        }
    }
}
