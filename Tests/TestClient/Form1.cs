using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BZFlag.Services;
using BZFlag.Networking.Messages.BZFS;

namespace TestClient
{
    public class Config
    {
        public string Callsign = string.Empty;
        public string Motto = string.Empty;
        public string Password = string.Empty;

        public void Save()
        {
            FileInfo file = new FileInfo(Path.Combine(Application.UserAppDataPath, "settings.xml"));
            if (file.Exists)
                file.Delete();

            XmlSerializer xml = new XmlSerializer(this.GetType());
            FileStream fs = file.OpenWrite();
            xml.Serialize(fs, this);
            fs.Close();
        }

        public static Config Load()
        {
            FileInfo file = new FileInfo(Path.Combine(Application.UserAppDataPath, "settings.xml"));
            if (!file.Exists)
                return new Config();

            XmlSerializer xml = new XmlSerializer(typeof(Config));
            FileStream fs = file.OpenRead();
            Config cfg = xml.Deserialize(fs) as Config;
            fs.Close();

            if (cfg == null)
                return new Config();

            return cfg;
        }

        public string GetPassword()
        {
            if (Password == string.Empty)
                return Password;

            return Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(Password), null, DataProtectionScope.CurrentUser));
        }

        public void SetPassword(string pass)
        {
            if (pass == string.Empty)
            {
                Password = pass;
                return;
            }

            Password = Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(pass), null, DataProtectionScope.CurrentUser));
        }
    }

    public partial class Form1 : Form
    {
        GameList ListServerLink = new GameList();

        BZFlag.Game.Client GameClient = null;

        protected Config UserConfig = new Config();

        public Form1()
        {
            InitializeComponent();
            ListServerLink.RequestCompleted += ListServerLink_RequestCompleted;

            UserConfig = Config.Load();

            Callsign.Text = UserConfig.Callsign;
            Motto.Text = UserConfig.Motto;
            Password.Text = UserConfig.GetPassword();

            SendChatLine_TextChanged(this, EventArgs.Empty);
            CheckLogin();
        }

        protected void CheckLogin()
        {
            AuthButton.Enabled = Callsign.Text != string.Empty;
        }

        private void ListServerLink_RequestCompleted(object sender, EventArgs e)
        {
            ServerList.Items.Clear();

            if (OnlyFilledServers.Checked)
            {
                List<object> things = new List<object>();
                foreach (var s in ListServerLink.ServerList)
                {
                    if (s.Info.TotalPlayers > 0)
                        things.Add(s);
                }

                ServerList.Items.AddRange(things.ToArray());
            }

            if (ServerList.Items.Count == 0)
                ServerList.Items.AddRange(ListServerLink.ServerList.ToArray());

            if (ServerList.Items.Count > 0)
                ServerList.SelectedIndex = 0;
        }

        private void AuthButton_Click(object sender, EventArgs e)
        {
            ListServerLink.GetList(Callsign.Text, Password.Text);

            UserConfig.Callsign = Callsign.Text;
            UserConfig.Motto = Motto.Text;
            UserConfig.SetPassword(Password.Text);
            UserConfig.Save();
        }


        private void ServerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ServerInfo.Text = string.Empty;
            GameList.ListServerData server = ServerList.SelectedItem as GameList.ListServerData;
            if (server == null)
                return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(server.Name);
            sb.AppendLine("Address : " + server.Address);
            sb.AppendLine("Players : " + server.Info.TotalPlayers.ToString());
            sb.AppendLine(server.Description);

            ServerInfo.Text = sb.ToString();
        }

        private void Join_Click(object sender, EventArgs e)
        {
            GameList.ListServerData server = ServerList.SelectedItem as GameList.ListServerData;
            if (server == null)
                return;

            BZFlag.Game.ClientParams p = new BZFlag.Game.ClientParams();
            p.Callsign = ListServerLink.LastCallsign;
            p.Token = ListServerLink.LastToken;

            p.Host = server.Host;
            p.Port = server.Port;

            p.Motto = Motto.Text;
            p.DesiredTeam = BZFlag.Data.Teams.TeamColors.ObserverTeam;
            p.CacheFolder = string.Empty;

            this.AuthPannel.Enabled = false;

            PlayersList.Items.Clear();

            GameClient = new BZFlag.Game.Client(p);

            GameClient.Chat.ChatMessageReceived += Chat_ChatMessageReceived;
            GameClient.PlayerList.PlayerInfoUpdated += PlayerList_PlayerInfoUpdated;
            GameClient.PlayerList.PlayerStateUpdated += PlayerList_PlayerStateUpdated;

            GameClient.ClientAccepted += GameClient_ClientAccepted;
            GameClient.WorldDownloadProgress += GameClient_WorldDownloadProgress;

            //    GameClient.FlagCreated += GameClient_FlagCreated;

            timer1.Start();
        }

        private void PlayerList_PlayerStateUpdated(object sender, BZFlag.Game.Players.Player e)
        {
            AddLogLine("Player Updated " + e.Callsign);
        }

        private void GameClient_FlagCreated(object sender, BZFlag.Game.Flags.FlagInstance e)
        {
            AddLogLine("Flag Created " + e.ID.ToString());
        }

        private void GameClient_WorldDownloadProgress(object sender, BZFlag.Game.Client.WorldDownloadProgressEventArgs e)
        {
            AddLogLine("World Download Progress " + (e.Paramater * 100).ToString());
            WorldLoadProgress.Value = (int)(e.Paramater * WorldLoadProgress.Maximum);
        }

        private void GameClient_ClientAccepted(object sender, EventArgs e)
        {
            Accepted.Checked = true;
            AddLogLine("Client Accepted");
        }

        private void PlayerList_PlayerInfoUpdated(object sender, BZFlag.Game.Players.Player e)
        {
            PlayersList.Items.Clear();
            PlayersList.Items.AddRange(GameClient.PlayerList.GetPlayerList());
            AddLogLine("Player Updated " + e.Callsign);
        }

        protected void AddChatLine(string text)
        {
            ChatArea.AppendText(string.Format("{0}\r\n", text));
        }

        protected void AddLogLine(string text)
        {
            LogArea.AppendText(string.Format("{0}\r\n", text));
        }

        private void Chat_ChatMessageReceived(object sender, BZFlag.Game.Chat.ChatSystem.ChatMessageEventArgs e)
        {
            var from = GameClient.PlayerList.GetPlayerByID(e.Message.From, true);
            if (from == null)
            {
                AddChatLine(string.Format("{0}-{1}", e.Message.From, e.Message.Message));
            }
            else
            {
                AddChatLine(string.Format("{0}-{1}", from, e.Message.Message));
            }
        }

        private void ServerList_DoubleClick(object sender, EventArgs e)
        {
            Join_Click(sender, e);
        }

        private void OnlyFilledServers_CheckedChanged(object sender, EventArgs e)
        {
            ListServerLink_RequestCompleted(sender, e);
        }

        private void Callsign_TextChanged(object sender, EventArgs e)
        {
            CheckLogin();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();

            if (GameClient != null)
                GameClient.Shutdown();
        }

        private void SendChat_Click(object sender, EventArgs e)
        {
            if (GameClient == null)
                return;

            var msg = new MsgMessage();
            msg.From = GameClient.PlayerList.LocalPlayerID;
            msg.To = GameClient.PlayerList.AllPlayers.PlayerID;
            msg.MessageText = SendChatLine.Text;
            msg.MessageType = MsgMessage.MessageTypes.ChatMessage;

            GameClient.SendMessage(msg);

            AddChatLine("Sent: " + SendChatLine.Text);
            SendChatLine.Text = string.Empty;
        }

        private void SendChatLine_TextChanged(object sender, EventArgs e)
        {
            SendChat.Enabled = GameClient != null && SendChatLine.Text != string.Empty;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GameClient != null)
                GameClient.Update();
        }
    }
}
