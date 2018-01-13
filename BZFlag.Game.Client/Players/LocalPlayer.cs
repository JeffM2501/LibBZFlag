using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.Data.Teams;
using BZFlag.Data.Players;
using BZFlag.Data.Types;
using BZFlag.Networking.Messages.BZFS.Player;

using BZFlag.Game.Flags;
using BZFlag.Game.Shots;
using BZFlag.Map.Elements.Shapes;
using BZFlag.LinearMath;

namespace BZFlag.Game.Players
{
    public class LocalPlayer : Player
    {
        public override bool IsLocalPlayer {get{return true;} }

        public Vector3F Velocity = Vector3F.Zero;
        public float RotationVelocity = 0;

        protected double LastTime = -1;

        public LocalPlayer(Client connection) : base() { }

        public LocalPlayer(int id, string name) : base()
        {
            PlayerID = id;
            Callsign = name;
        }

        public LocalPlayer(int id, string name, TeamColors team) : base()
        {
            PlayerID = id;
            Callsign = name;
            Team = team;
        }

        public void SetInput(float speed, float turn)
        {

        }

        public override void Update(double now, double delta)
        {
            base.Update(now, delta);
        }
    }
}
