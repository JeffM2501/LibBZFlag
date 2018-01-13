using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Info
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
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);
            buffer.WriteUInt16(SequenceNumber);
            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            SequenceNumber = ReadUInt16();
        }
    }
}
