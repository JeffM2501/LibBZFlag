using BZFlag.Game.Host.Players;
using BZFlag.Networking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Game.Host
{
    public class ServerMessageDispatcher
    {
        public delegate void MessageHandler(ServerPlayer player, NetworkMessage msg);
        protected Dictionary<int, MessageHandler> Handlers = new Dictionary<int, MessageHandler>();

        public bool DispatchMessage(ServerPlayer player, NetworkMessage Message)
        {
            if (!Handlers.ContainsKey(Message.Code))
                return false;

            Handlers[Message.Code](player, Message);
            return true;
        }

        public void Add(NetworkMessage msg, MessageHandler handler)
        {
            Handlers.Add(msg.Code, handler);
        }
    }
}
