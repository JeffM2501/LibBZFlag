using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Networking.Messages;
using BZFlag.Networking.Messages.BZFS;

namespace BZFlag.Game.Chat
{
	public class ChatSystem
	{
		public class ChatMessage
		{
			public int From = -1;
			public int To = -1;

			public DateTime Recieved = DateTime.Now;

			public bool IsAction = false;
			public string Message = string.Empty;
		}

		public class UserChatLog
		{
			public List<ChatMessage> Messages = new List<ChatMessage>();
		}

		public List<ChatMessage> GlobalChatLog = new List<ChatMessage>();

		public Dictionary<int, UserChatLog> UserLogs = new Dictionary<int, UserChatLog>();

		public int LocalPlayerID = -1;
		public int LocalTeam = -1;

		public class ChatMessageEventArgs : EventArgs
		{
			public ChatMessage Message = null;

			public ChatMessageEventArgs(ChatMessage m)
			{
				Message = m;
			}
		}

		public event EventHandler<ChatMessageEventArgs> ChatMessageReceived = null;
		public event EventHandler<ChatMessageEventArgs> DirectMessageReceived = null;
		public event EventHandler<ChatMessageEventArgs> TeamMessageReceived = null;

		public void HandleChatMessage(NetworkMessage msg)
		{
			MsgMessage t = msg as MsgMessage;

			ChatMessage chat = new ChatMessage();
			chat.From = t.From;
			chat.To = t.To;
			chat.IsAction = t.MessageType == MsgMessage.MessageTypes.ActionMessage;
			chat.Message = t.MessageText;
			GlobalChatLog.Add(chat);

			if(!UserLogs.ContainsKey(chat.From))
				UserLogs.Add(chat.From, new UserChatLog());

			UserLogs[chat.From].Messages.Add(chat);

			ChatMessageEventArgs args = new ChatMessageEventArgs(chat);

			if(chat.To == LocalPlayerID && DirectMessageReceived != null)
				DirectMessageReceived.Invoke(this, args);

			if(chat.To == LocalTeam && TeamMessageReceived != null)
				TeamMessageReceived.Invoke(this, args);

			if(ChatMessageReceived != null)
				ChatMessageReceived.Invoke(this, args);
		}
	}
}
