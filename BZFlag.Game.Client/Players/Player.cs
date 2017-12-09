using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;
using BZFlag.Data.Players;
using BZFlag.Data.Types;
using BZFlag.Networking.Messages.BZFS.Player;

using BZFlag.Game.Flags;
using BZFlag.Game.Shots;
using BZFlag.Map.Elements.Shapes;
using BZFlag.LinearMath;

namespace BZFlag.Game.Players
{
    public class Player : EventArgs
    {
        public int PlayerID = -1;

        public virtual bool IsLocalPlayer { get { return false; } }

        public PlayerTypes PlayerType = PlayerTypes.Unknown;

        public TeamColors Team = TeamColors.NoTeam;

        public string Callsign = string.Empty;
        public string Motto = string.Empty;

        public int Wins = 0;
        public int Losses = 0;
        public int TeamKills = 0;

        public int Handicap = 0;
        public byte[] IPAddress = new byte[0];

        public PlayerAttributes Attributes = PlayerAttributes.Unknown;

        public MsgPlayerUpdateBase LastUpdate = null;

        // infered values from updates
        public double PlayerSpawnTime = -1;
        public bool Active = false;
        public bool Teleporting = false;

        public bool AutoPilot = false;
        public bool Paused = false;
        public bool IsRabbit = false;

        // DR values
        public Vector3F Position = Vector3F.Zero;
        public float Azimuth = 0;

        // state values
        public FlagInstance CurrentFlag = null;

        public Dictionary<int, Shot> ShotList = new Dictionary<int, Shot>();

        public double TeleportStartTime = -1;
        public Teleporter PortSource = null;
        public Teleporter PortDestination = null;

        public Player() { }

        public Player(int id, string name)
        {
            PlayerID = id;
            Callsign = name;
        }

        public Player(int id, string name, TeamColors team)
        {
            PlayerID = id;
            Callsign = name;
            Team = team;
        }

        public override string ToString()
        {
            return string.Format("({0}) {1} : {2}", PlayerID, Callsign, Team);
        }

        public void SetTeleport(double time, Teleporter from, Teleporter to)
        {
            TeleportStartTime = time;
            PortSource = from;
            PortDestination = to;

            Teleporting = to != null;
        }

        public bool SetFlag(FlagInstance flag)
        {
            if (flag == CurrentFlag)
                return false;

            CurrentFlag = flag;
            return true;
        }

        public virtual void Update(double now, double delta)
        {
            double deltaFromUpdate = now - LastUpdate.TimeStamp;

            Position = LastUpdate.Position + (LastUpdate.Velocity * deltaFromUpdate);
            Azimuth = LastUpdate.Azimuth + (float)(LastUpdate.AngularVelocity * deltaFromUpdate);
        }

        public int AddShot(Shot shot)
        {
            if (ShotList.ContainsKey(shot.BZFSShotID))
            {
                Shot oldShot = ShotList[shot.BZFSShotID];
                ShotList[shot.BZFSShotID] = shot;
                return oldShot.GlobalID;
            }

            ShotList.Add(shot.BZFSShotID, shot);
            return shot.GlobalID;
        }

        public void RemoveShot(Shot shot)
        {
            if (ShotList.ContainsKey(shot.BZFSShotID) && ShotList[shot.BZFSShotID] == shot)
                ShotList.Remove(shot.BZFSShotID);
        }

        public virtual void Spawn(Vector3F position, float azimuth)
        {
            Position = position;
            Azimuth = azimuth;
        }
    }
}
