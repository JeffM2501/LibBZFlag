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

        public static readonly int RogueTeamID = 251;
        public static readonly int RedTeamID = 250;
        public static readonly int GreenTeamID = 249;
        public static readonly int BlueTeamID = 248;
        public static readonly int PurpleTeamID = 247;
        public static readonly int ObserverTeamID = 246;
        public static readonly int RabbitTeamID = 245;
        public static readonly int HunterTeamID = 244;

        public static readonly int MaxUseablePlayerID = 243;
        public static readonly int MinimumPlayerID = 1;

        public static bool PlayerIDIsUseable(int id)
        {
            return id >= MinimumPlayerID && id <= MaxUseablePlayerID;
        }

    }
}
