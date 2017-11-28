using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;

namespace BZFlag.Map.Elements
{
    public class SequenceParams
    {
        public float Period = 0;
        public float Offset = 0;
        public List<byte> Values = new List<byte>();
    };

    public class SinusoidParams
    {
        public float Period = 0;
        public float Offset = 0;
        public float Weight = 0;
    };

    public class ClampParams
    {
        public float Period = 0;
        public float Offset = 0;
        public float Width = 0;
    };


    public class DynamicColor : BasicObject
    {
        public enum SequenceStates
        {
            colorMin = 0,
            colorMid = 1,
            colorMax = 2,
        };

        public Color4F Color = Color4F.Empty;

        public class ChannelParams
        {

            public float MinValue = -1;
            public float MaxValue = 1;

            public float TotalWeight = 1; // tally of sinusoid weights
            public SequenceParams Sequence;
            public List<SinusoidParams> Sinusoids = new List<SinusoidParams>();
            public List<ClampParams> ClampUps = new List<ClampParams>();
            public List<ClampParams> ClampDowns = new List<ClampParams>();
        };

        public ChannelParams[] Channels = new ChannelParams[4];

        public DynamicColor() : base()
        {
            ObjectType = "DynamicColor";
        }
    }
}
