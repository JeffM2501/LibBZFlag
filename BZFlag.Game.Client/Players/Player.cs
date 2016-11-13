using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;
using BZFlag.Data.Players;
using BZFlag.Data.Types;
using BZFlag.Networking.Messages.BZFS.Player;

namespace BZFlag.Game.Players
{
	public class Player
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
	}
}
