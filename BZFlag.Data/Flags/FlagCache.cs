using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Data.Flags
{
    public static class FlagCache
    {
        public static Dictionary<string, FlagType> FlagList = new Dictionary<string, FlagType>();

        static void Add(FlagType f)
        {
            FlagList.Add(f.FlagAbbv, f);
        }

        static FlagCache()
        {
            Add(new FlagType("", "", FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam, ""));

            Add(new FlagType("Red Team", "R*", FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.RedTeam,
                            "If it's yours, prevent other teams from taking it.  If it's not take it to your base to capture it!"));
            Add(new FlagType("Green Team", "G*", FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.GreenTeam,
                            "If it's yours, prevent other teams from taking it.  If it's not take it to your base to capture it!"));
            Add(new FlagType("Blue Team", "B*", FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.BlueTeam,
                            "If it's yours, prevent other teams from taking it.  If it's not take it to your base to capture it!"));
            Add(new FlagType("Purple Team", "P*", FlagEndurances.FlagNormal, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.PurpleTeam,
                            "If it's yours, prevent other teams from taking it.  If it's not take it to your base to capture it!"));
            Add(new FlagType("High Speed", "V", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Tank moves faster.  Outrun bad guys."));
            Add(new FlagType("Quick Turn", "QT", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Tank turns faster.  Good for dodging."));
            Add(new FlagType("Oscillation Overthruster", "OO", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Can drive through buildings.  Can't backup or shoot while inside."));
            Add(new FlagType("Rapid Fire", "F", FlagEndurances.FlagUnstable, ShotTypes.SpecialShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Shoots more often.  Shells go faster but not as far."));
            Add(new FlagType("Machine Gun", "MG", FlagEndurances.FlagUnstable, ShotTypes.SpecialShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Very fast reload and very short range."));
            Add(new FlagType("Guided Missile", "GM", FlagEndurances.FlagUnstable, ShotTypes.SpecialShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Shots track a target.  Lock on with right button.  Can lock on or retarget after firing."));
            Add(new FlagType("Laser", "L", FlagEndurances.FlagUnstable, ShotTypes.SpecialShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Shoots a laser.  Infinite speed and range but long reload time."));
            Add(new FlagType("Ricochet", "R", FlagEndurances.FlagUnstable, ShotTypes.SpecialShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Shots bounce off walls.  Don't shoot yourself!"));
            Add(new FlagType("Super Bullet", "SB", FlagEndurances.FlagUnstable, ShotTypes.SpecialShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Shoots through buildings.  Can kill Phantom Zone."));
            Add(new FlagType("Invisible Bullet", "IB", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Your shots don't appear on other radars.  Can still see them out window."));
            Add(new FlagType("Stealth", "ST", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Tank is invisible on radar.  Shots are still visible.  Sneak up behind enemies!"));
            Add(new FlagType("Tiny", "T", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Tank is small and can get through small openings.  Very hard to hit."));
            Add(new FlagType("Narrow", "N", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Tank is super thin.  Very hard to hit from front but is normal size from side.  Can get through small openings."));
            Add(new FlagType("Shield", "SH", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Getting hit only drops flag.  Flag flies an extra-long time."));
            Add(new FlagType("Steamroller", "SR", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Destroys tanks you touch but you have to get really close."));
            Add(new FlagType("Shock Wave", "SW", FlagEndurances.FlagUnstable, ShotTypes.SpecialShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Firing destroys all tanks nearby.  Don't kill teammates!  Can kill tanks on/in buildings."));
            Add(new FlagType("Phantom Zone", "PZ", FlagEndurances.FlagUnstable, ShotTypes.SpecialShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Teleporting toggles Zoned effect.  Zoned tank can drive through buildings.  Zoned tank shoots Zoned bullets and can't be shot (except by superbullet, shock wave, and other Zoned tanks)."));
            Add(new FlagType("Genocide", "G", FlagEndurances.FlagUnstable, ShotTypes.NormalShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Killing one tank kills that tank's whole team."));
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
            Add(new FlagType("Thief", "TH", FlagEndurances.FlagUnstable, ShotTypes.SpecialShot, FlagQualities.FlagGood, TeamColors.NoTeam,
                            "Steal flags.  Small and fast but can't kill."));
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
    }
}
