using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Game.Players;
using BZFlag.Networking.Messages;
using BZFlag.Networking.Messages.BZFS.Player;
using BZFlag.Networking.Messages.BZFS.Info;
using BZFlag.Data.Players;
using BZFlag.Data.Flags;
using BZFlag.Networking.Messages.BZFS.Control;
using BZFlag.LinearMath;


namespace BZFlag.Game.Players
{
    public class PlayerManager
    {
        public Dictionary<int, Player> PlayerList = new Dictionary<int, Player>();

        // Special Players
        public readonly Player ServerPlayer = new Player(PlayerConstants.ServerPlayerID, "Server");
        public readonly Player AllPlayers = new Player(PlayerConstants.AllPlayersID, "Everyone");

        public readonly Player RogueTeam = new Player(PlayerConstants.RogueTeamID, "Rogue", Data.Teams.TeamColors.RogueTeam);
        public readonly Player RedTeam = new Player(PlayerConstants.RedTeamID, "Red", Data.Teams.TeamColors.RedTeam);
        public readonly Player GreenTeam = new Player(PlayerConstants.GreenTeamID, "Green", Data.Teams.TeamColors.GreenTeam);
        public readonly Player BlueTeam = new Player(PlayerConstants.BlueTeamID, "Blue", Data.Teams.TeamColors.BlueTeam);
        public readonly Player PurpleTeam = new Player(PlayerConstants.PurpleTeamID, "Purple", Data.Teams.TeamColors.PurpleTeam);
        public readonly Player ObserverTeam = new Player(PlayerConstants.ObserverTeamID, "Observers", Data.Teams.TeamColors.ObserverTeam);
        public readonly Player RabbitTeam = new Player(PlayerConstants.RabbitTeamID, "Rabbits", Data.Teams.TeamColors.RabbitTeam);
        public readonly Player HunterTeam = new Player(PlayerConstants.HunterTeamID, "Hunters", Data.Teams.TeamColors.HunterTeam);

        protected Dictionary<int, Player> SpecialPlayers = new Dictionary<int, Player>();

        public Player[] GetPlayerList() { return PlayerList.Values.ToArray(); }

        public int LocalPlayerID = -1;
        public LocalPlayer Me = null;

        public GameTime Clock = new GameTime();

        public Player GetPlayerByID(int id)
        {
            if (!PlayerList.ContainsKey(id))
                return null;

            return PlayerList[id];
        }

        public Player GetPlayerByID(int id, bool allowSpecial)
        {
            if (!PlayerList.ContainsKey(id))
            {
                if (allowSpecial)
                {
                    if (SpecialPlayers.ContainsKey(id))
                        return SpecialPlayers[id];
                }
                return null;
            }

            return PlayerList[id];
        }

        public event EventHandler<Player> PlayerAdded = null;
        public event EventHandler<Player> PlayerRemoved = null;

        public event EventHandler<Player> PlayerSpawned = null;
        public event EventHandler<Player> SelfSpawned = null;

        public class KilledEventArgs : EventArgs
        {
            public Player Victim = null;
            public Player Killer = null;

            public BlowedUpReasons Reason = BlowedUpReasons.Unknown;
            public int InstrumentID = -1; // shot or physics
            public FlagType KilledByFlag = null;
        }

        public event EventHandler<KilledEventArgs> PlayerKilled = null;

        public event EventHandler<Player> PlayerInfoUpdated = null;
        public event EventHandler<Player> PlayerAdminInfoUpdated = null;
        public event EventHandler<Player> PlayerStateUpdated = null;

        public event EventHandler<Player> PlayerPaused = null;
        public event EventHandler<Player> PlayerAutoPiloted = null;
        public event EventHandler<Player> PlayerMadeRabbit = null;

        public event EventHandler<Player> SelfAdded = null;
        public event EventHandler<Player> SelfRemoved = null;


        public PlayerManager()
        {
            SpecialPlayers.Add(ServerPlayer.PlayerID, ServerPlayer);
            SpecialPlayers.Add(AllPlayers.PlayerID, AllPlayers);

            SpecialPlayers.Add(RogueTeam.PlayerID, RogueTeam);
            SpecialPlayers.Add(RedTeam.PlayerID, RedTeam);
            SpecialPlayers.Add(GreenTeam.PlayerID, GreenTeam);
            SpecialPlayers.Add(BlueTeam.PlayerID, BlueTeam);
            SpecialPlayers.Add(PurpleTeam.PlayerID, PurpleTeam);
            SpecialPlayers.Add(ObserverTeam.PlayerID, ObserverTeam);
            SpecialPlayers.Add(RabbitTeam.PlayerID, RabbitTeam);
            SpecialPlayers.Add(HunterTeam.PlayerID, HunterTeam);
        }

        public void Update()
        {
            foreach (Player p in PlayerList.Values)
            {
                if (!p.Active || p.Team == Data.Teams.TeamColors.ObserverTeam)
                    continue;

                p.Update(Clock.StepTime, Clock.StepDelta);
            }
        }

        public void HandleAddPlayer(NetworkMessage msg)
        {
            MsgAddPlayer ap = msg as MsgAddPlayer;
            if (!PlayerList.ContainsKey(ap.PlayerID))
            {
                if (LocalPlayerID == ap.PlayerID)
                    PlayerList.Add(ap.PlayerID, new LocalPlayer(null));
                else
                    PlayerList.Add(ap.PlayerID, new Player());
            }

            Player player = PlayerList[ap.PlayerID];
            player.PlayerID = ap.PlayerID;

            player.Callsign = ap.Callsign;
            player.Motto = ap.Motto;

            player.PlayerType = (PlayerTypes)ap.PlayerType;
            player.Team = ap.Team;
            player.Wins = ap.Wins;
            player.Losses = ap.Losses;
            player.TeamKills = ap.TeamKills;

            if (player.IsLocalPlayer) // hey it's us!
            {
                Me = player as LocalPlayer;
                if (SelfAdded != null)
                    SelfAdded.Invoke(this, player);
            }

            if (PlayerAdded != null)
                PlayerAdded.Invoke(this, player);
        }

        public void HandleRemovePlayer(NetworkMessage msg)
        {
            MsgRemovePlayer rp = msg as MsgRemovePlayer;

            var player = GetPlayerByID(rp.PlayerID);
            if (player == null)
                return;

            PlayerList.Remove(player.PlayerID);

            if (PlayerRemoved != null)
                PlayerRemoved.Invoke(this, player);

            if (player.IsLocalPlayer)   //oh shit it's us!
            {
                Me = null;
                if (SelfRemoved != null)
                    SelfRemoved.Invoke(this, player);
            }
        }

        public void HandlePlayerInfo(NetworkMessage msg)
        {
            MsgPlayerInfo info = msg as MsgPlayerInfo;
            foreach (var p in info.PlayerUpdates)
            {
                Player player = GetPlayerByID(p.PlayerID);
                if (player != null)
                {
                    player.Attributes = p.Attributes;

                    if (PlayerInfoUpdated != null)
                        PlayerInfoUpdated.Invoke(this, player);
                }
            }
        }

        public void HandleScoreUpdate(NetworkMessage msg)
        {
            MsgScore sc = msg as MsgScore;
            Player player = GetPlayerByID(sc.PlayerID);
            if (player == null)
                return;

            player.Wins = sc.Wins;
            player.Losses = sc.Losses;
            player.TeamKills = sc.TeamKills;

            if (PlayerInfoUpdated != null)
                PlayerInfoUpdated.Invoke(this, player);
        }

        public void HandlePlayerUpdate(NetworkMessage msg)
        {
            MsgPlayerUpdateBase upd = msg as MsgPlayerUpdateBase;
            Player player = GetPlayerByID(upd.PlayerID);
            if (player == null)
                return;

            player.LastUpdate = upd;
            if (PlayerStateUpdated != null)
                PlayerStateUpdated.Invoke(this, player);
        }

        public void HandleAlive(NetworkMessage msg)
        {
            MsgAlive alive = msg as MsgAlive;

            Player player = GetPlayerByID(alive.PlayerID);

            if (player == null)
                return;

            player.Active = true;
            player.PlayerSpawnTime = Clock.StepTime;
            player.SetTeleport(-1, null, null);

            player.Spawn(alive.Position, alive.Azimuth);

            if (alive.PlayerID == LocalPlayerID)
                SelfSpawned?.Invoke(this, player);

            PlayerSpawned?.Invoke(this, player);
        }

        public void HandleKilled(NetworkMessage msg)
        {
            MsgKilled killed = msg as MsgKilled;

            KilledEventArgs args = new KilledEventArgs();

            args.Victim = GetPlayerByID(killed.VictimID);
            args.Killer = GetPlayerByID(killed.KillerID);
            args.Reason = killed.Reason;
            args.KilledByFlag = FlagTypeList.GetFromAbriv(killed.FlagAbreviation);
            if (killed.Reason == BlowedUpReasons.DeathTouch)
                args.InstrumentID = killed.PhysicsDriverID;
            else
            {
                args.InstrumentID = killed.ShotID;

                if (killed.ShotID > 0)
                {
                    // kill shot 
                    int bzfsShotID = (killed.KillerID * byte.MaxValue) + killed.ShotID;
                    // tell the shot manager to kill that thar shot
                }
            }

            if (args.Victim != null)
            {
                args.Victim.SetTeleport(-1, null, null);
                args.Victim.Active = false;
                args.Victim.IsRabbit = false;
            }


            if (PlayerKilled != null)
                PlayerKilled.Invoke(this, args);
        }

        public void HandleHandicap(NetworkMessage msg)
        {
            MsgHandicap update = msg as MsgHandicap;

            foreach (var u in update.Handicaps)
            {
                Player p = GetPlayerByID(u.Key);
                if (p != null)
                {
                    p.Handicap = u.Value;
                    if (PlayerInfoUpdated != null)
                        PlayerInfoUpdated.Invoke(this, p);
                }
            }
        }

        public void HandlePause(NetworkMessage msg)
        {
            MsgPause pa = msg as MsgPause;
            Player p = GetPlayerByID(pa.PlayerID);
            if (p != null)
            {
                p.Paused = pa.Paused;

                if (PlayerPaused != null)
                    PlayerStateUpdated.Invoke(this, p);
            }
        }
        public void HandleAutoPilot(NetworkMessage msg)
        {
            MsgAutoPilot ap = msg as MsgAutoPilot;
            Player p = GetPlayerByID(ap.PlayerID);
            if (p != null)
            {
                p.AutoPilot = ap.AutoPilot;

                if (PlayerAutoPiloted != null)
                    PlayerAutoPiloted.Invoke(this, p);
            }
        }

        public void HandleNewRabbit(NetworkMessage msg)
        {
            MsgNewRabbit nr = msg as MsgNewRabbit;
            Player p = GetPlayerByID(nr.PlayerID);
            if (p != null)
            {
                p.IsRabbit = true;

                if (PlayerMadeRabbit != null)
                    PlayerMadeRabbit.Invoke(this, p);
            }
        }

        public void HandleAdminInfo(NetworkMessage msg)
        {
            MsgAdminInfo s = msg as MsgAdminInfo;
            foreach (var i in s.Records)
            {
                Player p = GetPlayerByID(i.PlayerID);
                if (p == null)
                    continue;

                p.IPAddress = i.IPAddress;
                if (PlayerAdminInfoUpdated != null)
                    PlayerAdminInfoUpdated.Invoke(this, p);
            }
        }
    }
}
