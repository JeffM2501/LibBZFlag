using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Flags;
using BZFlag.Data.Players;

namespace BZFlag.Networking.Messages.BZFS.Flags
{
    public class MsgFlagType : NetworkMessage
    {
        public string Abreviation = string.Empty;
        public FlagQualities Quality = FlagQualities.NumQualities;
        public ShotTypes ShotType = ShotTypes.Unknown;
        public string Name = string.Empty;
        public string HelpText = string.Empty;

        public MsgFlagType()
        {
            Code = CodeFromChars("ft");
        }

        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();

            Abreviation = ReadFixedSizeString(data, 2);
            Quality = (FlagQualities)ReadByte(data);
            ShotType = (ShotTypes)ReadByte(data);

            Name = ReadULongPascalString(data);
            HelpText = ReadULongPascalString(data);
        }
    }
}
