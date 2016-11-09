using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Control
{
    public class MsgAutoPilot : NetworkMessage
    {
        public int PlayerID = -1;
        public bool AutoPilot = false;

        public MsgAutoPilot()
        {
            Code = CodeFromChars("au");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);

            buffer.WriteByte(PlayerID);
            buffer.WriteByte(AutoPilot ? 1 : 0);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();
            PlayerID = ReadByte(data);
            AutoPilot = ReadByte(data) != 0;
        }
    }
}
