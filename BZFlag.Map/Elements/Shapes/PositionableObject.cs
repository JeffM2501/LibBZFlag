using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.LinearMath;


namespace BZFlag.Map.Elements.Shapes
{
    public abstract class PositionableObject : BasicObject
    {
        public Vector3F Position = new Vector3F();
        public float Rotation = 0;
        public Vector3F Size = new Vector3F();
    }
}
