using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Players;
using BZFlag.Data.Types;

namespace BZFlag.Networking.Messages.BZFS.Player
{
    public class MsgKilled : NetworkMessage
    {
        public int VictimID = -1;
        public int KillerID = -1;
        public int ShotID = 0;
        public BlowedUpReasons Reason = BlowedUpReasons.Unknown;
        public string FlagAbreviation = string.Empty;

        public int PhysicsDriverID = -1;

        public MsgKilled()
        {
            Code = CodeFromChars("kl");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);

            buffer.WriteByte(VictimID);
            buffer.WriteByte(KillerID);

            int r = (int)Reason;
            if (Reason == BlowedUpReasons.DeathTouch)
                r = Constants.PhysicsDriverDeath;
            buffer.WriteInt16(r);

            buffer.WriteInt16(ShotID);
            buffer.WriteFixedSizeString(FlagAbreviation, 2);

            if (Reason == BlowedUpReasons.DeathTouch)
                buffer.WriteInt32(PhysicsDriverID);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();

            VictimID = ReadByte(data);
            KillerID = ReadByte(data);

            int r = ReadInt16(data);
            if (r == Constants.PhysicsDriverDeath)
                Reason = BlowedUpReasons.DeathTouch;
            else
                Reason = (BlowedUpReasons)r;

            ShotID = ReadInt16(data);
            FlagAbreviation = ReadFixedSizeString(data, 2);

            if (Reason == BlowedUpReasons.DeathTouch)
                PhysicsDriverID = ReadInt32(data);
        }
    }
}
