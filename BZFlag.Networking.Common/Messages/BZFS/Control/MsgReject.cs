using BZFlag.Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Control
{
    public class MsgReject : NetworkMessage
    {
        public enum RejectionCodes
        {
            RejectBadRequest = 0x0000,
            RejectBadTeam = 0x0001,
            RejectBadType = 0x0002,
            RejectBadMotto = 0x0003,
            RejectTeamFull = 0x0004,
            RejectServerFull = 0x0005,
            RejectBadCallsign = 0x0006,
            RejectRepeatCallsign = 0x0007,
            RejectRejoinWaitTime = 0x0008,
            RejectIPBanned = 0x0009,
            RejectHostBanned = 0x000A,
            RejectIDBanned = 0x000B,
            RejectUnknown = 0xFFFF,
        };


        public RejectionCodes ReasonCode = RejectionCodes.RejectUnknown;

        public string ReasonMessage = string.Empty;

        public MsgReject()
        {
            Code = CodeFromChars("rj");
        }

        public MsgReject(RejectionCodes rc, string reason)
        {
            Code = CodeFromChars("rj");
            ReasonCode = rc;
            ReasonMessage = reason;
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);

            buffer.WriteUInt16((UInt16)ReasonCode);
            buffer.WriteNullTermString(ReasonMessage);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            ReasonCode = (RejectionCodes)ReadUInt16();
            ReasonMessage = ReadNullTermString(true);
        }
    }
}
