using System;
using System.Collections.Generic;

using BZFlag.Data.Flags;
using BZFlag.Game.Host.Players;
using BZFlag.LinearMath;
using BZFlag.Networking.Messages.BZFS.Flags;

namespace BZFlag.Game.Host.World
{
    partial class FlagManager
    {
        public class FlagEventArgs : EventArgs
        {
            public ServerPlayer Player = null;
            public FlagInstance Flag = null;
            public bool Allow = true;
        }
        public event EventHandler<FlagEventArgs> FlagPreGrab;

        public delegate void FlagCallback(ServerPlayer player, FlagInstance flag);
        public FlagCallback OnGrantFlag = null;
        public FlagCallback ComputeFlagDrop = null;
        public FlagCallback ComputeFlagAdd = null;

        public void HandleFlagGrab(ServerPlayer player, MsgGrabFlag message)
        {
            if (message == null)
                return;

            int flagID = message.FlagData.FlagID;

            FlagInstance candidateFlag = FindFlagByID(flagID);

            if (candidateFlag == null || !candidateFlag.Grabable())
                return;

            float dist = Vector3F.Distance(player.Info.LastSentUpdate.Position, candidateFlag.Postion);
            if (false && dist > GetFlagGrabDistance(player))
                return;

            Logger.Log4("Player " + player.Callsign + " wants to grab flag " + candidateFlag.FlagID.ToString() + " " + candidateFlag.ToString());

            GrantFlag(player, candidateFlag);
        }

        protected float GetFlagGrabDistance(ServerPlayer player)
        {
            float grabRadius = Cache.FlagRadius.Value + Cache.TankRadius.Value;

            float speedDeviation = (float)((GameTime.Now - player.Info.LastSentUpdate.TimeStamp) * Cache.TankSpeed);

            return grabRadius + speedDeviation;
        }

        public bool GrantFlag(ServerPlayer player, FlagInstance flag)
        {
            if (flag.Owner != null || player.Info.CariedFlag != null)
                return false;

            OnGrantFlag?.Invoke(player, flag);

            FlagEventArgs args = new FlagEventArgs();
            args.Player = player;
            args.Flag = flag;

            FlagPreGrab?.Invoke(this, args);

            if (flag.Status == FlagStatuses.FlagNoExist)
                return false;

            lock (ActiveFlags)
            {
                if (!args.Allow || flag.Owner != null)
                    return false;

                lock (WorldFlags)
                    WorldFlags.Remove(flag.FlagID);

                lock (CarriedFlags)
                    CarriedFlags.Add(flag.FlagID, flag);

                flag.Owner = player;
                flag.Status = FlagStatuses.FlagOnTank;
                flag.OwnerID = player.PlayerID;
            }

            FlagGrabbed?.Invoke(this, flag);

            player.Info.CariedFlag = flag;

            MsgGrabFlag grabMessage = new MsgGrabFlag();
            grabMessage.PlayerID = player.PlayerID;
            grabMessage.FlagData = flag;

            Logger.Log2("Player " + player.Callsign + " granted flag " + flag.FlagID.ToString() + " " + flag.ToString());

            Players.SendToAll(grabMessage, false);
            return true;
        }

        public void HandleDropFlag(ServerPlayer player, MsgDropFlag message)
        {
            if (message == null)
                return;

            if (player.Info.CariedFlag == null)
                return;

            player.Info.CariedFlag.Postion = message.Postion;

            DropFlag(player.Info.CariedFlag);
        }

        public void StandardDrop(ServerPlayer player, FlagInstance flag)
        {
            flag.LaunchPosition = flag.Postion + new Vector3F(0,0,Cache.TankHeight);

            float thrownAltitude = Cache.FlagAltitude;
            if (flag.Flag == FlagTypeList.Shield)
                thrownAltitude *= Cache.ShieldFlight;

            // TODO, compute the intersection point
            flag.FlightTime = 0;

            if (player == null)
            {
                flag.FlightEnd = 2.0f * (float)Math.Sqrt(-2.0f * Cache.FlagAltitude / Cache.Gravity);

                flag.InitalVelocity = -0.5f * Cache.Gravity * flag.FlightEnd;
                flag.Status = player == null ? FlagStatuses.FlagComing;
                flag.LandingPostion = new Vector3F(flag.Postion.X, flag.Postion.Y, 0);
            }
            else if (flag.Endurance == FlagEndurances.FlagUnstable)
            {
                flag.FlightEnd = 2.0f * (float)Math.Sqrt(-2.0f * Cache.FlagAltitude / Cache.Gravity);

                flag.InitalVelocity = -0.5f * Cache.Gravity * flag.FlightEnd;
                flag.Status =FlagStatuses.FlagGoing;

                flag.LandingPostion = new Vector3F(flag.Postion.X, flag.Postion.Y, flag.Postion.Z + thrownAltitude);
            }
            else
            {
                float maxAltitude = flag.Postion.Z + thrownAltitude;

                float upTime = (float)Math.Sqrt(-2.0f * thrownAltitude / Cache.Gravity);
                float downTime = (float)Math.Sqrt(-2.0f * (maxAltitude - flag.Postion.Z) / Cache.Gravity);
                flag.FlightEnd = upTime + downTime;
                flag.InitalVelocity = -Cache.Gravity * upTime;

                flag.Status = FlagStatuses.FlagInAir;
                flag.LandingPostion = new Vector3F(flag.Postion.X, flag.Postion.Y, 0);
            }

            flag.DropStarted = GameTime.Now;
        }

        public void DropFlag(FlagInstance flag)
        {
            if (flag == null || flag.Owner == null)
                return;

            ComputeFlagDrop?.Invoke(flag.Owner, flag);

            MsgDropFlag drop = new MsgDropFlag();
            drop.FlagID = flag.FlagID;
            drop.PlayerID = flag.Owner.PlayerID;
            drop.Data = flag;

            flag.Owner.Info.CariedFlag = null;
            flag.Owner = null;

            Players.SendToAll(drop, false);

            lock (CarriedFlags)
            {
                if (CarriedFlags.ContainsKey(flag.FlagID))
                    CarriedFlags.Remove(flag.FlagID);
            }

            lock (WorldFlags)
                WorldFlags.Add(flag.FlagID, flag);

            FlagDropped?.Invoke(this, flag);
        }
    }
}
