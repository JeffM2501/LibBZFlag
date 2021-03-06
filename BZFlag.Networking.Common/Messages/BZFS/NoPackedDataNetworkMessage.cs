using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS
{
    public abstract class NoPackedDataNetworkMessage : NetworkMessage
    {
        public override byte[] Pack()
        {
            return DynamicOutputBuffer.Get(Code).GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
        }
    }
}