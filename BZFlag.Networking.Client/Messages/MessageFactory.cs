using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Networking.Messages.BZFS;
using BZFlag.Networking.Messages.BZFS.UDP;
using BZFlag.Networking.Messages.BZFS.World;
using BZFlag.Networking.Messages.BZFS.BZDB;
using BZFlag.Networking.Messages.BZFS.Player;
using BZFlag.Networking.Messages.BZFS.Info;
using BZFlag.Networking.Messages.BZFS.Shots;
using BZFlag.Networking.Messages.BZFS.Flags;

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
			RegisterMessageType(new MsgEnter());
			RegisterMessageType(new MsgExit());
            RegisterMessageType(new MsgQueryGame());
			RegisterMessageType(new MsgSuperKill());
			RegisterMessageType(new MsgWantWHash());
			RegisterMessageType(new MsgNegotiateFlags());
			RegisterMessageType(new MsgGameTime());
			RegisterMessageType(new MsgMessage());
			RegisterMessageType(new MsgAccept());
			RegisterMessageType(new MsgReject());
			RegisterMessageType(new MsgSetVars());
			RegisterMessageType(new MsgTeamUpdate());
			RegisterMessageType(new MsgUDPLinkRequest());
            RegisterMessageType(new MsgUDPLinkEstablished());
            RegisterMessageType(new MsgLagPing());
            RegisterMessageType(new MsgFlagUpdate());
			RegisterMessageType(new MsgAddPlayer());
			RegisterMessageType(new MsgRemovePlayer());
			RegisterMessageType(new MsgPlayerInfo());
			RegisterMessageType(new MsgGetWorld());
            RegisterMessageType(new MsgPlayerUpdateSmall());
            RegisterMessageType(new MsgPlayerUpdate());
            RegisterMessageType(new MsgQueryPlayers());
            RegisterMessageType(new MsgScore());
			RegisterMessageType(new MsgAlive());
			RegisterMessageType(new MsgShotBegin());
			RegisterMessageType(new MsgShotEnd());
			RegisterMessageType(new MsgDropFlag());
			RegisterMessageType(new MsgGrabFlag());
			RegisterMessageType(new MsgTransferFlag());
			RegisterMessageType(new MsgGMUpdate());
            RegisterMessageType(new MsgKilled());
            RegisterMessageType(new MsgTeleport());
            RegisterMessageType(new MsgCaptureFlag());
            RegisterMessageType(new MsgNearFlag());
        }
	}
}
