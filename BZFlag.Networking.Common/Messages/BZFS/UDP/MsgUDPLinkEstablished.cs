using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.UDP
{
    public class MsgUDPLinkEstablished : NoPackedDataNetworkMessage
    {
        public MsgUDPLinkEstablished()
        {
            Code = CodeFromChars("og");
        }
    }
}
