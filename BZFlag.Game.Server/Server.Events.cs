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

        public event EventHandler PublicPreList;
        public event EventHandler PublicPostList;

        public event EventHandler<ServerPlayer> NewConnection;

        public event EventHandler<ServerPlayer> PlayerBanned;
        public event EventHandler<ServerPlayer> PlayerRejected;
        public event EventHandler<ServerPlayer> PlayerAccepted;

        public event EventHandler<BooleanResultPlayerEventArgs> CheckPlayerAcceptance;

        public event EventHandler<TCPConnectionManager.NetworkBanEventArgs> ConnectionBanned;


        private void RegisterProcessorEvents()
        {
            SecurityArea.CheckPlayerAcceptance += this.SecurityArea_CheckPlayerAcceptance;
            SecurityArea.PlayerAccepted += this.SecurityArea_PlayerAccepted;
            SecurityArea.PlayerBanned += this.SecurityArea_PlayerBanned;
            SecurityArea.PlayerRejected += SecurityArea_PlayerRejected;

            TCPConnections.ConnectionHostBanned += TCPConnections_ConnectionHostBanned;
            TCPConnections.ConnectionIPBanned += TCPConnections_ConnectionIPBanned;
        }

        private void TCPConnections_ConnectionIPBanned(object sender, TCPConnectionManager.NetworkBanEventArgs e)
        {
            ConnectionBanned?.Invoke(this, e);
        }

        private void TCPConnections_ConnectionHostBanned(object sender, TCPConnectionManager.NetworkBanEventArgs e)
        {
            ConnectionBanned?.Invoke(this, e);
        }

        private void SecurityArea_PlayerBanned(object sender, ServerPlayer e)
        {
            PlayerBanned?.Invoke(this, e);
        }

        private void SecurityArea_PlayerAccepted(object sender, ServerPlayer e)
        {
            PlayerAccepted?.Invoke(this, e);
        }

        private void SecurityArea_PlayerRejected(object sender, ServerPlayer e)
        {
            PlayerRejected?.Invoke(this, e);
        }

        private void SecurityArea_CheckPlayerAcceptance(object sender, BooleanResultPlayerEventArgs e)
        {
            CheckPlayerAcceptance?.Invoke(this, e);
        }
    }
}
