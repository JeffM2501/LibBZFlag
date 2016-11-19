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
		public float InitalVelocity = 0;

        public void Update(double now, double delta, WorldMap map)
        {
            switch (Status)
            {
                default:
                    break;

                case FlagStatuses.FlagInAir:
                    FlightTime += delta;
                    if (FlightTime >= FlightEnd)
                    {
                        Status = FlagStatuses.FlagOnGround;
                        CurrentPosition = LandingPostion;
                    }
                    else
                    {
                        double p = FlightTime / FlightEnd;
                        double ip = 1.0 - p;
                        CurrentPosition = (ip * LaunchPosition) + (p * LandingPostion);
                        CurrentPosition.Z += (float)FlightTime * InitalVelocity + 0.5f * map.Constants.Gravity;
                    }
                    break;

                case FlagStatuses.FlagComing:
                    FlightTime += delta;
                    if (FlightTime >= FlightEnd)
                    {

                    }

                    break;
            }
        }

    }
}
