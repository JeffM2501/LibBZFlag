/*
        Copyright (C) 2013 Jeffry Myers
        Author: Jeffry Myers
 * 
        Note this code file does NOT derive from any previous work.
 * 
	    This is free software; you can redistribute it and/or modify
        it under the terms of the GNU General Public License as published by
        the Free Software Foundation; either version 2 of the License, or
        (at your option) any later version.

        This software is distributed in the hope that it will be useful,
        but WITHOUT ANY WARRANTY; without even the implied warranty of
        MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
        GNU General Public License for more details.

        You should have received a copy of the GNU General Public License
        along with Spacenerds in Space; if not, write to the Free Software
        Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/



using System;
using System.ComponentModel;

using BZFlag.LinearMath;

namespace BZFlag.LinearMath.Geometry
{
    [Serializable]
    //[TypeConverter(typeof(PlaneConverter))]
    public class Plane : IEquatable<Plane>
    {

        #region Static

        public static Plane Up = new Plane(0, 0, 1, 0);
        public static Plane Down = new Plane(0, 0, -1, 0);
        public static Plane Empty = new Plane(0, 0, 0, 0);

        public static float InsersectionTolerance = 0.0001f;
        #endregion Static

        #region Public Fields


        public float D;
        public Vector3F Normal;


        #endregion Public Fields

        #region Constructors

        public Plane()
        {
            D = 0;
            Normal = Vector3F.Zero;
        }

        public Plane(Vector4F value)
            : this(new Vector3F(value.X, value.Y, value.Z), value.W)
        {


        }


        public Plane(Vector3F normal, float d)
        {
            Normal = normal;
            D = d;
        }


        public Plane(Vector3F a, Vector3F b, Vector3F c)
        {
            Vector3F ab = b - a;
            Vector3F ac = c - a;


            Vector3F cross = Vector3F.Cross(ab, ac);
            Normal = Vector3F.Normalize(cross);
            D = -(Vector3F.Dot(cross, a));
        }


        public Plane(float a, float b, float c, float d)
            : this(new Vector3F(a, b, c), d)
        {


        }


        #endregion Constructors

        #region Public Methods


        public static bool operator !=(Plane plane1, Plane plane2)
        {
            return !plane1.Equals(plane2);
        }


        public static bool operator ==(Plane plane1, Plane plane2)
        {
            return plane1.Equals(plane2);
        }

        public float Distance(Vector3F point)
        {
            return PerpendicularDistance(ref point, this);
        }


        public float Distance(Sphere sphere)
        {
            return PerpendicularDistance(ref sphere.Center, this) - sphere.Radius;
        }


        public override bool Equals(object other)
        {
            return (other is Plane) ? this.Equals((Plane)other) : false;
        }


        public bool Equals(Plane other)
        {
            return ((Normal == other.Normal) && (D == other.D));
        }


        public override int GetHashCode()
        {
            return Normal.GetHashCode() ^ D.GetHashCode();
        }


        public PlaneIntersectionType Intersects(AxisAlignedBox box)
        {
            return box.Intersects(this);
        }


        public void Intersects(ref AxisAlignedBox box, out PlaneIntersectionType result)
        {
            result = Intersects(box);
        }


        public PlaneIntersectionType Intersects(Frustum frustum)
        {
            return frustum.Intersects(this);
        }


        public PlaneIntersectionType Intersects(Sphere sphere)
        {
            return sphere.Intersects(this);
        }

        public PlaneIntersectionType Intersects(Vector3F point)
        {
            float dist = ClassifyPoint(ref point, this);
            if (dist > InsersectionTolerance)
                return PlaneIntersectionType.Front;
            if (dist < -InsersectionTolerance)
                return PlaneIntersectionType.Back;
            return PlaneIntersectionType.Intersecting;
        }

        public PlaneIntersectionType IntersectsPoint(Vector3F point)
        {
            Vector3F vec = Normal * D - point;

            float dot = Vector3F.Dot(vec, Normal);

            if (dot < -InsersectionTolerance)
                return PlaneIntersectionType.Front;
            if (dot > InsersectionTolerance)
                return PlaneIntersectionType.Back;
            return PlaneIntersectionType.Intersecting;
        }

        public void Intersects(ref Sphere sphere, out PlaneIntersectionType result)
        {
            result = Intersects(sphere);
        }


        public override string ToString()
        {
            return string.Format("{{Normal:{0} D:{1}}}", Normal, D);
        }


        #endregion

        #region Internal Methods

        // Indicating which side (positive/negative) of a plane a point is on.
        // Returns > 0 if on the positive side, < 0 if on the negative size, 0 if on the plane.
        internal static float ClassifyPoint(ref Vector3F point, Plane plane)
        {
            return point.X * plane.Normal.X + point.Y * plane.Normal.Y + point.Z * plane.Normal.Z + plane.D;
        }

        // Calculates the perpendicular distance from a point to a plane
        internal static float PerpendicularDistance(ref Vector3F point, Plane plane)
        {
            // dist = (ax + by + cz + d) / sqrt(a*a + b*b + c*c)
            return (float)System.Math.Abs((plane.Normal.X * point.X +
                                           plane.Normal.Y * point.Y +
                                           plane.Normal.Z * point.Z) /
                                          System.Math.Sqrt(plane.Normal.X * plane.Normal.X +
                                                           plane.Normal.Y * plane.Normal.Y +
                                                           plane.Normal.Z * plane.Normal.Z));
        }

        #endregion
    }

    public class PlaneHelper
    {
        // sets a plane from 3 points with out a copy
        public static void Set(ref Plane plane, Vector3F p1, Vector3F p2, Vector3F p3)
        {
            // get normal by crossing v1 and v2 and normalizing
            plane.Normal = Vector3F.Cross(p1, p2);
            plane.Normal.Normalize();
            plane.D = -Vector3F.Dot(p3, plane.Normal);
        }
    }
}