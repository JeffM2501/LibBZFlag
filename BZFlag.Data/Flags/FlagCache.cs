using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Players;
using BZFlag.Data.Teams;

namespace BZFlag.Data.Flags
{
    public static class FlagTypeList
    {
        private static Dictionary<string, FlagType> FlagList = new Dictionary<string, FlagType>();
        private static List<FlagType> GoodFlagList = new List<FlagType>();
        private static List<FlagType> BadFlagList = new List<FlagType>();

        public static FlagType[] Flags {  get { return FlagList.Values.ToArray(); } }

        public static string[] Names { get { return FlagList.Keys.ToArray(); } }

        public static FlagType None { get { return FlagTypeList.Flags[0]; } }

        public static FlagType Shield { get; private set; } = null;
        public static FlagType GuidedMissile { get; private set; } = null;
        public static FlagType Thief { get; private set; } = null;
        public static FlagType Jumping { get; private set; } = null;
        public static FlagType Ricochet { get; private set; } = null;
        public static FlagType RapidFire { get; private set; } = null;
        public static FlagType MachineGun { get; private set; } = null;

        public static FlagType Genocide { get; private set; } = null;

        public static FlagType[] GoodFlags { get { return GoodFlagList.ToArray(); } }
        public static FlagType[] BadFlags { get { return BadFlagList.ToArray(); } }


        static FlagTypeList()
        {
            Add(new FlagType(FlagStrings.NullFlagName,string.Empty, FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagUnknown, TeamColors.NoTeam, FlagStrings.NullFlagDescription));

            // teams
            Add(new FlagType(FlagStrings.RedTeamName, "R*", FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.RedTeam, FlagStrings.RedTeamDescription));
            Add(new FlagType(FlagStrings.GreenTeamName, "G*", FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.GreenTeam, FlagStrings.GreenTeamDescription));
            Add(new FlagType(FlagStrings.BlueTeamName, "B*", FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.BlueTeam, FlagStrings.BlueTeamDescription));
            Add(new FlagType(FlagStrings.PurpleTeamName, "P*", FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.PurpleTeam, FlagStrings.PurpleTeamDescription));

            // good flags
            Add(new FlagType(FlagStrings.HighSpeedName, "V", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.HighSpeedDescription));
            Add(new FlagType(FlagStrings.QuickTurnName, "QT", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.QuickTurnDescription));
            Add(new FlagType(FlagStrings.OOName, "OO", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,FlagStrings.OODescription));
            RapidFire = Add(new FlagType(FlagStrings.RapidFireName, "F", FlagEndurances.FlagUnstable, ShotTypes.SpecialShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.RapidFireDescription));
            MachineGun = Add(new FlagType(FlagStrings.MachineGunName, "MG", FlagEndurances.FlagUnstable, ShotTypes.SpecialShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.MachineGunDescription));
            GuidedMissile = Add(new FlagType(FlagStrings.GMName, "GM", FlagEndurances.FlagUnstable, ShotTypes.GuidedShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.GMDescription));
            Add(new FlagType(FlagStrings.LaserName, "L", FlagEndurances.FlagUnstable, ShotTypes.Laser, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.LaserDescription));
            Ricochet = Add(new FlagType(FlagStrings.RiccoName, "R", FlagEndurances.FlagUnstable, ShotTypes.RicochetShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.RiccoDescription));
            Add(new FlagType(FlagStrings.SBName, "SB", FlagEndurances.FlagUnstable, ShotTypes.SuperShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.SBDescription));
            Add(new FlagType(FlagStrings.IBName, "IB", FlagEndurances.FlagUnstable, ShotTypes.InvisibleShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.IBDescription));
            Add(new FlagType(FlagStrings.StealthName, "ST", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.StealthDescription));
            Add(new FlagType(FlagStrings.TinyName, "T", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.TinyDescription));
            Add(new FlagType(FlagStrings.NarrowName, "N", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.NarrowDescription));
            Shield = Add(new FlagType(FlagStrings.ShieldName, "SH", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.ShieldDescription));
            Add(new FlagType(FlagStrings.SteamrollerName, "SR", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.SteamrollerDescription));
            Add(new FlagType(FlagStrings.ShockWaveName, "SW", FlagEndurances.FlagUnstable, ShotTypes.Shockwave, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.ShieldDescription));
            Add(new FlagType(FlagStrings.PZName, "PZ", FlagEndurances.FlagUnstable, ShotTypes.PhantomShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.PZDescription));
            Jumping = Add(new FlagType(FlagStrings.JumpingName, "JP", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.JumpingDescription));
            Add(new FlagType(FlagStrings.IdentifyName, "ID", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.IdentifyDescription));
            Add(new FlagType(FlagStrings.CloakingName, "CL", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.CloakingDescription));
            Add(new FlagType(FlagStrings.UselessName, "US", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.UselessDescription));
            Add(new FlagType(FlagStrings.MasqueradeName, "MQ", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.MasqueradeDescription));
            Add(new FlagType(FlagStrings.SeerName, "SE", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.SeerDescription));
            Thief = Add(new FlagType(FlagStrings.ThiefName, "TH", FlagEndurances.FlagUnstable, ShotTypes.ThiefShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.ThiefDescription));
            Add(new FlagType(FlagStrings.BurrowName, "BU", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.BurrowDescription));
            Add(new FlagType(FlagStrings.WingsName, "WG", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.WingsDescription));
            Add(new FlagType(FlagStrings.AgilityName, "A", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.AgilityDescription));

            // stupid flags
            Genocide = Add(new FlagType(FlagStrings.GenoName, "G", FlagEndurances.FlagUnstable, ShotTypes.GenocideShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.GenoDescription));

            // bad flags
            Add(new FlagType(FlagStrings.RCName, "RC", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam, FlagStrings.RCDescription));
            Add(new FlagType(FlagStrings.CBName, "CB", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam, FlagStrings.CBDescription));
            Add(new FlagType(FlagStrings.ObesityName, "O", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam, FlagStrings.ObesityDescription));
            Add(new FlagType(FlagStrings.LTName, "LT", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam, FlagStrings.LTDescription));
            Add(new FlagType(FlagStrings.RTName, "RT", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam, FlagStrings.RTDescription));
            Add(new FlagType(FlagStrings.FOName, "FO", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam, FlagStrings.FODescription));
            Add(new FlagType(FlagStrings.ROName, "RO", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam, FlagStrings.RODescription));
            Add(new FlagType(FlagStrings.MomentumName, "M", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,  FlagStrings.MomentumDescription));
            Add(new FlagType(FlagStrings.BlindnessName, "B", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam, FlagStrings.BlindnessDescription));
            Add(new FlagType(FlagStrings.JammingName, "JM", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam, FlagStrings.JammingDescription));
            Add(new FlagType(FlagStrings.WAName, "WA", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam, FlagStrings.WADescription));
            Add(new FlagType(FlagStrings.NoJumpName, "NJ", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam, FlagStrings.NoJumpDescription));
            Add(new FlagType(FlagStrings.TRName, "TR", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,  FlagStrings.TRDescription));
            Add(new FlagType(FlagStrings.BouncyName, "BY", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,  FlagStrings.BouncyDescription));
        }

        public static FlagType Add(FlagType f)
        {
            if (f.FlagQuality == FlagQualities.FlagGood && f.FlagTeam == TeamColors.NoTeam && f.FlagShot != ShotTypes.GenocideShot)
                GoodFlagList.Add(f);
            else if (f.FlagQuality == FlagQualities.FlagBad)
                BadFlagList.Add(f);

            FlagList.Add(f.FlagAbbv, f);

            return f;
        }

        public static FlagType GetFromAbriv(string abriviation)
        {
            if (!FlagList.ContainsKey(abriviation))
                return null;

            return FlagList[abriviation];
        }
    }
}
