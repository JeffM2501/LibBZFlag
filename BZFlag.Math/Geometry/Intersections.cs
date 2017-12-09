using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZFlag.LinearMath.Geometry
{
    public enum ContainmentType
    {
        Disjoint,
        Contains,
        Intersects,
    }

    public enum PlaneIntersectionType
    {
        Front,
        Back,
        Intersecting,
    }
}
