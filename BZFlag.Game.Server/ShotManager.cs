using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.LinearMath;
using BZFlag.Data.Players;
using BZFlag.Data.Teams;
using BZFlag.Game.Host.Players;
using BZFlag.Networking.Messages.BZFS.Shots;
using BZFlag.Data.Flags;

namespace BZFlag.Game.Host
{
    public class ShotManager
    {
        public Server ServerHost = null;

        public class ShotInfo
        {
            public int GlobalID = -1;
            public ServerPlayer Owner = null;
            public int PlayerShotID = -1;

            public TeamColors TeamColor = TeamColors.NoTeam;
            public ShotTypes ShotType = ShotTypes.NormalShot;
            public FlagType SourceFlag = null;

            public Vector3F InitalPostion = Vector3F.Zero;
            public Vector3F InitalVector = Vector3F.Zero;
            public double FireTimestamp = double.MinValue;
            public double Lifetime = 0;
        }

        protected List<ShotInfo> Shots = new List<ShotInfo>();

        public void HandleShotBegin(ServerPlayer sender, MsgShotBegin shot)
        {
            ServerHost.State.Players.SendToAll(shot, shot.FromUDP);
        }

        public void HandleShotEnd(ServerPlayer sender, MsgShotEnd shot)
        {
            ServerHost.State.Players.SendToAll(shot, shot.FromUDP);
        }

    }
}
