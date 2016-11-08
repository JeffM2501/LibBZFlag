using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using BZFlag.Data.Types;

namespace BZFlag.Networking.Messages.BZFS
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
            ResetOffset();
            PlayerID = ReadByte(data);
            Position = ReadVector3F(data);
            Azimuth = ReadFloat(data);
        }
    }
}
