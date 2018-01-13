using BZFlag.Data.Flags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;
using BZFlag.LinearMath;

namespace BZFlag.Networking.Messages.BZFS.Flags
{
    public class MsgGrabFlag : NetworkMessage
    {
        public int PlayerID = -1;
        public FlagUpdateData FlagData = new FlagUpdateData();


        public MsgGrabFlag()
        {
            Code = CodeFromChars("gf");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);

            if (IsServer())
            {
                buffer.WriteByte(PlayerID);
                buffer.WriteFlagUpdateData(FlagData, false);
            }
            else
            {
                buffer.WriteUInt16(FlagData.FlagID);
            }

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);

            if (IsServer())
            {
                FlagData.FlagID = ReadUInt16();
            }
            else
            {
                PlayerID = ReadByte();
                FlagData = ReadFlagUpdateData();
            }
        }
    }
}
