using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS
{
    public class MsgLagPing : NetworkMessage
    {
        public UInt16 SequenceNumber = UInt16.MaxValue;

        public MsgLagPing()
        {
            Code = CodeFromChars("pi");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);
            buffer.WriteUInt16(SequenceNumber);
            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();
            SequenceNumber = ReadUInt16(data);
        }
    }
}
