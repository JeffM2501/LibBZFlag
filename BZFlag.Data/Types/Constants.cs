using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Data.Types
{
	public static class Constants
	{
        // string buffer sizes
		public static readonly int CallsignLen = 32;
		public static readonly int MottoLen = 128;
		public static readonly int TokenLen = 22;
		public static readonly int VersionLen = 60;

        // update compression constants
        public static readonly float SmallScale = 32766.0f; // the full scale of a int16_t  (less 1.0 for safety)
        public static readonly float SmallMaxDist = 0.02f * SmallScale; // 2 cm resolution  (range: +/- 655.32 meters)
        public static readonly float SmallMaxVel = 0.01f * SmallScale;        // 1 cm/sec resolution  (range: +/- 327.66 meters/sec)       
        public static readonly float SmallMaxAngVel = 0.001f * SmallScale; // 0.001 radians/sec resolution  (range: +/- 32.766 rads/sec)
    }
}
