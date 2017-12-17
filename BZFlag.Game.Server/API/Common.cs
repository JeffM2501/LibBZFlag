using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Game.Host.API
{
    public static class Common
    {
        public static Server ServerInstnace = null;

        internal static int MaxFlags = 0;

        public static void SetDesiredMaxFlags(int flags)
        {
            MaxFlags += (int)Math.Abs(flags);
        }
    }
}
