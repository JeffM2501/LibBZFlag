using System;

using BZFlag.Game.Players;
using BZFlag.Data.Types;
using BZFlag.Data.Teams;

namespace BZFlag.Game.Shots
{
	public class Shot : EventArgs
	{
		public int GlobalID = int.MinValue;
		public int BZFSShotID = int.MinValue;

		public Player Owner = null;

		public double TimeSent = 0;

		public Vector3F InitalPosition = Vector3F.Zero;
		public Vector3F InitalVelocity = Vector3F.Zero;

		public float DeltaTime = 0;

		public TeamColors Team = TeamColors.NoTeam;
		public string Flag = string.Empty;

		public float Lifetime = float.MinValue;

		public Vector3F Position = Vector3F.Zero;
		public Vector3F Velocity = Vector3F.Zero;

		public double LastUpdate = 0;
		public Player Target = null;
	}
}
