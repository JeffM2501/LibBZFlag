using BZFlag.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;


namespace ConnectionTester
{
    class Program
    {
        public static bool testClient = true;

        public static bool useSimple = false;
        public static bool useThreads = true;

        static bool GotList = false;

        private static GameList Link = new GameList();

        static void Main(string[] args)
        {
            Link.RequestCompleted += Link_RequestCompleted;
            Link.RequestErrored += Link_RequestErrored;

            if (testClient)
            {
                if (useSimple)
                    SimpleLogger.Run(args);
                else
                    TestClients(GetPlayers(), string.Empty, -1);
            }
            else
            {
                if (useSimple)
                    SimpleHoster.Run(args);
            }
        }

        static List<Tuple<string, string, string>>  GetPlayers()
        {
            List<Tuple<string, string, string>> players = new List<Tuple<string, string, string>>();

            players.Add(new Tuple<string, string, string>("Billy D. Bugger", "I live to test", string.Empty));

            players.Reverse();
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

        public static void TestClients( List<Tuple<string,string,string>> players, string host, int port )
        {
            List<ClientTester> clients = new List<ClientTester>();

            GetList("A_REGULAR_HUMAN_PLAYER", string.Empty);
            var server = Link.FindServerWithMostPlayers();

            if (host != string.Empty)
            {
                server.Host = host;
                server.Port = port;
            }

            for (int i =0; i < players.Count; i++)
            {
                ClientTester t = BuildClient(i + 1, server.Host, server.Port, players[i].Item1, players[i].Item2, players[i].Item3);
                clients.Add(t);
                if (useThreads)
                    t.Run();
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
