using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;

using BZFlag.Data.Game;
using BZFlag.Data.Teams;


namespace BZFlag.Game.Host
{
    public class ServerConfig
    {
        public int Port = 5154;

        // plugins
        public List<string> PlugIns { get; set; } = new List<string>();

        public string LogFile { get; set; } = string.Empty;
        public int LogLevel { get; set; } = 4;

        // external databases
        public string BanListFile { get; set; } = string.Empty;

        // public data
        public bool ListPublicly { get; set; } = false;
        public string PublicHost { get; set; } = string.Empty;
        public string PublicListKey { get; set; } = string.Empty;
        public List<string> PublicAdvertizeGroups { get; set; } = new List<string>();
        public string PublicTitle { get; set; } = string.Empty;

        // authentication data
        public bool AllowAnonUsers { get; set; } = true;
        public bool ProtectRegisteredNames { get; set; } = false;

        public List<string> SecurityGroups { get; set; } = new List<string>();

        // gameplay data
        public class GameInfo
        {
            // world data
            public string MapFile { get; set; } = string.Empty;
            public string MapURL { get; set; } = string.Empty;

            // player data

            public GameTypes GameType { get; set; } = GameTypes.Unknown;
            public GameOptionFlags GameOptions { get; set; } = GameOptionFlags.NoStyle;

            public bool IsTeamGame = false;

            public int MaxPlayers { get; set; } = 200;
            public int MaxShots { get; set; } = 1;
            public float LinearAcceleration { get; set; } = 0.1f;
            public float AngularAcceleration { get; set; } = 0.1f;

            public int ShakeWins { get; set; } = 0;
            public float ShakeTimeout { get; set; } = 0;
        }

        public GameInfo GameData { get; set; } = new GameInfo();

        public class TeamInfo
        {
            public bool ForceAutomaticTeams { get; set; } = false;
            public class TeamLimits
            {
                public TeamColors Team { get; set; } = TeamColors.NoTeam;
                public int Maxium { get; set; } = 100; 
            }
            public List<TeamLimits> Limits { get; set; } = new List<TeamLimits>();

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

        public TeamInfo TeamData { get; set; } = new TeamInfo();

        public class ExtraConfigInfo
        {
            public string Name = string.Empty;
            public string Value = string.Empty;
        }

        public List<ExtraConfigInfo> CustomConfigItems = new List<ExtraConfigInfo>();

        public static ServerConfig ReadXML(string filepath)
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

        public static bool WriteXML(ServerConfig config, string filepath)
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



        public static ServerConfig ReadYAML(string filepath)
        {
            FileInfo f = new FileInfo(filepath);
            if (!f.Exists)
                return new ServerConfig();

            try
            {
                YamlDotNet.Serialization.Deserializer yaml = new YamlDotNet.Serialization.Deserializer();

                var fs = f.OpenText();
                ServerConfig cfg = yaml.Deserialize<ServerConfig>(fs);
                fs.Close();

                return cfg == null ? new ServerConfig() : cfg;
            }
            catch (Exception /*ex*/)
            {
                return new ServerConfig();
            }
        }

        public static bool WriteYAML(ServerConfig config, string filepath)
        {
            FileInfo f = new FileInfo(filepath);
            if (f.Exists)
                f.Delete();

            try
            {
                YamlDotNet.Serialization.Serializer yaml = new YamlDotNet.Serialization.Serializer();
             
                var fs = f.AppendText();
                yaml.Serialize(fs, config, typeof(ServerConfig));
                fs.Close();

                return true;
            }
            catch (Exception /*ex*/)
            {
                return false;
            }
        }

        public static ServerConfig ReadJSON(string filepath)
        {
            FileInfo f = new FileInfo(filepath);
            if (!f.Exists)
                return new ServerConfig();

            try
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(ServerConfig));

                var fs = f.OpenRead();
                ServerConfig cfg = json.ReadObject(fs) as ServerConfig;
                fs.Close();

                return cfg == null ? new ServerConfig() : cfg;
            }
            catch (Exception /*ex*/)
            {
                return new ServerConfig();
            }
        }

        public static bool WriteJSON(ServerConfig config, string filepath)
        {
            FileInfo f = new FileInfo(filepath);
            if (f.Exists)
                f.Delete();

            try
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(ServerConfig));

                var fs = f.OpenWrite();
                json.WriteObject(fs, config);
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
