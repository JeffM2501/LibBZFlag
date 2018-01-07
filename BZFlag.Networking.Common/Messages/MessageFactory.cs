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
            Factory.RegisterMessageType(new MsgEnter());
            Factory.RegisterMessageType(new MsgExit());
            Factory.RegisterMessageType(new MsgQueryGame());
            Factory.RegisterMessageType(new MsgSuperKill());
            Factory.RegisterMessageType(new MsgWantWHash());
            Factory.RegisterMessageType(new MsgCacheURL());
            Factory.RegisterMessageType(new MsgNegotiateFlags());
            Factory.RegisterMessageType(new MsgGameTime());
            Factory.RegisterMessageType(new MsgMessage());
            Factory.RegisterMessageType(new MsgAccept());
            Factory.RegisterMessageType(new MsgReject());
            Factory.RegisterMessageType(new MsgSetVars());
            Factory.RegisterMessageType(new MsgTeamUpdate());
            Factory.RegisterMessageType(new MsgUDPLinkRequest());
            Factory.RegisterMessageType(new MsgUDPLinkEstablished());
            Factory.RegisterMessageType(new MsgLagPing());
            Factory.RegisterMessageType(new MsgFlagUpdate());
            Factory.RegisterMessageType(new MsgAddPlayer());
            Factory.RegisterMessageType(new MsgRemovePlayer());
            Factory.RegisterMessageType(new MsgPlayerInfo());
            Factory.RegisterMessageType(new MsgGetWorld());
            Factory.RegisterMessageType(new MsgPlayerUpdateSmall());
            Factory.RegisterMessageType(new MsgPlayerUpdate());
            Factory.RegisterMessageType(new MsgQueryPlayers());
            Factory.RegisterMessageType(new MsgScore());
            Factory.RegisterMessageType(new MsgAlive());
            Factory.RegisterMessageType(new MsgShotBegin());
            Factory.RegisterMessageType(new MsgShotEnd());
            Factory.RegisterMessageType(new MsgDropFlag());
            Factory.RegisterMessageType(new MsgGrabFlag());
            Factory.RegisterMessageType(new MsgTransferFlag());
            Factory.RegisterMessageType(new MsgGMUpdate());
            Factory.RegisterMessageType(new MsgKilled());
            Factory.RegisterMessageType(new MsgTeleport());
            Factory.RegisterMessageType(new MsgCaptureFlag());
            Factory.RegisterMessageType(new MsgNearFlag());
            Factory.RegisterMessageType(new MsgTimeUpdate());
            Factory.RegisterMessageType(new MsgScoreOver());
            Factory.RegisterMessageType(new MsgPause());
            Factory.RegisterMessageType(new MsgAutoPilot());
            Factory.RegisterMessageType(new MsgFlagType());
            Factory.RegisterMessageType(new MsgNewRabbit());
            Factory.RegisterMessageType(new MsgHandicap());
            Factory.RegisterMessageType(new MsgCustomSound());
            Factory.RegisterMessageType(new MsgWantSettings());
            Factory.RegisterMessageType(new MsgGameSettings());
            Factory.RegisterMessageType(new MsgFetchResources());
            Factory.RegisterMessageType(new MsgAdminInfo());
            Factory.RegisterMessageType(new MsgReplayReset());
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

    public static class StagingMessageFacotry
    {
        public static MessageManager Factory = new MessageManager();

        static StagingMessageFacotry()
        {
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

    public static class GameServerMessageFacotry
    {
        public static MessageManager Factory = new MessageManager();

        static GameServerMessageFacotry()
        {
            Factory.RegisterMessageType(new MsgExit());

            Factory.RegisterMessageType(new MsgGameTime());
            Factory.RegisterMessageType(new MsgLagPing());

            Factory.RegisterMessageType(new MsgUDPLinkRequest());
            Factory.RegisterMessageType(new MsgUDPLinkEstablished());

            Factory.RegisterMessageType(new MsgMessage());

            Factory.RegisterMessageType(new MsgQueryPlayers());

            Factory.RegisterMessageType(new MsgAutoPilot());
            Factory.RegisterMessageType(new MsgPlayerUpdateSmall());
            Factory.RegisterMessageType(new MsgPlayerUpdate());
            Factory.RegisterMessageType(new MsgAlive());
            Factory.RegisterMessageType(new MsgKilled());
            Factory.RegisterMessageType(new MsgTeleport());
            Factory.RegisterMessageType(new MsgPause());

            Factory.RegisterMessageType(new MsgScore());
            Factory.RegisterMessageType(new MsgTimeUpdate());

            Factory.RegisterMessageType(new MsgShotBegin());
            Factory.RegisterMessageType(new MsgShotEnd());
            Factory.RegisterMessageType(new MsgGMUpdate());

            Factory.RegisterMessageType(new MsgFlagType());
            Factory.RegisterMessageType(new MsgGrabFlag());
            Factory.RegisterMessageType(new MsgDropFlag());
        }
    }

    public static class UDPServerMessageFactory
    {
        public static MessageManager Factory = new MessageManager();

        static UDPServerMessageFactory()
        {
            Factory.RegisterMessageType(new MsgUDPLinkRequest());
            Factory.RegisterMessageType(new MsgUDPLinkEstablished());

            Factory.RegisterMessageType(new MsgPlayerUpdateSmall());
            Factory.RegisterMessageType(new MsgPlayerUpdate());
    
            Factory.RegisterMessageType(new MsgScore());
            Factory.RegisterMessageType(new MsgTimeUpdate());

            Factory.RegisterMessageType(new MsgShotBegin());
            Factory.RegisterMessageType(new MsgShotEnd());
            Factory.RegisterMessageType(new MsgGMUpdate());
        }
    }

    public class MessageManager
    {
        private Dictionary<int, Type> MessageTypes = new Dictionary<int, Type>();

        public void RegisterMessageType(int code, Type t)
        {
            if (t.IsAbstract || !t.IsSubclassOf(typeof(NetworkMessage)))
                return;

            lock (MessageTypes)
            {
                if (!MessageTypes.ContainsKey(code))
                    MessageTypes.Add(code, t);
            }
        }

        public Type[] GetMessageTypes()
        {
            lock (MessageTypes)
                return MessageTypes.Values.ToArray();
        }

        public void RegisterMessageType(NetworkMessage m)
        {
            RegisterMessageType(m.Code, m.GetType());
        }

        public NetworkMessage Unpack(int code, byte[] buffer)
        {
            return Unpack(code, buffer, false);
        }

        public NetworkMessage Unpack(int code, byte[] buffer, bool udp)
        {
            Type t = null;
            lock (MessageTypes)
            {
                if (!MessageTypes.ContainsKey(code))
                    return new UnknownMessage(code, buffer);

                t = MessageTypes[code];
            }

            NetworkMessage msg = Activator.CreateInstance(t) as NetworkMessage;
            msg.Code = code;
            msg.Unpack(buffer);
            msg.FromUDP = udp;
            return msg;
        }

    }
}