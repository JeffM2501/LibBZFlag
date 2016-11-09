using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Info
{
    public class MsgWantSettings : NoPackedDataNetworkMessage
    {
        public MsgWantSettings()
        {
            Code = CodeFromChars("ws");
        }
    }
}
