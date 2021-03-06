﻿using System;
using System.Text;
using System.Threading;
using System.IO;

using BZFlag.Game;
using BZFlag.Services;
using BZFlag.Data.Teams;
using BZFlag.Game.Players;
using BZFlag.Networking.Messages.BZFS.Player;
using BZFlag.Data.Types;
using BZFlag.LinearMath;

namespace ConnectionTester
{
    public class ClientTester
    {
        ClientParams StartupParams = new ClientParams();

        Client GameClient = null;

        GameList Link = new GameList();
        bool GotList = false;

        protected Vector3F Position = Vector3F.Zero;


        public string Callsign = "Billy D. Bugger";
        public string Motto = "I live to test";
        public string Version = string.Empty;
        public string Host = string.Empty;
        public int Port = 5154;

        public string LogFileName = "log.txt";

        public bool UseThread = false;
        protected Thread Worker = null;

        public void Kill()
        {
            if (Worker != null)
                Worker.Abort();

            Worker = null;
        }

        public StreamWriter LogStream = null; 
        public void WriteLine(string text)
        {
            Console.WriteLine(text);
            LogStream.WriteLine(text);
            LogStream.Flush();
        }

        public void WriteEmptyLine()
        {
            Console.WriteLine();
            LogStream.WriteLine();
            LogStream.Flush();
        }

        public void Write(string text)
        {
            Console.Write(text);
            LogStream.Write(text);
        }

        public ClientTester()
        {
            Link.RequestCompleted += Link_RequestCompleted;
            Link.RequestErrored += Link_RequestErrored;
        }

        public virtual void Startup()
        {
            LogStream = new StreamWriter(LogFileName);

            Random rng = new Random();
            switch(rng.Next(4))
            {
                case 0:
                    StartupParams.DesiredTeam = TeamColors.RedTeam;
                    break;

                case 1:
                    StartupParams.DesiredTeam = TeamColors.GreenTeam;
                    break;

                case 2:
                    StartupParams.DesiredTeam = TeamColors.BlueTeam;
                    break;

                case 3:
                    StartupParams.DesiredTeam = TeamColors.PurpleTeam;
                    break;
            }

            StartupParams.Callsign = Callsign;
            StartupParams.Motto = Motto;
            StartupParams.VersionOveride = Version;

            StartupParams.Host = Host;
            StartupParams.Port = Port;

    
            Position = new Vector3F(((float)rng.NextDouble() * 200.0f) - 100.0f, ((float)rng.NextDouble() * 200.0f) - 100.0f, 0);
        }

        private void GetList(string pass)
        {
            Link.GetList(StartupParams.Callsign, pass);

            while (GotList == false)
                Thread.Sleep(10);
        }

        private void Link_RequestErrored(object sender, EventArgs e)
        {
            WriteLine("List Request Error " + Link.LastError);
            GotList = true;
        }

        private void Link_RequestCompleted(object sender, EventArgs e)
        {
            WriteLine("List Request Completed " + Link.LastToken);
            WriteLine("List Server Count " + Link.ServerList.Count.ToString());
            StartupParams.Token = Link.LastToken;
            GotList = true;
        }

        public void Run()
        {
            Startup();

            GameClient = new Client(StartupParams);
            GameClient.HostIsNotBZFlag += GameClient_HostIsNotBZFlag;

            GameClient.WorldDownloadProgress += GameClient_WorldDownloadProgress;

            GameClient.TCPConnected += GameClient_TCPConnected;
            GameClient.ClientAccepted += GameClient_ClientAccepted;
            GameClient.ClientRejected += GameClient_ClientRejected;
            GameClient.UDPLinkEstablished += GameClient_UDPLinkEstablished;

            GameClient.PlayerList.SelfAdded += GameClient_SelfAdded;
            GameClient.PlayerList.PlayerAdded += GameClient_PlayerAdded;
     
            GameClient.PlayerList.PlayerRemoved += GameClient_PlayerRemoved;
            GameClient.PlayerList.PlayerStateUpdated += GameClient_PlayerStateUpdated;
            GameClient.PlayerList.PlayerInfoUpdated += GameClient_PlayerInfoUpdated;
            GameClient.PlayerList.PlayerSpawned += GameClient_PlayerSpawned;
            GameClient.PlayerList.SelfSpawned += this.GameClient_SelfSpawned;
            GameClient.PlayerList.PlayerKilled += GameClient_PlayerKilled;

            GameClient.ReceivedUnknownMessage += GameClient_ReceivedUnknownMessage;

            GameClient.Chat.ChatMessageReceived += Chat_ChatMessageReceived;

            GameClient.FlagCreated += GameClient_FlagCreated;
            GameClient.FlagUpdated += GameClient_FlagUpdated;
            GameClient.FlagGrabbed += GameClient_FlagGrabbed;
            GameClient.FlagDropped += GameClient_FlagDropped;
            GameClient.FlagTransfered += GameClient_FlagTransfered;
            GameClient.FlagIsNear += GameClient_FlagIsNear;

            GameClient.ShotMan.ShotCreated += ShotMan_ShotCreated;
            GameClient.ShotMan.ShotRemoved += ShotMan_ShotRemoved;
            GameClient.ShotMan.ShotUpdated += ShotMan_ShotUpdated;

            if (UseThread)
            {
                Worker = new Thread(new ThreadStart(ProcessUpdate));
                Worker.Start();
            }
            else
                ProcessUpdate();
        }

        public bool SendUpdates = false;
        protected int UpdateOrder = 1;
        protected float UpdateTimeStamp = 1;

        private void GameClient_SelfSpawned(object sender, Player e)
        {
            SendUpdates = true;
        }

        protected void ProcessUpdate()
        {
            while (true)
            {
                Update();
                Thread.Sleep(50);

                if (GameClient.NetClient.ConnectionError != string.Empty)
                {
                    WriteLine("Connection Failure " + GameClient.NetClient.ConnectionError);
                    return;
                }
            }
        }

        public void Update()
        {
            if (SendUpdates)
            {
                MsgPlayerUpdate upd = new MsgPlayerUpdate();
                upd.PlayerID = GameClient.PlayerList.LocalPlayerID;
                upd.Position = Position;
                upd.Azimuth = 32;
                upd.Status = BZFlag.Data.Players.PlayerStatuses.Alive;
                upd.Order = UpdateOrder;
                UpdateOrder++;
                upd.TimeStamp = UpdateTimeStamp;
                UpdateTimeStamp += 0.01f;

                GameClient.SendMessage(upd);
            }
            GameClient.Update();
        }

        private void GameClient_UDPLinkEstablished(object sender, EventArgs e)
        {
            WriteLine("UDP Link Established");
        }

        private void GameClient_ReceivedUnknownMessage(object sender, Client.UnknownMessageEventArgs e)
        {
            WriteLine("Unknown message " + e.CodeAbriv + " (" + e.CodeID.ToString("X2") + ")");
        }

        private void Chat_ChatMessageReceived(object sender, BZFlag.Game.Chat.ChatSystem.ChatMessageEventArgs e)
        {
            WriteLine("Chat Recieved " + e.Message.From.ToString() + " " + e.Message.Message);
        }

        private void GameClient_WorldDownloadProgress(object sender, Client.WorldDownloadProgressEventArgs e)
        {
            WriteLine("World Download Progress " + (e.Paramater * 100.0f).ToString() + "%");
        }

        private void GameClient_TCPConnected(object sender, EventArgs e)
        {
            WriteLine("TCP Connected to " + StartupParams.Host + ":" + StartupParams.Port.ToString());
        }

        private void GameClient_ClientRejected(object sender, Client.ClientRejectionEventArgs e)
        {
            WriteLine("Client Rejected " + e.Code.ToString() + " " + e.Reason);
        }

        private void GameClient_ClientAccepted(object sender, EventArgs e)
        {
            WriteLine("Client Accepted " + GameClient.PlayerList.LocalPlayerID.ToString());

            BZFlag.Networking.Messages.BZFS.Player.MsgAlive spawn = new BZFlag.Networking.Messages.BZFS.Player.MsgAlive();
            spawn.IsSpawn = true;

            GameClient.SendMessage(spawn);
        }

        private void GameClient_PlayerRemoved(object sender, Player player)
        {
            WriteLine("Player Removed " + player.Callsign);
        }

        private void GameClient_PlayerAdded(object sender, Player player)
        {
            WriteLine("Player Added " + player.Callsign);
        }

        private void GameClient_PlayerKilled(object sender, PlayerManager.KilledEventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Player ");
            builder.Append(e.Victim.Callsign);
            builder.Append(" was killed (");
            builder.Append(e.Reason.ToString());
            builder.Append(") with ");
            if (e.KilledByFlag != null)
                builder.Append(e.KilledByFlag.FlagName);
            else
                builder.Append("Divine Judgment");

            builder.Append(" by ");
            if (e.Killer != null)
                builder.Append(e.Killer.Callsign);
            else
                builder.Append("God herself");

            WriteLine(builder.ToString());
        }

        private void GameClient_PlayerSpawned(object sender, Player player)
        {
            WriteLine("Player spawned " + player.Callsign);
        }

        private void GameClient_PlayerInfoUpdated(object sender, Player player)
        {
            WriteLine("Player Info Updated " + player.Callsign);
        }

        private void GameClient_PlayerStateUpdated(object sender, Player player)
        {
            WriteLine("Player Status Updated " + player.Callsign + " Last Pos " + string.Format("X{0} Y{1} Z{2}", player.LastUpdate.Position.X, player.LastUpdate.Position.Y, player.LastUpdate.Position.Z));
        }

        private void GameClient_SelfAdded(object sender, Player player)
        {
            WriteLine("Self Added " + player.Callsign);
        }

        private void GameClient_FlagIsNear(object sender, Client.NearFlagEventArgs e)
        {
            WriteLine("Flag " + e.Name.ToString() + " Is Near ");
        }

        private void GameClient_FlagTransfered(object sender, BZFlag.Game.Flags.FlagInstance e)
        {
            WriteLine("Flag Instance " + e.ID.ToString() + " Transfered to " + e.Owner.Callsign);
        }

        private void GameClient_FlagDropped(object sender, BZFlag.Game.Flags.FlagInstance e)
        {
            WriteLine("Flag Instance " + e.ID.ToString() + " dropped");
        }

        private void GameClient_FlagGrabbed(object sender, BZFlag.Game.Flags.FlagInstance e)
        {
            WriteLine("Flag Instance " + e.ID.ToString() + " Grabbed by " + e.Owner.Callsign);
        }

        private void GameClient_FlagUpdated(object sender, BZFlag.Game.Flags.FlagInstance e)
        {
            WriteLine("Flag Instance " + e.ID.ToString() + " Updated");
        }

        private void GameClient_FlagCreated(object sender, BZFlag.Game.Flags.FlagInstance e)
        {
            WriteLine("Flag Instance " + e.ID.ToString() + " Created");
        }

        private void ShotMan_ShotUpdated(object sender, BZFlag.Game.Shots.Shot e)
        {
            WriteLine("ShotID " + e.GlobalID.ToString() + " Updated");
        }

        private void ShotMan_ShotRemoved(object sender, BZFlag.Game.Shots.Shot e)
        {
            WriteLine("ShotID " + e.GlobalID.ToString() + " Removed");
        }

        private void ShotMan_ShotCreated(object sender, BZFlag.Game.Shots.Shot e)
        {
            WriteLine("ShotID " + e.GlobalID.ToString() + " Created");
        }

        private void GameClient_HostIsNotBZFlag(object sender, EventArgs e)
        {
            WriteLine("Host is not BZFS");
        }
    }
}
