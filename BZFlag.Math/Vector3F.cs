using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.LinearMath
{
    public class Vector3F : IEnumerable<float>
    {
        protected float[] Pos = new float[] { 0, 0, 0 };

        public float X
        {
            get { return Pos[0]; }
            set { Pos[0] = value; }
        }

        public float Y
        {
            get { return Pos[1]; }
            set { Pos[1] = value; }
        }

        public float Z
        {
            get { return Pos[2]; }
            set { Pos[2] = value; }
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

        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        public float LengthSquared
        {
            get
            {
                return X * X + Y * Y + Z * Z;
            }
        }

        public static float Dot(Vector3F left, Vector3F right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        public static void Dot(ref Vector3F left, ref Vector3F right, out float result)
        {
            result = left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        public static Vector3F Cross(Vector3F left, Vector3F right)
        {
            return new Vector3F(left.Y * right.Z - left.Z * right.Y,
                               left.Z * right.X - left.X * right.Z,
                               left.X * right.Y - left.Y * right.X);
        }

        public static void Cross(ref Vector3F left, ref Vector3F right, ref Vector3F result)
        {
            result.X = left.Y * right.Z - left.Z * right.Y;
            result.Y = left.Z * right.X - left.X * right.Z;
            result.Z = left.X * right.Y - left.Y * right.X;
        }

        public static Vector3F Normalize(Vector3F vec)
        {
            float l = vec.Length;
            return new Vector3F(vec.X /= l, vec.Y /= l, vec.Z /= l);
        }

        public void Normalize()
        {
            float l = Length;
            X /= l;
            Y /= l;
            Z /= l;
        }

        public static float Distance(Vector3F v1, Vector3F v2)
        {
            return (float)System.Math.Sqrt((v2.X - v1.X) * (v2.X - v1.X) + (v2.Y - v1.Y) * (v2.Y - v1.Y) + (v2.Z - v1.Z) * (v2.Z - v1.Z));
        }

        public static float DistanceSquared(Vector3F v1, Vector3F v2)
        {
            return (v2.X - v1.X) * (v2.X - v1.X) + (v2.Y - v1.Y) * (v2.Y - v1.Y) + (v2.Z - v1.Z) * (v2.Z - v1.Z);
        }

        public static Vector3F Min(Vector3F v1, Vector3F v2)
        {
            Vector3F ret = new Vector3F(v1);
            if (v2.X < ret.X)
                ret.X = v2.X;

            if (v2.Y < ret.Y)
                ret.Y = v2.Y;

            if (v2.Z < ret.Z)
                ret.Z = v2.Z;

            return ret;
        }

        public static Vector3F Max(Vector3F v1, Vector3F v2)
        {
            Vector3F ret = new Vector3F(v1);
            if (v2.X > ret.X)
                ret.X = v2.X;

            if (v2.Y > ret.Y)
                ret.Y = v2.Y;

            if (v2.Z > ret.Z)
                ret.Z = v2.Z;

            return ret;
        }

        public static Vector3F FromQuaternion(Quaternion quat)
        {
            quat.Normalize();

            Vector3F angs = new Vector3F();
            angs.Y = (float)System.Math.Asin(2.0f * quat.X * quat.Y + 2.0f * quat.Z * quat.W);

            if (quat.X * quat.Y + quat.Z * quat.W == 0.5f)
                angs.X = 2.0f * (float)System.Math.Atan2(quat.X, quat.W);
            else if (quat.X * quat.Y + quat.Z * quat.W == -0.5f)
                angs.X = -2.0f * (float)System.Math.Atan2(quat.X, quat.W);
            else
            {
                angs.X = (float)System.Math.Atan2(2.0f * quat.Y * quat.W - 2.0f * quat.X * quat.Z, 1.0f - 2.0f * (quat.Y * quat.Y) - 2 * (quat.Z * quat.Z));
                angs.Z = (float)System.Math.Atan2(2.0f * quat.X * quat.W - 2.0f * quat.Y * quat.Z, 1.0f - 2.0f * (quat.X * quat.X) - 2 * (quat.Z * quat.Z));
            }

            return angs;
        }

        public static Vector3F FromAngle(float angle)
        {
            return new Vector3F((float)System.Math.Cos(angle.ToRad()), (float)System.Math.Sin(angle.ToRad()), 0);
        }

        public Vector3F() { }

        public Vector3F(float x, float y, float z)
        {
            Pos[0] = x;
            Pos[1] = y;
            Pos[2] = z;
        }

        public Vector3F(float[] v, int start = 0)
        {
            Pos[0] = v[start];
            Pos[1] = v[start + 1];
            Pos[2] = v[start + 2];
        }

        public Vector3F(Vector2F v)
        {
            Pos[0] = v.X;
            Pos[1] = v.Y;
        }

        public Vector3F(Vector3F v)
        {
            Pos[0] = v.X;
            Pos[1] = v.Y;
            Pos[2] = v.Z;
        }

        public Vector3F(Vector4F v)
        {
            Pos[0] = v.X;
            Pos[1] = v.Y;
            Pos[2] = v.Z;
        }

        public static Vector3F operator +(Vector3F lhs, Vector3F rhs)
        {
            return new Vector3F(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        public static Vector3F operator -(Vector3F lhs, Vector3F rhs)
        {
            return new Vector3F(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        public static Vector3F operator *(Vector3F lhs, float rhs)
        {
            return new Vector3F(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
        }

        public static Vector3F operator *(Vector3F lhs, double rhs)
        {
            return new Vector3F((float)(lhs.X * rhs), (float)(lhs.Y * rhs), (float)(lhs.Z * rhs));
        }

        public static Vector3F operator *(double lhs, Vector3F rhs)
        {
            return new Vector3F((float)(lhs * rhs.X), (float)(lhs * rhs.Y), (float)(lhs * rhs.Z));
        }

        public static Vector3F operator /(Vector3F lhs, double rhs)
        {
            return new Vector3F((float)(lhs.X / rhs), (float)(lhs.Y / rhs), (float)(lhs.Z / rhs));
        }


        public static bool operator ==(Vector3F left, Vector3F right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3F left, Vector3F right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            Vector3F rhs = obj as Vector3F;
            if (rhs == null)
                return false;

            return this == rhs;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() | Y.GetHashCode() | Z.GetHashCode();
        }

        public bool IsZero()
        {
            return Pos[0] == 0.0f && Pos[1] == 0.0f && Pos[2] == 0.0f;
        }

        public static readonly Vector3F Zero = new Vector3F(0, 0, 0);
        public static readonly Vector3F UnitX = new Vector3F(1, 0, 0);
        public static readonly Vector3F UnitY = new Vector3F(0, 1, 0);
        public static readonly Vector3F UnitZ = new Vector3F(0, 0, 1);
    }

    public static class TrigTools
    {
        public static double RadCon = System.Math.PI / 180.0;
        public static double DegCon = 180.0 / System.Math.PI;

        public static float ToDeg(this float rad)
        {
            return rad * (float)DegCon;
        }

        public static float ToRad(this float deg)
        {
            return deg * (float)RadCon;
        }
    }
}
