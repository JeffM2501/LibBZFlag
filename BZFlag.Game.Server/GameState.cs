using System;
using System.Collections.Generic;

using BZFlag.Data.BZDB;
using BZFlag.Data.Time;
using BZFlag.Game.Host.Players;
using BZFlag.Game.Host.World;

namespace BZFlag.Game.Host
{
    public class GameState
    {
        public bool IsPublic = false;

        public Clock GameTime;

        public Database BZDatabase;
        public BZDBCache Cache = null;

        public GameWorld World = null;
        public FlagManager Flags = null;

        public PlayerManager Players = null;
        public ChatManager Chat = null;

        public ShotManager Shots = null;

        public ServerConfig ConfigData = null;

        public void Create()
        {
            GameTime = new Clock();

            BZDatabase = new Database();
            Cache = null;

            World = new GameWorld();
            Flags = new FlagManager();

            Players = new PlayerManager();
            Chat = new ChatManager();

            Shots = new ShotManager();
        }

        internal void Set(GameState state)
        {
            IsPublic = state.IsPublic;
            GameTime = state.GameTime;

            BZDatabase = state.BZDatabase;
            Cache = BZDatabase.Cache;

            World = state.World;
            Flags = state.Flags;

            Players = state.Players;
            Chat = state.Chat;

            Shots = state.Shots;

            ConfigData = state.ConfigData;
        }

        public void Init(ServerConfig config)
        {
            Create();
            ConfigData = config;

            Cache = BZDatabase.Cache;

            Flags.Set(this);
            Shots.Set(this);
            Chat.Set(this);
            Players.Set(this);
        }
    }
}
