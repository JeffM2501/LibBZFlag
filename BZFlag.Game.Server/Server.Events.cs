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
        public event EventHandler BZDBDefaultsLoaded;

        public event EventHandler WorldPreload;
        public event EventHandler WorldPostload;

        public event EventHandler PublicPreList;
        public event EventHandler PublicPostList;

        public event EventHandler<ServerPlayer> NewConnection;

        public event EventHandler<ServerPlayer> PlayerBanned;
        public event EventHandler<ServerPlayer> NewConnection;
    }
}
