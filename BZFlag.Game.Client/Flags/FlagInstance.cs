using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Flags;
using BZFlag.Data.Types;

using BZFlag.Game.Players;

namespace BZFlag.Game.Flags
{
	public class FlagInstance : EventArgs
	{
		public int ID = -1;
		public FlagType Flag = null;

		public FlagStatuses Status = FlagStatuses.FlagNoExist;
		public FlagEndurances Endurance = FlagEndurances.FlagNormal;

		public Player Owner = null;

		public Vector3F CurrentPosition = Vector3F.Zero;

		public Vector3F LastUpdatePostion = Vector3F.Zero;
		public Vector3F LaunchPosition = Vector3F.Zero;
		public Vector3F LandingPostion = Vector3F.Zero;

		public float FlightTime = 0;
		public float FlightEnd = 0;
		public float InitalVelocity = 0;

	}
}
