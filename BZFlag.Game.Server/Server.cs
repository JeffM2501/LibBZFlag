
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BZFlag.Services;
using BZFlag.Game.Host.Processors;
using BZFlag.Game.Host.Players;
using BZFlag.Data.Players;
using BZFlag.Game.Host.API;

using BZFlag.Game.Host.World;
using BZFlag.Data.Teams;

namespace BZFlag.Game.Host
{
    public partial class Server
    {
        public TCPConnectionManager TCPConnections = null;
        public UDPConnectionManager UDPConnections = new UDPConnectionManager();

        private RestrictedAccessZone SecurityArea = null;
        private StagingZone StagingArea = null;
        private GamePlayZone GameZone = null;

        public ServerConfig ConfigData = new ServerConfig();

        protected Dictionary<int, ServerPlayer> ConnectedPlayers = new Dictionary<int, ServerPlayer>();
        protected int LastPlayerID = 0;

        public PublicServer PubServer = new PublicServer();

        public class GameState
        {
            public bool IsPublic = false;

            public BZFlag.Data.BZDB.Database BZDatabase = new BZFlag.Data.BZDB.Database();

            public GameWorld World = new GameWorld();
            public FlagManager Flags = new FlagManager();

            // public PlayerManager Players = new PlayerManager();
            // public ShotManager Shots = new ShotManager();
        }
        public GameState State = new GameState();
        // World Contents


        public Server(ServerConfig cfg)
        {
            Networking.Messages.NetworkMessage.IsOnServer = true;

            Logger.SetLogFilePath(cfg.LogFile);
            Logger.LogLevel = cfg.LogLevel;

            Logger.Log0("Server startup");

            ConfigData = cfg;

            SetupAPI();
            SetupConfig();
            SetupBZDB();
            SetupWorld();
            UpdatePublicListServer();

            SecurityArea = new RestrictedAccessZone(ConfigData);
            SecurityArea.PromotePlayer += SecurityArea_PromotePlayer;

            SecurityArea.Flags = State.Flags;
            SecurityArea.World = State.World;

            StagingArea = new StagingZone(ConfigData);
            StagingArea.PromotePlayer += this.StagingArea_PromotePlayer;

            GameZone = new GamePlayZone(this);
            GameZone.UpdatePublicListServer += new EventHandler((s, e) => UpdatePublicListServer());

            RegisterProcessorEvents();
        }

        protected virtual void BZFSProtocolConnectionAccepted(object sender, TCPConnectionManager.PendingClient e)
        {
            var player = AcceptTCPConnection(e);
            if (player == null) // could not make a player for some reason
            {
                e.ClientConnection.Client.Disconnect(false);
                return;
            }

            // send them into the restricted zone until they validate
            SecurityArea.AddPendingConnection(player);
        }

        private void SecurityArea_PromotePlayer(object sender, ServerPlayer e)
        {
            // they passed muster
            StagingArea.AddPendingConnection(e);
        }

        private void StagingArea_PromotePlayer(object sender, ServerPlayer e)
        {
            GameZone.AddPendingConnection(e);
        }

        private void SetupBZDB()
        {
            Logger.Log2("Setup BZDB defaults");

            BZFlag.Game.Host.BZDB.Defaults.Setup(State.BZDatabase);

            State.BZDatabase.FinishLoading();

            BZDBDefaultsLoaded?.Invoke(this, EventArgs.Empty);
        }

        private void SetupAPI()
        {
            Logger.Log2("Load API");

            API.Functions.ServerInstnace = this;

            PluginLoader.LoadFromAssembly(Assembly.GetExecutingAssembly());
            foreach (var f in ConfigData.PlugIns)
            {
                try
                {
                    var a = Assembly.LoadFile(f);
                    if (a != null)
                        PluginLoader.LoadFromAssembly(a);
                }
                catch (System.Exception ex)
                {
                    Logger.Log1("Unable to load plug-in " + f + " :" + ex.ToString());
                }
            }

            APILoadComplete?.Invoke(this, EventArgs.Empty);
            ConfigLoaded?.Invoke(this, EventArgs.Empty);
        }

        // cache some items from the config.
        private void SetupConfig()
        {
            if (ConfigData.GameData.GameType == Data.Game.GameTypes.ClassicCTF || ConfigData.GameData.GameType == Data.Game.GameTypes.TeamFFA)
                ConfigData.GameData.IsTeamGame = true;
        }

        private void SetupWorld()
        {
            WorldPreload?.Invoke(this, EventArgs.Empty);

            if (ConfigData.GameData.MapFile != string.Empty)
            {
                System.IO.FileInfo map = new System.IO.FileInfo(ConfigData.GameData.MapFile);
                System.IO.StreamReader sr = map.OpenText();
                State.World.Map = BZFlag.IO.BZW.Reader.ReadMap(sr);
                sr.Close();

                if (State.World.Map == null)
                    State.World.Map = new Map.WorldMap();
            }

            WorldPostload?.Invoke(this, EventArgs.Empty);
            State.World.Map.Validate();

        }

        private void UpdatePublicListServer()
        {
            if (ConfigData.ListPublicly)
            {
                Logger.Log1("Updating Public List Server");

                PublicPreList?.Invoke(this, EventArgs.Empty);

                PubServer.Address = ConfigData.PublicHost;
                PubServer.Description = ConfigData.PublicTitle;
                PubServer.Name = ConfigData.PublicHost;
                PubServer.Port = ConfigData.Port;
                PubServer.Key = ConfigData.PublicListKey;
                PubServer.AdvertGroups = string.Join(",", ConfigData.PublicAdvertizeGroups.ToArray());
                PubServer.Info = GetGameInfo();

                PubServer.RequestCompleted += PubServer_RequestCompleted;
                PubServer.RequestErrored += PubServer_RequestErrored;
            }
        }


        public GameInfo GetGameInfo()
        {
            GameInfo info = new GameInfo();

            info.GameOptions = (int)ConfigData.GameData.GameOptions;
            info.GameType = (int)ConfigData.GameData.GameType;

            info.MaxShots = ConfigData.GameData.MaxShots;
            info.ShakeWins = ConfigData.GameData.ShakeWins;
            info.ShakeTimeout = (int)ConfigData.GameData.ShakeTimeout;      // 1/10ths of second
            info.MaxPlayerScore = 0;
            info.MaxTeamScore = ConfigData.GameData.MaxShots;
            info.MaxTime = 0;       // seconds
            info.MaxPlayers = ConfigData.GameData.MaxPlayers;

            info.RogueCount = GameZone.GetTeamPlayerCount(TeamColors.RogueTeam);
            info.RogueMax = ConfigData.TeamData.GetTeamLimit(TeamColors.RogueTeam);

            info.RedCount = GameZone.GetTeamPlayerCount(TeamColors.RedTeam);
            info.RedMax = ConfigData.TeamData.GetTeamLimit(TeamColors.RedTeam);

            info.GreenCount = GameZone.GetTeamPlayerCount(TeamColors.GreenTeam);
            info.GreenMax = ConfigData.TeamData.GetTeamLimit(TeamColors.GreenTeam);

            info.BlueCount = GameZone.GetTeamPlayerCount(TeamColors.BlueTeam);
            info.BlueMax = ConfigData.TeamData.GetTeamLimit(TeamColors.BlueTeam);

            info.PurpleCount = GameZone.GetTeamPlayerCount(TeamColors.PurpleTeam);
            info.PurpleMax = ConfigData.TeamData.GetTeamLimit(TeamColors.PurpleTeam);

            info.ObserverCount = GameZone.GetTeamPlayerCount(TeamColors.ObserverTeam);
            info.ObserverMax = ConfigData.TeamData.GetTeamLimit(TeamColors.ObserverTeam); 

            return info;
        }

        private void PubServer_RequestErrored(object sender, EventArgs e)
        {
            State.IsPublic = false;

            Logger.Log1("Public List Failed: " + PubServer.LastError);

            PublicPostList?.Invoke(this, EventArgs.Empty);
        }

        private void PubServer_RequestCompleted(object sender, EventArgs e)
        {
            State.IsPublic = true;

            Logger.Log3("Public List Update Complete");

            PublicPostList?.Invoke(this, EventArgs.Empty);
        }

        protected int FindPlayerID()
        {
            LastPlayerID++;
            if (LastPlayerID > PlayerConstants.MaxUseablePlayerID)
                LastPlayerID = PlayerConstants.MinimumPlayerID;

            lock (ConnectedPlayers)
            {
                if (!ConnectedPlayers.ContainsKey(LastPlayerID))
                    return LastPlayerID;
                else
                {
                    for (int i = PlayerConstants.MinimumPlayerID; i <= PlayerConstants.MaxUseablePlayerID; i++)
                    {
                        if (!ConnectedPlayers.ContainsKey(i))
                        {
                            LastPlayerID = i;
                            return LastPlayerID;
                        }
                    }
                }
            }

            // we are full up
            LastPlayerID = 0;
            return -1;
        }

        protected virtual ServerPlayer AcceptTCPConnection(TCPConnectionManager.PendingClient client)
        {
            ServerPlayer p = NewPlayerRecord(client);
            p.PlayerID = FindPlayerID();
            if (p.PlayerID < 0)
                return null;

            lock (ConnectedPlayers)
                ConnectedPlayers.Add(p.PlayerID, p);

            NewConnection?.Invoke(this, p);

            return p;
        }

        protected virtual ServerPlayer NewPlayerRecord(TCPConnectionManager.PendingClient client) { return new ServerPlayer(client); }

        public void Listen()
        {
            int port = ConfigData.Port;

            Logger.Log1("Listening on port " + port.ToString());

            TCPConnections = new TCPConnectionManager(port, this);
            TCPConnections.BZFSProtocolConnectionAccepted += BZFSProtocolConnectionAccepted;

            SecurityArea.Bans = TCPConnections.Bans;

            SecurityArea.Setup();
            TCPConnections.StartUp();

            UDPConnections = new UDPConnectionManager();
            UDPConnections.Listen(port);

            if (ConfigData.ListPublicly)
            {
                PubServer.UpdateMasterServer();
                Logger.Log1("Listening on port " + port.ToString());
            }
        }

        public void Run()
        {
            Listen();

            while (true)
            {
                // do any monitoring we need done here....

                ServerPlayer[] sp = null;
                lock (ConnectedPlayers)
                    sp = ConnectedPlayers.Values.ToArray();

                // remove any deaded connections
                foreach (var p in sp)
                {
                    if (!p.Active)
                    {
                        lock (ConnectedPlayers)
                            ConnectedPlayers.Remove(p.PlayerID);
                    }
                }


                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
