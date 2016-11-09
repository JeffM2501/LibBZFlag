using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();
            URL = ReadNullTermString(data,true);
        }
    }
}
