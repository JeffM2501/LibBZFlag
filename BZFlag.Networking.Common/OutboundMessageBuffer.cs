using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

using BZFlag.Networking.Messages;

namespace BZFlag.Networking
{
    public class OutboundMessageBuffer
    {
        public static int MaxToProcess = 10;

        private List<byte[]> Outbound = new List<byte[]>();

        private List<NetworkMessage> UnprocessedOutbound = new List<NetworkMessage>();

        public virtual void Process()
        {
            for (int i = 0; i< MaxToProcess; i++)
            {
                NetworkMessage msg = GetNextUnprocessedMessage();
                if (msg == null)
                    return;

                byte[] buffer = msg.Pack();
                if (buffer != null)
                    PushDirectMessage(buffer);
            }
        }


        public byte[] Pop()
        {
            lock (Outbound)
            {
                if (Outbound.Count == 0)
                    return null;

                byte[] b = Outbound[0];
                Outbound.RemoveAt(0);
                return b;
            }
        }

        public byte[][] PopAll()
        {
            lock (Outbound)
            {
                if (Outbound.Count == 0)
                    return null;

                byte[][] b = Outbound.ToArray();
                Outbound.Clear();
                return b;
            }
        }

        public void PushDirectMessage(byte[] buffer)
        {
            lock (Outbound)
                Outbound.Add(buffer);
        }

        public void Push(NetworkMessage msg)
        {
            lock (UnprocessedOutbound)
                UnprocessedOutbound.Add(msg);
        }

        protected NetworkMessage GetNextUnprocessedMessage()
        {
            lock (UnprocessedOutbound)
            {
                if (UnprocessedOutbound.Count == 0)
                    return null;

                NetworkMessage b = UnprocessedOutbound[0];
                UnprocessedOutbound.RemoveAt(0);
                return b;
            }
        }

        public void Clear()
        {
            lock (Outbound)
                Outbound.Clear();
        }
    }
}