
using System;
using System.Collections.Generic;

using BZFlag.Data.Flags;
using BZFlag.Data.Players;
using BZFlag.Game.Host;
using BZFlag.Game.Host.Players;

using static BZFlag.Game.Host.Players.PlayerManager.PlayerInfo;

namespace BZFS.StandardRuleset
{
    public partial class Rules
    {
        protected virtual bool DoPlayerScore(ServerPlayer victim, ref ScoreInfo victimScoreDelta, ServerPlayer killer, ref ScoreInfo killerScoreDelta, BlowedUpReasons eventReason)
        {
            if (victim.ActualTeam == killer.ActualTeam && killer.ActualTeam != BZFlag.Data.Teams.TeamColors.RogueTeam)
                killerScoreDelta.TeamKills = 1;

            victimScoreDelta.Losses = 1;

            if (eventReason == BlowedUpReasons.GotShot || eventReason == BlowedUpReasons.GotRunOver)
            {
                if (killer.Info.CariedFlag != null && killer.Info.CariedFlag.Flag == FlagTypeList.Genocide && victim.ActualTeam != BZFlag.Data.Teams.TeamColors.RogueTeam)
                    killerScoreDelta.Wins = victim.Info.Team.Members.FindAll((x)=>x.Info.Alive).Count;
                else
                    killerScoreDelta.Wins = 1;
            }

            return !victimScoreDelta.Empty && !killerScoreDelta.Empty;
        }
    }
}
