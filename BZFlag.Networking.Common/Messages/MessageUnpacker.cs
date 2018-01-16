using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace BZFlag.Networking.Messages
{
    public class MessageUnpacker
    {
        public MessageManager Factory = ServerMessageFactory.Factory;

        protected virtual NetworkMessage ProcessOne(InboundMessageBuffer.CompletedMessage buffer)
        {
            NetworkMessage msg = Factory.Unpack(buffer.ID, buffer.Data);
            if (msg != null)
            {
                msg.FromUDP = buffer.UDP;
                return msg;
            }

            return null;
        }
    }
}