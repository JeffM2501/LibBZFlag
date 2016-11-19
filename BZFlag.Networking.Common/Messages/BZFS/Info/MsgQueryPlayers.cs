using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Info
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
            Reset(data);
            NumTeams = ReadUInt16();
            NumPlayers = ReadUInt16();
        }
    }
}
