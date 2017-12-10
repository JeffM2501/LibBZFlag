using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Info
{
    public class MsgCacheURL : NetworkMessage
    {
        public string URL = string.Empty;

        public MsgCacheURL()
        {
            Code = CodeFromChars("cu");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);

            buffer.WriteNullTermString(URL);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            URL = ReadNullTermString(true);
        }
    }
}
