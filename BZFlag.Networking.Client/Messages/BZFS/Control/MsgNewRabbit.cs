using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Control
{
    public class MsgNewRabbit : NetworkMessage
    {
        public int PlayerID = -1;
 
        public MsgNewRabbit()
        {
            Code = CodeFromChars("nR");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);
            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();
            PlayerID = ReadByte(data);
        }
    }
}
