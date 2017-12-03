using BZFlag.Data.Types;
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
                    TestClients(GetPlayers(), "diver-sion.freemyip.com", 5154);// "bzflag.allejo.io", 5162);// "diver-sion.freemyip.com", 5155);
            }
            else
            {
                if (useSimple)
                    SimpleHoster.Run(args);
            }
        }

        static List<Tuple<string, string, string>>  GetPlayers()
        {
            //if (false)
                return GetFuckedPlayers();

            List<Tuple<string, string, string>> players = new List<Tuple<string, string, string>>();

            players.Add(new Tuple<string, string, string>("Billy D. Bugger", "I live to test", string.Empty));

            players.Reverse();
            return players;
        }

        static List<Tuple<string, string, string>> GetFuckedPlayers()
        {
            List<Tuple<string, string, string>> players = new List<Tuple<string, string, string>>();

            if (false)
            {
                players.Add(new Tuple<string, string, string>("Somebody once", "told me the world is gonna roll me", "2.4.85.2134-MAINT-max64xc721-SDL2"));
                players.Add(new Tuple<string, string, string>("I aint the sharpest", "tool in the shed", "4.1.10.1999-DEV-WIN32"));
                players.Add(new Tuple<string, string, string>("She was looking", "kind of dumb with her finger and her thumb", "2.4.2.2018-EXP-OSX-METAL"));
                players.Add(new Tuple<string, string, string>("In the shape of an", "L on her forehead", "Why.Do.You.Care"));
                players.Add(new Tuple<string, string, string>("Well the years start", "coming and they don't stop coming", "Do.you.get.off.on.this?"));
                players.Add(new Tuple<string, string, string>("Fed to the rules", "and I hit the ground running", "Really"));
                players.Add(new Tuple<string, string, string>("Didnt make sense", "not to live for fun", "I.mean.who.cares.what.version"));
                players.Add(new Tuple<string, string, string>("Your brain gets smart", "but your head gets dumb", "you.cant.tell.people.what"));
                players.Add(new Tuple<string, string, string>("So much to do", "so much to see", "to.run"));
                players.Add(new Tuple<string, string, string>("So what's wrong ", "with taking the back streets?", "just.deal.with.what"));
                players.Add(new Tuple<string, string, string>("Youll never know", "if you don't go", "you.get"));
                players.Add(new Tuple<string, string, string>("Youll never shine", "if you don't glow", "1.1.1.1.993-cheaters-always-prosper"));
                players.Add(new Tuple<string, string, string>("Hey now", "", "2.4.8.2134-MAINT-Linux-ButClosedSource-SDL4"));
                players.Add(new Tuple<string, string, string>("youre an all-star", "", "2.4.8.2013-MAINT-BSD-SDL2"));
                players.Add(new Tuple<string, string, string>("get your game on", "", "2.4.85.2134-MAINT-Computers-SDL2"));
                players.Add(new Tuple<string, string, string>("go play", "", "2.5.0.2019-MAINT-Amazon.Alxea-Native"));
                players.Add(new Tuple<string, string, string>("Hey now youre a rock star", "get the show on, get paid", "2.4.85.2134-DEV-TOTALY_NOT_THE_NSA-SDL2"));
                players.Add(new Tuple<string, string, string>("And all that glitters", " is gold", "2.4.85.2134-ALPHA1-SECRET_BUILD-SDL2"));
                players.Add(new Tuple<string, string, string>("Only shooting stars", "break the mold", "2.4.99.00003-TEST-WIN64-UNITY"));

                players.Reverse();
            }
            else
            {
                players.Add(new Tuple<string, string, string>("Beta2", "Open Source Rules", "2.4.8.2000-MAINT-LINUX-SDL2"));
                players.Add(new Tuple<string, string, string>("Gama3", "GPL fo Life!", "2.4.8.2000-MAINT-BSD-SDL2"));
                players.Add(new Tuple<string, string, string>("Delta4", "If it aint free it aint me", "2.4.8.2000-MAINT-DEBIAN-SDL2"));
                players.Add(new Tuple<string, string, string>("Epsilon5", "I love linux, like for real", "2.4.8.2000-MAINT-REDHAT-SDL2"));
                players.Add(new Tuple<string, string, string>("Zeta6", "I am linus", "2.4.8.2000-MAINT-ANDROID-SDL2"));
                players.Add(new Tuple<string, string, string>("Eta7", "Stalman in tha HOUSE", "2.4.8.2000-MAINT-ANDROID-SDL2"));
                players.Add(new Tuple<string, string, string>("Theta8", "let Freedom ring", "2.4.8.2000-MAINT-WINDOWS-SDL2"));
                players.Add(new Tuple<string, string, string>("Mu9", "Make America Suck Again", "2.4.8.2000-MAINT-IOS-SDL2"));

                players.Add(new Tuple<string, string, string>("Blastman", "", "2.4.8.2230-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("Akezaleos", "", "2.4.8.2100-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("ZZZ_Zetabyte", "", "2.4.8.2053-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("porcine", "", "2.4.8.2111-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("wehoo", "", "2.4.8.1000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("Dull", "", "2.4.8.0000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("NULL", "", "2.4.8.1000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("Devlin", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("BobbyJimJam", "", "2.4.8.5000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("CiCee", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("Macross", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("LinuxLife", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("ElQuapo", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("Pudge", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("DJ Sanford", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("Lucid", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("Viper_slow", "", "2.4.8.2000-MAINT-WebGL-SDL2"));

                players.Add(new Tuple<string, string, string>("FishTacos", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("SmashMouth", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("AllStart", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("SmokeySinger", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("SupraMan", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("Buttman", "The Dork Knight", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("The Flush", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("Wander Woman", " ", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("The Lunar Man Hunter", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("Captain Quark", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("The Purple Arrow", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("The Stoppable Bulk", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("Lieutenant Amsterdam", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
                players.Add(new Tuple<string, string, string>("Copper Man", "", "2.4.8.2000-MAINT-WebGL-SDL2"));
            }



            bool fill = false;

         //   fill = true;

            if (fill)
            {
                Random rng = new Random();

                while (players.Count < 99)
                {
                    players.Add(new Tuple<string, string, string>("Not_Alljo" + rng.Next().ToString(), "BZAdmin." + rng.Next().ToString() + rng.Next().ToString(), "idiot." + rng.Next().ToString() + "." + rng.Next().ToString()));
                }
            }

           
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

            //      GetList("A_REGULAR_HUMAN_PLAYER", string.Empty);
            var server = new GameList.ListServerData(); // Link.FindServerWithMostPlayers();

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
