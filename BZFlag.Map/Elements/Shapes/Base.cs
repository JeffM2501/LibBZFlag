using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;

namespace BZFlag.Map.Elements.Shapes
{
    public class Base : Box
    {
        public TeamColors TeamColor = TeamColors.NoTeam;

        public Base()
        {
            ObjectType = "Base";
        }
    }
}
