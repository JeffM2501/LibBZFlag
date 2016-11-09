using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;
using BZFlag.Data.Game;

namespace BZFlag.Networking.Messages.BZFS.Info
{
	public class MsgQueryGame : NetworkMessage
	{
		public class TeamInfo
		{
			public TeamColors Team = TeamColors.ObserverTeam;

			public int Size = 0;
			public int Max = 0;
		}

		public GameTypes GameStyle = GameTypes.Unknown;
		public GameOptionFlags GameOptions = GameOptionFlags.NoStyle;
		public int MaxPlayers = -1;
		public int MaxShots = -1;

		public Dictionary<TeamColors, TeamInfo> TeamData = new Dictionary<TeamColors, TeamInfo>();

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

			GameStyle = (GameTypes)ReadUInt16(data);
			GameOptions = (GameOptionFlags)ReadUInt16(data);
			MaxPlayers = ReadUInt16(data);
			MaxShots = ReadUInt16(data);

			TeamData.Clear();
			for(TeamColors t = TeamColors.RogueTeam; t <= TeamColors.ObserverTeam; t++)
			{
				TeamInfo info = new TeamInfo();
				info.Team = t;
				info.Size = ReadUInt16(data);
				TeamData.Add(t, info);
			}
			for(TeamColors t = TeamColors.RogueTeam; t <= TeamColors.ObserverTeam; t++)
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
