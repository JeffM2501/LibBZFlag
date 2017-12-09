using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.LinearMath;

namespace BZFlag.Map.Elements
{
    public class Physics : BasicObject
    {
        public Vector3F Linear = new Vector3F();
        public Vector3F Angular = new Vector3F();
        public Vector3F Radial = new Vector3F();

        public float Slide = 0;
        public string Death = string.Empty;

        public Physics()
        {
            ObjectType = "Physics";
        }
    }
}
