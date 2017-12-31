using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS
{
    public class MsgMessage : NetworkMessage
    {
        public enum MessageTypes
        {
            ChatMessage = 0,
            ActionMessage
        };

        public int From = 0;
        public int To = 0;
        public MessageTypes MessageType = MessageTypes.ChatMessage;
        public string MessageText = string.Empty;

        public MsgMessage()
        {
            Code = CodeFromChars("mg");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);

            if (!IsServer())
            {
                
                buffer.WriteByte((byte)To);
                buffer.WriteFixedSizeString(MessageText, BZFlag.Data.Types.Constants.ChatMessageLenght);

                
            }
            else
            {
                buffer.WriteByte((byte)From);
                buffer.WriteByte((byte)To);
                buffer.WriteByte((byte)MessageType);
            }

            buffer.WriteFixedSizeString(MessageText, BZFlag.Data.Types.Constants.ChatMessageLenght);
            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            if (IsServer())
                To = ReadByte();
            else
            {
                Reset(data);
                From = ReadByte();
                To = ReadByte();
                MessageType = (MessageTypes)ReadByte();
            }

            MessageText = ReadFixedSizeString(BZFlag.Data.Types.Constants.ChatMessageLenght);
        }
    }
}
