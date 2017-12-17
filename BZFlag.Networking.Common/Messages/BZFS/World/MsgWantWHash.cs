using BZFlag.Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.World
{
    public class MsgWantWHash : NetworkMessage
    {
        public bool IsRandomMap = true;

        public string WorldHash = string.Empty;

        public MsgWantWHash()
        {
            Code = CodeFromChars("wh");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);

            if (NetworkMessage.IsOnServer)
                buffer.WriteNullTermString((IsRandomMap ? "t" : string.Empty) + WorldHash);
                
            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            string t = ReadNullTermString(true);
            if (t.Length > 0)
            {
                IsRandomMap = t[0] == 't';
                WorldHash = t.Substring(1).TrimEnd('\0');
            }
        }
    }
}
