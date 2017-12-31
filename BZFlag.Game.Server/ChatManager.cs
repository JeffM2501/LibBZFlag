using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BZFlag.Data.Players;
using BZFlag.Data.Teams;
using BZFlag.Game.Host.Players;
using BZFlag.Networking.Messages;
using BZFlag.Networking.Messages.BZFS;


namespace BZFlag.Game.Host
{
    public class ChatManager
    {
        public Server ServerHost = null;

        // use ServerHost.State.Players for player list

        public class ChatMessageEventArgs : EventArgs
        {
            public enum ChatMessageTypes
            {
                Unknown,
                PrivateMessage, 
                PublicMessage,
                TeamMessage,
                ServerPrivateMessage,
                ServerAnnouncemnt,
                ServerTeamMessage,
                GroupMessage,
                ServerGroupMessage,
            };

            public ChatMessageTypes ChatType = ChatMessageTypes.Unknown;

            public ServerPlayer From = null;
            public ServerPlayer To = null;
            public TeamColors ToTeam = TeamColors.NoTeam;
            public GroupInfo ToGroup = null;

            public bool Action = false;
            public string MessageText = string.Empty;

            public bool Allow = false;
            public bool Filtered = false;
        }

        public event EventHandler<ChatMessageEventArgs> MessageReceived = null;
        public event EventHandler<ChatMessageEventArgs> MessageFilter = null;
        public event EventHandler<ChatMessageEventArgs> MessagePreSend = null;
        public event EventHandler<ChatMessageEventArgs> MessageSent = null;

        public delegate bool TextCommandAcceptanceCallback(ServerPlayer player, MsgMessage message);

        public TextCommandAcceptanceCallback AcceptTextCommand = null;

        public delegate bool ChatFilterCallback(ChatMessageEventArgs message);
        public ChatFilterCallback DefaultFilter = null;

        public class GroupInfo
        {
            public string Name = string.Empty;
            public int BroadcastID = 0;
            public List<ServerPlayer> Members = new List<ServerPlayer>();

            public void AddPlayer(ServerPlayer player )
            {
                player.Exited += Player_Exited;

                lock (Members)
                    Members.Add(player);
            }

            public void RemovePlayer(ServerPlayer player)
            {
                if (player == null)
                    return;

                player.Exited -= Player_Exited;
            }

            private void Player_Exited(object sender, Networking.Common.Peer e)
            {
                RemovePlayer(e as ServerPlayer);
            }

            public void SentTo(NetworkMessage message, bool useUDP)
            {
                lock (Members)
                {
                    foreach (var member in Members)
                        member.SendMessage(!useUDP, message);
                }
            }
        }

        protected List<GroupInfo> CustomGroups = new List<GroupInfo>();
        public GroupInfo AdminGroup = null;

        public ChatManager()
        {
            AdminGroup = new GroupInfo();
            AdminGroup.Name = "Admin";
            AdminGroup.BroadcastID = PlayerConstants.AdminGroup;

            AddGroup(AdminGroup);
        }

        public void Shutdown()
        {
            lock(PendingInboundChats)
            {
                if (FilterWorker != null)
                    FilterWorker.Abort();

                FilterWorker = null;
                PendingInboundChats.Clear();
            }
        }

        public void AddGroup(GroupInfo group)
        {
            lock (CustomGroups)
                CustomGroups.Add(group);
        }

        public GroupInfo GetGroupByName(string name)
        {
            lock (CustomGroups)
                return CustomGroups.Find((x) => x.Name == name);
        }

        public GroupInfo[] GetGroups()
        {
            lock (CustomGroups)
                return CustomGroups.ToArray();
        }

        public void HandleChatMessage(ServerPlayer sender, MsgMessage message)
        {
            if (message == null || (AcceptTextCommand != null && AcceptTextCommand(sender, message)) || !sender.Allowances.AllowChat)
                return;

            ChatMessageEventArgs inChat = new ChatMessageEventArgs();

            inChat.To = null;
            inChat.Allow = true;
            inChat.Filtered = false;
            inChat.From = sender;
            inChat.MessageText = message.MessageText;
            inChat.Action = message.MessageType == MsgMessage.MessageTypes.ActionMessage;

            if (message.To <= PlayerConstants.MaxUseablePlayerID)
            {
                inChat.To = ServerHost.State.Players.GetPlayerByID(message.To);
                if (inChat.To == null)
                    return;
                inChat.ToTeam = inChat.To.ActualTeam;
                inChat.ChatType = ChatMessageEventArgs.ChatMessageTypes.PrivateMessage;
            }
            else if (message.To == PlayerConstants.AllPlayersID)
                inChat.ChatType = ChatMessageEventArgs.ChatMessageTypes.PublicMessage;
            else if (message.To >= PlayerConstants.FirstTeamID && message.To <= PlayerConstants.LastTeamID)
            {
                TeamColors toTeam = PlayerConstants.GetTeamColorFromID(message.To);

                if (toTeam != sender.ActualTeam || ServerHost.State.Players.GetTeamPlayerCount(toTeam) == 0)
                    return;

                inChat.ToTeam = toTeam;
                inChat.ChatType = ChatMessageEventArgs.ChatMessageTypes.TeamMessage;
            }
            else 
            {
                foreach (var group in GetGroups())
                {
                    if (message.To == group.BroadcastID)
                    {
                        inChat.To = null;
                        inChat.ToGroup = group;
                        inChat.ChatType = ChatMessageEventArgs.ChatMessageTypes.GroupMessage;
                    }
                }
            }

            if (inChat.ChatType == ChatMessageEventArgs.ChatMessageTypes.Unknown)
                return;

            MessageReceived?.Invoke(this, inChat);
            if (!inChat.Allow)
                return;

            PushMessageToFilter(inChat);
        }

        public void PushMessageToFilter(ChatMessageEventArgs message)
        {
            lock (PendingInboundChats)
                PendingInboundChats.Add(message);

            CheckFilterThread();
        }

        protected List<ChatMessageEventArgs> PendingInboundChats = new List<ChatMessageEventArgs>();
        protected Thread FilterWorker = null;

        protected ChatMessageEventArgs PopInboundChat()
        {
            lock(PendingInboundChats)
            {
                if (PendingInboundChats.Count == 0)
                    return null;

                ChatMessageEventArgs m = PendingInboundChats[0];
                PendingInboundChats.RemoveAt(0);
                return m;
            }
        }

        protected void CheckFilterThread()
        {
            lock (PendingInboundChats)
            {
                if (PendingInboundChats.Count == 0)
                    return;

                if (FilterWorker == null)
                {
                    FilterWorker = new Thread(new ThreadStart(FilterChat));
                    FilterWorker.Start();
                }
            }
        }

        public void FilterChat()
        {
            bool done = false;

            while (!done)
            {
                ChatMessageEventArgs message = PopInboundChat();
                if (message == null)
                    done = true;
                else
                {
                    if (DefaultFilter != null)
                        message.Filtered = DefaultFilter(message);

                    MessageFilter?.Invoke(this, message);

                    DispatchChatMessage(message);
                }
            }

            lock (PendingInboundChats)
                FilterWorker = null;
        }

        public void DispatchChatMessage(ChatMessageEventArgs messaage)
        {
            MessagePreSend?.Invoke(this, messaage);

            if (!messaage.Allow || messaage.ChatType == ChatMessageEventArgs.ChatMessageTypes.Unknown)
                return;

            switch(messaage.ChatType)
            {
                case ChatMessageEventArgs.ChatMessageTypes.PrivateMessage:
                case ChatMessageEventArgs.ChatMessageTypes.ServerPrivateMessage:

                    SendChatToUser(messaage.From, messaage.To, messaage.MessageText, messaage.Action);
                    return;

                case ChatMessageEventArgs.ChatMessageTypes.PublicMessage:
                case ChatMessageEventArgs.ChatMessageTypes.ServerAnnouncemnt:
                    SendChatToAll(messaage.From, messaage.MessageText, messaage.Action);
                    return;

                case ChatMessageEventArgs.ChatMessageTypes.TeamMessage:
                case ChatMessageEventArgs.ChatMessageTypes.ServerTeamMessage:
                    SendChatToTeam(messaage.From,messaage.ToTeam, messaage.MessageText, messaage.Action);
                    return;

                case ChatMessageEventArgs.ChatMessageTypes.GroupMessage:
                case ChatMessageEventArgs.ChatMessageTypes.ServerGroupMessage:
                    SendChatToGroup(messaage.From, messaage.ToGroup, messaage.MessageText, messaage.Action);
                    return;
            }
        }

        public void SendChatToUser(ServerPlayer from, ServerPlayer to, string chat, bool action)
        {
            ChatMessageEventArgs args = new ChatMessageEventArgs();
            args.From = from;
            args.To = to;
            args.ToTeam = to.ActualTeam;
            args.MessageText = chat;

            args.Action = action;
            args.ChatType = from == null ? ChatMessageEventArgs.ChatMessageTypes.ServerPrivateMessage : ChatMessageEventArgs.ChatMessageTypes.PrivateMessage;

            if (args.To == null)
                return;

            MsgMessage msg = new MsgMessage();
            msg.MessageText = chat;
            msg.From = from == null ? PlayerConstants.ServerPlayerID : from.PlayerID;
            msg.To = to.PlayerID;
            msg.MessageType = action ? MsgMessage.MessageTypes.ActionMessage : MsgMessage.MessageTypes.ChatMessage;

            to.SendMessage(false,msg);

            MessageSent?.Invoke(this,args);
        }

        public void SendChatToAll (ServerPlayer from, string chat, bool action)
        {
            ChatMessageEventArgs args = new ChatMessageEventArgs();
            args.From = from;
            args.To = null;
            args.MessageText = chat;

            args.Action = action;
            args.ChatType = from == null ? ChatMessageEventArgs.ChatMessageTypes.ServerAnnouncemnt : ChatMessageEventArgs.ChatMessageTypes.PublicMessage;

            MsgMessage msg = new MsgMessage();
            msg.MessageText = chat;
            msg.From = from == null ? PlayerConstants.ServerPlayerID : from.PlayerID;
            msg.To = PlayerConstants.AllPlayersID;

            msg.MessageType = action ? MsgMessage.MessageTypes.ActionMessage : MsgMessage.MessageTypes.ChatMessage;

            ServerHost.State.Players.SendToAll(msg, false);

            MessageSent?.Invoke(this, args);
        }

        public void SendChatToTeam(ServerPlayer from, TeamColors to, string chat, bool action)
        {
            ChatMessageEventArgs args = new ChatMessageEventArgs();
            args.From = from;
            args.To = null;
            args.ToTeam = to;
            args.MessageText = chat;

            args.Action = action;
            args.ChatType = from == null ? ChatMessageEventArgs.ChatMessageTypes.ServerTeamMessage : ChatMessageEventArgs.ChatMessageTypes.TeamMessage;

            if (args.ToTeam == TeamColors.NoTeam)
                return;

            MsgMessage msg = new MsgMessage();
            msg.MessageText = chat;
            msg.From = from == null ? PlayerConstants.ServerPlayerID : from.PlayerID;
            msg.To = PlayerConstants.GetTeamPlayerID(to);

            msg.MessageType = action ? MsgMessage.MessageTypes.ActionMessage : MsgMessage.MessageTypes.ChatMessage;

            ServerHost.State.Players.SendToTeam(msg, to, false);

            MessageSent?.Invoke(this, args);
        }

        public void SendChatToGroup(ServerPlayer from, GroupInfo to, string chat, bool action)
        {
            ChatMessageEventArgs args = new ChatMessageEventArgs();
            args.From = from;
            args.To = null;
            args.ToTeam = TeamColors.NoTeam;
            args.ToGroup = to;
            args.MessageText = chat;

            args.Action = action;
            args.ChatType = from == null ? ChatMessageEventArgs.ChatMessageTypes.ServerGroupMessage : ChatMessageEventArgs.ChatMessageTypes.GroupMessage;

            if (args.ToTeam == TeamColors.NoTeam)
                return;

            MsgMessage msg = new MsgMessage();
            msg.MessageText = chat;
            msg.From = from == null ? PlayerConstants.ServerPlayerID : from.PlayerID;
            msg.To = to.BroadcastID;

            msg.MessageType = action ? MsgMessage.MessageTypes.ActionMessage : MsgMessage.MessageTypes.ChatMessage;

            to.SentTo(msg, false);

            MessageSent?.Invoke(this, args);
        }
    }
}
