using System;
using System.Collections.Generic;

using BZFlag.Data.Flags;
using BZFlag.Game.Host.Players;

namespace BZFS.StandardRuleset
{
    public partial class Rules
    {
        protected virtual void OnPlayerHitWhileHoldingFlag(ServerPlayer victim, ServerPlayer assilant, ShotManager.ShotInfo shot)
        {
            if (shot.SourceFlag == FlagTypeList.Shield)
            {
                if (victim.Info.ShotImmunities > 0)
                    victim.Info.ShotImmunities--;
        
                if (victim.Info.ShotImmunities == 0)
                {
                    // drop flag?
                }
            }
        }
    }
}
