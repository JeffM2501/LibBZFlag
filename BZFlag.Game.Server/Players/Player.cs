
using System;
using System.Collections.Generic;
using System.Net.Sockets;

using BZFlag.Networking.Messages;
using BZFlag.Networking.Common;
using BZFlag.Networking.Messages.BZFS.Info;
using BZFlag.Data.Teams;
using BZFlag.Game.Host.World;
using System.Net;

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
        public IPHostEntry HostEntry = new IPHostEntry();

        public class LagInfo
        {
            public Dictionary<UInt16, double> OutstandingPings = new Dictionary<UInt16, double>();

            protected UInt64 SentPings = 0;
            protected UInt64 ReceivedPings = 0;

            public double InstantLagTime { get; protected set; } = 0;
            public double InstantJitter { get; protected set; } = 0;

            public double AverageLag { get; protected set; } = 0;
            public double AverageJitter { get; protected set; } = 0;

            public double TotalPacketLoss { get; protected set; } = 0;

            protected List<double> LagLog = new List<double>();

            protected UInt16 LastPing = 1;

            public double LastPingSent = double.MinValue;

            public static readonly int MaxLagHistory = 20;

            public void ReceivePing(UInt16 id, double now)
            {
                if (OutstandingPings.ContainsKey(id))
                {
                    ReceivedPings++;

                    if (ReceivedPings == 0)
                        TotalPacketLoss = 1.0;
                    else
                        TotalPacketLoss = (double)SentPings / (double)ReceivedPings;

                    double sendTime = OutstandingPings[id];
                    OutstandingPings.Remove(id);

                    double lag = now - sendTime;

                    InstantJitter = InstantLagTime - lag;
                    InstantLagTime = lag;
                    LagLog.Add(lag);

                    while (LagLog.Count > MaxLagHistory)
                        LagLog.RemoveAt(0);

                    double lagTotal = 0;
                    double jitterTotal = 0;
                    for (int i = 0; i < LagLog.Count; i++)
                    {
                        lagTotal += LagLog[i];
                        if (i > 0)
                            jitterTotal += LagLog[i] - LagLog[i - 1];
                    }

                    AverageLag = lagTotal / LagLog.Count;
                    if (LagLog.Count > 2)
                        AverageJitter = jitterTotal / (LagLog.Count - 1);
                    else
                        AverageJitter = InstantJitter;
                }
            }

            protected void GetNextPingID()
            {
                if(LastPing == UInt16.MaxValue - 1)
                    LastPing = 0;
                else
                    LastPing++;
            }

            public UInt16 GetPing(double now)
            {
                GetNextPingID();

                while (!OutstandingPings.ContainsKey(LastPing))
                    GetNextPingID();

                OutstandingPings.Add(LastPing, now);

                SentPings++;

                return LastPing;
            }
        }
        public LagInfo Lag = new LagInfo();

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


        public enum UDPConenctionStatuses
        {
            Unknown,
            RequestSent,
            Connected,
        }
        public UDPConenctionStatuses UDPStatus = UDPConenctionStatuses.Unknown;

        public bool SentSettings = false;

        public ServerPlayer(TcpClient pc)
        {
            Link(pc);
        }

        public string GetIPAsString()
        {
            if (TCP == null)
                return string.Empty;

            return ((IPEndPoint)TCP.Client.RemoteEndPoint).Address.ToString();
        }

        public IPAddress GetIPAddress()
        {
            if (TCP == null)
                return IPAddress.None;

            return ((IPEndPoint)TCP.Client.RemoteEndPoint).Address;
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

        public bool CanDoPlayActions()
        {
            return Accepted && Active && Info.Alive && Allowances.AllowPlay  && ActualTeam != TeamColors.ObserverTeam;
        }
    }
}
