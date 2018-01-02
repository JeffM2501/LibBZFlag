using BZFlag.Data.BZDB;
using BZFlag.Data.Players;
using BZFlag.Game.Host;
using BZFlag.Game.Host.API;
using BZFlag.Game.Host.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
