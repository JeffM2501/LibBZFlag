using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.LinearMath;

namespace BZFlag.LinearMath
{
    public class MatrixHelper4
    {
        // matrix grid methods
        public static float M11(Matrix4F m) { return m.Row0.X; }
        public static void M11(ref Matrix4F m, float value) { m.Row0.X = value; }

        public static float M12(Matrix4F m) { return m.Row0.Y; }
        public static void M12(ref Matrix4F m, float value) { m.Row0.Y = value; }

        public static float M13(Matrix4F m) { return m.Row0.Z; }
        public static void M13(ref Matrix4F m, float value) { m.Row0.Z = value; }

        public static float M14(Matrix4F m) { return m.Row0.W; }
        public static void M14(ref Matrix4F m, float value) { m.Row0.W = value; }

        public static float M21(Matrix4F m) { return m.Row1.X; }
        public static void M21(ref Matrix4F m, float value) { m.Row1.X = value; }

        public static float M22(Matrix4F m) { return m.Row1.Y; }
        public static void M22(ref Matrix4F m, float value) { m.Row1.Y = value; }

        public static float M23(Matrix4F m) { return m.Row1.Z; }
        public static void M23(ref Matrix4F m, float value) { m.Row1.Z = value; }

        public static float M24(Matrix4F m) { return m.Row1.W; }
        public static void M24(ref Matrix4F m, float value) { m.Row1.W = value; }

        public static float M31(Matrix4F m) { return m.Row2.X; }
        public static void M31(ref Matrix4F m, float value) { m.Row2.X = value; }

        public static float M32(Matrix4F m) { return m.Row2.Y; }
        public static void M32(ref Matrix4F m, float value) { m.Row2.Y = value; }

        public static float M33(Matrix4F m) { return m.Row2.Z; }
        public static void M33(ref Matrix4F m, float value) { m.Row2.Z = value; }

        public static float M34(Matrix4F m) { return m.Row2.W; }
        public static void M34(ref Matrix4F m, float value) { m.Row2.W = value; }

        public static float M41(Matrix4F m) { return m.Row3.X; }
        public static void M41(ref Matrix4F m, float value) { m.Row3.X = value; }

        public static float M42(Matrix4F m) { return m.Row3.Y; }
        public static void M42(ref Matrix4F m, float value) { m.Row3.Y = value; }

        public static float M43(Matrix4F m) { return m.Row3.Z; }
        public static void M43(ref Matrix4F m, float value) { m.Row3.Z = value; }

        public static float M44(Matrix4F m) { return m.Row3.W; }
        public static void M44(ref Matrix4F m, float value) { m.Row3.W = value; }

        // matrix index methods
        //         public static float m0(Matrix4F m) { return m.Row0.X; }
        //         public static void m0(ref Matrix4F m, float value) { m.Row0.X = value; }
        // 
        //         public static float m1(Matrix4F m) { return m.Row0.Y; }
        //         public static void m1(ref Matrix4F m, float value) { m.Row0.Y = value; }
        // 
        //         public static float m2(Matrix4F m) { return m.Row0.Z; }
        //         public static void m2(ref Matrix4F m, float value) { m.Row0.Z = value; }
        //      
        //         public static float m3(Matrix4F m) { return m.Row0.W; }
        //         public static void m3(ref Matrix4F m, float value) { m.Row0.W = value; }
        // 
        //         public static float m4(Matrix4F m) { return m.Row1.X; }
        //         public static void m4(ref Matrix4F m, float value) { m.Row1.X = value; }
        // 
        //         public static float m5(Matrix4F m) { return m.Row1.Y; }
        //         public static void m5(ref Matrix4F m, float value) { m.Row1.Y = value; }
        // 
        //         public static float m6(Matrix4F m) { return m.Row1.Z; }
        //         public static void m6(ref Matrix4F m, float value) { m.Row1.Z = value; }
        // 
        //         public static float m7(Matrix4F m) { return m.Row1.W; }
        //         public static void m7(ref Matrix4F m, float value) { m.Row1.W = value; }
        // 
        //         public static float m8(Matrix4F m) { return m.Row2.X; }
        //         public static void m8(ref Matrix4F m, float value) { m.Row2.X = value; }
        // 
        //         public static float m9(Matrix4F m) { return m.Row2.Y; }
        //         public static void m9(ref Matrix4F m, float value) { m.Row1.Y = value; }
        // 
        //         public static float m10(Matrix4F m) { return m.Row2.Z; }
        //         public static void m10(ref Matrix4F m, float value) { m.Row2.Z = value; }
        // 
        //         public static float m11(Matrix4F m) { return m.Row2.W; }
        //         public static void m11(ref Matrix4F m, float value) { m.Row2.W = value; }
        // 
        //         public static float m12(Matrix4F m) { return m.Row3.X; }
        //         public static void m12(ref Matrix4F m, float value) { m.Row3.X = value; }
        // 
        //         public static float m13(Matrix4F m) { return m.Row3.Y; }
        //         public static void m13(ref Matrix4F m, float value) { m.Row3.Y = value; }
        // 
        //         public static float m14(Matrix4F m) { return m.Row3.Z; }
        //         public static void m14(ref Matrix4F m, float value) { m.Row3.Z = value; }
        //      
        //         public static float m15(Matrix4F m) { return m.Row3.W; }
        //         public static void m15(ref Matrix4F m, float value) { m.Row3.W = value; }

        // col major
        public static float m0(Matrix4F m) { return m.Row0.X; }
        public static void m0(ref Matrix4F m, float value) { m.Row0.X = value; }

        public static float m1(Matrix4F m) { return m.Row1.X; }
        public static void m1(ref Matrix4F m, float value) { m.Row1.X = value; }

        public static float m2(Matrix4F m) { return m.Row2.X; }
        public static void m2(ref Matrix4F m, float value) { m.Row2.X = value; }

        public static float m3(Matrix4F m) { return m.Row3.X; }
        public static void m3(ref Matrix4F m, float value) { m.Row3.X = value; }

        public static float m4(Matrix4F m) { return m.Row0.Y; }
        public static void m4(ref Matrix4F m, float value) { m.Row0.Y = value; }

        public static float m5(Matrix4F m) { return m.Row1.Y; }
        public static void m5(ref Matrix4F m, float value) { m.Row1.Y = value; }

        public static float m6(Matrix4F m) { return m.Row2.Y; }
        public static void m6(ref Matrix4F m, float value) { m.Row2.Y = value; }

        public static float m7(Matrix4F m) { return m.Row3.Y; }
        public static void m7(ref Matrix4F m, float value) { m.Row2.Y = value; }

        public static float m8(Matrix4F m) { return m.Row0.Z; }
        public static void m8(ref Matrix4F m, float value) { m.Row0.Z = value; }

        public static float m9(Matrix4F m) { return m.Row1.Z; }
        public static void m9(ref Matrix4F m, float value) { m.Row1.Z = value; }

        public static float m10(Matrix4F m) { return m.Row2.Z; }
        public static void m10(ref Matrix4F m, float value) { m.Row2.Z = value; }

        public static float m11(Matrix4F m) { return m.Row3.Z; }
        public static void m11(ref Matrix4F m, float value) { m.Row3.Z = value; }

        public static float m12(Matrix4F m) { return m.Row0.W; }
        public static void m12(ref Matrix4F m, float value) { m.Row0.W = value; }

        public static float m13(Matrix4F m) { return m.Row1.W; }
        public static void m13(ref Matrix4F m, float value) { m.Row1.W = value; }

        public static float m14(Matrix4F m) { return m.Row2.W; }
        public static void m14(ref Matrix4F m, float value) { m.Row2.W = value; }

        public static float m15(Matrix4F m) { return m.Row3.W; }
        public static void m15(ref Matrix4F m, float value) { m.Row3.W = value; }
    }
}
