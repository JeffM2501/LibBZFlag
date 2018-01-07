using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using BZFlag.Game.Host;
using BZFlag.Game.Host.API;
using BZFlag.Game.Host.Players;

using BZFS.SlashCommands;

namespace BZFS.PlayerAdministration
{
    public partial class Admin : PlugIn
    {
        public override string Name => "PlayerAdmin";

        public override string Description => "Handles player administration functions";

        private PermissionProcessor Permissions = new PermissionProcessor();

        Thread ExpireCheck = null;

        private Server.AddressBanCallback OrigonalAddressBan = null;
        private Server.PlayerBanCallback OrigonaPlayerBan = null;

        public override void Shutdown(Server serverInstance)
        {
            if (ExpireCheck != null)
                ExpireCheck.Abort();
            ExpireCheck = null;

            Permissions.Shutdown();
        }

        public override void Startup(Server serverInstance)
        {
            Permissions.Init(serverInstance, State.ConfigData.Security);
            BanDatabase.Init(State.ConfigData.Security);

            serverInstance.APILoadComplete += ServerInstance_APILoadComplete;

            serverInstance.CheckPlayerAcceptance += ServerInstance_CheckPlayerAcceptance;

            OrigonalAddressBan = serverInstance.IsAddressBanned;
            OrigonaPlayerBan = serverInstance.IsPlayerBanned;

            serverInstance.IsAddressBanned = ServerInstance_CheckAddressBan;
            serverInstance.IsPlayerBanned = ServerInstance_CheckIDBan;

            ExpireCheck = new Thread(new ThreadStart(CheckExpired));
            ExpireCheck.Start();

            Commands.RegisterHandler("KICK", Kick);
            Commands.RegisterHandler("BAN", Ban);
        }

        private void CheckExpired()
        {
            // TODO, just track the nearest expiration and check on that, instead of firing off a query every time.

            int CheckInterval = 5; // every 5 min;
            while (true)
            {
                Thread.Sleep(CheckInterval * 60 * 1000);
                BanDatabase.CheckExpired();
            }
        }

        private bool ServerInstance_CheckIDBan(ServerPlayer player, ref string reason)
        {
            var results = BanDatabase.CheckIDBan(player.BZID);
            reason = results.Reason;

            if (results.Active)
                return true;

            if (OrigonaPlayerBan != null)
                return OrigonaPlayerBan(player, ref reason);
            return false;
        }

        private bool ServerInstance_CheckAddressBan(string addres, bool IsIP, ref string reason)
        {
            var results = IsIP ? BanDatabase.CheckAddressBan(addres) : BanDatabase.CheckHostBan(addres);
            reason = results.Reason;

            if (results.Active)
                return true;

            if (OrigonalAddressBan != null)
                return OrigonalAddressBan(addres, IsIP, ref reason);
            return false;
        }

        private void ServerInstance_CheckPlayerAcceptance(object sender, Server.BooleanResultPlayerEventArgs e)
        {
            e.Result = true;
        }

        private void ServerInstance_APILoadComplete(object sender, EventArgs e)
        {
            BanDatabase.Init(State.ConfigData.Security);
        }
    }
}
