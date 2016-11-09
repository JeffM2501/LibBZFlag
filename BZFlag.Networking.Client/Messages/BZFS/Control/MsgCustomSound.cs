using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Control
{
    public class MsgCustomSound : NetworkMessage
    {
        public int SoundID = -1;
        public string SoundName = string.Empty;

        public MsgCustomSound()
        {
            Code = CodeFromChars("cs");
        }

        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();
            SoundID = ReadUInt16(data);
            SoundName = ReadUShortPascalString(data);
        }
    }
}
