using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using BZFlag.Game.Host;
using BZFlag.Game.Host.API;
using BZFlag.Game.Host.Players;

namespace BZFS.PlayerAdministration
{
    public class Startup : PlugIn
    {
        public string Name => "PlayerAdmin";

        public string Description => "Handles player administration functions";


        private Server Instance = null;
        Thread ExpireCheck = null;

        public void Shutdown(Server serverInstance)
        {
            if (ExpireCheck != null)
                ExpireCheck.Abort();
            ExpireCheck = null;
        }

        void PlugIn.Startup(Server serverInstance)
        {
            Instance = serverInstance;

            Instance.APILoadComplete += ServerInstance_APILoadComplete;

            Instance.CheckPlayerAcceptance += ServerInstance_CheckPlayerAcceptance;
            Instance.IsAddressBanned = ServerInstance_CheckAddressBan;
            Instance.IsPlayerBanned = ServerInstance_CheckIDBan;

            ExpireCheck = new Thread(new ThreadStart(CheckExpired));
            ExpireCheck.Start();
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
            return results.Active;
        }

        private bool ServerInstance_CheckAddressBan(string addres, bool IsIP, ref string reason)
        {
            var results = IsIP ? BanDatabase.CheckAddressBan(addres) : BanDatabase.CheckHostBan(addres);
            reason = results.Reason;
            return results.Active;
        }

        private void ServerInstance_CheckPlayerAcceptance(object sender, Server.BooleanResultPlayerEventArgs e)
        {
            e.Result = true;
        }

        private void ServerInstance_APILoadComplete(object sender, EventArgs e)
        {
            BanDatabase.Init(Instance.ConfigData.Security);
        }
    }
}
