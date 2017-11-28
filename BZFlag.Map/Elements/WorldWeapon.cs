using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;

namespace BZFlag.Map.Elements
{
    public class WorldWeapon : BasicObject
    {
        public string Flag = string.Empty;
        public Vector3F Position = Vector3F.Zero;
        public float Rotation = 0;
        public float InitalDelay = -1;

        public List<float> DelayList = new List<float>();


        public WorldWeapon()
        {
            ObjectType = "WorldWeapon";
        }
    }
}
