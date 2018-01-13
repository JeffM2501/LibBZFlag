using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Info
{
    public class MsgGameTime : NetworkMessage
    {
        public Int64 NetTime = Int64.MinValue;

        public MsgGameTime()
        {
            Code = CodeFromChars("gt");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);
            buffer.WriteInt64(NetTime);
            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            NetTime = ReadInt64();
        }
    }
}
