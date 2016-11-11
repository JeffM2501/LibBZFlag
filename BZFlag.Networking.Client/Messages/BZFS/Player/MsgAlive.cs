using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Player
{
    public class MsgAlive : NetworkMessage
    {
        public int PlayerID = -1;
        public Vector3F Position = Vector3F.Zero;
        public float Azimuth = 0;

        public MsgAlive()
        {
            Code = CodeFromChars("al");
        }

        public override byte[] Pack()
        {
            return new DynamicOutputBuffer(Code).GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            PlayerID = ReadByte();
            Position = ReadVector3F();
            Azimuth = ReadFloat();
        }
    }
}
