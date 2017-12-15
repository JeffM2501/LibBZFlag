using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.Game.Host.Players;
using BZFlag.Data.Teams;

namespace BZFlag.Game.Host
{
    public partial class Server
    {
        public delegate BZFlag.Data.Teams.TeamColors TeamSelectorCB(ServerPlayer player);

        protected TeamSelectorCB TeamSelector = null;

        private void SetupFactories()
        {
            TeamSelector = DefaultSelectTeam;
        }

        public void SetTeamSelector(TeamSelectorCB selector)
        {
            if (selector == null)
                TeamSelector = DefaultSelectTeam;
            else
                TeamSelector = selector;
        }

        public TeamColors GetPlayerTeam(ServerPlayer player)
        {
            return TeamSelector(player);
        }

        protected bool ValidPlayerTeam(TeamColors color)
        {
            switch(color)
            {
                case TeamColors.RedTeam:
                case TeamColors.BlueTeam:
                case TeamColors.GreenTeam:
                case TeamColors.PurpleTeam:
                case TeamColors.RogueTeam:
                case TeamColors.ObserverTeam:
                    return true;
            }
            return false;
        }

        public TeamColors GetSmallestTeam(bool includeRogue)
        {
            int size = int.MaxValue;
            TeamColors team = includeRogue ? TeamColors.RogueTeam : TeamColors.RedTeam;

            for(TeamColors t = TeamColors.RogueTeam; t < TeamColors.ObserverTeam; t++)
            {
                int count = State.Players.GetTeamPlayerCount(t);
                if (count < size)
                {
                    size = count;
                    team = t;
                }
            }

            return team;
        }

        public TeamColors GetLargestTeam(bool includeRogue)
        {
            int size = int.MinValue;
            TeamColors team = includeRogue ? TeamColors.RogueTeam : TeamColors.RedTeam;

            for (TeamColors t = TeamColors.RogueTeam; t < TeamColors.ObserverTeam; t++)
            {
                int count = State.Players.GetTeamPlayerCount(t);
                if (count > size)
                {
                    size = count;
                    team = t;
                }
            }

            return team;
        }

        public TeamColors DefaultSelectTeam(ServerPlayer player)
        {
            if (ConfigData.TeamData.ForceAutomaticTeams || !ValidPlayerTeam(player.DesiredTeam))
            {
                TeamColors smallTeam = GetSmallestTeam(!ConfigData.GameData.IsTeamGame);
                int count = State.Players.GetTeamPlayerCount(smallTeam);
                if (count >= ConfigData.TeamData.GetTeamLimit(smallTeam))
                    return TeamColors.NoTeam;

                return smallTeam;
            }
            return player.DesiredTeam;
        }
    }
}
