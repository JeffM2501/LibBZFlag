
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.Game.Players;
using BZFlag.Map;
using BZFlag.LinearMath;

namespace BZFlag.Game.Shots
{
    public class ShotPath
    {
        public class Segment
        {
            public Vector3F StartPoint = Vector3F.Zero;
            public Vector3F EndPoint = Vector3F.Zero;

            public double StartT = -1;
            public double EndT = -1;

            public double SegmentT = 0;
        }

        public List<Segment> Segments = new List<Segment>();

        public static readonly ShotPath Empty = new ShotPath();
    }

    public interface ShotPathGenerator
    {
        ShotPath GetShotPath(Shot shot, Player player, WorldMap map);
    }
}
