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

namespace BZFlag.Game.Players
{
	public class PlayerManager
	{
		public Dictionary<int, Player> PlayerList = new Dictionary<int, Player>();

		public int LocalPlayerID = -1;
		public Player LocalPlayer = null;

		public GameTime Clock = new GameTime();
		public FlagTypeList FlagTypes = null;

		public Player GetPlayerByID(int id)
		{
			if(!PlayerList.ContainsKey(id))
				return null;

			return PlayerList[id];
		}

		public event EventHandler<Player> PlayerAdded = null;
		public event EventHandler<Player> PlayerRemoved = null;

		public event EventHandler<Player> PlayerSpawned = null;

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
		public event EventHandler<Player> PlayerStateUpdated = null;

		public event EventHandler<Player> SelfAdded = null;
		public event EventHandler<Player> SelfRemoved = null;

		public void HandleAddPlayer(NetworkMessage msg)
		{
			MsgAddPlayer ap = msg as MsgAddPlayer;
			if(!PlayerList.ContainsKey(ap.PlayerID))
				PlayerList.Add(ap.PlayerID, new Player());

			Player player = PlayerList[ap.PlayerID];

			player.Callsign = ap.Callsign;
			player.Motto = ap.Motto;

			player.PlayerType = (PlayerTypes)ap.PlayerType;
			player.Team = ap.Team;
			player.Wins = ap.Wins;
			player.Losses = ap.Losses;
			player.TeamKills = ap.TeamKills;

			if(LocalPlayerID == player.PlayerID) // hey it's us!
			{
				LocalPlayer = player;
				if(SelfAdded != null)
					SelfAdded.Invoke(this, player);
			}

			if(PlayerAdded != null)
				PlayerAdded.Invoke(this, player);
		}

		public void HandleRemovePlayer(NetworkMessage msg)
		{
			MsgRemovePlayer rp = msg as MsgRemovePlayer;

			var player = GetPlayerByID(rp.PlayerID);
			if(player == null)
				return;

			PlayerList.Remove(player.PlayerID);

			if(PlayerRemoved != null)
				PlayerRemoved.Invoke(this, player);

			if(LocalPlayer == player)   //oh shit it's us!
			{
				LocalPlayer = null;
				if(SelfRemoved != null)
					SelfRemoved.Invoke(this, player);
			}
		}

		public void HandlePlayerInfo(NetworkMessage msg)
		{
			MsgPlayerInfo info = msg as MsgPlayerInfo;
			foreach(var p in info.PlayerUpdates)
			{
				Player player = GetPlayerByID(p.PlayerID);
				if(player != null)
				{
					player.Attributes = p.Attributes;

					if(PlayerInfoUpdated != null)
						PlayerInfoUpdated.Invoke(this, player);
				}
			}
		}

		public void HandleScoreUpdate(NetworkMessage msg)
		{
			MsgScore sc = msg as MsgScore;
			Player player = GetPlayerByID(sc.PlayerID);
			if(player == null)
				return;

			player.Wins = sc.Wins;
			player.Losses = sc.Losses;
			player.TeamKills = sc.TeamKills;

			if(PlayerInfoUpdated != null)
				PlayerInfoUpdated.Invoke(this, player);
		}

		public void HandlePlayerUpdate(NetworkMessage msg)
		{
			MsgPlayerUpdateBase upd = msg as MsgPlayerUpdateBase;
			Player player = GetPlayerByID(upd.PlayerID);
			if(player == null)
				return;

			player.LastUpdate = upd;
			if(PlayerStateUpdated != null)
				PlayerStateUpdated.Invoke(this, player);
		}

		public void HandleAlive(NetworkMessage msg)
		{
			MsgAlive alive = msg as MsgAlive;

			Player player = GetPlayerByID(alive.PlayerID);

			player.Active = true;
			player.PlayerSpawnTime = Clock.GetStepTime();

			player.Position = alive.Position;
			player.Azimuth = alive.Azimuth;

			if(PlayerSpawned != null)
				PlayerSpawned.Invoke(this, player);
		}

		public void HandleKilled(NetworkMessage msg)
		{
			MsgKilled killed = msg as MsgKilled;

			KilledEventArgs args = new KilledEventArgs();

			args.Victim = GetPlayerByID(killed.VictimID);
			args.Killer = GetPlayerByID(killed.KillerID);
			args.Reason = killed.Reason;
			args.KilledByFlag = FlagTypes.GetFromAbriv(killed.FlagAbreviation);
			if(killed.Reason == BlowedUpReasons.DeathTouch)
				args.InstrumentID = killed.PhysicsDriverID;
			else
			{
				args.InstrumentID = killed.ShotID;

				if(killed.ShotID > 0)
				{
					// kill shot 
					int bzfsShotID = (killed.KillerID * byte.MaxValue) + killed.ShotID;
					// tell the shot manager to kill that thar shot
				}
			}

			if(args.Victim != null)
				args.Victim.Active = false;

			if(PlayerKilled != null)
				PlayerKilled.Invoke(this, args);
		}
	}
}
