using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;
using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Info
{
    public class MsgTeamUpdate : NetworkMessage
    {
        public class TeamUpdate
        {
            public TeamColors TeamID = 0;
            public int Size = 0;
            public int Wins = 0;
            public int Losses = 0;
        }

        public List<TeamUpdate> TeamUpdates = new List<TeamUpdate>();

        public MsgTeamUpdate()
        {
            Code = CodeFromChars("tu");
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);

            buffer.WriteByte(TeamUpdates.Count);
            foreach (TeamUpdate upd in TeamUpdates)
            {
                buffer.WriteUInt16((int)upd.TeamID);
                buffer.WriteUInt16((int)upd.Size);
                buffer.WriteUInt16((int)upd.Wins);
                buffer.WriteUInt16((int)upd.Losses);
            }

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            TeamUpdates.Clear();
            Reset(data);

            int count = ReadByte();
            for (int i = 0; i < count; i++)
            {
                TeamUpdate team = new TeamUpdate();
                team.TeamID = (TeamColors)ReadUInt16();
                team.Size = ReadUInt16();
                team.Wins = ReadUInt16();
                team.Losses = ReadUInt16();
                TeamUpdates.Add(team);
            }
        }
    }
}
