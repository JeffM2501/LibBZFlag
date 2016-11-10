using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;

namespace BZFlag.Map.Elements.Shapes
{
    public class Zone : PositionableObject
    {
        public List<string> Flags = new List<string>();
        public List<string> ZoneFlags = new List<string>();
        public List<TeamColors> Safe = new List<TeamColors>();
        public List<TeamColors> Team = new List<TeamColors>();

        public Zone()
        {
            ObjectType = "Zone";
        }
    }
}
