using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.LinearMath;
using BZFlag.Data.Flags;
using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Flags
{
    public class MsgFlagUpdate : NetworkMessage
    {
        public List<FlagUpdateData> FlagUpdates = new List<FlagUpdateData>();

        public MsgFlagUpdate()
        {
            Code = CodeFromChars("fu");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer();

            buffer.WriteUInt16(FlagUpdates.Count);
            foreach (FlagUpdateData f in FlagUpdates)
                buffer.WriteFlagUpdateData(f);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);

            int count = ReadUInt16();
            for (int i = 0; i < count; i++)
                FlagUpdates.Add(ReadFlagUpdateData());
        }
    }
}
