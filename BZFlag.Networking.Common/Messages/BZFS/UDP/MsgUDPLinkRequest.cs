using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.UDP
{
    public class MsgUDPLinkRequest : NetworkMessage
    {
        int PlayerID = -1;
        public MsgUDPLinkRequest()
        {
            Code = CodeFromChars("of");
        }

        public MsgUDPLinkRequest(int pid)
        {
            Code = CodeFromChars("of");
            PlayerID = pid;
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);
            buffer.WriteByte((byte)PlayerID);
            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            PlayerID = ReadByte();
        }
    }
}
