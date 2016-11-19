using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.Data.Flags;

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
            throw new NotImplementedException();
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
