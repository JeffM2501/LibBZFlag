using System;
using System.Collections.Generic;

using BZFlag.Game.Host;
using BZFlag.Game.Host.API;
using BZFlag.Game.Host.World;

namespace BZFS.StandardRuleset
{
    public partial class Rules : PlugIn
    {
        public override string Name => "StandardRuleset";

        public override string Description => "Implements the BZFlag Standard Ruleset";

        public override void Shutdown(Server serverInstance)
        {
        }

        public override void Startup(Server serverInstance)
        {
            State.Players.ComputeScores = DoPlayerScore;
            State.Flags.OnPlayerHitWhileHoldingFlag = OnPlayerHitWhileHoldingFlag;
            State.Flags.BuildRandomFlags = BuildRandomFlags;
            State.Flags.FlagGone += Flags_FlagGone;

            serverInstance.GetBZDBDefaults = LoadBZDBDefaults;

            serverInstance.PlayerPostAdd += ServerInstance_PlayerPostAdd;
        }

        private void ServerInstance_PlayerPostAdd(object sender, BZFlag.Game.Host.Players.ServerPlayer e)
        {
            foreach (string line in Resources.PreReleaseWarning.Split("\r\n".ToCharArray()))
                State.Chat.SendChatToUser(null, e, line, false);

            e.FlushTCP();
        }
    }
}
