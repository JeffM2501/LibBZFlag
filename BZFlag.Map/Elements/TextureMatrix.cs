using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;

namespace BZFlag.Map.Elements
{
    public class TextureMatrix : BasicObject
    {
        // time invariant
        public bool UseStatic = false;
        public float[] staticMatrix = new float[6];
        public float Rotation = 0;
        public Vector2F FixedShift = Vector2F.Zero;
        public Vector2F FixedScale = Vector2F.Zero;
        public Vector2F FixedCenter = Vector2F.Zero;

        // time varying
        public bool UseDynamic = false;
        public float SpinFreq = 0;
        public Vector2F ShiftFreq = Vector2F.Zero;
        public Vector2F ScaleFreq = Vector2F.Zero;
        public Vector2F Scale = Vector2F.Zero;
        public Vector2F Center = Vector2F.Zero;

        // the final result
        public float[] Matrix = new float[16] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 };

        public TextureMatrix() : base()
        {
            ObjectType = "TextureMatrix";
        }
    }
}
