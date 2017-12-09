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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

using BZFlag.LinearMath;

namespace BZFlag.LinearMath.Geometry
{
    [Serializable]
    public class Frustum : IEquatable<Frustum>
    {
        #region Private Fields

        protected Matrix4F matrix;
        protected Plane bottom;
        protected Plane far;
        protected Plane left;
        protected Plane right;
        protected Plane near;
        protected Plane top;
        protected Vector3F[] corners;

        #endregion Private Fields

        #region Public Constructors

        public Frustum(Matrix4F value)
        {
            this.matrix = value;
            CreatePlanes();
            CreateCorners();
        }

        #endregion Public Constructors

        #region Public Properties

        public Plane Bottom
        {
            get { return this.bottom; }
        }

        public Plane Far
        {
            get { return this.far; }
        }

        public Plane Left
        {
            get { return this.left; }
        }

        public Matrix4F Matrix
        {
            get { return this.matrix; }
            set
            {
                this.matrix = value;
                this.CreatePlanes();    // FIXME: The odds are the planes will be used a lot more often than the matrix
                this.CreateCorners();   // is updated, so this should help performance. I hope ;)
            }
        }

        public Plane Near
        {
            get { return this.near; }
        }

        public Plane Right
        {
            get { return this.right; }
        }

        public Plane Top
        {
            get { return this.top; }
        }

        #endregion Public Properties

        #region Public Methods

        public static bool operator ==(Frustum a, Frustum b)
        {
            if (object.Equals(a, null))
                return (object.Equals(b, null));

            if (object.Equals(b, null))
                return (object.Equals(a, null));

            return a.matrix == (b.matrix);
        }

        public static bool operator !=(Frustum a, Frustum b)
        {
            return !(a == b);
        }

        public ContainmentType Contains(AxisAlignedBox box)
        {
            ContainmentType result;
            this.Contains(ref box, out result);
            return result;
        }

        public void Contains(ref AxisAlignedBox box, out ContainmentType result)
        {
            // FIXME: Is this a bug?
            // If the bounding box is of W * D * H = 0, then return disjoint
            if (box.Min == box.Max)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            int i;
            ContainmentType contained;
            Vector3F[] corners = box.GetCorners();

            // First we assume completely disjoint. So if we find a point that is contained, we break out of this loop
            for (i = 0; i < corners.Length; i++)
            {
                this.Contains(ref corners[i], out contained);
                if (contained != ContainmentType.Disjoint)
                    break;
            }

            if (i == corners.Length) // This means we checked all the corners and they were all disjoint
            {
                result = ContainmentType.Disjoint;
                return;
            }

            if (i != 0)             // if i is not equal to zero, we can fastpath and say that this box intersects
            {                       // because we know at least one point is outside and one is inside.
                result = ContainmentType.Intersects;
                return;
            }

            // If we get here, it means the first (and only) point we checked was actually contained in the frustum.
            // So we assume that all other points will also be contained. If one of the points is disjoint, we can
            // exit immediately saying that the result is Intersects
            i++;
            for (; i < corners.Length; i++)
            {
                this.Contains(ref corners[i], out contained);
                if (contained != ContainmentType.Contains)
                {
                    result = ContainmentType.Intersects;
                    return;
                }
            }

            // If we get here, then we know all the points were actually contained, therefore result is Contains
            result = ContainmentType.Contains;
            return;
        }

        // TODO: Implement this
        public ContainmentType Contains(Frustum frustum)
        {
            if (this == frustum)                // We check to see if the two frustums are equal
                return ContainmentType.Contains;// If they are, there's no need to go any further.

            throw new NotImplementedException();
        }

        public ContainmentType Contains(Sphere sphere)
        {
            ContainmentType result;
            this.Contains(ref sphere, out result);
            return result;
        }

        public void Contains(ref Sphere sphere, out ContainmentType result)
        {
            float val;
            ContainmentType contained;

            // We first check if the sphere is inside the frustum
            this.Contains(ref sphere.Center, out contained);

            // The sphere is inside. Now we need to check if it's fully contained or not
            // So we see if the perpendicular distance to each plane is less than or equal to the sphere's radius.
            // If the perpendicular distance is less, just return Intersects.
            if (contained == ContainmentType.Contains)
            {
                val = Plane.PerpendicularDistance(ref sphere.Center, this.bottom);
                if (val < sphere.Radius)
                {
                    result = ContainmentType.Intersects;
                    return;
                }

                val = Plane.PerpendicularDistance(ref sphere.Center, this.far);
                if (val < sphere.Radius)
                {
                    result = ContainmentType.Intersects;
                    return;
                }

                val = Plane.PerpendicularDistance(ref sphere.Center, this.left);
                if (val < sphere.Radius)
                {
                    result = ContainmentType.Intersects;
                    return;
                }

                val = Plane.PerpendicularDistance(ref sphere.Center, this.near);
                if (val < sphere.Radius)
                {
                    result = ContainmentType.Intersects;
                    return;
                }

                val = Plane.PerpendicularDistance(ref sphere.Center, this.right);
                if (val < sphere.Radius)
                {
                    result = ContainmentType.Intersects;
                    return;
                }

                val = Plane.PerpendicularDistance(ref sphere.Center, this.top);
                if (val < sphere.Radius)
                {
                    result = ContainmentType.Intersects;
                    return;
                }

                // If we get here, the sphere is fully contained
                result = ContainmentType.Contains;
                return;
            }
            //duff idea : test if all corner is in same side of a plane if yes and outside it is disjoint else intersect
            // issue is that we can have some times when really close aabb 



            // If we're here, the the sphere's centre was outside of the frustum. This makes things hard :(
            // We can't use perpendicular distance anymore. I'm not sure how to code this.
            throw new NotImplementedException();
        }

        public ContainmentType Contains(Vector3F point)
        {
            ContainmentType result;
            this.Contains(ref point, out result);
            return result;
        }

        public void Contains(ref Vector3F point, out ContainmentType result)
        {
            float val;
            // If a point is on the POSITIVE side of the plane, then the point is not contained within the frustum

            // Check the top
            val = Plane.ClassifyPoint(ref point, this.top);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the bottom
            val = Plane.ClassifyPoint(ref point, this.bottom);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the left
            val = Plane.ClassifyPoint(ref point, this.left);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the right
            val = Plane.ClassifyPoint(ref point,  this.right);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the near
            val = Plane.ClassifyPoint(ref point, this.near);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the far
            val = Plane.ClassifyPoint(ref point, this.far);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // If we get here, it means that the point was on the correct side of each plane to be
            // contained. Therefore this point is contained
            result = ContainmentType.Contains;
        }


        public ContainmentType Contains(List<Vector3F> polygon)
        {
            ContainmentType result;
            this.Contains(ref polygon, out result);
            return result;
        }

        public void Contains(ref List<Vector3F> polygon, out ContainmentType result)
        {
            if (polygon.Count == 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            if (polygon.Count == 1)
            {
                result = Contains(polygon[0]);
                return;
            }

            int outCount = 0;
            int inCount = 0;

            foreach (Vector3F point in polygon)
            {
                if (Contains(point) == ContainmentType.Contains)
                    inCount++;
                else
                    outCount++;
            }

            if (outCount == 0)
                result = ContainmentType.Contains;
            if (inCount == 0)
                result = ContainmentType.Disjoint;

            result = ContainmentType.Intersects;
        }

        public bool Equals(Frustum other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            Frustum f = obj as Frustum;
            return (object.Equals(f, null)) ? false : (this == f);
        }

        public Vector3F[] GetCorners()
        {
            return corners;
        }

        public override int GetHashCode()
        {
            return this.matrix.GetHashCode();
        }

        public bool Intersects(AxisAlignedBox box)
        {
            foreach (Plane plane in FrustumHelper.GetPlanes(this))
            {
                if ((plane.Normal.X * box.Min.X) + (plane.Normal.Y * box.Min.Y) + (plane.Normal.Z * box.Min.Z) + plane.D > 0)
                    continue;

                if ((plane.Normal.X * box.Max.X) + (plane.Normal.Y * box.Min.Y) + (plane.Normal.Z * box.Min.Z) + plane.D > 0)
                    continue;

                if ((plane.Normal.X * box.Max.X) + (plane.Normal.Y * box.Min.Y) + (plane.Normal.Z * box.Max.Z) + plane.D > 0)
                    continue;

                if ((plane.Normal.X * box.Min.X) + (plane.Normal.Y * box.Min.Y) + (plane.Normal.Z * box.Max.Z) + plane.D > 0)
                    continue;

                if ((plane.Normal.X * box.Min.X) + (plane.Normal.Y * box.Max.Y) + (plane.Normal.Z * box.Min.Z) + plane.D > 0)
                    continue;

                if ((plane.Normal.X * box.Max.X) + (plane.Normal.Y * box.Max.Y) + (plane.Normal.Z * box.Min.Z) + plane.D > 0)
                    continue;

                if ((plane.Normal.X * box.Max.X) + (plane.Normal.Y * box.Max.Y) + (plane.Normal.Z * box.Max.Z) + plane.D > 0)
                    continue;

                if ((plane.Normal.X * box.Min.X) + (plane.Normal.Y * box.Max.Y) + (plane.Normal.Z * box.Max.Z) + plane.D > 0)
                    continue;

                // all points are behind the one plane so they can't be inside any other plane
                return false;
            }

            return true;
        }

        public bool Intersects(IEnumerable<Vector3F> polygon)
        {
            foreach (Plane plane in FrustumHelper.GetPlanes(this))
            {
                bool gotOne = false;
                foreach (Vector3F vert in polygon)
                {
                    if ((plane.Normal.X * vert.X) + (plane.Normal.Y * vert.Y) + (plane.Normal.Z * vert.Z) + plane.D > 0)
                    {
                        gotOne = true;
                        break;
                    }
                }
                if (gotOne)
                    continue;

                // all points are behind the one plane so they can't be inside any other plane
                return false;
            }
            return true;
        }

        public void Intersects(ref AxisAlignedBox box, out bool result)
        {
            result = Intersects(box);
        }

        public void Intersects(ref IEnumerable<Vector3F> polygon, out bool result)
        {
            result = Intersects(polygon);
        }

        public bool Intersects(Frustum frustum)
        {
            return Contains(frustum) != ContainmentType.Disjoint;
        }

        public bool Intersects(Sphere sphere)
        {
            return Contains(sphere) != ContainmentType.Disjoint;
        }

        public void Intersects(ref Sphere sphere, out bool result)
        {
            result = Contains(sphere) != ContainmentType.Disjoint;
        }

        public PlaneIntersectionType Intersects(Plane plane)
        {
            throw new NotImplementedException();
        }

        public void Intersects(ref Plane plane, out PlaneIntersectionType result)
        {
            throw new NotImplementedException();
        }

        public Nullable<float> Intersects(Ray ray)
        {
            throw new NotImplementedException();
        }

        public void Intersects(ref Ray ray, out Nullable<float> result)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append("{Near:");
            sb.Append(this.near.ToString());
            sb.Append(" Far:");
            sb.Append(this.far.ToString());
            sb.Append(" Left:");
            sb.Append(this.left.ToString());
            sb.Append(" Right:");
            sb.Append(this.right.ToString());
            sb.Append(" Top:");
            sb.Append(this.top.ToString());
            sb.Append(" Bottom:");
            sb.Append(this.bottom.ToString());
            sb.Append("}");
            return sb.ToString();
        }

        #endregion Public Methods

        #region Protected Methods

        protected void CreateCorners()
        {
            this.corners = new Vector3F[8];
            this.corners[0] = IntersectionPoint(ref this.near, ref this.left, ref this.top);
            this.corners[1] = IntersectionPoint(ref this.near, ref this.right, ref this.top);
            this.corners[2] = IntersectionPoint(ref this.near, ref this.right, ref this.bottom);
            this.corners[3] = IntersectionPoint(ref this.near, ref this.left, ref this.bottom);
            this.corners[4] = IntersectionPoint(ref this.far, ref this.left, ref this.top);
            this.corners[5] = IntersectionPoint(ref this.far, ref this.right, ref this.top);
            this.corners[6] = IntersectionPoint(ref this.far, ref this.right, ref this.bottom);
            this.corners[7] = IntersectionPoint(ref this.far, ref this.left, ref this.bottom);
        }

        protected void CreatePlanes()
        {
            // Pre-calculate the different planes needed
            this.left = new Plane(-matrix.M14 - matrix.M11, -matrix.M24 - matrix.M21,
                                  -matrix.M34 - matrix.M31, -matrix.M44 - matrix.M41);

            this.right = new Plane(matrix.M11 - matrix.M14, matrix.M21 - matrix.M24,
                                   matrix.M31 - matrix.M34, matrix.M41 - matrix.M44);

            this.top = new Plane(matrix.M12 - matrix.M14, matrix.M22 - matrix.M24,
                                 matrix.M32 - matrix.M34, matrix.M42 - matrix.M44);

            this.bottom = new Plane(-matrix.M14 - matrix.M12, -matrix.M24 - matrix.M22,
                                    -matrix.M34 - matrix.M32, -matrix.M44 - matrix.M42);

            this.near = new Plane(-matrix.M13, -matrix.M23, -matrix.M33, -matrix.M43);


            this.far = new Plane(matrix.M13 - matrix.M14, matrix.M23 - matrix.M24,
                                 matrix.M33 - matrix.M34, matrix.M43 - matrix.M44);

            this.NormalizePlane(ref this.left);
            this.NormalizePlane(ref this.right);
            this.NormalizePlane(ref this.top);
            this.NormalizePlane(ref this.bottom);
            this.NormalizePlane(ref this.near);
            this.NormalizePlane(ref this.far);
        }

        protected static Vector3F IntersectionPoint(ref Plane a, ref Plane b, ref Plane c)
        {
            // Formula used
            //                d1 ( N2 * N3 ) + d2 ( N3 * N1 ) + d3 ( N1 * N2 )
            //P =   -------------------------------------------------------------------------
            //                             N1 . ( N2 * N3 )
            //
            // Note: N refers to the normal, d refers to the displacement. '.' means dot product. '*' means cross product

            Vector3F v1, v2, v3;
            float f = -Vector3F.Dot(a.Normal, Vector3F.Cross(b.Normal, c.Normal));

            v1 = (a.D * (Vector3F.Cross(b.Normal, c.Normal)));
            v2 = (b.D * (Vector3F.Cross(c.Normal, a.Normal)));
            v3 = (c.D * (Vector3F.Cross(a.Normal, b.Normal)));

            Vector3F vec = new Vector3F(v1.X + v2.X + v3.X, v1.Y + v2.Y + v3.Y, v1.Z + v2.Z + v3.Z);
            return vec / f;
        }

        protected void NormalizePlane(ref Plane p)
        {
            float factor = 1f / p.Normal.Length;
            p.Normal.X *= factor;
            p.Normal.Y *= factor;
            p.Normal.Z *= factor;
            p.D *= factor;
        }

        #endregion
    }

    public class FrustumHelper
    {
        public static Plane[] GetPlanes(Frustum frustum)
        {
            Plane[] l = new Plane[6];
            l[0] = frustum.Near;
            l[1] = frustum.Left;
            l[2] = frustum.Right;
            l[3] = frustum.Top;
            l[4] = frustum.Bottom;
            l[5] = frustum.Far;

            return l;
        }
    }
}