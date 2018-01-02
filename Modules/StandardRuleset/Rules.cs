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

        public void Startup(Server serverInstance)
        {
            serverInstance.State.Players.ComputeScores = DoPlayerScore;
            serverInstance.State.Flags.OnPlayerHitWhileHoldingFlag = OnPlayerHitWhileHoldingFlag;

            serverInstance.GetBZDBDefaults = LoadBZDBDefaults;
        }
    }
}
