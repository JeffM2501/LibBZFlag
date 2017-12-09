using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.LinearMath;

namespace BZFlag.LinearMath.Geometry
{ 
    [Serializable]
    public class AxisAlignedBox : IEquatable<AxisAlignedBox>
    {
        #region Public Fields

        public Vector3F Min = Vector3F.Zero;
        public Vector3F Max = Vector3F.Zero;

        #endregion Public Fields

        #region Public Constructors
        public AxisAlignedBox() { }


        public AxisAlignedBox(Vector3F min, Vector3F max)
        {
            this.Min = min;
            this.Max = max;
        }


        #endregion Public Constructors

        #region Public Methods

        public static AxisAlignedBox Empty = new AxisAlignedBox(new Vector3F(0, 0, 0), new Vector3F(0, 0, 0));

        public ContainmentType Contains(AxisAlignedBox box)
        {
            //test if all corner is in the same side of a face by just checking min and max
            if (box.Max.X < Min.X
                || box.Min.X > Max.X
                || box.Max.Y < Min.Y
                || box.Min.Y > Max.Y
                || box.Max.Z < Min.Z
                || box.Min.Z > Max.Z)
                return ContainmentType.Disjoint;

            if (box.Min.X >= Min.X
                && box.Max.X <= Max.X
                && box.Min.Y >= Min.Y
                && box.Max.Y <= Max.Y
                && box.Min.Z >= Min.Z
                && box.Max.Z <= Max.Z)
                return ContainmentType.Contains;


            return ContainmentType.Intersects;
        }


        public void Contains(ref AxisAlignedBox box, out ContainmentType result)
        {
            result = Contains(box);
        }


        public ContainmentType Contains(Frustum frustum)
        {
            //TODO: bad done here need a fix. 
            //Because question is not frustum contain box but reverse and this is not the same
            int i;
            ContainmentType contained;
            Vector3F[] corners = frustum.GetCorners();


            // First we check if frustum is in box
            for (i = 0; i < corners.Length; i++)
            {
                this.Contains(ref corners[i], out contained);
                if (contained == ContainmentType.Disjoint)
                    break;
            }


            if (i == corners.Length) // This means we checked all the corners and they were all contain or instersect
                return ContainmentType.Contains;


            if (i != 0)             // if i is not equal to zero, we can fastpath and say that this box intersects
                return ContainmentType.Intersects;




            // If we get here, it means the first (and only) point we checked was actually contained in the frustum.
            // So we assume that all other points will also be contained. If one of the points is disjoint, we can
            // exit immediately saying that the result is Intersects
            i++;
            for (; i < corners.Length; i++)
            {
                this.Contains(ref corners[i], out contained);
                if (contained != ContainmentType.Contains)
                    return ContainmentType.Intersects;


            }


            // If we get here, then we know all the points were actually contained, therefore result is Contains
            return ContainmentType.Contains;
        }


        public ContainmentType Contains(Sphere sphere)
        {
            if (sphere.Center.X - Min.X > sphere.Radius
                && sphere.Center.Y - Min.Y > sphere.Radius
                && sphere.Center.Z - Min.Z > sphere.Radius
                && Max.X - sphere.Center.X > sphere.Radius
                && Max.Y - sphere.Center.Y > sphere.Radius
                && Max.Z - sphere.Center.Z > sphere.Radius)
                return ContainmentType.Contains;


            double dmin = 0;


            if (sphere.Center.X - Min.X <= sphere.Radius)
                dmin += (sphere.Center.X - Min.X) * (sphere.Center.X - Min.X);
            else if (Max.X - sphere.Center.X <= sphere.Radius)
                dmin += (sphere.Center.X - Max.X) * (sphere.Center.X - Max.X);
            if (sphere.Center.Y - Min.Y <= sphere.Radius)
                dmin += (sphere.Center.Y - Min.Y) * (sphere.Center.Y - Min.Y);
            else if (Max.Y - sphere.Center.Y <= sphere.Radius)
                dmin += (sphere.Center.Y - Max.Y) * (sphere.Center.Y - Max.Y);
            if (sphere.Center.Z - Min.Z <= sphere.Radius)
                dmin += (sphere.Center.Z - Min.Z) * (sphere.Center.Z - Min.Z);
            else if (Max.Z - sphere.Center.Z <= sphere.Radius)
                dmin += (sphere.Center.Z - Max.Z) * (sphere.Center.Z - Max.Z);


            if (dmin <= sphere.Radius * sphere.Radius)
                return ContainmentType.Intersects;


            return ContainmentType.Disjoint;
        }


        public void Contains(ref Sphere sphere, out ContainmentType result)
        {
            result = this.Contains(sphere);
        }


        public ContainmentType Contains(Vector3F point)
        {
            ContainmentType result;
            this.Contains(ref point, out result);
            return result;
        }


        public void Contains(ref Vector3F point, out ContainmentType result)
        {
            //first we get if point is out of box
            if (point.X < this.Min.X
                || point.X > this.Max.X
                || point.Y < this.Min.Y
                || point.Y > this.Max.Y
                || point.Z < this.Min.Z
                || point.Z > this.Max.Z)
            {
                result = ContainmentType.Disjoint;
            }//or if point is on box because coordonate of point is lesser or equal
            else if (point.X == this.Min.X
                || point.X == this.Max.X
                || point.Y == this.Min.Y
                || point.Y == this.Max.Y
                || point.Z == this.Min.Z
                || point.Z == this.Max.Z)
                result = ContainmentType.Intersects;
            else
                result = ContainmentType.Contains;
        }


        public static AxisAlignedBox CreateFromPoints(IEnumerable<Vector3F> points)
        {
            if (points == null)
                throw new ArgumentNullException();

            // TODO: Just check that Count > 0
            bool empty = true;
            Vector3F vector2 = Vector3F.Zero;
            Vector3F vector1 = Vector3F.Zero;
            foreach (Vector3F Vector3F in points)
            {
                if (vector2 == Vector3F.Zero)
                {
                    vector2 = new Vector3F(Vector3F);
                    vector1 = new Vector3F(Vector3F);
                }
                else
                {
                    vector2 = Vector3F.Min(vector2, Vector3F);
                    vector1 = Vector3F.Max(vector1, Vector3F);

                }
                empty = false;
            }
            if (empty)
                throw new ArgumentException();

            return new AxisAlignedBox(vector2, vector1);
        }


        public static AxisAlignedBox CreateFromSphere(Sphere sphere)
        {
            Vector3F vector1 = new Vector3F(sphere.Radius, sphere.Radius, sphere.Radius);
            return new AxisAlignedBox(sphere.Center - vector1, sphere.Center + vector1);
        }


        public static void CreateFromSphere(ref Sphere sphere, out AxisAlignedBox result)
        {
            result = AxisAlignedBox.CreateFromSphere(sphere);
        }


        public static AxisAlignedBox CreateFromCylinderXY(CylinderXY cylinder)
        {
            Vector3F vector1 = new Vector3F(cylinder.Radius, cylinder.Radius, cylinder.Radius);
            AxisAlignedBox box = new AxisAlignedBox(new Vector3F(cylinder.Center) - vector1, new Vector3F(cylinder.Center) + vector1);
            box.Max.Z = cylinder.MaxZ;
            box.Min.Z = cylinder.MinZ;
            return box;
        }


        public static void CreateFromCylinderXY(ref CylinderXY cylinder, out AxisAlignedBox result)
        {
            result = AxisAlignedBox.CreateFromCylinderXY(cylinder);
        }


        public static AxisAlignedBox CreateMerged(AxisAlignedBox original, AxisAlignedBox additional)
        {
            return new AxisAlignedBox(
                Vector3F.Min(original.Min, additional.Min), Vector3F.Max(original.Max, additional.Max));
        }


        public static void CreateMerged(ref AxisAlignedBox original, ref AxisAlignedBox additional, out AxisAlignedBox result)
        {
            result = AxisAlignedBox.CreateMerged(original, additional);
        }


        public bool Equals(AxisAlignedBox other)
        {
            return (this.Min == other.Min) && (this.Max == other.Max);
        }


        public override bool Equals(object obj)
        {
            return (obj is AxisAlignedBox) ? this.Equals((AxisAlignedBox)obj) : false;
        }


        public Vector3F[] GetCorners()
        {
            return new Vector3F[] {
                new Vector3F(this.Min.X, this.Max.Y, this.Max.Z),
                new Vector3F(this.Max.X, this.Max.Y, this.Max.Z),
                new Vector3F(this.Max.X, this.Min.Y, this.Max.Z),
                new Vector3F(this.Min.X, this.Min.Y, this.Max.Z),
                new Vector3F(this.Min.X, this.Max.Y, this.Min.Z),
                new Vector3F(this.Max.X, this.Max.Y, this.Min.Z),
                new Vector3F(this.Max.X, this.Min.Y, this.Min.Z),
                new Vector3F(this.Min.X, this.Min.Y, this.Min.Z)
            };
        }


        public override int GetHashCode()
        {
            return this.Min.GetHashCode() + this.Max.GetHashCode();
        }


        public bool Intersects(AxisAlignedBox box)
        {
            bool result;
            Intersects(ref box, out result);
            return result;
        }


        public void Intersects(ref AxisAlignedBox box, out bool result)
        {
            if ((this.Max.X >= box.Min.X) && (this.Min.X <= box.Max.X))
            {
                if ((this.Max.Y < box.Min.Y) || (this.Min.Y > box.Max.Y))
                {
                    result = false;
                    return;
                }


                result = (this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z);
                return;
            }


            result = false;
            return;
        }


        public bool Intersects(Frustum frustum)
        {
            return frustum.Contains(this) != ContainmentType.Disjoint;
        }


        public bool Intersects(Sphere sphere)
        {
            if (sphere.Center.X - Min.X > sphere.Radius
                && sphere.Center.Y - Min.Y > sphere.Radius
                && sphere.Center.Z - Min.Z > sphere.Radius
                && Max.X - sphere.Center.X > sphere.Radius
                && Max.Y - sphere.Center.Y > sphere.Radius
                && Max.Z - sphere.Center.Z > sphere.Radius)
                return true;

            double dmin = 0;

            if (sphere.Center.X - Min.X <= sphere.Radius)
                dmin += (sphere.Center.X - Min.X) * (sphere.Center.X - Min.X);
            else if (Max.X - sphere.Center.X <= sphere.Radius)
                dmin += (sphere.Center.X - Max.X) * (sphere.Center.X - Max.X);


            if (sphere.Center.Y - Min.Y <= sphere.Radius)
                dmin += (sphere.Center.Y - Min.Y) * (sphere.Center.Y - Min.Y);
            else if (Max.Y - sphere.Center.Y <= sphere.Radius)
                dmin += (sphere.Center.Y - Max.Y) * (sphere.Center.Y - Max.Y);


            if (sphere.Center.Z - Min.Z <= sphere.Radius)
                dmin += (sphere.Center.Z - Min.Z) * (sphere.Center.Z - Min.Z);
            else if (Max.Z - sphere.Center.Z <= sphere.Radius)
                dmin += (sphere.Center.Z - Max.Z) * (sphere.Center.Z - Max.Z);

            if (dmin <= sphere.Radius * sphere.Radius)
                return true;

            return false;
        }


        public void Intersects(ref Sphere sphere, out bool result)
        {
            result = Intersects(sphere);
        }

        public PlaneIntersectionType Intersects(Plane plane)
        {
            //check all corner side of plane
            Vector3F[] corners = this.GetCorners();
            float lastdistance = Vector3F.Dot(plane.Normal, corners[0]) + plane.D;

            for (int i = 1; i < corners.Length; i++)
            {
                float distance = Vector3F.Dot(plane.Normal, corners[i]) + plane.D;
                if ((distance <= 0.0f && lastdistance > 0.0f) || (distance >= 0.0f && lastdistance < 0.0f))
                    return PlaneIntersectionType.Intersecting;
                lastdistance = distance;
            }

            if (lastdistance > 0.0f)
                return PlaneIntersectionType.Front;

            return PlaneIntersectionType.Back;
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


        public static bool operator ==(AxisAlignedBox a, AxisAlignedBox b)
        {
            return a.Equals(b);
        }


        public static bool operator !=(AxisAlignedBox a, AxisAlignedBox b)
        {
            return !a.Equals(b);
        }


        public override string ToString()
        {
            return string.Format("{{Min:{0} Max:{1}}}", this.Min.ToString(), this.Max.ToString());
        }


        #endregion Public Methods
    }
}
