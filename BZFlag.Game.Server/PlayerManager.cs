
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BZFlag.Data.Teams;
using BZFlag.Game.Host.Players;
using BZFlag.Networking.Messages;
using BZFlag.Networking.Messages.BZFS.Player;
using BZFlag.Networking.Messages.BZFS.Info;
using BZFlag.Networking.Messages.BZFS;
using BZFlag.Data.Players;
using BZFlag.LinearMath;

namespace BZFlag.Game.Host
{
    public class PlayerManager
    {
        public Server ServerHost = null;

        public List<ServerPlayer> Players = new List<ServerPlayer>();

        public class TeamInfo : EventArgs
        {
            public TeamColors Team = TeamColors.NoTeam;

            public List<ServerPlayer> Members = new List<ServerPlayer>();
            public int MaxPlayers = int.MaxValue;

            public int Wins = 0;
            public int Losses = 0;

            public event EventHandler<TeamInfo> PlayerAdded = null;
            public event EventHandler<TeamInfo> PlayerRemoved = null;

            public int Size()
            {
                lock (Members)
                    return Members.Count();
            }

            public void Remove(ServerPlayer player)
            {
                lock(Members)
                    Members.Remove(player);

                PlayerRemoved?.Invoke(this, this);
            }

            public void Add(ServerPlayer player)
            {
                lock (Members)
                {
                    if (Members.Count == 0)
                    {
                        Wins = 0;
                        Losses = 0;
                    }
                    Members.Add(player);
                }
                PlayerAdded?.Invoke(this, this);
            }
        }

        public Dictionary<TeamColors, TeamInfo> Teams = new Dictionary<TeamColors, TeamInfo>();

        public event EventHandler<TeamInfo> TeamInited = null;
        public event EventHandler<TeamInfo> TeamEmpty = null;

        public class PlayerInfo
        {
            public class MotionInfo
            {
                public double TimeStamp = 0;
                public Vector3F Position = Vector3F.Zero;
                public Vector3F Velocity = Vector3F.Zero;
                public float Azimuth = 0;
                public float AngularVelocity = 0;
            }

            public MotionInfo CurrentState = new MotionInfo();
            public MotionInfo LastSentUpdate = new MotionInfo();
            public MotionInfo LastSpawnState = new MotionInfo();

            public bool Alive = false;

        }

        public virtual bool AddPlayer(ServerPlayer player)
        {
            ServerHost.PreAddPlayer(player);

            if (player.ActualTeam == TeamColors.NoTeam)
                return false;

            player.Info = new PlayerInfo();

            lock (Players)
                Players.Add(player);

            Logger.Log2("Player " + player.Callsign + " assigned to team " + player.ActualTeam.ToString());

            lock (Teams)
            {
                if (!Teams.ContainsKey(player.ActualTeam))
                {
                    TeamInfo team = new TeamInfo();
                    team.Team = player.ActualTeam;
                    team.PlayerAdded += Team_PlayerAdded;
                    team.PlayerRemoved += Team_PlayerRemoved;
                    Teams.Add(player.ActualTeam, team);
                }
                    

                Teams[player.ActualTeam].Add(player);
                if (Teams[player.ActualTeam].Members.Count == 1)
                    TeamInited?.Invoke(this, Teams[player.ActualTeam]);
            }

            // tell everyone they joined

            MsgAddPlayer add = new MsgAddPlayer();

            add.PlayerID = player.PlayerID;
            add.Callsign = player.Callsign;
            add.Team = player.ActualTeam;
            add.Motto = player.Motto;
            add.PlayerType = 0;
            add.Wins = player.Score.Wins;
            add.Losses = player.Score.Losses;
            add.TeamKills = player.Score.TeamKills;

            SendToAll(add, false);


            TeamInfo[] teams = null;
            lock (Teams)
                teams = Teams.Values.ToArray();

            MsgTeamUpdate tUpd = new MsgTeamUpdate();
            foreach (var team in teams)
            {
                if (team.Team == player.ActualTeam) // the event already sent this
                    continue;

                MsgTeamUpdate.TeamUpdate u = new MsgTeamUpdate.TeamUpdate();
                u.TeamID = team.Team;
                u.Size = team.Size();
                u.Wins = team.Wins;
                u.Losses = team.Losses;
                tUpd.TeamUpdates.Add(u);
            }
            player.SendMessage(tUpd);

            ServerHost.PostAddPlayer(player);

            if (player.ActualTeam == TeamColors.ObserverTeam)
            {
                MsgMessage observerMsg = new MsgMessage();
                observerMsg.From = PlayerConstants.ServerPlayerID;
                observerMsg.To = player.PlayerID;
                observerMsg.MessageType = MsgMessage.MessageTypes.ChatMessage;
                observerMsg.MessageText = "You are in observer mode.";
                player.SendMessage(observerMsg);
            }

            // send info bits
            MsgPlayerInfo info = new MsgPlayerInfo();

            MsgPlayerInfo.PlayerInfoData d = new MsgPlayerInfo.PlayerInfoData();
            d.PlayerID = player.PlayerID;
            if (player.BZID != string.Empty)
                d.Attributes = PlayerAttributes.IsVerified;

            info.PlayerUpdates.Add(d);

            SendToAll(info, false);

            return true;
        }

        public void StartSpawn(ServerPlayer player, MsgAlive spawnRequest)
        {
            MsgAlive spawnPostion = new MsgAlive();
            spawnPostion.IsSpawn = false;

            // TODO, run a thread task to find a spawn.

            if (ServerHost.State.World.GetSpawn(ref player.Info.LastSpawnState.Position, ref player.Info.LastSpawnState.Azimuth))
            {
                player.Info.Alive = true;

                spawnPostion.PlayerID = player.PlayerID;
                spawnPostion.Position = player.Info.LastSpawnState.Position;
                spawnPostion.Azimuth = player.Info.LastSpawnState.Azimuth;

                SendToAll(spawnPostion, false);
            }
        }

        protected void SendTeamUpdate(TeamInfo team)
        {
            if (team == null)
                return;

            MsgTeamUpdate tUpd = new MsgTeamUpdate();

            MsgTeamUpdate.TeamUpdate u = new MsgTeamUpdate.TeamUpdate();

            u.TeamID = team.Team;
            u.Size = team.Size();
            u.Wins = team.Wins;
            u.Losses = team.Losses;
            tUpd.TeamUpdates.Add(u);

            SendToAll(tUpd, false);
        }

        public void PlayerUpdate(ServerPlayer player, MsgPlayerUpdateBase updMessage)
        {
            if (updMessage != null)
                SendToAll(updMessage, updMessage.FromUDP);
        }

        private void Team_PlayerRemoved(object sender, TeamInfo e)
        {
            SendTeamUpdate(e);
        }

        private void Team_PlayerAdded(object sender, TeamInfo e)
        {
            SendTeamUpdate(e);
        }

        public virtual void RemovePlayer(ServerPlayer player)
        {
            lock (Teams)
            {
                if (Teams.ContainsKey(player.ActualTeam))
                    Teams[player.ActualTeam].Remove(player);

                if (Teams[player.ActualTeam].Members.Count == 0)
                    TeamEmpty?.Invoke(this, Teams[player.ActualTeam]);
            }
        }

        public int GetTeamPlayerCount(TeamColors team)
        {
            lock (Teams)
            {
                if (Teams.ContainsKey(team))
                    return Teams[team].Members.Count;
                return 0;
            }
        }

        public bool ValidPlayerTeam(TeamColors color)
        {
            switch (color)
            {
                case TeamColors.RedTeam:
                case TeamColors.BlueTeam:
                case TeamColors.GreenTeam:
                case TeamColors.PurpleTeam:
                case TeamColors.RogueTeam:
                case TeamColors.ObserverTeam:
                    return ServerHost.ConfigData.TeamData.GetTeamLimit(color) != 0;
            }
            return false;
        }

        public TeamColors GetSmallestTeam(bool includeRogue)
        {
            int size = int.MaxValue;
            TeamColors team = includeRogue ? TeamColors.RogueTeam : TeamColors.RedTeam;

            for (TeamColors t = TeamColors.RogueTeam; t < TeamColors.ObserverTeam; t++)
            {
                int count = GetTeamPlayerCount(t);
                if (count < size)
                {
                    size = count;
                    team = t;
                }
            }

            return team;
        }

        public TeamColors GetLargestTeam(bool includeRogue)
        {
            int size = int.MinValue;
            TeamColors team = includeRogue ? TeamColors.RogueTeam : TeamColors.RedTeam;

            for (TeamColors t = TeamColors.RogueTeam; t < TeamColors.ObserverTeam; t++)
            {
                int count = GetTeamPlayerCount(t);
                if (count > size)
                {
                    size = count;
                    team = t;
                }
            }

            return team;
        }


        protected virtual void SendToAll(NetworkMessage message, bool useUDP)
        {
            ServerPlayer[] locals = null;

            lock (Players)
                locals = Players.ToArray();

            foreach (ServerPlayer player in locals)
                player.SendMessage(!useUDP, message);
        }
    }
}
