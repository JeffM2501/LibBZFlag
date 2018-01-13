using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZFlag.LinearMath
{
    public static class Interpolatation
    {
        public static float LinInterp(float p1, float p2, float t)
        {
            return (1.0f - t) * p1 + t * p2;
        }

        public static Vector3F LinInterp3D(Vector3F p1, Vector3F p2, float t)
        {
            return new Vector3F(LinInterp(p1.X, p2.X, t), LinInterp(p1.Y, p2.Y, t), LinInterp(p1.Z, p2.Z, t));
        }

        public static Vector3F LinInterp3DEX(Vector3F p1, Vector3F p2, float p2ZShift, float t)
        {
            return new Vector3F(LinInterp(p1.X, p2.X, t), LinInterp(p1.Y, p2.Y, t), LinInterp(p1.Z, p2.Z + p2ZShift, t));
        }
    }
}
