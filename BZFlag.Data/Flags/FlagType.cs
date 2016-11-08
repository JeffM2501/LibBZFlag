using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;
using BZFlag.Data.Players;

namespace BZFlag.Data.Flags
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

	public class FlagType
	{
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
