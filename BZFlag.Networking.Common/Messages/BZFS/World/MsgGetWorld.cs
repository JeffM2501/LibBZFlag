using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.World
{
    public class MsgGetWorld : NetworkMessage
    {
        public UInt32 Offset = 0;
        public byte[] Data = new byte[0];

        public MsgGetWorld()
        {
            Code = CodeFromChars("gw");
        }

        public MsgGetWorld(UInt32 offset)
        {
            Code = CodeFromChars("gw");
            Offset = offset;
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);
            buffer.WriteUInt32(Offset);

            if (Data.Length > 0)
                buffer.WriteBytes(Data);
            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            Offset = ReadUInt32();
            Data = ReadRestOfBytes();
        }
    }
}
