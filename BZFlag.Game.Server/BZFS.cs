
using System;
using System.IO;
using System.Reflection;

using BZFlag.Data.Game;
using BZFlag.Data.Teams;

namespace BZFlag.Game.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.ShowDateTime = true;

            ServerConfig cfg = new ServerConfig();
            if (args.Length > 0)
            {
                if (args.Length > 1)
                {
                    if (args[0].ToLowerInvariant() == "-save_conf")
                    {
                        Logger.Log0("Saving config template to " + args[1]);

                        cfg = BuildSaveableConfig();
                        string ext = Path.GetExtension(args[1]).ToUpper();
                        if (ext == ".XML")
                            ServerConfig.WriteXML(cfg, args[1]);
                        else if (ext == ".JSON")
                            ServerConfig.WriteJSON(cfg, args[1]);
                        else if (ext == ".YAML")
                            ServerConfig.WriteYAML(cfg, args[1]);
                    }
                    else
                        Useage();

                    return;
                }
                else
                {
                    string ext = Path.GetExtension(args[0]).ToUpper();
                    if (ext == ".XML")
                        cfg = ServerConfig.ReadXML(args[0]);
                    else if (ext == ".JSON")
                        cfg = ServerConfig.ReadJSON(args[0]);
                    else if (ext == ".YAML")
                        cfg = ServerConfig.ReadYAML(args[0]);

                    Logger.Log1("Loading config from " + args[0]);
                }
            }
            else
            {
                Logger.Log0("No config, using default");
            }

            new Server(cfg).Run();
        }

        private static void Useage()
        {
            Console.WriteLine(Resources.UseageText);
            Environment.Exit(0);
        }

        private static ServerConfig BuildSaveableConfig()
        {
            ServerConfig config = new ServerConfig();

            config.PlugIns.Add("A_PLUGIN_PATH");
            config.PlugIns.Add("ANOTHER_PLUGIN_PATH");

            config.LogFile = "PATH_TO_LOG_FILE";
            config.LogLevel = 2;

            config.Security.BanDBBackend = "YAML";
            config.Security.BanDBFile = "PATH_TO_BANS_FILE";
            config.Security.BansReadOnly = false;

            ServerConfig.SecurityInfo.GroupPermissions group = new ServerConfig.SecurityInfo.GroupPermissions();
            group.Group = "SECURITY_GROUP_A";
            group.Permisions.Add("PERMISSION_A");
            group.Permisions.Add("PERMISSION_B");
            config.Security.Groups.Add(group);

            group = new ServerConfig.SecurityInfo.GroupPermissions();
            group.Group = "SECURITY_GROUP_B";
            group.Permisions.Add("PERMISSION_A");
            group.Permisions.Add("PERMISSION_C");
            config.Security.Groups.Add(group);

            config.ListPublicly = true;
            config.PublicHost = "PUBLIC_HOST_ADDRESS";
            config.PublicListKey = "PUBLIC_LIST_KEY";
            config.PublicAdvertizeGroups.Add("ADVERTISE_GROUP_A");
            config.PublicAdvertizeGroups.Add("ADVERTISE_GROUP_B");
            config.PublicTitle = "PUBLIC_TITLE";

            config.AllowAnonUsers = false;
            config.ProtectRegisteredNames = true;

            config.GameData.MapFile = "PATH_TO_MAP_FILE.BZW";
            config.GameData.MapURL = "HTTPS:\\\\URL.TO.MAP\\FILE.BZW";

            config.GameData.GameType = GameTypes.OpenFFA;
            config.GameData.GameOptions = GameOptionFlags.SuperFlagGameStyle | GameOptionFlags.InertiaGameStyle | GameOptionFlags.JumpingGameStyle | GameOptionFlags.RicochetGameStyle | GameOptionFlags.ShakableGameStyle
                                            | GameOptionFlags.AntidoteGameStyle | GameOptionFlags.HandicapGameStyle | GameOptionFlags.NoTeamKillsGameStyle;
            config.GameData.MaxPlayers = 100;
            config.GameData.MaxShots = 2;
            config.GameData.LinearAcceleration = 200f;
            config.GameData.AngularAcceleration = 200f;
            config.GameData.ShakeTimeout = 1;
            config.GameData.ShakeWins = 1;

            config.TeamData.ForceAutomaticTeams = false;

            ServerConfig.TeamInfo.TeamLimits limits = new ServerConfig.TeamInfo.TeamLimits();
            limits.Maxium = 60;
            limits.Team = TeamColors.ObserverTeam;
            config.TeamData.Limits.Add(limits);

            limits = new ServerConfig.TeamInfo.TeamLimits();
            limits.Maxium = 10;
            limits.Team = TeamColors.RedTeam;
            config.TeamData.Limits.Add(limits);

            limits = new ServerConfig.TeamInfo.TeamLimits();
            limits.Maxium = 10;
            limits.Team = TeamColors.BlueTeam;
            config.TeamData.Limits.Add(limits);

            limits = new ServerConfig.TeamInfo.TeamLimits();
            limits.Maxium = 10;
            limits.Team = TeamColors.GreenTeam;
            config.TeamData.Limits.Add(limits);

            limits = new ServerConfig.TeamInfo.TeamLimits();
            limits.Maxium = 10;
            limits.Team = TeamColors.PurpleTeam;
            config.TeamData.Limits.Add(limits);


            config.Flags.SpawnRandomFlags = true;
            config.Flags.RandomFlags.UseGoodFlags = true;
            config.Flags.RandomFlags.UseBadFlags = true;

            config.Flags.RandomFlags.MaxFlagCount = 100;
            config.Flags.RandomFlags.MinFlagCount = 50;

            config.Flags.RandomFlags.UseFlags.Add("ADDITIONAL_FLAG_TO_ALWAYS_USE");
            config.Flags.RandomFlags.UseFlags.Add("ANOTHER_ADDITIONAL_FLAG_TO_ALWAYS_USE");

            config.Flags.RandomFlags.IgnoreFlags.Add("A_FLAG_TO_NOT_USE");
            config.Flags.RandomFlags.IgnoreFlags.Add("ANOTHER_FLAG_TO_NOT_USE");

            ServerConfig.ExtraConfigInfo extra = new ServerConfig.ExtraConfigInfo();
            extra.Name = "CUSTOM_CONFIG_ITEM_NAME_1";
            extra.Value = "CUSTOM_CONFIG_ITEM_DATA_1";
            config.CustomConfigItems.Add(extra);

            extra = new ServerConfig.ExtraConfigInfo();
            extra.Name = "CUSTOM_CONFIG_ITEM_NAME_2";
            extra.Value = "CUSTOM_CONFIG_ITEM_DATA_2";
            config.CustomConfigItems.Add(extra);

            return config;
        }
    }
}
