using System;
using System.Collections.Generic;
using System.Text;

using BZFlag.Game.Host;
using BZFlag.Game.Host.Players;

namespace BZFS.PlayerAdministration
{
    public class PermissionProcessor
    {
        public class PlayerPermissions
        {
            public List<string> Perms = new List<string>();

            public bool HasPerm(string perm)
            {
                lock(Perms)
                    return Perms.Contains(perm);
            }
        }

        public static readonly string PermsTagName = "BZFS.PlayerAdministration.Permissions";

        public GameState State = null;

        public void Init(Server serverHost, ServerConfig.SecurityInfo info)
        {
            State = serverHost.State;
            serverHost.PlayerPreAdd += ServerHost_PlayerPreAdd;
            serverHost.PlayerAccepted += ServerHost_PlayerAccepted;
        }

        private void ServerHost_PlayerAccepted(object sender, ServerPlayer player)
        {
            if (GetPlayerPermissions(player).HasPerm(PermissionNames.AdminChannelReceive))
                State.Chat.AdminGroup.AddPlayer(player);
        }

        private void ServerHost_PlayerPreAdd(object sender, ServerPlayer player)
        {
            PlayerPermissions perms = new PlayerPermissions();
            player.SetTag(PermsTagName, perms, true);
            SetPerms(player, perms);

            if (perms.HasPerm(PermissionNames.Kick) || perms.HasPerm(PermissionNames.Ban) || perms.HasPerm(PermissionNames.AdminMarkShow))
                player.ShowAdminMark = true;
        }

        private void SetPerms(ServerPlayer player, PlayerPermissions perms)
        {
            lock (perms.Perms)
            {
                foreach (var group in player.GroupMemberships)
                    perms.Perms.AddRange(State.ConfigData.Security.GetGroupPerms(group));
            }
        }

        public void Shutdown()
        {

        }

        public static PlayerPermissions GetPlayerPermissions(ServerPlayer player)
        {
            PlayerPermissions perms = player.GetTag<PlayerPermissions>(PermsTagName);
            if (perms == null)
            {
                perms = new PlayerPermissions();
                player.SetTag(PermsTagName, perms, true);
            }

            return perms;
        }

        public static bool PlayerHasPermision(ServerPlayer player, string permissionName)
        {
            return GetPlayerPermissions(player).HasPerm(permissionName);
        }
    }
}
