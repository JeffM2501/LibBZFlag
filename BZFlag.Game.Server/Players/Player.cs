
using System;
using System.Collections.Generic;
using System.Net.Sockets;

using BZFlag.Networking.Messages;
using BZFlag.Networking.Common;
using BZFlag.Networking.Messages.BZFS.Info;
using BZFlag.Data.Teams;
using BZFlag.Game.Host.World;

namespace BZFlag.Game.Host.Players
{
    public class ServerPlayer : Peer
    {
        public int PlayerID = 0;
        public string BZID = string.Empty;

        public bool Accepted = false;
        public string RejectionReason = string.Empty;

        public string Callsign = string.Empty;
        public string Token = string.Empty;
        public string Motto = string.Empty;

        public bool ShowAdminMark = false;

        public TeamColors DesiredTeam = TeamColors.AutomaticTeam;
        public TeamColors ActualTeam = TeamColors.AutomaticTeam;

        public event EventHandler<Peer> Exited = null;

        public bool NeedStartupInfo = true;
        public bool HasValidEnter = false;

        public MsgNegotiateFlags ClientFlagList = null;

        public PlayerManager.PlayerInfo Info = null; 

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

        public class AllowableActions
        {
            public bool AllowChat = true;
            public bool AllowCommands = true;
            public bool AllowPlay= true;
        }
        public AllowableActions Allowances = new AllowableActions();

        public Dictionary<string, object> Tags = new Dictionary<string, object>();

        protected NetworkStream NetStream = null;

        public TCPConnectionManager.PendingClient ConnectionData = null;

        public enum UDPConenctionStatuses
        {
            Unknown,
            RequestSent,
            Connected,
        }
        public UDPConenctionStatuses UDPStatus = UDPConenctionStatuses.Unknown;

        public bool SentSettings = false;

        public ServerPlayer(TCPConnectionManager.PendingClient pc)
        {
            ConnectionData = pc;
            Link(ConnectionData.ClientConnection);
        }

        public void SetTag(string name, object value, bool force)
        {
            lock (Tags)
            {
                if (!Tags.ContainsKey(name))
                    Tags.Add(name, value);
                else if (force)
                    Tags[name] = value;
            }
        }

        public T GetTag<T>(string name) where T : class, new()
        {
            lock(Tags)
            {
                if (!Tags.ContainsKey(name))
                    Tags.Add(name, new T());

                return Tags[name] as T;
            }
        }

        public void SetExit()
        {
            Exited?.Invoke(this, this);
        }

        public void ProcessUDPMessage(NetworkMessage msg)
        {
            msg.FromUDP = true;
            InboundMessageProcessor.CompleteMessage(msg);
        }

        public bool CanDoPlayActions()
        {
            return Accepted && Active && Info.Alive && Allowances.AllowPlay  && ActualTeam != TeamColors.ObserverTeam;
        }
    }
}
