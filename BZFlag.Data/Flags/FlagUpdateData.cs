using BZFlag.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Data.Flags
{
	public class FlagUpdateData
	{
		public int FlagID = -1;
		public string Abreviation = string.Empty;
		public FlagStatuses Status = FlagStatuses.FlagNoExist;
		public FlagEndurances Endurance = FlagEndurances.FlagNormal;
		public int Owner = -1;
		public Vector3F Postion = Vector3F.Zero;
		public Vector3F LaunchPosition = Vector3F.Zero;
		public Vector3F LandingPostion = Vector3F.Zero;
		public float FlightTime = 0;
		public float FlightEnd = 0;
		public float InitalVelocity = 0;
	}

}
