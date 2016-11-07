using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.UDP
{
    public class MsgUDPLinkEstablished : NetworkMessage
    {
        public MsgUDPLinkEstablished()
        {
            Code = CodeFromChars('og');
        }

        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
