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
using BZFlag.Networking.Messages.BZFS.Control;

namespace BZFlag.Networking.Messages
{
	public static class ClientMessageFactory
	{
		public static MessageManager Factory = new MessageManager();

		static ClientMessageFactory()
		{
			Factory.RegisterBSFSClientMessages();
		}
	}

	public static class SecurityJailMessageFacotry
	{
		public static MessageManager Factory = new MessageManager();

		static SecurityJailMessageFacotry()
		{
			Factory.RegisterMessageType(new MsgEnter());
			Factory.RegisterMessageType(new MsgExit());

			Factory.RegisterMessageType(new MsgQueryGame());
			Factory.RegisterMessageType(new MsgQueryPlayers());

			Factory.RegisterMessageType(new MsgWantWHash());
			Factory.RegisterMessageType(new MsgGetWorld());

			Factory.RegisterMessageType(new MsgWantSettings());
			Factory.RegisterMessageType(new MsgNegotiateFlags());

			Factory.RegisterMessageType(new MsgUDPLinkRequest());
			Factory.RegisterMessageType(new MsgUDPLinkEstablished());
		}
	}

	public class MessageManager
	{
		private Dictionary<int, Type> MessageTypes = new Dictionary<int, Type>();

		public void RegisterMessageType(int code, Type t)
		{
			if(t.IsAbstract || !t.IsSubclassOf(typeof(NetworkMessage)))
				return;

			lock(MessageTypes)
			{
				if (!MessageTypes.ContainsKey(code))
					MessageTypes.Add(code, t);
			}	
		}

		public void RegisterMessageType(NetworkMessage m)
		{
			RegisterMessageType(m.Code, m.GetType());
		}

		public  NetworkMessage Unpack(int code, byte[] buffer)
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

		public void  RegisterBSFSClientMessages()
		{
			RegisterMessageType(new MsgEnter());
			RegisterMessageType(new MsgExit());
            RegisterMessageType(new MsgQueryGame());
			RegisterMessageType(new MsgSuperKill());
			RegisterMessageType(new MsgWantWHash());
            RegisterMessageType(new MsgCacheURL());
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
            RegisterMessageType(new MsgTimeUpdate());
            RegisterMessageType(new MsgScoreOver());
            RegisterMessageType(new MsgPause());
            RegisterMessageType(new MsgAutoPilot());
            RegisterMessageType(new MsgFlagType());
            RegisterMessageType(new MsgNewRabbit());
            RegisterMessageType(new MsgHandicap());
            RegisterMessageType(new MsgCustomSound());
            RegisterMessageType(new MsgWantSettings());
            RegisterMessageType(new MsgGameSettings());
            RegisterMessageType(new MsgFetchResources());
            RegisterMessageType(new MsgAdminInfo());
            RegisterMessageType(new MsgReplayReset());
		}
	}
}
