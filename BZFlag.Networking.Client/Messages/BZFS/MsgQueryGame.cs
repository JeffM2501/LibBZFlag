using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS
{
	public class MsgQueryGame : NetworkMessage
	{
		public int GameStyle = -1;
		public int GameOptions = -1;
		public int MaxPlayers = -1;
		public int MaxShots = -1;

		public enum Teams
		{
			Rogue = 0,
			Red,
			Green,
			Blue,
			Purple,
			Observer,
		}

		public class TeamInfo
		{
			public Teams Team = Teams.Observer;

			public int Size = 0;
			public int Max = 0;
		}
		public Dictionary<Teams, TeamInfo> TeamData = new Dictionary<Teams, TeamInfo>();

		public int ShakeWins = 0;
		public int ShakeTimeout = 0;

		public int MaxPlayerScore = 0;
		public int MaxTeamScore = 0;
		public int ElapsedTime = 0;
	

		public MsgQueryGame()
		{
			Code = CodeFromChars("qg");
		}

		public override byte[] Pack()
		{
			return new byte[] { 0, 0, 0x71, 0x67 };
		}

		public override void Unpack(byte[] data)
		{
			ResetOffset();

			GameStyle = ReadUInt16(data);
			GameOptions = ReadUInt16(data);
			MaxPlayers = ReadUInt16(data);
			MaxShots = ReadUInt16(data);

			TeamData.Clear();
			for(Teams t = Teams.Rogue; t < Teams.Observer; t++)
			{
				TeamInfo info = new TeamInfo();
				info.Team = t;
				info.Size = ReadUInt16(data);
				TeamData.Add(t, info);
			}
			for(Teams t = Teams.Rogue; t < Teams.Observer; t++)
			{
				TeamInfo info = TeamData[t];
				info.Max = ReadUInt16(data);
			}

			ShakeWins = ReadUInt16(data); ;
			ShakeTimeout = ReadUInt16(data); ;

			MaxPlayerScore = ReadUInt16(data); ;
			MaxTeamScore = ReadUInt16(data); ;
			ElapsedTime = ReadUInt16(data); ;
		}
	}
}
