using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Flags;
using BZFlag.Data.Utils;
using BZFlag.LinearMath;

namespace BZFlag.Networking.Messages.BZFS.Flags
{
    public class MsgDropFlag : NetworkMessage
    {
        public int PlayerID = -1;
        public int FlagID = -1;
        public FlagUpdateData Data = new FlagUpdateData();

        public Vector3F Postion = new Vector3F();

        public MsgDropFlag()
        {
            Code = CodeFromChars("df");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);

            if (IsServer())
            {
                buffer.WriteByte(PlayerID);
                buffer.WriteFlagUpdateData(Data,true);
            }
            else
                buffer.WriteVector3F(Postion);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            if (IsServer())
            {
                Postion = ReadVector3F();
            }
            else
            {
                PlayerID = ReadByte();
                FlagID = ReadUInt16();
                Data = ReadFlagUpdateData();
            }
        }
    }
}
