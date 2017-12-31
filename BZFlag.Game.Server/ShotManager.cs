using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.LinearMath;
using BZFlag.Data.Players;
using BZFlag.Data.Teams;
using BZFlag.Game.Host.Players;
using BZFlag.Networking.Messages.BZFS.Shots;
using BZFlag.Data.Flags;
using BZFlag.Data.Time;

namespace BZFlag.Game.Host
{
    public class ShotManager
    {
        public Server ServerHost = null;

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

            public bool Allow = false;
        }

        public delegate ShotTypes GetDefaultShotTypeCallback(ServerPlayer player);

        public GetDefaultShotTypeCallback GetDefaultShotType = new GetDefaultShotTypeCallback((x) => ShotTypes.NormalShot);

        protected List<ShotInfo> Shots = new List<ShotInfo>();
        protected List<ShotInfo> RecentlyDeadShots = new List<ShotInfo>();
        protected int LastShotID = -1;


        public event EventHandler<ShotInfo> ShotPreFire;
        public event EventHandler<ShotInfo> ShotFired;

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

            if (sender.CariedFlag == null)
            {
                shot.ShotType = GetDefaultShotType(sender);
                shot.SourceFlag = null;
            }
            else
            {
                shot.ShotType = sender.CariedFlag.Flag.FlagShot;
                shot.SourceFlag = sender.CariedFlag.Flag;
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
            ServerHost.State.Players.SendToAll(shotMessage, shotMessage.FromUDP);
        }

        public void FireShot(ShotInfo shot)
        {
            shot.CreateTimeStamp = ServerHost.State.GameTime.Now;

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

            lock (Shots)
                Shots.Add(shot);

            ServerHost.State.Players.SendToAll(shotMessage, false);

            ShotFired?.Invoke(this, shot);
            shot.Allow = true;
        }

        public void EndShot(ShotInfo shot)
        {
        }


        public void Update(Clock gameTime)
        {
            ShotInfo[] toProcess;
            lock (Shots)
                toProcess = Shots.ToArray();

            double now = ServerHost.State.GameTime.Now;

            List<ShotInfo> expired = new List<ShotInfo>();
            foreach (var shot in toProcess)
            {
                if (now - shot.CreateTimeStamp > shot.Lifetime)
                    expired.Add(shot);
                else
                {
                    // update the position?
                }
            }

            lock (Shots)
                Shots.RemoveAll((x) => expired.Contains(x));

            foreach (var shot in expired)
            {
                if (shot.Owner == null)
                {
                    shot.Allow = false;
                    EndShot(shot);
                }
            }

            expired.RemoveAll((x) => !x.Allow);

            // do something with the expired shots, we expect them to be removed shortly
            lock (RecentlyDeadShots)
                RecentlyDeadShots.AddRange(expired.ToArray());
        }
    }
}
