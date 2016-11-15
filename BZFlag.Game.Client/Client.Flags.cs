using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Flags;
using BZFlag.Data.Types;
using BZFlag.Networking.Messages.BZFS.Flags;
using BZFlag.Networking.Messages;
using BZFlag.Game.Players;
using BZFlag.Game.Flags;

namespace BZFlag.Game
{
	public partial class Client
	{
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

		protected FlagInstance CreateFlag(FlagUpdateData data)
		{
			FlagInstance inst = new FlagInstance();
			inst.ID = data.FlagID;
			inst.Flag = FlagTypes.GetFromAbriv(data.Abreviation);

			WorldFlags.Add(inst);
			return inst;
		}

		public event EventHandler<FlagInstance> FlagCreated = null;
		public event EventHandler<FlagInstance> FlagGrabbed = null;
		public event EventHandler<FlagInstance> FlagDropped = null;
		public event EventHandler<FlagInstance> FlagUpdated = null;
		public event EventHandler<FlagInstance> FlagTransfered = null;

        public class NearFlagEventArgs : EventArgs
        {
            public string Name = string.Empty;
            public Vector3F Location = Vector3F.Zero;

            public NearFlagEventArgs(string n, Vector3F l)
            {
                Name = n;
                Location = l;
            }
        }
        public event EventHandler<NearFlagEventArgs> FlagIsNear = null;

        private void HandleFlagUpdate(NetworkMessage msg)
		{
			MsgFlagUpdate update = msg as MsgFlagUpdate;

			foreach(var u in update.FlagUpdates)
				SetFlagUpdateData(u);
		}

		protected FlagInstance SetFlagUpdateData(FlagUpdateData u)
		{
			bool created = false;

			FlagInstance flag = FindFlagByID(u.FlagID);
			if(flag == null)
			{
				created = true;
				flag = CreateFlag(u);
			}

			flag.Status = u.Status;
			flag.Endurance = u.Endurance;

			var owner = PlayerList.GetPlayerByID(u.Owner);
			flag.Owner = owner;
			if(owner != null)
				owner.SetFlag(flag);

			flag.CurrentPosition = u.Postion;

			flag.LastUpdatePostion = u.Postion;
			flag.LaunchPosition = u.LaunchPosition;
			flag.LandingPostion = u.LandingPostion;

			flag.FlightTime = u.FlightTime;
			flag.FlightEnd = u.FlightEnd;
			flag.InitalVelocity = u.InitalVelocity;

			if(created && FlagCreated != null)
				FlagCreated.Invoke(this, flag);
			else if(!created && FlagUpdated != null)
				FlagUpdated.Invoke(this, flag);

			return flag;
		}

		private void HandleDropFlag(NetworkMessage msg)
		{
			MsgDropFlag df = msg as MsgDropFlag;

			var owner = PlayerList.GetPlayerByID(df.PlayerID);
			FlagInstance flag = FindFlagByID(df.FlagID);

			if(owner != null)
				owner.SetFlag(null);

			if (flag != null)
			{
				flag.Owner = null;
				flag.Status = FlagStatuses.FlagInAir;
				if(owner != null)
					flag.CurrentPosition = owner.Position;

				if(FlagDropped != null)
					FlagDropped.Invoke(this, flag);
			}
		}

		private void HandleGrabFlag(NetworkMessage msg)
		{
			MsgGrabFlag gf = msg as MsgGrabFlag;

			var owner = PlayerList.GetPlayerByID(gf.PlayerID);
			FlagInstance flag = SetFlagUpdateData(gf.FlagData);

			if(FlagGrabbed != null)
				FlagGrabbed.Invoke(this, flag);
		}

		private void HandleTransferFlag(NetworkMessage msg)
		{
			MsgTransferFlag tf = msg as MsgTransferFlag;

			var flag = FindFlagByID(tf.FlagID);
			var from = PlayerList.GetPlayerByID(tf.FromID);
			var to = PlayerList.GetPlayerByID(tf.ToID);

			if(from != null)
				from.SetFlag(null);

			if(to != null)
				to.SetFlag(flag);

			if(flag != null)
				flag.Owner = to;

			if(FlagTransfered != null)
				FlagTransfered.Invoke(this, flag);
		}

        protected void HandleNearFlag(NetworkMessage msg)
        {
            MsgNearFlag nf = msg as MsgNearFlag;

            if (FlagIsNear != null)
            {
                FlagIsNear.Invoke(this, new NearFlagEventArgs(nf.FlagName, nf.Position));
            }
        }

        protected void HandleCaptureFlag(NetworkMessage msg)
        {
            MsgCaptureFlag cp = msg as MsgCaptureFlag;

            var capturer = PlayerList.GetPlayerByID(cp.PlayerID);
            var flag = FindFlagByID(cp.FlagID);

            if (capturer != null)
                capturer.SetFlag(null);

            if (flag != null)
            {
                flag.Owner = null;
                flag.Status = FlagStatuses.FlagInAir;
            }
        }
    }
}
