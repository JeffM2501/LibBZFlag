using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BZFlag.Authentication;
using BZFlag.Networking.Messages.BZFS;

namespace TestClient
{
    public partial class Form1 : Form
    {
        ServiceLink ListServerLink = new ServiceLink();

        BZFlag.Game.Client GameClient = null;

        public Form1()
        {
            InitializeComponent();
            ListServerLink.RequestCompleted += ListServerLink_RequestCompleted;

            SendChatLine_TextChanged(this, EventArgs.Empty);
            CheckLogin();
        }

        public void Application_Idle(object sender, EventArgs e)
        {
            if (GameClient != null)
                GameClient.Update();
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
                foreach(var s in ListServerLink.ServerList)
                {
                    if (s.TotalPlayers > 0)
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
        }


        private void ServerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ServerInfo.Text = string.Empty;
            ServiceLink.ListServerData server = ServerList.SelectedItem as ServiceLink.ListServerData;
            if (server == null)
                return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(server.Name);
            sb.AppendLine("Address : " + server.Address);
            sb.AppendLine("Players : " + server.TotalPlayers.ToString());
            sb.AppendLine(server.Description);

            ServerInfo.Text = sb.ToString();
        }

        private void Join_Click(object sender, EventArgs e)
        {
            ServiceLink.ListServerData server = ServerList.SelectedItem as ServiceLink.ListServerData;
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
        }

        private void PlayerList_PlayerInfoUpdated(object sender, BZFlag.Game.Players.Player e)
        {
            PlayersList.Items.Clear();
            PlayersList.Items.AddRange(GameClient.PlayerList.GetPlayerList());
        }

        protected void AddChatLine(string text)
        {
            ChatArea.Text += string.Format("{0}\r\n", text);
 
            ChatArea.SelectionStart = ChatArea.Text.Length;
            ChatArea.SelectionLength = 0;
        }

        private void Chat_ChatMessageReceived(object sender, BZFlag.Game.Chat.ChatSystem.ChatMessageEventArgs e)
        {
            var from = GameClient.PlayerList.GetPlayerByID(e.Message.From);
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
            if (GameClient != null)
                GameClient.Shutdown();
        }

        private void SendChat_Click(object sender, EventArgs e)
        {
            if (GameClient == null)
                return;

            var msg = new MsgMessage();
            msg.From = GameClient.PlayerList.LocalPlayerID;
            msg.To = 254;
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
    }
}
