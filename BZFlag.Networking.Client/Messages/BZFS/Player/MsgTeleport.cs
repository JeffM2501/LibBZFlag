using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Player
{
    public class MsgTeleport : NetworkMessage
    {
        public int PlayerID = -1;
        public int FromTPID = -1;
        public int ToTPID = -1;

        public MsgTeleport()
        {
            Code = CodeFromChars("tp");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);
            buffer.WriteByte(PlayerID);
            buffer.WriteUInt16((UInt16)FromTPID);
            buffer.WriteUInt16((UInt16)ToTPID);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();

            PlayerID = ReadByte(data);
            FromTPID = ReadUInt16(data);
            ToTPID = ReadUInt16(data);

        }
    }
}
