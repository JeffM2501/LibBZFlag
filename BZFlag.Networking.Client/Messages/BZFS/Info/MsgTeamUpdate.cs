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
			Reset(data);

			int count = ReadByte();
			for (int i =0; i < count; i++)
			{
				TeamUpdate team = new TeamUpdate();
				team.TeamID = ReadUInt16();
				team.Size = ReadUInt16();
				team.Wins = ReadUInt16();
				team.Losses = ReadUInt16();
				TeamUpdates.Add(team);
			}
		}
	}
}
