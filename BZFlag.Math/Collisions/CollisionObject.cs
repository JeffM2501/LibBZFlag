using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.LinearMath.Culling;
using BZFlag.LinearMath.Geometry;

namespace BZFlag.LinearMath.Collisions
{
    public class CollisionObject : IOctreeObject
    {
        public virtual AxisAlignedBox GetOctreeBounds()
        {
            return null;
        }

        public virtual ContainmentType CollideSphere(Sphere sphere)
        {

        }

        public virtual ContainmentType CollideBox()
    }
}
