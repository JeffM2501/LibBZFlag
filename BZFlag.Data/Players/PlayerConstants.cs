using BZFlag.Data.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Data.Players
{
    public static class PlayerConstants
    {
        public static readonly int ServerPlayerID = 253;
        public static readonly int AllPlayersID = 254;

        public static readonly int InvalidPlayerID = 255;

        public static readonly int FirstTeamID = 244;
        public static readonly int LastTeamID = 251;

        public static readonly int RogueTeamID = 251;
        public static readonly int RedTeamID = 250;
        public static readonly int GreenTeamID = 249;
        public static readonly int BlueTeamID = 248;
        public static readonly int PurpleTeamID = 247;
        public static readonly int ObserverTeamID = 246;
        public static readonly int RabbitTeamID = 245;
        public static readonly int HunterTeamID = 244;

        public static readonly int AdminGroup = 252;

        public static readonly int MaxUseablePlayerID = 199;
        public static readonly int MinimumPlayerID = 0;

        public static int GetTeamPlayerID(TeamColors team)
        {
            switch (team)
            {
                case TeamColors.RogueTeam:
                    return RogueTeamID;

                case TeamColors.RedTeam:
                    return RedTeamID;

                case TeamColors.GreenTeam:
                    return GreenTeamID;

                case TeamColors.BlueTeam:
                    return BlueTeamID;

                case TeamColors.PurpleTeam:
                    return PurpleTeamID;

                case TeamColors.ObserverTeam:
                    return ObserverTeamID;

                case TeamColors.RabbitTeam:
                    return RabbitTeamID;

                case TeamColors.HunterTeam:
                    return HunterTeamID;
            }
            return InvalidPlayerID;
        }


        public static TeamColors GetTeamColorFromID(int team)
        {
            if (team == RogueTeamID)
                return TeamColors.RogueTeam;
            else if (team == RedTeamID)
                return TeamColors.RedTeam;
            else if (team == GreenTeamID)
                return TeamColors.GreenTeam;
            else if (team == BlueTeamID)
                return TeamColors.BlueTeam;
            else if (team == PurpleTeamID)
                return TeamColors.PurpleTeam;
            else if (team == ObserverTeamID)
                return TeamColors.ObserverTeam;
            else if (team == RabbitTeamID)
                return TeamColors.RabbitTeam;
            else if (team == HunterTeamID)
                return TeamColors.HunterTeam;

            return TeamColors.NoTeam;
        }

        public static bool PlayerIDIsUseable(int id)
        {
            return id >= MinimumPlayerID && id <= MaxUseablePlayerID;
        }

    }
}
