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


        public delegate void FlagCallback( ServerPlayer player, FlagInstance flag );
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
            if (dist > GetFlagGrabDistance(player))
                return;

            Logger.Log4("Player " + player.Callsign + " wans to grab flag " + candidateFlag.FlagID.ToString() + " " + candidateFlag.ToString());

            GrantFlag(player, candidateFlag);
        }

        protected float GetFlagGrabDistance(ServerPlayer player)
        {
            float grabRadius = Cache.FlagRadius + Cache.TankRadius;

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

            float dropFudge = 1.0f;

            if (Vector3F.Distance(player.Info.LastSentUpdate.Position, message.Postion) > GetFlagGrabDistance(player) * dropFudge)
                player.Info.CariedFlag.Postion = player.Info.LastSentUpdate.Position + new Vector3F(0, 0, Cache.TankHeight.Value * 1.1f);
            else
                player.Info.CariedFlag.Postion = message.Postion;

            DropFlag(player.Info.CariedFlag);
        }

        public void StandardDrop(ServerPlayer player, FlagInstance flag)
        {
            Vector3F endPos = Vector3F.FromAngle((float)(World.RNG.NextDouble() * 360)) * Cache.TankRadius;

            float distanceUp = Cache.FlagAltitude;
            if (flag.Flag == FlagTypeList.Shield)
                distanceUp *= Cache.ShieldFlight;

            float distanceDown = flag.Postion.Z + distanceUp;
            // TODO, compute the intersection point

            // compute the height
            if (player == null)
            {
                /// i9t's an add
            }
            else
            {

            }
        }

        public void DropFlag(FlagInstance flag)
        {
            if (flag.Owner == null)
                return;

            ComputeFlagDrop?.Invoke(flag.Owner, flag);

            MsgDropFlag drop = new MsgDropFlag();
            drop.FlagID = flag.FlagID;
            drop.PlayerID = flag.Owner.PlayerID;

            Players.SendToAll(drop, false);

            flag.Postion = flag.Owner.Info.LastSentUpdate.Position;
            flag.Status = FlagStatuses.FlagOnGround; // TODO properly handle the landing time
            flag.Owner = null;

            lock (CarriedFlags)
            {
                if (WorldFlags.ContainsKey(flag.FlagID))
                    WorldFlags.Remove(flag.FlagID);
            }

            lock (WorldFlags)
                WorldFlags.Add(flag.FlagID, flag);

            FlagDropped?.Invoke(this, flag);
        }
    }
}
