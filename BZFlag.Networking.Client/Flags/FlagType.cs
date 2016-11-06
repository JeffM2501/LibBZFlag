using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Flags
{
	public enum TeamColors
	{
		AutomaticTeam = -2,
		NoTeam = -1,
		RogueTeam = 0,
		RedTeam = 1,
		GreenTeam = 2,
		BlueTeam = 3,
		PurpleTeam = 4,
		ObserverTeam = 5,
		RabbitTeam = 6,
		HunterTeam = 7
	};

	public static class FlagCache
	{
		public static Dictionary<string, FlagType> FlagList = new Dictionary<string, FlagType>();

		static void Add(FlagType f)
		{
			FlagList.Add(f.FlagAbbv, f);
		}

		static FlagCache()
		{
			Add(new FlagType("", "", FlagType.FlagEndurances.FlagNormal, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam, ""));

			Add(new FlagType("Red Team", "R*", FlagType.FlagEndurances.FlagNormal, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.RedTeam,
							"If it's yours, prevent other teams from taking it.  If it's not take it to your base to capture it!"));
			Add(new FlagType("Green Team", "G*", FlagType.FlagEndurances.FlagNormal, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.GreenTeam,
							"If it's yours, prevent other teams from taking it.  If it's not take it to your base to capture it!"));
			Add(new FlagType("Blue Team", "B*", FlagType.FlagEndurances.FlagNormal, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.BlueTeam,
							"If it's yours, prevent other teams from taking it.  If it's not take it to your base to capture it!"));
			Add(new FlagType("Purple Team", "P*", FlagType.FlagEndurances.FlagNormal, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.PurpleTeam,
							"If it's yours, prevent other teams from taking it.  If it's not take it to your base to capture it!"));
			Add(new FlagType("High Speed", "V", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Tank moves faster.  Outrun bad guys."));
			Add(new FlagType("Quick Turn", "QT", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Tank turns faster.  Good for dodging."));
			Add(new FlagType("Oscillation Overthruster", "OO", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Can drive through buildings.  Can't backup or shoot while inside."));
			Add(new FlagType("Rapid Fire", "F", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.SpecialShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Shoots more often.  Shells go faster but not as far."));
			Add(new FlagType("Machine Gun", "MG", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.SpecialShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Very fast reload and very short range."));
			Add(new FlagType("Guided Missile", "GM", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.SpecialShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Shots track a target.  Lock on with right button.  Can lock on or retarget after firing."));
			Add(new FlagType("Laser", "L", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.SpecialShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Shoots a laser.  Infinite speed and range but long reload time."));
			Add(new FlagType("Ricochet", "R", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.SpecialShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Shots bounce off walls.  Don't shoot yourself!"));
			Add(new FlagType("Super Bullet", "SB", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.SpecialShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Shoots through buildings.  Can kill Phantom Zone."));
			Add(new FlagType("Invisible Bullet", "IB", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Your shots don't appear on other radars.  Can still see them out window."));
			Add(new FlagType("Stealth", "ST", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Tank is invisible on radar.  Shots are still visible.  Sneak up behind enemies!"));
			Add(new FlagType("Tiny", "T", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Tank is small and can get through small openings.  Very hard to hit."));
			Add(new FlagType("Narrow", "N", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Tank is super thin.  Very hard to hit from front but is normal size from side.  Can get through small openings."));
			Add(new FlagType("Shield", "SH", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Getting hit only drops flag.  Flag flies an extra-long time."));
			Add(new FlagType("Steamroller", "SR", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Destroys tanks you touch but you have to get really close."));
			Add(new FlagType("Shock Wave", "SW", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.SpecialShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Firing destroys all tanks nearby.  Don't kill teammates!  Can kill tanks on/in buildings."));
			Add(new FlagType("Phantom Zone", "PZ", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.SpecialShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Teleporting toggles Zoned effect.  Zoned tank can drive through buildings.  Zoned tank shoots Zoned bullets and can't be shot (except by superbullet, shock wave, and other Zoned tanks)."));
			Add(new FlagType("Genocide", "G", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Killing one tank kills that tank's whole team."));
			Add(new FlagType("Jumping", "JP", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Tank can jump.  Use Tab key.  Can't steer in the air."));
			Add(new FlagType("Identify", "ID", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Identifies type of nearest flag."));
			Add(new FlagType("Cloaking", "CL", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Makes your tank invisible out-the-window.  Still visible on radar."));
			Add(new FlagType("Useless", "US", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"You have found the useless flag. Use it wisely."));
			Add(new FlagType("Masquerade", "MQ", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"In opponent's hud, you appear as a teammate."));
			Add(new FlagType("Seer", "SE", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"See stealthed, cloaked and masquerading tanks as normal."));
			Add(new FlagType("Thief", "TH", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.SpecialShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Steal flags.  Small and fast but can't kill."));
			Add(new FlagType("Burrow", "BU", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Tank burrows underground, impervious to normal shots, but can be steamrolled by anyone!"));
			Add(new FlagType("Wings", "WG", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Tank can drive in air."));
			Add(new FlagType("Agility", "A", FlagType.FlagEndurances.FlagUnstable, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagGood, TeamColors.NoTeam,
							"Tank is quick and nimble making it easier to dodge."));
			Add(new FlagType("ReverseControls", "RC", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Tank driving controls are reversed."));
			Add(new FlagType("Colorblindness", "CB", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Can't tell team colors.  Don't shoot teammates!"));
			Add(new FlagType("Obesity", "O", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Tank becomes very large.  Can't fit through teleporters."));
			Add(new FlagType("Left Turn Only", "LT", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Can't turn right."));
			Add(new FlagType("Right Turn Only", "RT", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Can't turn left."));
			Add(new FlagType("Forward Only", "FO", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Can't drive in reverse."));
			Add(new FlagType("ReverseOnly", "RO", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Can't drive forward."));
			Add(new FlagType("Momentum", "M", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Tank has inertia.  Acceleration is limited."));
			Add(new FlagType("Blindness", "B", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Can't see out window.  Radar still works."));
			Add(new FlagType("Jamming", "JM", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Radar doesn't work.  Can still see."));
			Add(new FlagType("Wide Angle", "WA", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Fish-eye lens distorts view."));
			Add(new FlagType("No Jumping", "NJ", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Tank can't jump."));
			Add(new FlagType("Trigger Happy", "TR", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Tank can't stop firing."));
			Add(new FlagType("Bouncy", "BY", FlagType.FlagEndurances.FlagSticky, FlagType.ShotTypes.NormalShot, FlagType.FlagQualities.FlagBad, TeamColors.NoTeam,
							"Tank can't stop bouncing."));
		}

	}
	public class FlagType
	{
		public enum FlagStatuses
		{
			/// the flag is not present in the world
			FlagNoExist = 0,
			/// the flag is sitting on the ground and can be picked up
			FlagOnGround,
			/// the flag is being carried by a tank
			FlagOnTank,
			/// the flag is falling through the air
			FlagInAir,
			/// the flag is entering the world
			FlagComing,
			/// the flag is leaving the world
			FlagGoing
		};
		public enum FlagEndurances
		{
			/// permanent flag
			FlagNormal = 0,
			/// disappears after use
			FlagUnstable = 1,
			/// can't be dropped normally
			FlagSticky = 2
		};

		public enum FlagQualities
		{
			FlagGood = 0,
			FlagBad = 1,
			NumQualities
		};

		public enum ShotTypes
		{
			NormalShot = 0,
			SpecialShot = 1
		};

		public string FlagName = string.Empty;
		public string FlagAbbv = string.Empty;
		public string FlagHelp = string.Empty;
		public FlagEndurances FlagEndurance = FlagEndurances.FlagNormal;
		public FlagQualities FlagQuality = FlagQualities.FlagGood;
		public ShotTypes FlagShot = ShotTypes.NormalShot;
		public TeamColors FlagTeam = TeamColors.AutomaticTeam;
		public bool Custom = false;


		public FlagType(string name, string abbv, FlagEndurances _endurance, ShotTypes sType, FlagQualities quality, TeamColors team, string help, bool _custom )
		{
			FlagName = name;
			FlagAbbv = abbv;
			FlagEndurance = _endurance;
			FlagShot = sType;
			FlagQuality = quality;
			FlagTeam = team;
			FlagHelp = help;
			Custom = _custom;
		}

		public FlagType(string name, string abbv, FlagEndurances _endurance, ShotTypes sType, FlagQualities quality, TeamColors team, string help)
		{
			FlagName = name;
			FlagAbbv = abbv;
			FlagEndurance = _endurance;
			FlagShot = sType;
			FlagQuality = quality;
			FlagTeam = team;
			FlagHelp = help;
			Custom = false;
		}
	}
}
