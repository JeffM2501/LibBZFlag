using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Map.Elements.Shapes
{
    public class Teleporter : PhaseableObject
    {
        public static int TeleporterCount = 0;

        public float Border = 0;
        public int Index = 0;

        public bool Horizontal = false;

        public Teleporter()
        {
            ObjectType = "Teleporter";

            TeleporterCount++;
            Index = TeleporterCount;
        }
    }
}
