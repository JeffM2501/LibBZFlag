
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using BZFlag.Networking.Messages;
using BZFlag.Networking.Common;
using BZFlag.Networking.Messages.BZFS.Info;
using System.Net;

namespace BZFlag.Game.Host.Players
{
    public class ServerPlayer : Peer
    {
        public int PlayerID = 0;
        public object Tag = 0;

        public bool Accepted = false;
        public string RejectionReason = string.Empty;

        public string Callsign = string.Empty;
        public string Token = string.Empty;
        public string Motto = string.Empty;

        public string BZID = string.Empty;

        public bool NeedStartupInfo = true;

        public MsgNegotiateFlags ClientFlagList = null;

        public enum AuthStatuses
        {
            Unknown,
            NoneProvided,
            InProgress,
            Valid,
            Failed,
        }

        public AuthStatuses AuthStatus = AuthStatuses.Unknown;
        public List<string> GroupMemberships = new List<string>();

        protected NetworkStream NetStream = null;

        public TCPConnectionManager.PendingClient ConnectionData = null;

        public ServerPlayer(TCPConnectionManager.PendingClient pc)
        {
            ConnectionData = pc;
            Link(ConnectionData.ClientConnection);
        }
    }
}
