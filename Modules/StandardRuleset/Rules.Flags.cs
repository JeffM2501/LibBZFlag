using System;
using System.Collections.Generic;

using BZFlag.Data.Game;
using BZFlag.Data.Flags;
using BZFlag.Game.Host;
using BZFlag.Game.Host.Players;
using BZFlag.Game.Host.World;

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
                    State.Flags.DropFlag(victim);
                }
            }
        }

        protected List<FlagType> SpawnableFlags = new List<FlagType>();

        protected int DesiredFlagCount = 0;

        protected int GetRandomFlagCount()
        {
            int val = 0;
            if (State.ConfigData.Flags.RandomFlags.MinFlagCount == State.ConfigData.Flags.RandomFlags.MaxFlagCount)
                val = State.ConfigData.Flags.RandomFlags.MaxFlagCount;
            else
                val = State.World.RNG.Next(State.ConfigData.Flags.RandomFlags.MinFlagCount, State.ConfigData.Flags.RandomFlags.MaxFlagCount + 1);

            if (val < 0)
                val = 0;
            if (val > FlagManager.MaxFlagID)
                val = FlagManager.MaxFlagID;

            return val;
        }

        protected FlagType GetRandomFlag()
        {
            if (SpawnableFlags.Count == 0)
                return null;

            if (SpawnableFlags.Count == 1)
                return SpawnableFlags[0];

            return SpawnableFlags[State.World.RNG.Next(DesiredFlagCount)];
        }

        protected bool FlagValidForGameType(FlagType flag)
        {
            if (flag == FlagTypeList.Jumping && State.ConfigData.GameData.GameOptions.HasFlag(GameOptionFlags.JumpingGameStyle))
                return false;

            if (flag == FlagTypeList.Ricochet && State.ConfigData.GameData.GameOptions.HasFlag(GameOptionFlags.RicochetGameStyle))
                return false;

            if (State.ConfigData.GameData.MaxShots == 1 && (flag == FlagTypeList.MachineGun || flag == FlagTypeList.RapidFire))
                return false;

            if (flag == FlagTypeList.GuidedMissile || flag == FlagTypeList.Thief)
                return false; // for now these flags are unsupported

            return true;
        }

        private void Flags_FlagGone(object sender, FlagManager.FlagInstance e)
        {
            if (!State.ConfigData.Flags.SpawnRandomFlags || SpawnableFlags.Count < 2)
                State.Flags.AddFlag(e.Flag);    // just replace it somewhere
            else
            {
                int count = State.Flags.GetActiveFlags().Length;
                if (count < State.ConfigData.Flags.RandomFlags.MinFlagCount)
                {
                    DesiredFlagCount = GetRandomFlagCount();
                    for (int i = count; i < DesiredFlagCount; i++)
                    {
                        var ft = GetRandomFlag();
                        while (FlagValidForGameType(ft))
                            ft = GetRandomFlag();

                        State.Flags.AddFlag(ft);
                    }
                }
            }
        }

        protected void BuildRandomFlags(FlagManager manager, ServerConfig.FlagInfo flagInfo)
        {
            if (!flagInfo.SpawnRandomFlags)
                return;

            SpawnableFlags.Clear();

            if (flagInfo.RandomFlags.UseGoodFlags)
                SpawnableFlags.AddRange(FlagTypeList.GoodFlags);

            if (flagInfo.RandomFlags.UseBadFlags)
            {
                SpawnableFlags.AddRange(FlagTypeList.BadFlags);
                if (flagInfo.AllowGeno)
                    SpawnableFlags.Add(FlagTypeList.Genocide);
            }

            foreach (var f in flagInfo.RandomFlags.UseFlags)
            {
                FlagType t = FlagTypeList.GetFromAbriv(f);
                if (t != null && !SpawnableFlags.Contains(t))
                    SpawnableFlags.Add(t);
            }

            foreach (var f in flagInfo.RandomFlags.IgnoreFlags)
            {
                FlagType t = FlagTypeList.GetFromAbriv(f);
                if (t != null && SpawnableFlags.Contains(t))
                    SpawnableFlags.Remove(t);
            }

            DesiredFlagCount = 0;
            if (SpawnableFlags.Count == 0)
                return;

            // randomize the flag list
            int count = SpawnableFlags.Count;
            List<FlagType> randoFlags = new List<FlagType>();
            for (int i = 0; i < count; i++)
            {
                FlagType t = SpawnableFlags[State.World.RNG.Next(SpawnableFlags.Count)];
                SpawnableFlags.Remove(t);
                randoFlags.Add(t);
            }
            SpawnableFlags = randoFlags;


            // see how many random flags we need to spawn this time
            DesiredFlagCount = GetRandomFlagCount();

            if (DesiredFlagCount > 0)
                Logger.Log2("Spawning " + DesiredFlagCount.ToString() + " flags from pool " + String.Join<FlagType>(" ", SpawnableFlags.ToArray()));

            for (int i = 0; i < DesiredFlagCount; i++)
            {
                FlagType ft = null;
                if (DesiredFlagCount < SpawnableFlags.Count)
                    ft = SpawnableFlags[i];    // no dupes
                else
                    ft = GetRandomFlag();

                if (FlagValidForGameType(ft))
                    manager.InitFlag(ft);
            }
               
        }
    }
}
