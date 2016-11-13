using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;
using BZFlag.Networking.Messages.BZFS.Info;
using BZFlag.Networking.Messages;

namespace BZFlag.Game
{
    public partial class Client
    {
		public class TeamInfo
		{
			public TeamColors Color = TeamColors.NoTeam;

			public int PlayerCount = -1;
			public int Wins = 0;
			public int Losses = 0;


			public TeamInfo(TeamColors c)
			{
				Color = c;
			}
		}

		public Dictionary<TeamColors, TeamInfo> Teams = new Dictionary<TeamColors, TeamInfo>();

		private  void HandleTeamUpdate(NetworkMessage msg)
		{
			MsgTeamUpdate upd = msg as MsgTeamUpdate;

			foreach(var t in upd.TeamUpdates)
			{
				if(!Teams.ContainsKey(t.TeamID))
					Teams.Add(t.TeamID, new TeamInfo(t.TeamID));

				TeamInfo team = Teams[t.TeamID];
				team.PlayerCount = t.Size;
				team.Wins = t.Wins;
				team.Losses = t.Losses;
			}
		}
	}
}
