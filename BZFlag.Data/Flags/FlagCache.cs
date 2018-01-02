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

        public static FlagType[] Flags {  get { return FlagList.Values.ToArray(); } }

        public static string[] Names { get { return FlagList.Keys.ToArray(); } }

        public static FlagType None { get { return FlagTypeList.Flags[0]; } }

        public static FlagType Shield { get; private set; } = null;
        public static FlagType GuidedMissile { get; private set; } = null;
        public static FlagType Thief { get; private set; } = null;

        public static FlagType Genocide { get; private set; } = null;


        static FlagTypeList()
        {
            Add(new FlagType(FlagStrings.NullFlagName,string.Empty, FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.NullFlagDescription));

            // teams
            Add(new FlagType(FlagStrings.RedTeamName, "R*", FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.RedTeam, FlagStrings.RedTeamDescription));
            Add(new FlagType(FlagStrings.GreenTeamName, "G*", FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.GreenTeam, FlagStrings.GreenTeamDescription));
            Add(new FlagType(FlagStrings.BlueTeamName, "B*", FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.BlueTeam, FlagStrings.BlueTeamDescription));
            Add(new FlagType(FlagStrings.PurpleTeamName, "P*", FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.PurpleTeam, FlagStrings.PurpleTeamDescription));

            // good flags
            Add(new FlagType(FlagStrings.HighSpeedName, "V", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.HighSpeedDescription));
            Add(new FlagType(FlagStrings.QuickTurnName, "QT", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, FlagStrings.QuickTurnDescription));
            Add(new FlagType(FlagStrings.OOName, "OO", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,FlagStrings.OODescription));

            Add(new FlagType(FlagStrings.RapidFireName, "F", FlagEndurances.FlagUnstable, ShotTypes.SpecialShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Shoots more often.  Shells go faster but not as far."));
            Add(new FlagType(FlagStrings.MachineGunName, "MG", FlagEndurances.FlagUnstable, ShotTypes.SpecialShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Very fast reload and very short range."));

            GuidedMissile = new FlagType(FlagStrings.GMName, "GM", FlagEndurances.FlagUnstable, ShotTypes.GuidedShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Shots track a target.  Lock on with right button.  Can lock on or retarget after firing.");
            Add(GuidedMissile);

            Add(new FlagType(FlagStrings.LaserName, "L", FlagEndurances.FlagUnstable, ShotTypes.Laser, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Shoots a laser.  Infinite speed and range but long reload time."));
            Add(new FlagType(FlagStrings.RiccoName, "R", FlagEndurances.FlagUnstable, ShotTypes.RicochetShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Shots bounce off walls.  Don't shoot yourself!"));
            Add(new FlagType(FlagStrings.SBName, "SB", FlagEndurances.FlagUnstable, ShotTypes.SuperShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Shoots through buildings.  Can kill Phantom Zone."));
            Add(new FlagType(FlagStrings.IBName, "IB", FlagEndurances.FlagUnstable, ShotTypes.InvisibleShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Your shots don't appear on other radars.  Can still see them out window."));
            Add(new FlagType(FlagStrings.StealthName, "ST", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Tank is invisible on radar.  Shots are still visible.  Sneak up behind enemies!"));
            Add(new FlagType(FlagStrings.TinyName, "T", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Tank is small and can get through small openings.  Very hard to hit."));
            Add(new FlagType(FlagStrings.NarrowName, "N", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Tank is super thin.  Very hard to hit from front but is normal size from side.  Can get through small openings."));

            Shield = new FlagType(FlagStrings.ShieldName, "SH", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Getting hit only drops flag.  Flag flies an extra-long time.");
            Add(Shield);

            Add(new FlagType(FlagStrings.SteamrollerName, "SR", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Destroys tanks you touch but you have to get really close."));
            Add(new FlagType(FlagStrings.ShockWaveName, "SW", FlagEndurances.FlagUnstable, ShotTypes.Shockwave, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Firing destroys all tanks nearby.  Don't kill teammates!  Can kill tanks on/in buildings."));
            Add(new FlagType(FlagStrings.PZName, "PZ", FlagEndurances.FlagUnstable, ShotTypes.PhantomShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Teleporting toggles Zoned effect.  Zoned tank can drive through buildings.  Zoned tank shoots Zoned bullets and can't be shot (except by superbullet, shock wave, and other Zoned tanks)."));


            Genocide = new FlagType(FlagStrings.GenoName, "G", FlagEndurances.FlagUnstable, ShotTypes.GenocideShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Killing one tank kills that tank's whole team.");
            Add(Genocide);

            Add(new FlagType("Jumping", "JP", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Tank can jump.  Use Tab key.  Can't steer in the air."));
            Add(new FlagType("Identify", "ID", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Identifies type of nearest flag."));
            Add(new FlagType("Cloaking", "CL", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Makes your tank invisible out-the-window.  Still visible on radar."));
            Add(new FlagType("Useless", "US", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "You have found the useless flag. Use it wisely."));
            Add(new FlagType("Masquerade", "MQ", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "In opponent's hud, you appear as a teammate."));
            Add(new FlagType("Seer", "SE", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "See stealthed, cloaked and masquerading tanks as normal."));

            Thief = new FlagType("Thief", "TH", FlagEndurances.FlagUnstable, ShotTypes.ThiefShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Steal flags.  Small and fast but can't kill.");
            Add(Thief);

            Add(new FlagType("Burrow", "BU", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Tank burrows underground, impervious to normal shots, but can be steamrolled by anyone!"));
            Add(new FlagType("Wings", "WG", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Tank can drive in air."));
            Add(new FlagType("Agility", "A", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Tank is quick and nimble making it easier to dodge."));



            Add(new FlagType("ReverseControls", "RC", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Tank driving controls are reversed."));
            Add(new FlagType("Colorblindness", "CB", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Can't tell team colors.  Don't shoot teammates!"));
            Add(new FlagType("Obesity", "O", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Tank becomes very large.  Can't fit through teleporters."));
            Add(new FlagType("Left Turn Only", "LT", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Can't turn right."));
            Add(new FlagType("Right Turn Only", "RT", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Can't turn left."));
            Add(new FlagType("Forward Only", "FO", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Can't drive in reverse."));
            Add(new FlagType("ReverseOnly", "RO", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Can't drive forward."));
            Add(new FlagType("Momentum", "M", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Tank has inertia.  Acceleration is limited."));
            Add(new FlagType("Blindness", "B", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Can't see out window.  Radar still works."));
            Add(new FlagType("Jamming", "JM", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Radar doesn't work.  Can still see."));
            Add(new FlagType("Wide Angle", "WA", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Fish-eye lens distorts view."));
            Add(new FlagType("No Jumping", "NJ", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Tank can't jump."));
            Add(new FlagType("Trigger Happy", "TR", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Tank can't stop firing."));
            Add(new FlagType("Bouncy", "BY", FlagEndurances.FlagSticky, ShotTypes.NormalShot, FlagQualities.FlagBad, TeamColors.NoTeam,
                            "Tank can't stop bouncing."));
        }

        public static void Add(FlagType f)
        {
            FlagList.Add(f.FlagAbbv, f);
        }

        public static FlagType GetFromAbriv(string abriviation)
        {
            if (!FlagList.ContainsKey(abriviation))
                return null;

            return FlagList[abriviation];
        }
    }
}
