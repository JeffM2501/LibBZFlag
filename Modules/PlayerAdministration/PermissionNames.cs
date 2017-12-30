using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZFS.PlayerAdministration
{
    public static class PermissionNames
    {
        public static readonly string Kick = "BZFS.Kick";
        public static readonly string KickImmunity = "BZFS.Kick.Immune";

        public static readonly string Ban = "BZFS.Ban";
        public static readonly string BanImmunity = "BZFS.Ban.Immune";

        public static readonly string AdminChannelReceive = "BZFS.Chat.Admin.Receive";
        public static readonly string AdminMarkShow = "BZFS.Player.Admin.Show";
    }
}
          