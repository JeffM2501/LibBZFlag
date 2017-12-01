
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Networking.Messages;

namespace BZFlag.Networking
{
    public class InboundMessageCallbackProcessor
    {
        public delegate void MessageHandler(NetworkMessage msg);
        protected  Dictionary<int, MessageHandler> Handlers = new Dictionary<int, MessageHandler>();

        public bool DispatchMessage(NetworkMessage Message)
        {
            if (!Handlers.ContainsKey(Message.Code))
                return false;

            Handlers[Message.Code](Message);
            return true;
        }

        public void Add(NetworkMessage msg, MessageHandler handler)
        {
            Handlers.Add(msg.Code, handler);
        }
    }

}