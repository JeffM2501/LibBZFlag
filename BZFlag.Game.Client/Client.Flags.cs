using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Flags;
using BZFlag.Data.Types;
using BZFlag.Networking.Messages.BZFS.Flags;
using BZFlag.Networking.Messages;

namespace BZFlag.Game
{
	public partial class Client
	{
		public class FlagInstance
		{
			public int ID = -1;
			public FlagType Flag = null;

			public FlagStatuses Status = FlagStatuses.FlagNoExist;
			public FlagEndurances Endurance = FlagEndurances.FlagNormal;

			public int Owner = -1;

			public Vector3F CurrentPosition = Vector3F.Zero;

			public Vector3F LastUpdatePostion = Vector3F.Zero;
			public Vector3F LaunchPosition = Vector3F.Zero;
			public Vector3F LandingPostion = Vector3F.Zero;

			public float FlightTime = 0;
			public float FlightEnd = 0;
			public float InitalVelocity = 0;

		}

		public FlagTypeList FlagTypes = new FlagTypeList();
		public List<FlagInstance> WorldFlags = new List<FlagInstance>();

		protected FlagInstance FindFlagByID(int id)
		{
			return WorldFlags.Find(x => x.ID == id);
		}

		protected FlagInstance GetFlag(FlagUpdateData data)
		{
			FlagInstance inst = FindFlagByID(data.FlagID);
			if (inst != null)
				return inst;

			inst = new FlagInstance();
			inst.ID = data.FlagID;
			inst.Flag = FlagTypes.GetFromAbriv(data.Abreviation);

			WorldFlags.Add(inst);
			return inst;
		}

		private void HandleFlagUpdate(NetworkMessage msg)
		{
			MsgFlagUpdate update = msg as MsgFlagUpdate;

			foreach(var u in update.FlagUpdates)
			{
				FlagInstance flag = GetFlag(u);

				flag.Status = u.Status;
				flag.Endurance = u.Endurance;

				flag.Owner = u.Owner;

				flag.CurrentPosition = u.Postion;

				flag.LastUpdatePostion = u.Postion;
				flag.LaunchPosition = u.LaunchPosition;
				flag.LandingPostion = u.LandingPostion;

				flag.FlightTime = u.FlightTime;
				flag.FlightEnd = u.FlightEnd;
				flag.InitalVelocity = u.InitalVelocity;
			}
		}
	}
}
