using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BZFlag.Game.Host.Players;
using BZFS.PlayerAdministration.Databases;

namespace BZFS.PlayerAdministration
{
    public static class API
    {
        internal static Dictionary<string, IBanDBBackend> Backends = new Dictionary<string, IBanDBBackend>();

        public static int AddBan(string id, string hostmask, string addressmask, string reason, string author, int days = 0, int minutes = 0)
        {
            return BanDatabase.AddBan(id, addressmask, hostmask, reason, author, days, minutes);
        }

        public static void KickUser(ServerPlayer player)
        {
            player.Disconnect();
        }

        public static void RegisterDBBackend(string name, IBanDBBackend backend)
        {
            lock (Backends)
            {
                if (backend != null && !Backends.ContainsKey(name.ToUpperInvariant()))
                    Backends.Add(name.ToUpperInvariant(),backend);
            }
        }
    }
}
