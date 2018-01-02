using BZFlag.Data.Flags;
using BZFlag.Game.Host.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZFlag.Game.Host.World
{
    public partial class FlagManager
    {
        public class FlagHitEventArgs : EventArgs
        {
            public FlagType FlagInvolved = null;
            public ServerPlayer FlagHolder = null;

            public ServerPlayer OtherPlayer = null;
            public FlagType OtherPlayerFlag = null;
        }

        public event EventHandler<FlagHitEventArgs> FlagKilledPlayer = null;
        public event EventHandler<FlagHitEventArgs> PlayerHitWhileHoldingFlag = null;

        public delegate void PlayerHitWhileHoldingFlagCallback(ServerPlayer victim, ServerPlayer assilant, ShotManager.ShotInfo shot);
        public PlayerHitWhileHoldingFlagCallback OnPlayerHitWhileHoldingFlag= null;

        public delegate void FlagKilledPlayerCallback(ServerPlayer victim, ServerPlayer assaliant, FlagType damageFlagType);
        public FlagKilledPlayerCallback OnFlagKilledPlayer = null;


        public void HandlePlayerTakeHit(ServerPlayer victim, ServerPlayer assilant, ShotManager.ShotInfo shot)
        {
            OnPlayerHitWhileHoldingFlag?.Invoke(victim, assilant, shot);

            FlagHitEventArgs args = new FlagHitEventArgs();
            args.FlagInvolved = victim.Info.CariedFlag?.Flag;
            args.FlagHolder = victim;
            args.OtherPlayer = assilant;
            args.OtherPlayerFlag = shot.SourceFlag;

           
            PlayerHitWhileHoldingFlag?.Invoke(this, args);
        }

        public void HandlePlayerDoDamage(ServerPlayer victim, ServerPlayer assaliant, FlagType damageFlagType)
        {
            OnFlagKilledPlayer?.Invoke(victim, assaliant, damageFlagType);

            FlagHitEventArgs args = new FlagHitEventArgs();
            args.FlagInvolved = damageFlagType;
            args.FlagHolder = assaliant;
            args.OtherPlayer = victim;
            args.OtherPlayerFlag = victim.Info.CariedFlag?.Flag;

            FlagKilledPlayer?.Invoke(this, args);
        }
    }
}
