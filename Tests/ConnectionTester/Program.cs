using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using BZFlag.Game.Host;
using BZFlag.Services;

namespace ConnectionTester
{
    class Program
    {
        public static bool testClient = true;

        public static bool useSimple = false;
        public static bool useThreads = true;

        static bool GotList = false;

        private static GameList Link = new GameList();

        public static bool SaveConfig = false;

        static void Main(string[] args)
        {
            if (SaveConfig)
                DoConfigSave();

            Link.RequestCompleted += Link_RequestCompleted;
            Link.RequestErrored += Link_RequestErrored;

            if (testClient)
            {
                if (useSimple)
                    SimpleLogger.Run(args);
                else
                    TestClients(GetPlayers(), "localhost", 5154);
            }
            else
            {
                if (useSimple)
                    SimpleHoster.Run(args);
            }
        }

        static void DoConfigSave()
        {
            ServerConfig cfg = new ServerConfig();

            var limits = new ServerConfig.TeamInfo.TeamLimits();
            limits.Team = BZFlag.Data.Teams.TeamColors.RedTeam;
            limits.Maxium = 100;
            cfg.TeamData.Limits.Add(limits);

            limits = new ServerConfig.TeamInfo.TeamLimits();
            limits.Team = BZFlag.Data.Teams.TeamColors.BlueTeam;
            limits.Maxium = 100;
            cfg.TeamData.Limits.Add(limits);

            cfg.GameData.GameType = BZFlag.Data.Game.GameTypes.OpenFFA;
            cfg.LogFile = "./logs/";
            cfg.BanListFile = "./bans/localbans.txt";
            cfg.PlugIns.Add("sample.dll");
            cfg.PlugIns.Add("random_map.dll");

            cfg.ProtectRegisteredNames = true;
            cfg.TeamData.ForceAutomaticTeams = true;

            ServerConfig.WriteYAML(cfg, "config.yaml");
        }

        static List<Tuple<string, string, string>>  GetPlayers()
        {
            List<Tuple<string, string, string>> players = new List<Tuple<string, string, string>>();

            players.Add(new Tuple<string, string, string>("Billy D. Bugger", "I live to test", string.Empty));

            return players;
        }

       
        private static void GetList(string user, string pass)
        {
            Link.GetList(user, pass);

            while (GotList == false)
                Thread.Sleep(10);
        }

        private static void Link_RequestErrored(object sender, EventArgs e)
        {
            GotList = true;
        }

        private static void Link_RequestCompleted(object sender, EventArgs e)
        {
            GotList = true;
        }

        public static bool delayJoin = true;

        public static void TestClients( List<Tuple<string,string,string>> players, string host, int port )
        {
            List<ClientTester> clients = new List<ClientTester>();

           
            GameList.ListServerData server = new GameList.ListServerData();

            if (host != string.Empty)
            {
                server.Host = host;
                server.Port = port;
            }
            else
            {
                GetList("A_REGULAR_HUMAN_PLAYER", string.Empty);
                server = Link.FindServerWithMostPlayers();
            }
                

            for (int i =0; i < players.Count; i++)
            {
                ClientTester t = BuildClient(i + 1, server.Host, server.Port, players[i].Item1, players[i].Item2, players[i].Item3);
                clients.Add(t);
                if (useThreads)
                    t.Run();

                if (delayJoin)
                    Thread.Sleep(2000);
            }

            if (!useThreads)
            {
                while (true)
                {
                    foreach (var t in clients)
                        t.Update();

                    Thread.Sleep(10);
                }
            }
            else
            {
                while (true)
                    Thread.Sleep(100);
            }
        }

        private static ClientTester BuildClient(int ID, string host, int port, string name, string motto, string version)
        {
            ClientTester t = new ClientTester();

            t.Callsign = name;
            t.Motto = motto;
            t.Version = version;

            t.Host = host;
            t.Port = port;

            FileInfo f = new FileInfo("Log_" + ID.ToString() + ".txt");
            if (f.Exists)
                f.Delete();

            t.LogFileName = f.FullName;
            t.UseThread = useThreads;

            return t;

        }
    }
}
