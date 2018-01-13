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
using System.Collections.Generic;
using System.Globalization;

using BZFlag.LinearMath;

namespace BZFlag.LinearMath.Geometry
{
    [Serializable]
    public class Sphere : IEquatable<Sphere>
    {
        #region Public Fields


        public Vector3F Center = Vector3F.Zero;
        public float Radius = 0;


        #endregion Public Fields

        #region Constructors

        public Sphere()
        {
        }

        public Sphere(Vector3F center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        #endregion Constructors

        #region Public Methods

        public static Sphere Empty = new Sphere(new Vector3F(0, 0, 0), 0);

        public ContainmentType Contains(AxisAlignedBox box)
        {
            //check if all corner is in sphere
            bool inside = true;
            foreach (Vector3F corner in box.GetCorners())
            {
                if (this.Contains(corner) == ContainmentType.Disjoint)
                {
                    inside = false;
                    break;
                }
            }


            if (inside)
                return ContainmentType.Contains;


            //check if the distance from sphere center to cube face < radius
            double dmin = 0;


            if (Center.X < box.Min.X)
                dmin += (Center.X - box.Min.X) * (Center.X - box.Min.X);


            else if (Center.X > box.Max.X)
                dmin += (Center.X - box.Max.X) * (Center.X - box.Max.X);


            if (Center.Y < box.Min.Y)
                dmin += (Center.Y - box.Min.Y) * (Center.Y - box.Min.Y);


            else if (Center.Y > box.Max.Y)
                dmin += (Center.Y - box.Max.Y) * (Center.Y - box.Max.Y);


            if (Center.Z < box.Min.Z)
                dmin += (Center.Z - box.Min.Z) * (Center.Z - box.Min.Z);


            else if (Center.Z > box.Max.Z)
                dmin += (Center.Z - box.Max.Z) * (Center.Z - box.Max.Z);


            if (dmin <= Radius * Radius)
                return ContainmentType.Intersects;

            //else disjoint
            return ContainmentType.Disjoint;


        }


        public void Contains(ref AxisAlignedBox box, out ContainmentType result)
        {
            result = this.Contains(box);
        }


        public ContainmentType Contains(Frustum frustum)
        {
            //check if all corner is in sphere
            bool inside = true;


            Vector3F[] corners = frustum.GetCorners();
            foreach (Vector3F corner in corners)
            {
                if (this.Contains(corner) == ContainmentType.Disjoint)
                {
                    inside = false;
                    break;
                }
            }
            if (inside)
                return ContainmentType.Contains;


            //check if the distance from sphere center to frustrum face < radius
            double dmin = 0;
            //TODO : calcul dmin


            if (dmin <= Radius * Radius)
                return ContainmentType.Intersects;


            //else disjoint
            return ContainmentType.Disjoint;
        }


        public ContainmentType Contains(Sphere sphere)
        {
            float val = Vector3F.Distance(sphere.Center, Center);


            if (val > sphere.Radius + Radius)
                return ContainmentType.Disjoint;


            else if (val <= Radius - sphere.Radius)
                return ContainmentType.Contains;


            else
                return ContainmentType.Intersects;
        }


        public void Contains(ref Sphere sphere, out ContainmentType result)
        {
            result = Contains(sphere);
        }


        public ContainmentType Contains(Vector3F point)
        {
            float distance = Vector3F.Distance(point, Center);

            if (distance > this.Radius)
                return ContainmentType.Disjoint;


            else if (distance < this.Radius)
                return ContainmentType.Contains;


            return ContainmentType.Intersects;
        }


        public void Contains(ref Vector3F point, out ContainmentType result)
        {
            result = Contains(point);
        }


        public static Sphere CreateFromBoundingBox(AxisAlignedBox box)
        {
            // Find the center of the box.
            Vector3F center = new Vector3F((box.Min.X + box.Max.X) / 2.0f,
                                         (box.Min.Y + box.Max.Y) / 2.0f,
                                         (box.Min.Z + box.Max.Z) / 2.0f);


            // Find the distance between the center and one of the corners of the box.
            float radius = Vector3F.Distance(center, box.Max);


            return new Sphere(center, radius);
        }


        public static void CreateFromBoundingBox(ref AxisAlignedBox box, out Sphere result)
        {
            result = CreateFromBoundingBox(box);
        }


        public static Sphere CreateFromFrustum(Frustum frustum)
        {
            return Sphere.CreateFromPoints(frustum.GetCorners());
        }


        public static Sphere CreateFromPoints(IEnumerable<Vector3F> points)
        {
            if (points == null)
                throw new ArgumentNullException("points");


            float radius = 0;
            Vector3F center = new Vector3F();
            // First, we'll find the center of gravity for the point 'cloud'.
            int num_points = 0; // The number of points (there MUST be a better way to get this instead of counting the number of points one by one?)

            foreach (Vector3F v in points)
            {
                center += v;    // If we actually knew the number of points, we'd get better accuracy by adding v / num_points.
                ++num_points;
            }

            center /= (float)num_points;


            // Calculate the radius of the needed sphere (it equals the distance between the center and the point further away).
            foreach (Vector3F v in points)
            {
                float distance = ((Vector3F)(v - center)).Length;

                if (distance > radius)
                    radius = distance;
            }


            return new Sphere(center, radius);
        }


        public static Sphere CreateMerged(Sphere original, Sphere additional)
        {
            Vector3F ocenterToaCenter = additional.Center -original.Center;
            float distance = ocenterToaCenter.Length;
            if (distance <= original.Radius + additional.Radius)//intersect
            {
                if (distance <= original.Radius - additional.Radius)//original contain additional
                    return original;
                if (distance <= additional.Radius - original.Radius)//additional contain original
                    return additional;
            }


            //else find center of new sphere and radius
            float leftRadius = System.Math.Max(original.Radius - distance, additional.Radius);
            float Rightradius = System.Math.Max(original.Radius + distance, additional.Radius);
            ocenterToaCenter = ocenterToaCenter + (((leftRadius - Rightradius) / (2 * ocenterToaCenter.Length)) * ocenterToaCenter);//oCenterToResultCenter

            Sphere result = new Sphere();
            result.Center = original.Center + ocenterToaCenter;
            result.Radius = (leftRadius + Rightradius) / 2;
            return result;
        }


        public static void CreateMerged(ref Sphere original, ref Sphere additional, out Sphere result)
        {
            result = Sphere.CreateMerged(original, additional);
        }


        public float Distance(Sphere sphere)
        {
            Vector3F dist = Center - sphere.Center;
            return dist.Length - sphere.Radius - Radius;
        }


        public float Distance(Vector3F point)
        {
            Vector3F dist = Center - point;
            return dist.Length - Radius;
        }


        public float DistanceSquared(Sphere sphere)
        {
            Vector3F dist = Center - sphere.Center;
            return dist.LengthSquared - sphere.Radius * sphere.Radius - Radius * Radius;
        }


        public float DistanceSquared(Vector3F point)
        {
            Vector3F dist = Center - point;
            return dist.LengthSquared - Radius * Radius;
        }


        public bool Equals(Sphere other)
        {
            return this.Center == other.Center && this.Radius == other.Radius;
        }


        public override bool Equals(object obj)
        {
            if (obj is Sphere)
                return this.Equals((Sphere)obj);


            return false;
        }


        public override int GetHashCode()
        {
            return this.Center.GetHashCode() + this.Radius.GetHashCode();
        }


        public bool Intersects(AxisAlignedBox box)
        {
            return box.Intersects(this);
        }


        public void Intersects(ref AxisAlignedBox box, out bool result)
        {
            result = Intersects(box);
        }


        public bool Intersects(Frustum frustum)
        {
            if (frustum == null)
                throw new NullReferenceException();


            throw new NotImplementedException();
        }


        public bool Intersects(Sphere sphere)
        {
            float val = Vector3F.Distance(sphere.Center, Center);
            if (val > sphere.Radius + Radius)
                return false;
            return true;
        }


        public void Intersects(ref Sphere sphere, out bool result)
        {
            result = Intersects(sphere);
        }


        public PlaneIntersectionType Intersects(Plane plane)
        {
            float distance = Vector3F.Dot(plane.Normal, this.Center) + plane.D;
            if (distance > this.Radius)
                return PlaneIntersectionType.Front;
            if (distance < -this.Radius)
                return PlaneIntersectionType.Back;
            //else it intersect
            return PlaneIntersectionType.Intersecting;
        }


        public void Intersects(ref Plane plane, out PlaneIntersectionType result)
        {
            result = Intersects(plane);
        }


        public Nullable<float> Intersects(Ray ray)
        {
            return ray.Intersects(this);
        }


        public void Intersects(ref Ray ray, out Nullable<float> result)
        {
            result = Intersects(ray);
        }


        public static bool operator ==(Sphere a, Sphere b)
        {
            return a.Equals(b);
        }


        public static bool operator !=(Sphere a, Sphere b)
        {
            return !a.Equals(b);
        }


        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{Center:{0} Radius:{1}}}", this.Center.ToString(), this.Radius.ToString());
        }


        #endregion Public Methods
    }
}