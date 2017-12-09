using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.LinearMath;

namespace BZFlag.Networking.Messages.BZFS.Flags
{
    public class MsgNearFlag : NetworkMessage
    {
        public Vector3F Position = Vector3F.Zero;
        public string FlagName = string.Empty;

        public MsgNearFlag()
        {
            Code = CodeFromChars("Nf");
        }
        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            Position = ReadVector3F();
            FlagName = ReadULongPascalString();
        }
    }
}
