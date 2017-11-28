using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Shots
{
    public class MsgShotEnd : NetworkMessage
    {
        public int PlayerID = 0;
        public int ShotID = 0;

        public bool Exploded = false;

        public MsgShotEnd()
        {
            Code = CodeFromChars("se");
        }
        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);

            buffer.WriteByte(PlayerID);
            buffer.WriteInt16((UInt16)ShotID);
            buffer.WriteUInt16((UInt16)(Exploded ? 0 : 1));
            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);

            PlayerID = ReadByte();
            ShotID = ReadInt16();
            Exploded = ReadUInt16() == 0;
        }
    }
}
