using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Flags;
using BZFlag.Data.Types;

using BZFlag.Game.Players;
using BZFlag.Map;

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

		public double FlightTime = 0;
		public double FlightEnd = 0;
		public float InitialVelocity = 0;

        public void Update(double now, double delta, WorldMap map)
        {
			float gravFactor = (InitialVelocity + 0.5f * map.Constants.Gravity * (float)FlightTime);

			switch(Status)
			{
				case FlagStatuses.FlagInAir:
					FlightTime += delta;
					if(FlightTime >= FlightEnd)
					{
						Status = FlagStatuses.FlagOnGround;
						CurrentPosition = LandingPostion;
					}
					else
					{
						double p = FlightTime / FlightEnd;
						double ip = 1.0 - p;
						CurrentPosition = (ip * LaunchPosition) + (p * LandingPostion);
						CurrentPosition.Z += (float)FlightTime * gravFactor;
					}
					break;

				case FlagStatuses.FlagComing:
					FlightTime += delta;
					if(FlightTime >= FlightEnd)
					{
						Status = FlagStatuses.FlagOnGround;
						CurrentPosition.Z = 0;
					}
					else if(FlightTime >= 0.5f * FlightEnd)
						CurrentPosition.Z = (float)FlightTime * gravFactor + LandingPostion.Z;
					else
						CurrentPosition.Z = 0.5f * (float)FlightEnd * (InitialVelocity + 0.25f * map.Constants.Gravity * (float)FlightEnd) + LandingPostion.Z;

					break;

				case FlagStatuses.FlagGoing:
					FlightTime += delta;

					if(FlightTime > FlightEnd)
						Status = FlagStatuses.FlagNoExist;
					else if(FlightTime < 0.5f * FlightEnd)
						CurrentPosition.Z = (float)FlightTime * gravFactor + LastUpdatePostion.Z;
					else
						CurrentPosition.Z = 0.5f * (float)FlightEnd * (InitialVelocity + 0.25f * map.Constants.Gravity * (float)FlightEnd) + LandingPostion.Z;

					break;

				case FlagStatuses.FlagOnTank:
					if(Owner != null)
						CurrentPosition = new Vector3F(Owner.Position);
					break;
				default:
					break;

			}
		}
    }
}
