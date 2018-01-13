using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;
using BZFlag.LinearMath;

namespace BZFlag.Networking.Messages.BZFS.Flags
{
    public class MsgTransferFlag : NetworkMessage
    {
        public int FromID = -1;
        public int ToID = -1;
        public int FlagID = -1;

        public MsgTransferFlag()
        {
            Code = CodeFromChars("tf");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);

            buffer.WriteByte(FromID);
            buffer.WriteByte(ToID);

            if (IsServer())
                buffer.WriteUInt16((UInt16)FlagID);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            FromID = ReadByte();
            ToID = ReadByte();
            if (!IsServer())
                FlagID = ReadUInt16();
        }
    }
}
