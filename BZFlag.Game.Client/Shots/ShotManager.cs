﻿using System;
using System.Collections.Generic;

using BZFlag.Networking.Messages;
using BZFlag.Networking.Messages.BZFS.Shots;
using BZFlag.Game.Players;
using BZFlag.Data.Types;
using BZFlag.Map;
using BZFlag.LinearMath;

namespace BZFlag.Game.Shots
{
    public class ShotManager : ShotPathGenerator
    {
        public Dictionary<int, Shot> ShotList = new Dictionary<int, Shot>();
        protected Dictionary<int, int> BZFStoGlobalIDMap = new Dictionary<int, int>();

        public PlayerManager PlayerList = null;
        public WorldMap Map = null;

        public event EventHandler<Shot> ShotCreated = null;
        public event EventHandler<Shot> ShotRemoved = null;
        public event EventHandler<Shot> ShotUpdated = null;

        public Dictionary<string, ShotPathGenerator> PathGenerators = new Dictionary<string, ShotPathGenerator>();

        public ShotPathGenerator DefaultShotPathGenerator = null;

        public class ExplosionEventArgs : EventArgs
        {
            public enum Reasons
            {
                LifetimeEnd,
                ImpactEnd,
            }

            public Reasons Reson = Reasons.LifetimeEnd;

            public Vector3F Position = Vector3F.Zero;
            public Vector3F LastVelocity = Vector3F.Zero;
        }

        public event EventHandler<ExplosionEventArgs> ExplosionCreated = null;

        public ShotManager(PlayerManager p, WorldMap map)
        {
            DefaultShotPathGenerator = this;

            PlayerList = p;
            Map = map;
        }

        public void Update()
        {
            if (PlayerList == null)
                return;

            var Clock = PlayerList.Clock;

            foreach (Shot s in ShotList.Values)
            {
                if (!s.Active)
                    continue;

                s.Update(Clock.StepTime, Clock.StepDelta);
            }
        }

        protected ShotPath GetShotPath(Shot shot)
        {
            ShotPathGenerator gen = DefaultShotPathGenerator;

            if (PathGenerators.ContainsKey(shot.Flag))
                gen = PathGenerators[shot.Flag];

            if (gen == null)
                return ShotPath.Empty;

            return gen.GetShotPath(shot, shot.Owner, Map);
        }

        protected int LastGlobalShotID = 0;
        protected int NewShotID()
        {
            LastGlobalShotID++;
            return LastGlobalShotID;
        }

        public static int BuildBZFSShotID(int playerID, int localShotIndex)
        {
            return (playerID * byte.MaxValue) + localShotIndex;
        }

        public void HandleShotBegin(NetworkMessage msg)
        {
            MsgShotBegin sb = msg as MsgShotBegin;

            Shot s = new Shot();
            s.GlobalID = NewShotID();
            s.BZFSShotID = BuildBZFSShotID(sb.PlayerID, sb.ShotID);

            if (BZFStoGlobalIDMap.ContainsKey(s.BZFSShotID))
                BZFStoGlobalIDMap[s.BZFSShotID] = s.GlobalID;
            else
                BZFStoGlobalIDMap.Add(s.BZFSShotID, s.GlobalID);

            s.Owner = PlayerList.GetPlayerByID(sb.PlayerID);
            s.InitalPosition = sb.Position;
            s.InitalVelocity = sb.Velocity;

            s.Position = new Vector3F(s.InitalPosition.X, s.InitalPosition.Y, s.InitalPosition.Z);
            s.Velocity = new Vector3F(s.InitalVelocity.X, s.InitalVelocity.Y, s.InitalVelocity.Z);

            s.TimeSent = sb.TimeSent;
            s.Team = sb.Team;
            s.DeltaTime = sb.DeltaTime;
            s.Lifetime = sb.Lifetime;

            s.Path = GetShotPath(s);

            if (s.Owner != null)
            {
                int id = s.Owner.AddShot(s);
                if (id != s.GlobalID)       // the player had a replacement, so kill the shot that was replaced
                    RemoveShotByGID(id);
            }

            ShotList.Add(s.GlobalID, s);
            if (ShotCreated != null)
                ShotCreated.Invoke(this, s);
        }

        public void RemoveShotByGID(int id)
        {
            if (ShotList.ContainsKey(id))
            {
                Shot s = ShotList[id];

                if (s.Owner != null)
                    s.Owner.RemoveShot(s);

                ShotList.Remove(id);
                BZFStoGlobalIDMap.Remove(s.BZFSShotID);

                if (ShotRemoved != null)
                    ShotRemoved.Invoke(this, s);
            }
        }

        public Shot GetShotByID(int id)
        {
            if (ShotList.ContainsKey(id))
                return ShotList[id];
            return null;
        }

        public Shot GetShotByBZFSID(int bzfsID)
        {
            if (BZFStoGlobalIDMap.ContainsKey(bzfsID))
                return GetShotByID(BZFStoGlobalIDMap[bzfsID]);
            return null;
        }

        public void RemoveShotByBZFSID(int bzfsID)
        {
            Shot s = GetShotByBZFSID(bzfsID);
            if (s == null)
                return;

            RemoveShotByGID(s.GlobalID);
        }

        public void RemoveShotByImpact(int bzfsID)
        {
            Shot s = GetShotByBZFSID(bzfsID);
            if (s == null)
                return;

            RemoveShotByGID(s.GlobalID);

            if (ExplosionCreated != null)
            {
                ExplosionEventArgs args = new ExplosionEventArgs();
                args.Position = s.Position;
                args.LastVelocity = s.Velocity;
                args.Reson = ExplosionEventArgs.Reasons.ImpactEnd;

                ExplosionCreated.Invoke(this, args);
            }
        }

        public void HandleShotEnd(NetworkMessage msg)
        {
            MsgShotEnd se = msg as MsgShotEnd;

            Shot s = GetShotByBZFSID(BuildBZFSShotID(se.PlayerID, se.ShotID));
            if (s == null)
                return;

            RemoveShotByGID(s.GlobalID);

            if (ExplosionCreated != null)
            {
                ExplosionEventArgs args = new ExplosionEventArgs();
                args.Position = s.Position;
                args.LastVelocity = s.Velocity;
                args.Reson = se.Exploded ? ExplosionEventArgs.Reasons.ImpactEnd : ExplosionEventArgs.Reasons.LifetimeEnd;

                ExplosionCreated.Invoke(this, args);
            }

        }

        public void HandleGMUpdate(NetworkMessage msg)
        {
            MsgGMUpdate gm = msg as MsgGMUpdate;

            Shot s = GetShotByBZFSID(BuildBZFSShotID(gm.PlayerID, gm.ShotID));
            if (s == null)
                return;

            s.Position = gm.Position;
            s.Velocity = gm.Velocity;
            s.LastUpdate = PlayerList.Clock.StepTime;
            s.DeltaTime = gm.DeltaTime;
            s.Target = PlayerList.GetPlayerByID(gm.TargetID);

            if (ShotUpdated != null)
                ShotUpdated.Invoke(this, s);
        }

        // dumb generator
        public ShotPath GetShotPath(Shot shot, Player player, WorldMap map)
        {
            ShotPath path = new ShotPath();

            ShotPath.Segment seg = new ShotPath.Segment();
            seg.StartPoint = shot.InitalPosition;
            seg.EndPoint = shot.InitalPosition + (shot.InitalVelocity * shot.Lifetime);
            seg.StartT = shot.TimeSent;
            seg.EndT = shot.TimeSent + shot.Lifetime;
            path.Segments.Add(seg);

            return path;
        }
    }
}
