using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Player
{
    public class MsgExit : NoPackedDataNetworkMessage
    {
        public MsgExit()
        {
            Code = CodeFromChars("ex");
        }
    }
}
