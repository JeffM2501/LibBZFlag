using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Flags
{
    public class MsgDropFlag : NetworkMessage
    {
        public int PlayerID = -1;
        public int FlagID = -1;

        public MsgDropFlag()
        {
            Code = CodeFromChars("df");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);
            buffer.WriteByte(PlayerID);
            buffer.WriteUInt16((UInt16)FlagID);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            FlagID = ReadByte();
            FlagID = ReadUInt16();
        }
    }
}
