using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.LinearMath
{
    public class Vector4F : IEnumerable<float>
    {
        protected float[] Pos = new float[] { 0, 0, 0, 0 };

        public float A
        {
            get { return Pos[0]; }
            set { Pos[0] = value; }
        }

        public float W
        {
            get { return Pos[0]; }
            set { Pos[0] = value; }
        }

        public float X
        {
            get { return Pos[1]; }
            set { Pos[1] = value; }
        }

        public float Y
        {
            get { return Pos[2]; }
            set { Pos[2] = value; }
        }

        public float Z
        {
            get { return Pos[3]; }
            set { Pos[3] = value; }
        }

        public float this[int key]
        {
            get
            {
                return Pos[key];
            }
            set
            {
                Pos[key] = value;
            }
        }

        public IEnumerator<float> GetEnumerator()
        {
            return ((IEnumerable<float>)Pos).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<float>)Pos).GetEnumerator();
        }

        public Vector3F Xyz { get { return new Vector3F(X, Y, Z); } set { X = value.X; Y = value.Y; Z = value.Z; } }


        public Vector4F() { }

        public Vector4F(float x, float y, float z)
        {
            Pos[0] = 0;
            Pos[1] = x;
            Pos[2] = y;
            Pos[3] = z;
        }

        public Vector4F(float a, float x, float y, float z)
        {
            Pos[0] = a;
            Pos[1] = x;
            Pos[2] = y;
            Pos[3] = z;
        }

        public Vector4F(float[] v, int start = 0)
        {
            Pos[0] = v[start];
            Pos[1] = v[start + 1];
            Pos[2] = v[start + 2];
            Pos[3] = v[start + 3];
        }

        public Vector4F(Vector2F v)
        {
            Pos[1] = v.X;
            Pos[2] = v.Y;
        }

        public Vector4F(Vector3F v)
        {
            Pos[1] = v.X;
            Pos[2] = v.Y;
            Pos[3] = v.Z;
        }

        public Vector4F(Vector3F axis, float angle)
        {
            Pos[0] = angle;
            Pos[1] = axis.X;
            Pos[2] = axis.Y;
            Pos[3] = axis.Z;
        }

        public static Vector4F operator +(Vector4F left, Vector4F right)
        {
            left.X += right.X;
            left.Y += right.Y;
            left.Z += right.Z;
            left.W += right.W;
            return left;
        }

        public static Vector4F operator -(Vector4F left, Vector4F right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            left.Z -= right.Z;
            left.W -= right.W;
            return left;
        }

        public static Vector4F operator -(Vector4F vec)
        {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            vec.Z = -vec.Z;
            vec.W = -vec.W;
            return vec;
        }

        public static Vector4F operator *(Vector4F vec, float f)
        {
            vec.X *= f;
            vec.Y *= f;
            vec.Z *= f;
            vec.W *= f;
            return vec;
        }

        public static Vector4F operator *(float f, Vector4F vec)
        {
            vec.X *= f;
            vec.Y *= f;
            vec.Z *= f;
            vec.W *= f;
            return vec;
        }

        public static Vector4F operator /(Vector4F vec, float f)
        {
            float mult = 1.0f / f;
            vec.X *= mult;
            vec.Y *= mult;
            vec.Z *= mult;
            vec.W *= mult;
            return vec;
        }

        public static bool operator ==(Vector4F left, Vector4F right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector4F left, Vector4F right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            Vector4F rhs = obj as Vector4F;
            if (rhs == null)
                return false;

            return this == rhs;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() | Y.GetHashCode() | Z.GetHashCode() | W.GetHashCode();
        }

        public static readonly Vector4F Zero = new Vector4F(0, 0, 0, 0);

        public static readonly Vector4F UnitX = new Vector4F(0, 1, 0, 0);
        public static readonly Vector4F UnitY = new Vector4F(0, 0, 1, 0);
        public static readonly Vector4F UnitZ = new Vector4F(0, 0, 0, 1);
        public static readonly Vector4F UnitW = new Vector4F(1, 0, 0, 0);
    }
}
