
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
using BZFlag.Data.Time;
using BZFlag.Data.Flags;
using BZFlag.Game.Host.World;

namespace BZFlag.Game.Host.Players
{
    public class PlayerManager  : Server.GameState
    {
        public Server ServerHost = null;

        public List<ServerPlayer> PlayerList = new List<ServerPlayer>();

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
                player.Info.Team = null;
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
                player.Info.Team = this;
                PlayerAdded?.Invoke(this, this);
            }

            public void SentTo(NetworkMessage message, bool useUDP)
            {
                lock(Members)
                {
                    foreach (var member in Members)
                        member.SendMessage(!useUDP, message);
                }
            }
        }

        public Dictionary<TeamColors, TeamInfo> Teams = new Dictionary<TeamColors, TeamInfo>();

        public event EventHandler<TeamInfo> TeamInited = null;
        public event EventHandler<TeamInfo> TeamEmpty = null;

        public class PlayerInfo
        {
            public TeamInfo Team = null;

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

            public int ShotImmunities = 0;

            public FlagManager.FlagInstance CariedFlag = null;

            public class ScoreInfo 
            {
                public int Wins = 0;
                public int Losses = 0;
                public int TeamKills = 0;

                public void ApplyScore(ScoreInfo delta)
                {
                    Wins += delta.Wins;
                    Losses += delta.Losses;
                    TeamKills += delta.TeamKills;
                }

                public void Pack(int playerID, MsgScore msg)
                {
                    MsgScore.ScoreData data = new MsgScore.ScoreData();

                    data.PlayerID = playerID;
                    data.Wins = Wins;
                    data.Losses = Losses;
                    data.TeamKills = TeamKills;

                    msg.Scores.Add(data);
                }

                public bool Empty
                {
                    get
                    {
                        return Wins == 0 && Losses == 0 && TeamKills == 0;
                    }
                }

                public override string ToString()
                {
                    return "W:" + Wins.ToString() + "L:" + Losses.ToString() + "TK:" + TeamKills.ToString();
                }
            }
            public ScoreInfo Score = new ScoreInfo();
        }

        public delegate bool ScoreUpdateCallback(ServerPlayer victim, ref PlayerInfo.ScoreInfo victimScoreDelta, ServerPlayer killer, ref PlayerInfo.ScoreInfo killerScoreDelta, BlowedUpReasons eventReason);
        public ScoreUpdateCallback ComputeScores = null;

        public event EventHandler<ServerPlayer> ScoreUpdated = null;


        public delegate bool PlayerSpawnCallback(ServerPlayer player, GameWorld map, ref Vector3F position, ref float rotation);
        public PlayerSpawnCallback ComputeSpawn = SimpleSpawn;

        public event EventHandler<ServerPlayer> PlayerSpawned = null;

        public class KilledEventArgs : EventArgs
        {
            public ServerPlayer Victim = null;
            public ServerPlayer Killer = null;

            public MsgKilled KillInfo = null;

            public ShotManager.ShotInfo Shot = null;
        }
        public event EventHandler<KilledEventArgs> PlayerKilled = null;

        public void Update(Clock gameTime)
        {
        }

        public virtual bool AddPlayer(ServerPlayer player)
        {
            player.Exited += Player_Exited;
            player.Disconnected += Player_Exited;

            ServerHost.PreAddPlayer(player);

            if (player.ActualTeam == TeamColors.NoTeam)
                return false;

            player.Info = new PlayerInfo();

            lock (PlayerList)
                PlayerList.Add(player);

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

            Flags.SendInitialFlagUpdate(player);

            // tell them about everyone except them
            ServerPlayer[] locals = null;

            lock (PlayerList)
                locals = PlayerList.ToArray();

            MsgPlayerInfo info = new MsgPlayerInfo();
            foreach (ServerPlayer peer in locals)
            {
                if (peer == player)
                    continue;

                player.SendMessage(true, BuildPlayerAdd(peer));
                info.PlayerUpdates.Add(GetPlayerInfo(peer));
            }
            player.SendMessage(true, info);

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

            // tell everyone they joined
            SendToAll(BuildPlayerAdd(player), false);

            // send info bits to everyone
            info = new MsgPlayerInfo();
            info.PlayerUpdates.Add(GetPlayerInfo(player));
            SendToAll(info, false);

            ServerHost.PostAddPlayer(player);

            if (player.ActualTeam == TeamColors.ObserverTeam)
                Chat.SendChatToUser(null, player, Resources.ObserverModeNotificatioMessage, false);

            return true;
        }

        internal void HandleKilled(ServerPlayer player, MsgKilled msgKilled)
        {
            if (msgKilled == null || player == null)
                return;

            KilledEventArgs args = new KilledEventArgs();
            msgKilled.VictimID = player.PlayerID;
            args.Victim = player;
            args.Killer = GetPlayerByID(msgKilled.KillerID);
            args.Shot = Shots.FindKillableShot(msgKilled.KillerID, msgKilled.ShotID);
            args.KillInfo = msgKilled;

            KillPlayer(player, args);
        }

        public void KillPlayer(ServerPlayer player, KilledEventArgs args)
        {
            if (args == null || player == null)
                return;

            if (!player.Info.Alive)
                Logger.Log0("Player " + player.Callsign + " killed while dead");

            player.Info.Alive = false;

            PlayerKilled?.Invoke(this, args);

            Logger.Log4("Player " + player.Callsign + " killed by " + args.KillInfo.Reason.ToString());

            bool wasFromAFlag = false;
            switch (args.KillInfo.Reason)
            {
                case BlowedUpReasons.GotKilledMsg:
                    break;

                case BlowedUpReasons.GotShot:
                    wasFromAFlag = true;
                    Shots.RemoveShotForDeath(player, args.KillInfo.KillerID, args.KillInfo.ShotID);

                    if (args.Shot != null)// tell the flag it took a hit
                        Flags.HandlePlayerTakeHit(player, args.Killer, args.Shot);
                    break;

                case BlowedUpReasons.GotRunOver:
                    wasFromAFlag = true;
                    break;

                case BlowedUpReasons.GotCaptured:
                    break;

                case BlowedUpReasons.GenocideEffect:
                    break;

                case BlowedUpReasons.SelfDestruct:
                    break;

                case BlowedUpReasons.WaterDeath:
                    break;

                case BlowedUpReasons.DeathTouch:
                    break;

                case BlowedUpReasons.LastReason:
                case BlowedUpReasons.Unknown:
                    Logger.Log0("Player " + player.Callsign + " killed by a method that should not happen");
                    break;
            }

            if (wasFromAFlag)   // tell the flag it killed
                Flags.HandlePlayerDoDamage(player, args.Killer, FlagTypeList.GetFromAbriv(args.KillInfo.FlagAbreviation));

            // process any scores
            PlayerInfo.ScoreInfo vicScores = new PlayerInfo.ScoreInfo();
            PlayerInfo.ScoreInfo killerScores = new PlayerInfo.ScoreInfo();
            if (ComputeScores(args.Victim, ref vicScores, args.Killer, ref killerScores, args.KillInfo.Reason))
            {
                args.Victim.Info.Score.ApplyScore(vicScores);
                if (args.Killer != null)
                    args.Killer.Info.Score.ApplyScore(killerScores);

                MsgScore scoreMessage = new MsgScore();
                if (!vicScores.Empty)
                {
                    Logger.Log3("Player " + player.Callsign + " score updated by " + vicScores.ToString());

                    ScoreUpdated?.Invoke(this, args.Victim);
                    args.Victim.Info.Score.Pack(args.Victim.PlayerID, scoreMessage);
                }

                if (args.Killer != null && !killerScores.Empty)
                {
                    Logger.Log3("Player " + player.Callsign + " score updated by " + killerScores.ToString());

                    ScoreUpdated?.Invoke(this, args.Killer);
                    args.Killer.Info.Score.Pack(args.Killer.PlayerID, scoreMessage);
                }

                if (scoreMessage.Scores.Count > 0)
                    SendToAll(scoreMessage, false);
            }

            MsgKilled killedMessage = new MsgKilled();
            killedMessage.VictimID = args.Victim.PlayerID;
            killedMessage.KillerID = args.Killer != null ? args.Killer.PlayerID : PlayerConstants.ServerPlayerID;
            killedMessage.ShotID = args.Shot != null ? args.Shot.PlayerShotID : -1;
            killedMessage.Reason = args.KillInfo.Reason;
            killedMessage.FlagAbreviation = (args.Shot != null && args.Shot.SourceFlag != null) ? args.Shot.SourceFlag.FlagAbbv : string.Empty;
            killedMessage.PhysicsDriverID = args.KillInfo.PhysicsDriverID;

            SendToAll(killedMessage, false);
        }

        private MsgPlayerInfo.PlayerInfoData GetPlayerInfo(ServerPlayer player)
        {
            MsgPlayerInfo.PlayerInfoData d = new MsgPlayerInfo.PlayerInfoData();
            d.PlayerID = player.PlayerID;
            if (player.BZID != string.Empty)
                d.Attributes = PlayerAttributes.IsVerified;

            if (player.ShowAdminMark)
                d.Attributes |= PlayerAttributes.IsAdmin;

            return d;
        }

        private MsgAddPlayer BuildPlayerAdd(ServerPlayer player)
        {
            MsgAddPlayer add = new MsgAddPlayer();

            add.PlayerID = player.PlayerID;
            add.Callsign = player.Callsign;
            add.Team = player.ActualTeam;
            add.Motto = player.Motto;
            add.PlayerType = 0;
            add.Wins = player.Info.Score.Wins;
            add.Losses = player.Info.Score.Losses;
            add.TeamKills = player.Info.Score.TeamKills;

            return add;
        }

        private void Player_Exited(object sender, Networking.Common.Peer e)
        {
            ServerPlayer sp = e as ServerPlayer;
            if (sp == null || !PlayerList.Contains(sp))
                return;

            lock (PlayerList)
                PlayerList.Remove(sp);

            MsgRemovePlayer exit = new MsgRemovePlayer();
            exit.PlayerID = sp.PlayerID;
            SendToAll(exit,false);

            if (Teams.ContainsKey(sp.ActualTeam))
                Teams[sp.ActualTeam].Remove(sp);
        }

        protected static bool SimpleSpawn(ServerPlayer player, GameWorld map, ref Vector3F position, ref float rotation)
        {
            return map.GetSpawn(ref player.Info.LastSpawnState.Position, ref player.Info.LastSpawnState.Azimuth);
        }

        public void StartSpawn(ServerPlayer player, MsgAlive spawnRequest)
        {
            MsgAlive spawnPostion = new MsgAlive();
            spawnPostion.IsSpawn = false;

            // TODO, run a thread task to find a spawn.
            bool ret = false;
            if (ComputeSpawn != null)
                ret = ComputeSpawn(player, World, ref player.Info.LastSpawnState.Position, ref player.Info.LastSpawnState.Azimuth);

            if (ret)
            {
                player.Info.Alive = true;

                PlayerSpawned?.Invoke(this, player);

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

        public virtual ServerPlayer GetPlayerByID(int playerID)
        {
            lock (PlayerList)
                return PlayerList.Find((x) => x.PlayerID == playerID);
        }

        public virtual ServerPlayer GetPlayerByCallsign(string callsign)
        {
            string c = callsign.ToUpperInvariant();

            lock (PlayerList)
                return PlayerList.Find((x) => x.Callsign.ToUpperInvariant() == c);
        }

        public virtual void SendToAll(NetworkMessage message, bool useUDP)
        {
            ServerPlayer[] locals = null;

            lock (PlayerList)
                locals = PlayerList.ToArray();

            foreach (ServerPlayer player in locals)
                player.SendMessage(!useUDP, message);
        }

        public virtual void SendToTeam(NetworkMessage message, TeamColors team, bool useUDP)
        {
            if (Teams.ContainsKey(team))
                Teams[team].SentTo(message, useUDP);
        }

        public virtual void SendToPlayerID(NetworkMessage message, bool useUDP, int playerID)
        {
            ServerPlayer player = GetPlayerByID(playerID);
            if (player == null)
                return;

            player.SendMessage(!useUDP, message);
        }
    }
}
