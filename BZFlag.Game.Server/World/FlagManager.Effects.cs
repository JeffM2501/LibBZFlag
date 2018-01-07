using BZFlag.Data.Flags;
using BZFlag.Game.Host.Players;
using BZFlag.LinearMath;
using BZFlag.Networking.Messages.BZFS.Flags;
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

        public class FlagCheckData
        {
            public static string TagName = "FlagMaager.FlagCheckData";

            public FlagInstance LastIdentifiedFlag = null;
            public double LastIdentifySendTime = double.MinValue;
        }

        public static double IdentFlagUpdateTime = 0.1;

        public void DoPlayerFlagChecks(ServerPlayer player)
        {
            if (player == null || player.Info.CariedFlag == null)
                return;

            if (player.Info.CariedFlag.Flag == FlagTypeList.Identify)
            {
                FlagCheckData data = player.GetTag<FlagCheckData>(FlagCheckData.TagName);
                if (data == null)
                    return;

                if (data.LastIdentifySendTime + IdentFlagUpdateTime < GameTime.Now || data.LastIdentifiedFlag == null)
                {
                    data.LastIdentifySendTime = GameTime.Now;

                    var flag = GetNearestFlag(player.Info.LastSentUpdate.Position);

                    if (flag != null && data.LastIdentifiedFlag != flag)
                    {
                        MsgNearFlag nf = new MsgNearFlag();
                        nf.FlagName = flag.Flag.FlagAbbv;
                        nf.Position = flag.Position;
                        player.SendMessage(nf);

                        data.LastIdentifiedFlag = flag;
                    }

                    data.LastIdentifySendTime = GameTime.Now;
                }
            }
        }

        public FlagInstance GetNearestFlag(Vector3F postion)
        {
            var flags = GetGroundFlags();
            if (flags.Length == 0)
                return null;

            FlagInstance nearest = null;
            float dist = float.MaxValue;
            foreach (var f in flags)
            {
                float d = Vector3F.DistanceSquared(postion, f.Position);
                if (d < dist)
                {
                    dist = d;
                    nearest = f;
                }
            }

            return nearest;
        }
    }
}
