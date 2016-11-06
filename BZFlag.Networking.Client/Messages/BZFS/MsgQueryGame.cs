using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS
{
	public class MsgQueryGame : NetworkMessage
	{
		public enum GameTypes
		{
			Unknown = -1,
			TeamFFA = 0,       // normal teamed FFA
			ClassicCTF = 1,    // your normal CTF
			OpenFFA = 2,       // teamless FFA
			RabbitChase = 3,    // hunt the rabbit mode
		};

		[Flags] public enum GameOptionFlags
		{
			NoStyle = 0,
			SuperFlagGameStyle = 0x0002, // superflags allowed
			JumpingGameStyle = 0x0008, // jumping allowed
			InertiaGameStyle = 0x0010, // momentum for all
			RicochetGameStyle = 0x0020, // all shots ricochet
			ShakableGameStyle = 0x0040, // can drop bad flags
			AntidoteGameStyle = 0x0080, // anti-bad flags
			HandicapGameStyle = 0x0100, // handicap players based on score (eek! was TimeSyncGameStyle)
			NoTeamKillsGameStyle = 0x0400
			// add here before reusing old ones above
		};

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

		public GameTypes GameStyle = GameTypes.Unknown;
		public GameOptionFlags GameOptions = GameOptionFlags.NoStyle;
		public int MaxPlayers = -1;
		public int MaxShots = -1;

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

			GameStyle = (GameTypes)ReadUInt16(data);
			GameOptions = (GameOptionFlags)ReadUInt16(data);
			MaxPlayers = ReadUInt16(data);
			MaxShots = ReadUInt16(data);

			TeamData.Clear();
			for(Teams t = Teams.Rogue; t <= Teams.Observer; t++)
			{
				TeamInfo info = new TeamInfo();
				info.Team = t;
				info.Size = ReadUInt16(data);
				TeamData.Add(t, info);
			}
			for(Teams t = Teams.Rogue; t <= Teams.Observer; t++)
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
