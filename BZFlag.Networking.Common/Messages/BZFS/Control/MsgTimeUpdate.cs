using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;

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
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);

            buffer.WriteInt32(TimeLeft);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            TimeLeft = ReadInt32();
        }
    }
}
