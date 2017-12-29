using BZFlag.Game.Host.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZFlag.Game.Host
{
    public partial class Server
    {
        public class BooleanResultPlayerEventArgs : EventArgs
        {
            public bool Result = true;
            public ServerPlayer Player = null;

            public BooleanResultPlayerEventArgs(ServerPlayer p)
            {
                Player = p;
            }

            public BooleanResultPlayerEventArgs(ServerPlayer p, bool r)
            {
                Player = p;
                Result = r;
            }
        }

        public event EventHandler BZDBDefaultsLoaded;

        public event EventHandler WorldPreload;
        public event EventHandler WorldPostload;

        public event EventHandler APILoadComplete;
        public event EventHandler ModuleLoadComplete;

        public event EventHandler ConfigLoaded;

        public event EventHandler PublicPreList;
        public event EventHandler PublicPostList;

        public event EventHandler<ServerPlayer> NewConnection;

        public event EventHandler<TCPConnectionManager.PendingClient> PlayerAddressBanned;
        public event EventHandler<ServerPlayer> PlayerIDBanned;
        public event EventHandler<ServerPlayer> PlayerRejected;
        public event EventHandler<ServerPlayer> PlayerAccepted;

        public event EventHandler<ServerPlayer> PlayerPreAdd;
        public event EventHandler<ServerPlayer> PlayerPostAdd;
        public event EventHandler<ServerPlayer> PlayerRemoved;

        public event EventHandler<BooleanResultPlayerEventArgs> CheckPlayerAcceptance;


        private void RegisterProcessorEvents()
        {
           
            SecurityArea.PlayerAccepted += this.SecurityArea_PlayerAccepted;
            SecurityArea.PlayerBanned += this.SecurityArea_PlayerBanned;
            SecurityArea.PlayerRejected += SecurityArea_PlayerRejected;

            GameZone.PlayerRejected += SecurityArea_PlayerRejected; // can still be rejected by team

            TCPConnections.CheckIPBan = CheckTCPIPBan;
            TCPConnections.CheckHostBan = CheckTCPHostBan;
            SecurityArea.CheckIDBan = CheckBZIDBan;
            SecurityArea.CheckPlayerAcceptance += this.SecurityArea_CheckPlayerAcceptance;
        }

        // ban hooks
        public delegate bool AddressBanCallback(string addres, bool IsIP, ref string reason);
        public delegate bool PlayerBanCallback(ServerPlayer player, ref string reason);

        public AddressBanCallback IsAddressBanned = null;
        public PlayerBanCallback IsPlayerBanned = null;

        private bool CheckBZIDBan(ServerPlayer player, ref string reason)
        {
            if (IsPlayerBanned == null)
                return false;

            return IsPlayerBanned(player, ref reason);
        }

        private bool CheckTCPHostBan(TCPConnectionManager.PendingClient player, ref string reason)
        {
            if (IsAddressBanned == null)
                return false;

            bool banned = IsAddressBanned(player.HostEntry.HostName, false, ref reason);
            if (banned)
                PlayerAddressBanned?.Invoke(this, player);

            return banned;
        }

        private bool CheckTCPIPBan(TCPConnectionManager.PendingClient player, ref string reason)
        {
            if (IsAddressBanned == null)
                return false;

            bool banned = IsAddressBanned(player.GetIPAsString(), false, ref reason);
            if (banned)
                PlayerAddressBanned?.Invoke(this, player);

            return banned;
        }

        private void SecurityArea_CheckPlayerAcceptance(object sender, BooleanResultPlayerEventArgs e)
        {
            CheckPlayerAcceptance?.Invoke(this, e);
        }

        private void SecurityArea_PlayerBanned(object sender, ServerPlayer e)
        {
            PlayerIDBanned?.Invoke(this, e);
        }

        private void SecurityArea_PlayerAccepted(object sender, ServerPlayer e)
        {
            PlayerAccepted?.Invoke(this, e);
        }

        private void SecurityArea_PlayerRejected(object sender, ServerPlayer e)
        {
            PlayerRejected?.Invoke(this, e);
        }

        public void PreAddPlayer(ServerPlayer p)
        {
            PlayerPreAdd?.Invoke(this, p);
        }

        public void PostAddPlayer(ServerPlayer p)
        {
            PlayerPostAdd?.Invoke(this, p);
        }

        public void RemovedPlayer(ServerPlayer p)
        {
            UDPConnections.RemoveAcceptablePlayer(p.ConnectionData.GetIPAddress(),p);

            PlayerRemoved?.Invoke(this, p);
        }
    }
}
