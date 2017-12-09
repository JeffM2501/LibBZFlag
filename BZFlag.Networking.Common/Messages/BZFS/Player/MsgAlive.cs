using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.Data.Utils;
using BZFlag.LinearMath;

namespace BZFlag.Networking.Messages.BZFS.Player
{
    public class MsgAlive : NetworkMessage
    {
        public int PlayerID = -1;
        public Vector3F Position = Vector3F.Zero;
        public float Azimuth = 0;

        public bool IsSpawn = false;

        public MsgAlive()
        {
            Code = CodeFromChars("al");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);
            if (!IsSpawn)
            {
                buffer.WriteByte(PlayerID);
                buffer.WriteVector3F(Position);
                buffer.WriteFloat(Azimuth);
            }

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            if (data.Length == 0)
                IsSpawn = true;
            else
            {
                PlayerID = ReadByte();
                Position = ReadVector3F();
                Azimuth = ReadFloat();
            }
        }
    }
}
