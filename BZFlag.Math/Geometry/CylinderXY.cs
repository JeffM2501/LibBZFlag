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
    public class CylinderXY : IEquatable<CylinderXY>
    {
        #region Public Fields

        public Vector2F Center = new Vector2F();
        public float MaxZ;
        public float MinZ;
        public float Radius;

        #endregion Public Fields

        #region Public Constructors

        public CylinderXY(Vector3F cp, float height, float radius)
        {
            Center.X = cp.X;
            Center.Y = cp.Y;

            Radius = radius;

            MinZ = cp.Z;
            MaxZ = cp.Z + height;
        }

        #endregion Public Constructors

        #region Public Methods

        public static CylinderXY Empty = new CylinderXY(new Vector3F(0, 0, 0), 0, 0);

        public ContainmentType Contains(AxisAlignedBox box)
        {
            // above or below
            if (box.Min.Z > MaxZ || box.Max.Z < MinZ)
                return ContainmentType.Disjoint;

            // for containment it MUST fit in Z
            if (MaxZ <= box.Max.Z && MinZ >= box.Min.Z)
            {
                if (!pointInXY(box.Max.X, box.Max.Y) || !pointInXY(box.Min.X, box.Max.Y) || !pointInXY(box.Min.X, box.Min.Y) || !pointInXY(box.Max.X, box.Min.Y))
                    return ContainmentType.Intersects;

                return ContainmentType.Contains;
            }

            if (!pointInXY(box.Max.X, box.Max.Y) || !pointInXY(box.Min.X, box.Max.Y) || !pointInXY(box.Min.X, box.Min.Y) || !pointInXY(box.Max.X, box.Min.Y))
                return ContainmentType.Disjoint;

            return ContainmentType.Intersects;
        }

        public bool Equals(CylinderXY other)
        {
            return this.Center == other.Center && this.Radius == other.Radius && this.MinZ == other.MinZ && this.MaxZ == other.MaxZ;
        }

        public override bool Equals(object obj)
        {
            if (obj is CylinderXY)
                return this.Equals((CylinderXY)obj);

            return false;
        }

        public override int GetHashCode()
        {
            return this.Center.GetHashCode() + this.Radius.GetHashCode() + this.MaxZ.GetHashCode() + this.MinZ.GetHashCode();
        }

        public static bool operator ==(CylinderXY a, CylinderXY b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(CylinderXY a, CylinderXY b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{Center:{0} Radius:{1} MinZ:{2} MaxZ:{3}}}", this.Center.ToString(), this.Radius.ToString(), this.MinZ.ToString(), this.MaxZ.ToString());
        }

        #endregion Public Methods

        #region Private Methods

        bool pointInXY(float X, float Y)
        {
            float distSquare = (X - Center.X) * (X - Center.X) + (Y - Center.Y) * (Y - Center.Y);
            return distSquare <= Radius * Radius;
        }

        #endregion Private Methods
    }
}