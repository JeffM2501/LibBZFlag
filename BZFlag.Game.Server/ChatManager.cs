using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.Data.Players;
using BZFlag.Data.Teams;
using BZFlag.Game.Host.Players;
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
                PrivateMessage, 
                PublicMessage,
                TeamMessage,
                ServerPrivateMessage,
                ServerAnnouncemnt,
                ServerTeamMessage,
            };

            public ChatMessageTypes ChatType = ChatMessageTypes.PublicMessage;

            public ServerPlayer From = null;
            public ServerPlayer To = null;
            public TeamColors ToTeam = TeamColors.NoTeam;

            public bool Action;
            public string MessageText = string.Empty;

            public bool Allow = false;
            public bool Filtered = false;
        }

        public event EventHandler<ChatMessageEventArgs> MessageReceived = null;
        public event EventHandler<ChatMessageEventArgs> MessageFilter = null;
        public event EventHandler<ChatMessageEventArgs> MessagePreSend = null;
        public event EventHandler<ChatMessageEventArgs> MessageSent = null;


        public void HandleChatMessage(ServerPlayer sender, MsgMessage message)
        {
            if (message == null)
                return;

            ChatMessageEventArgs inChat = new ChatMessageEventArgs();

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

            if (args.To == null)
                return;

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
    }
}
