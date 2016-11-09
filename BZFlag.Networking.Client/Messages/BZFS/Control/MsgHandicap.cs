using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Control
{
    public class MsgHandicap : NetworkMessage
    {
        public Dictionary<int,int> Handicaps = new Dictionary<int, int>();

        public MsgHandicap()
        {
            Code = CodeFromChars("hc");
        }

        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();

            int count = ReadUInt16(data);
            for (int i = 0; i < count; i++)
                Handicaps.Add(ReadByte(data), ReadInt16(data));
        }
    }
}
