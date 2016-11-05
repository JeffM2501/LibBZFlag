using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages
{
	public static class MessageFactory
	{
		private static Dictionary<int, Type> MessageTypes = new Dictionary<int, Type>();

		public static void RegisterMessageType(int code, Type t)
		{
			if(t.IsAbstract || !t.IsSubclassOf(typeof(NetworkMessage)))
				return;

			lock(MessageTypes)
			{
				if (!MessageTypes.ContainsKey(code))
					MessageTypes.Add(code, t);
			}	
		}

		public static void RegisterMessageType(NetworkMessage m)
		{
			RegisterMessageType(m.Code, m.GetType());
		}

		public static NetworkMessage Unpack(int code, byte[] buffer)
		{
			Type t = null;
			lock(MessageTypes)
			{
				if(!MessageTypes.ContainsKey(code))
					return new UnknownMessage(code, buffer);

				t = MessageTypes[code];
			}

			NetworkMessage msg = Activator.CreateInstance(t) as NetworkMessage;
			msg.Code = code;
			msg.Unpack(buffer);
			return msg;
		}

		public static void  RegisterBSFSMessages()
		{
			RegisterMessageType(new BZFS.MsgEnter());
			RegisterMessageType(new BZFS.MsgQueryGame());
		}
	}
}
