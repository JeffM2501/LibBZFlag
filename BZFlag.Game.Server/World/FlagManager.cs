
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.Networking.Messages.BZFS.Info;
using BZFlag.Data.Flags;
using BZFlag.Game.Host.Players;
using BZFlag.LinearMath;
using BZFlag.Networking.Messages.BZFS.Flags;

namespace BZFlag.Game.Host.World
{
    public class FlagManager
    {
        public Server ServerHost = null;

        public class FlagInstance : EventArgs
        {
            public bool Active = false;

            public FlagType Flag = FlagTypeList.None;
            public FlagUpdateData LastUpdate = new FlagUpdateData();

            public ServerPlayer Owner = null;
        }

        protected Dictionary<int,FlagInstance> ActiveFlags = new Dictionary<int, FlagInstance>();

        protected List<int> EmptyFlagIDs = new List<int>();

        public FlagInstance[] GetActiveFlags()
        {
            lock (ActiveFlags)
                return ActiveFlags.Values.ToArray();
        }

        public event EventHandler<FlagInstance> FlagAdded = null;
        public event EventHandler<FlagInstance> FlagRemoved = null;

        protected int GetNewFlagID()
        {
            lock (ActiveFlags)
            {
                if (ActiveFlags.Count >= API.Common.MaxFlags)
                    return -1;
            }

            lock(EmptyFlagIDs)
            {
                if (EmptyFlagIDs.Count > 0)
                {
                    int id = EmptyFlagIDs[0];
                    EmptyFlagIDs.RemoveAt(0);
                    return id;
                }
            }

            lock (ActiveFlags)
                return ActiveFlags.Count;
        }

        protected FlagInstance SetupNewFlag(FlagType flag, Vector3F location, bool spawnInAir)
        {
            FlagInstance inst = new FlagInstance();
            inst.Flag = flag;
            inst.LastUpdate.Postion = location;
            inst.Owner = null;
            inst.Active = true;
            inst.LastUpdate.Owner = -1;

            if (spawnInAir)
            {
                inst.LastUpdate.Status = FlagStatuses.FlagComing;
                inst.LastUpdate.LaunchPosition = location;
                inst.LastUpdate.LandingPostion = new Vector3F(location.X, location.Y, 0); // TODO, project ray into octree
                inst.LastUpdate.FlightEnd = 1;
            }
            else
            {
                inst.LastUpdate.Status = FlagStatuses.FlagOnGround;
            }

            inst.LastUpdate.FlagID = GetNewFlagID();
            if (inst.LastUpdate.FlagID < 0)
                return null;

            lock (ActiveFlags)
                ActiveFlags.Add(inst.LastUpdate.FlagID, inst);

            return inst;
        }

        public bool AddFlag(FlagType flag, Vector3F location, bool spawnInAir)
        {
            FlagInstance inst = SetupNewFlag(flag,location,spawnInAir);
            if (inst != null)
                FlagAdded?.Invoke(this, inst);

            return inst != null;
        }

        public bool InitFlag(FlagType flag, Vector3F location)
        {
            return SetupNewFlag(flag, location, false) != null;
        }

        public void RemoveFlag(FlagInstance instnace)
        {
            lock (EmptyFlagIDs)
                EmptyFlagIDs.Add(instnace.LastUpdate.FlagID);

            FlagRemoved?.Invoke(this, instnace);

            lock (ActiveFlags)
                ActiveFlags.Remove(instnace.LastUpdate.FlagID);
        }

        public MsgNegotiateFlags GetFlagNegotiation(MsgNegotiateFlags inFlags)
        {
            MsgNegotiateFlags outFlags = new MsgNegotiateFlags();

            foreach (var flag in FlagTypeList.Flags)
            {
                if (flag.FlagAbbv != string.Empty && !inFlags.Contains(flag.FlagAbbv))
                    outFlags.FlagAbrevs.Add(flag.FlagAbbv);
            }

            return outFlags;
        }
        public void SendInitialFlagUpdate(ServerPlayer player)
        {
            int maxFlagsPerUpdate = 10;

            MsgFlagUpdate msg = new MsgFlagUpdate();

            bool sentOne = false;

            foreach (FlagInstance flag in GetActiveFlags())
            {
                if (msg.FlagUpdates.Count == maxFlagsPerUpdate)
                {
                    sentOne = true;
                    player.SendMessage(msg);
                    msg = new MsgFlagUpdate();
                }

                msg.FlagUpdates.Add(flag.LastUpdate);
            }

            if (!sentOne || msg.FlagUpdates.Count > 0)
                player.SendMessage(msg);
        }
    }
}
