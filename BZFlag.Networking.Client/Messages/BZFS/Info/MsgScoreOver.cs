using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;

namespace BZFlag.Networking.Messages.BZFS.Info
{

    public class MsgScoreOver : NetworkMessage
    {
        public int PlayerID = -1;
        public TeamColors Team = TeamColors.NoTeam;

        public MsgScoreOver()
        {
            Code = CodeFromChars("so");
        }

        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            PlayerID = ReadByte();
            Team = (TeamColors)ReadInt16();
        }
    }
}
