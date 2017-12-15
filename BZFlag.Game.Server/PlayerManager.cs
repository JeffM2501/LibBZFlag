
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

        public virtual bool AddPlayer(ServerPlayer player)
        {
            ServerHost.PreAddPlayer(player);

            if (player.ActualTeam == TeamColors.NoTeam)
                return false;

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

            player.NeedStartupInfo = false;

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

            return true;
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
