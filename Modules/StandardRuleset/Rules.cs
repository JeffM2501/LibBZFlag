using System;
using System.Collections.Generic;

using BZFlag.Game.Host;
using BZFlag.Game.Host.API;

namespace BZFS.StandardRuleset
{
    public partial class Rules : PlugIn
    {
        public string Name => "StandardRuleset";

        public string Description => "Implements the BZFlag Standard Ruleset";

        public void Shutdown(Server serverInstance)
        {
        }

        protected Server Instance = null;

        public void Startup(Server serverInstance)
        {
            Instance = serverInstance;

            serverInstance.State.Players.ComputeScores = DoPlayerScore;
            serverInstance.State.Flags.OnPlayerHitWhileHoldingFlag = OnPlayerHitWhileHoldingFlag;

            serverInstance.GetBZDBDefaults = LoadBZDBDefaults;

            serverInstance.PlayerPostAdd += ServerInstance_PlayerPostAdd;
        }

        private void ServerInstance_PlayerPostAdd(object sender, BZFlag.Game.Host.Players.ServerPlayer e)
        {
            foreach (string line in Resources.PreReleaseWarning.Split("\r\n".ToCharArray()))
                Instance.State.Chat.SendChatToUser(null, e, line, false);

            e.FlushTCP();
        }
    }
}
