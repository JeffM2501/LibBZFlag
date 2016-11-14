using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;
using BZFlag.Data.Players;
using BZFlag.Data.Types;
using BZFlag.Networking.Messages.BZFS.Player;

using BZFlag.Game.Flags;

namespace BZFlag.Game.Players
{

	public class Player : EventArgs
	{
		public int PlayerID = -1;

		public PlayerTypes PlayerType = PlayerTypes.Unknown;

		public TeamColors Team = TeamColors.NoTeam;

		public string Callsign = string.Empty;
		public string Motto = string.Empty;

		public int Wins = 0;
		public int Losses = 0;
		public int TeamKills = 0;

		public PlayerAttributes Attributes = PlayerAttributes.Unknown;

		public MsgPlayerUpdateBase LastUpdate = null;

		// DR values
		public Vector3F Position = Vector3F.Zero;
		public float Azimuth = 0;

		// state values
		public FlagInstance CurrentFlag = null;

		public bool SetFlag(FlagInstance flag)
		{
			if(flag == CurrentFlag)
				return false;

			CurrentFlag = flag;
			return true;
		}
	}
}
