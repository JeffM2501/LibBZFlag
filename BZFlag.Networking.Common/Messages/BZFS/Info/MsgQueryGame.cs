using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;
using BZFlag.Data.Game;
using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Info
{
    public class MsgQueryGame : NetworkMessage
    {
        public class TeamInfo
        {
            public TeamColors Team = TeamColors.ObserverTeam;

            public int Size = 0;
            public int Max = 0;
        }

        public GameTypes GameStyle = GameTypes.Unknown;
        public GameOptionFlags GameOptions = GameOptionFlags.NoStyle;
        public int MaxPlayers = -1;
        public int MaxShots = -1;

        public Dictionary<TeamColors, TeamInfo> TeamData = new Dictionary<TeamColors, TeamInfo>();

        public int ShakeWins = 0;
        public int ShakeTimeout = 0;

        public int MaxPlayerScore = 0;
        public int MaxTeamScore = 0;
        public int ElapsedTime = 0;


        public MsgQueryGame()
        {
            Code = CodeFromChars("qg");
        }

        public override byte[] Pack()
        {
            return DynamicOutputBuffer.Get(Code).GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);

            GameStyle = (GameTypes)ReadUInt16();
            GameOptions = (GameOptionFlags)ReadUInt16();
            MaxPlayers = ReadUInt16();
            MaxShots = ReadUInt16();

            TeamData.Clear();
            for (TeamColors t = TeamColors.RogueTeam; t <= TeamColors.ObserverTeam; t++)
            {
                TeamInfo info = new TeamInfo();
                info.Team = t;
                info.Size = ReadUInt16();
                TeamData.Add(t, info);
            }
            for (TeamColors t = TeamColors.RogueTeam; t <= TeamColors.ObserverTeam; t++)
            {
                TeamInfo info = TeamData[t];
                info.Max = ReadUInt16();
            }

            ShakeWins = ReadUInt16(); ;
            ShakeTimeout = ReadUInt16(); ;

            MaxPlayerScore = ReadUInt16(); ;
            MaxTeamScore = ReadUInt16(); ;
            ElapsedTime = ReadUInt16(); ;
        }
    }
}
