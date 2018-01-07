using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.LinearMath;
using BZFlag.Data.Players;
using BZFlag.Data.Teams;
using BZFlag.Networking.Messages.BZFS.Shots;
using BZFlag.Data.Flags;
using BZFlag.Data.Time;

namespace BZFlag.Game.Host.Players
{
    public class ShotManager : Server.GameState
    {
        public class ShotInfo : EventArgs
        {
            public int GlobalID = -1;
            public ServerPlayer Owner = null;
            public int PlayerShotID = -1;

            public TeamColors TeamColor = TeamColors.NoTeam;
            public ShotTypes ShotType = ShotTypes.NormalShot;
            public FlagType SourceFlag = null;

            public Vector3F InitalPostion = Vector3F.Zero;
            public Vector3F InitalVector = Vector3F.Zero;
            public double FireTimestamp = double.MinValue;
            public double CreateTimeStamp = double.MinValue;
            public double Lifetime = 0;


            public Vector3F UpdatePostion = Vector3F.Zero;
            public Vector3F UpdateVector = Vector3F.Zero;
            public double UpdateTimestamp = double.MinValue;

            public bool Allow = false;
        }

        public delegate ShotTypes GetDefaultShotTypeCallback(ServerPlayer player);

        public GetDefaultShotTypeCallback GetDefaultShotType = new GetDefaultShotTypeCallback((x) => ShotTypes.NormalShot);

        protected List<ShotInfo> ShotList = new List<ShotInfo>();
        protected List<ShotInfo> RecentlyDeadShots = new List<ShotInfo>();

        protected int LastShotID = -1;
        protected double ShotPergetoryTime = -3;

        public event EventHandler<ShotInfo> ShotPreFire;
        public event EventHandler<ShotInfo> ShotFired;
        public event EventHandler<ShotInfo> ShotExpired;
        public event EventHandler<ShotInfo> ShotDied;
        public event EventHandler<ShotInfo> ShotHit;

        public class ShotUpdatedEventArgs : EventArgs
        {
            public ShotInfo Shot = null;
            public ServerPlayer Target = null;
        }
        public event EventHandler<ShotUpdatedEventArgs> ShotPreUpdate;
        public event EventHandler<ShotInfo> ShotUpdated;

        public class ShotHitArgs : EventArgs
        {
            public ShotInfo Shot = null;
            public ServerPlayer Target = null;

            public ShotHitArgs(ShotInfo s, ServerPlayer p) { Shot = s; Target = p; }
        }

        public event EventHandler<ShotHitArgs> ShotKilled;

        protected int NewShotID()
        {
            if (LastShotID == int.MaxValue)
                LastShotID = 0;
            else
                LastShotID++;

            return LastShotID;
        }

        public void HandleShotBegin(ServerPlayer sender, MsgShotBegin shotMessage)
        {
            ShotInfo shot = new ShotInfo();

            shot.GlobalID = NewShotID();
            shot.Owner = sender;
            shot.PlayerShotID = shotMessage.ShotID;
            shot.Allow = true;

            if (sender.Info.CariedFlag == null)
            {
                shot.ShotType = GetDefaultShotType(sender);
                shot.SourceFlag = null;
            }
            else
            {
                shot.ShotType = sender.Info.CariedFlag.Flag.FlagShot;
                shot.SourceFlag = sender.Info.CariedFlag.Flag;
            }

            shot.InitalPostion = shotMessage.Position;
            shot.InitalVector = shotMessage.Velocity;
            shot.FireTimestamp = shotMessage.TimeSent;
            shot.TeamColor = sender.ActualTeam;
            shot.Lifetime = shotMessage.Lifetime;

            ShotPreFire?.Invoke(this, shot);

            if (shot.Allow)
                FireShot(shot);
        }

        public void HandleShotEnd(ServerPlayer sender, MsgShotEnd shotMessage)
        {
            ShotInfo shot = FindShot(shotMessage.PlayerID, shotMessage.ShotID);

            if (shot == null)
                return;

            bool valid = false;
            if (sender.Info.ShotImmunities > 0)
            {
                sender.Info.ShotImmunities--;
                valid = true;
            }

            if (shot.ShotType == ShotTypes.ThiefShot || shot.ShotType == ShotTypes.GuidedShot)
                valid = true;

            if (!valid)     // don't penalize them for sending this, they just always send it
                return;     // we know that all other cases must be followed by a death to be valid, so just remove the shot then.

            Flags.HandlePlayerTakeHit(sender, shot.Owner, shot);

            ShotHit?.Invoke(this, shot);
            EndShot(shot, shotMessage.Exploded);
        }

        public void FireShot(ShotInfo shot)
        {
            shot.CreateTimeStamp = GameTime.Now;

            MsgShotBegin shotMessage = new MsgShotBegin();

            if (shot.Owner == null)
                shotMessage.PlayerID = BZFlag.Data.Players.PlayerConstants.ServerPlayerID;
            else
                shotMessage.PlayerID = shot.Owner.PlayerID;

            shotMessage.ShotID = shot.PlayerShotID;
            shotMessage.Team = shot.TeamColor;
            shotMessage.TimeSent = (float)shot.FireTimestamp;
            shotMessage.Position = shot.InitalPostion;
            shotMessage.Velocity = shot.InitalVector;
            shotMessage.Lifetime = (float)shot.Lifetime;
            if (shot.SourceFlag != null)
                shotMessage.Flag =  shot.SourceFlag.FlagAbbv;

            lock (ShotList)
                ShotList.Add(shot);

            Players.SendToAll(shotMessage, false);

            ShotFired?.Invoke(this, shot);
            shot.Allow = true;
        }

        public void EndShot(ShotInfo shot, bool exploded)
        {
            lock (ShotList)
                ShotList.Remove(shot);

            ShotDied?.Invoke(this, shot);

            MsgShotEnd endShot = new MsgShotEnd();
            endShot.Exploded = exploded;
            endShot.PlayerID = shot.Owner.PlayerID;
            endShot.ShotID = shot.PlayerShotID;

            Players.SendToAll(endShot, false);
        }

        public ShotInfo FindShot(int playerID, int shotID)
        {
            lock (ShotList)
                return ShotList.Find((x) => x.PlayerShotID == shotID && x.Owner.PlayerID == playerID);
        }

        public ShotInfo FindUndeadShot(int playerID, int shotID)
        {
            lock (RecentlyDeadShots)
                return RecentlyDeadShots.Find((x) => x.PlayerShotID == shotID && x.Owner.PlayerID == playerID);
        }

        public ShotInfo FindKillableShot(int playerID, int shotID)
        {
            ShotInfo i = FindShot(playerID, shotID);
            if (i == null)
                i = FindUndeadShot(playerID, shotID);
            return i;
        }

        public void Update(Clock gameTime)
        {
            ShotInfo[] toProcess;
            lock (ShotList)
                toProcess = ShotList.ToArray();

            double now = GameTime.Now;

            List<ShotInfo> expired = new List<ShotInfo>();
            foreach (var shot in toProcess)
            {
                if (now - shot.CreateTimeStamp > shot.Lifetime)
                {
                    ShotExpired?.Invoke(this, shot);
                    expired.Add(shot);
                }
                else
                {
                    // update the position?
                }
            }

            lock (ShotList)
                ShotList.RemoveAll((x) => expired.Contains(x));

            foreach (var shot in expired)
            {
                if (shot.Owner == null)
                {
                    shot.Allow = false;
                    shot.Lifetime = -1;
                    EndShot(shot, true);
                }
            }

            expired.RemoveAll((x) => !x.Allow);

            // do something with the expired shots, we expect them to be removed shortly
            lock (RecentlyDeadShots)
                toProcess = RecentlyDeadShots.ToArray();

            foreach (var shot in toProcess)
                shot.Lifetime -= gameTime.Delta;

            RecentlyDeadShots.RemoveAll((x)=>x.Lifetime < ShotPergetoryTime);
        }

        public void RemoveShotForDeath(ServerPlayer player, int killerID, int shotID)
        {
            ShotInfo shot = FindShot(killerID, shotID);

            if (shot == null)
            {
                shot = FindUndeadShot(killerID, shotID);
                if (shot == null)
                {
                    Logger.Log1("Player " + player.Callsign + " killed by unknown shot");
                    return;
                }
            }
            else
                EndShot(shot, false);

            ShotHit?.Invoke(this, shot);
            ShotKilled?.Invoke(this, new ShotHitArgs(shot, player));
        }

        internal void HandleGMUpdate(ServerPlayer player, MsgGMUpdate message)
        {
            if (message == null && player == null || !player.CanDoPlayActions())
                return;

            ShotInfo shot = FindShot(player.PlayerID, message.ShotID);
            ServerPlayer target = Players.GetPlayerByID(message.TargetID);

            if (target == null || shot == null || !target.CanDoPlayActions() || shot.ShotType != ShotTypes.GuidedShot)
                return;

            UpdateShot(shot, message.Position, message.Velocity, message.DeltaTime, target);
        }

        public void UpdateShot(ShotInfo shot, Vector3F postion, Vector3F velocity, float delta, ServerPlayer target)
        {
            if (shot == null || target == null)
                return;

            shot.UpdateTimestamp = GameTime.Now;
            shot.UpdatePostion = postion;
            shot.UpdateVector = velocity;

            ShotUpdatedEventArgs args = new ShotUpdatedEventArgs();
            args.Shot = shot;
            args.Target = target;

            ShotPreUpdate?.Invoke(this, args);

            MsgGMUpdate update = new MsgGMUpdate();

            if (shot.Owner != null)
                update.PlayerID = shot.Owner.PlayerID;
            else
                update.PlayerID = PlayerConstants.ServerPlayerID;

            update.ShotID = shot.PlayerShotID;
            update.TargetID = args.Target.PlayerID;
            update.Position = shot.UpdatePostion;
            update.Velocity = shot.UpdateVector;
            update.DeltaTime = delta;
            update.Team = shot.TeamColor;

            Players.SendToAll(update, false);
            ShotUpdated?.Invoke(this, shot);
        }
    }
}
