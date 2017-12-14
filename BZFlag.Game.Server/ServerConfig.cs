using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text;

using BZFlag.Data.Game;
using BZFlag.Data.Teams;

namespace BZFlag.Game.Host
{
    public class ServerConfig
    {
        public int Port = 5154;

        // plugins
        public List<string> PlugIns = new List<string>();

        public string LogFile = string.Empty;
        public int LogLevel = 4;

        // external databases
        public string BanListFile = string.Empty;


        // public data
        public bool ListPublicly = false;
        public string PublicHost = string.Empty;
        public string PublicListKey = string.Empty;
        public List<string> PublicAdvertizeGroups = new List<string>();
        public string PublicTitle = string.Empty;

        // authentication data
        public bool AllowAnonUsers = true;
        public bool ProtectRegisteredNames = false;

        public List<string> SecurityGroups = new List<string>();

        // gameplay data
        public class GameInfo
        {
            // world data
            public string MapFile = string.Empty;
            public string MapURL = string.Empty;

            // player data

            public GameTypes GameType = GameTypes.Unknown;
            public GameOptionFlags GameOptions = GameOptionFlags.NoStyle;

            public bool IsTeamGame = false;

            public int MaxPlayers = 200;
            public int MaxShots = 1;
            public int MaxFlags = -1;
            public float LinearAcceleration = 0.1f;
            public float AngularAcceleration = 0.1f;

            public int ShakeWins = 0;
            public float ShakeTimeout = 0;
        }

        public GameInfo GameData = new GameInfo();

        public class TeamInfo
        {
            public bool ForceAutomaticTeams = false;
            public class TeamLimits
            {
                public TeamColors Team = TeamColors.NoTeam;
                public int Maxium = 100; 
            }
            public List<TeamLimits> Limits = new List<TeamLimits>();

            public int GetTeamLimit(TeamColors team)
            {
                foreach (var t in Limits)
                {
                    if (t.Team == team)
                        return t.Maxium;
                }
                return 200;
            }
        }

        public TeamInfo TeamData = new TeamInfo();


        public static ServerConfig Read(string filepath)
        {
            FileInfo f = new FileInfo(filepath);
            if (!f.Exists)
                return new ServerConfig();

            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(ServerConfig));
                var fs = f.OpenRead();
                ServerConfig cfg = xml.Deserialize(fs) as ServerConfig;
                fs.Close();

                return cfg == null ? new ServerConfig() : cfg;
            }
            catch (Exception /*ex*/)
            {
                return new ServerConfig();
            }
        }

        public static bool Write(ServerConfig config, string filepath)
        {
            FileInfo f = new FileInfo(filepath);
            if (f.Exists)
                f.Delete();

            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(ServerConfig));
                var fs = f.OpenWrite();
                xml.Serialize(fs, config);
                fs.Close();

                return true;
            }
            catch (Exception /*ex*/)
            {
                return false;
            }
        }
    }
}
