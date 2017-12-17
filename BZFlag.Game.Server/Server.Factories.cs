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

        public TeamColors DefaultSelectTeam(ServerPlayer player)
        {
            if (ConfigData.TeamData.ForceAutomaticTeams || !State.Players.ValidPlayerTeam(player.DesiredTeam))
            {
                TeamColors smallTeam = State.Players.GetSmallestTeam(!ConfigData.GameData.IsTeamGame);
                int count = State.Players.GetTeamPlayerCount(smallTeam);
                if (count >= ConfigData.TeamData.GetTeamLimit(smallTeam))
                    return TeamColors.NoTeam;

                return smallTeam;
            }
            return player.DesiredTeam;
        }
    }
}
