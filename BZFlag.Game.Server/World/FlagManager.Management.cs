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

        public void HandleFlagGrab(ServerPlayer player, MsgGrabFlag message)
        {
            if (message == null)
                return;

            int flagID = message.FlagData.FlagID;

            FlagInstance candidateFlag = FindFlagByID(flagID);

            if (candidateFlag == null || !candidateFlag.Grabable())
                return;

            float grabRadius = ServerHost.State.BZDatabase.Cache.FlagRadius.Value + ServerHost.State.BZDatabase.Cache.TankRadius.Value;

            float speedDeviation = (float)((ServerHost.State.GameTime.Now - player.Info.LastSentUpdate.TimeStamp) * ServerHost.State.BZDatabase.Cache.TankSpeed.Value);

            float dist = Vector3F.Distance(player.Info.LastSentUpdate.Position, candidateFlag.Postion);
            if (dist > grabRadius + speedDeviation)
                return;

            Logger.Log4("Player " + player.Callsign + " wans to grab flag " + candidateFlag.FlagID.ToString() + " " + candidateFlag.ToString());

            GrantFlag(player, candidateFlag);
        }

        public bool GrantFlag(ServerPlayer player, FlagInstance flag)
        {
            if (flag.Owner != null)
                return false;

            FlagEventArgs args = new FlagEventArgs();
            args.Player = player;
            args.Flag = flag;

            FlagPreGrab?.Invoke(this, args);

            lock (ActiveFlags)
            {
                if (!args.Allow || flag.Owner != null)
                    return false;

                lock (WorldFlags)
                    WorldFlags.Remove(flag.FlagID);

                lock (CarriedFlags)
                    CarriedFlags.Add(flag.FlagID, flag);

                flag.Owner = player;
                flag.Status = Data.Flags.FlagStatuses.FlagOnTank;
                flag.OwnerID = player.PlayerID;
            }

            FlagGrabbed?.Invoke(this, flag);

            MsgGrabFlag grabMessage = new MsgGrabFlag();
            grabMessage.PlayerID = player.PlayerID;
            grabMessage.FlagData = flag;

            Logger.Log2("Player " + player.Callsign + " granted flag " + flag.FlagID.ToString() + " " + flag.ToString());

            ServerHost.State.Players.SendToAll(grabMessage, false);
            return true;
        }

        public void HandleDropFlag(ServerPlayer player, MsgDropFlag message)
        {
            if (message == null)
                return;
        }

        public void DropFlag(FlagInstance flag)
        {
            if (flag.Owner == null)
                return;

            MsgDropFlag drop = new MsgDropFlag();
            drop.FlagID = flag.FlagID;
            drop.PlayerID = flag.Owner.PlayerID;

            ServerHost.State.Players.SendToAll(drop, false);

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
