using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Control
{
    public class MsgTimeUpdate : NetworkMessage
    {
        public int TimeLeft = -1;

        public MsgTimeUpdate()
        {
            Code = CodeFromChars("to");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);

            buffer.WriteInt32(TimeLeft);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();
            TimeLeft = ReadInt32(data);
        }
    }
}
