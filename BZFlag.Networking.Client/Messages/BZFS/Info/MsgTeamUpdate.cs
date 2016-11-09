using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Info
{
	public class MsgTeamUpdate : NetworkMessage
	{
		public class TeamUpdate
		{
			public int TeamID = 0;
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
			throw new NotImplementedException();
		}

		public override void Unpack(byte[] data)
		{
			TeamUpdates.Clear();
			ResetOffset();

			int count = ReadByte(data);
			for (int i =0; i < count; i++)
			{
				TeamUpdate team = new TeamUpdate();
				team.TeamID = ReadUInt16(data);
				team.Size = ReadUInt16(data);
				team.Wins = ReadUInt16(data);
				team.Losses = ReadUInt16(data);
				TeamUpdates.Add(team);
			}
		}
	}
}
