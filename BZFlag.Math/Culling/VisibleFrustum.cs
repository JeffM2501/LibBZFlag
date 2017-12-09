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
using System.Linq;
using System.Text;

using BZFlag.LinearMath;
using BZFlag.LinearMath.Geometry;

namespace BZFlag.LinearMath.Culling
{
    public class VisibleFrustum : Frustum
    {
        #region Properties
        public Matrix4F view = new Matrix4F();
        public Matrix4F ViewMatrix
        {
            get { return view; }
        }
        public Matrix4F projection = new Matrix4F();
        public Matrix4F ProjectionMatrix
        {
            get { return projection; }
        }

        Matrix4F billboard = new Matrix4F();
        public Matrix4F BillboardMatrix
        {
            get { return billboard; }
        }

        bool zIsUp = true;
        public bool ZIsUp
        {
            get { return ZIsUp; }
            set { zIsUp = ZIsUp; BuildFrustum(); }
        }
        #endregion

        #region Protected Variables
        protected Vector3F RightVec = new Vector3F();
        protected Vector3F Up = new Vector3F();
        protected Vector3F ViewDir = new Vector3F();
        protected Vector3F EyePoint = new Vector3F();

        protected float nearClip = 0;
        protected float farClip = 0;

        protected Vector3F[] edge;
        #endregion

        #region Public Constructors

        public VisibleFrustum()
            : base(Matrix4F.Identity)
        {
            BuildFrustum();
        }

        public VisibleFrustum(VisibleFrustum value)
            : base(value.matrix)
        {
            projection = new Matrix4F(value.projection.Row0, value.projection.Row1, value.projection.Row2, value.projection.Row3);
            view = new Matrix4F(value.view.Row0, value.view.Row1, value.view.Row2, value.view.Row3);
            matrix = new Matrix4F(value.matrix.Row0, value.matrix.Row1, value.matrix.Row2, value.matrix.Row3);
            billboard = new Matrix4F(value.billboard.Row0, value.billboard.Row1, value.billboard.Row2, value.billboard.Row3);

            zIsUp = value.zIsUp;
            nearClip = value.nearClip;
            farClip = value.farClip;

            ViewDir = new Vector3F(value.ViewDir);
            EyePoint = new Vector3F(value.EyePoint);
            RightVec = new Vector3F(value.RightVec);
            Up = new Vector3F(value.Up);

            this.left = new Plane(value.left.Normal, value.left.D);
            this.right = new Plane(value.right.Normal, value.right.D);
            this.top = new Plane(value.top.Normal, value.top.D);
            this.bottom = new Plane(value.bottom.Normal, value.bottom.D);
            this.near = new Plane(value.near.Normal, value.near.D);
            this.far = new Plane(value.far.Normal, value.far.D);

            edge = new Vector3F[4];
            edge[0] = new Vector3F(value.edge[0]);
            edge[1] = new Vector3F(value.edge[1]);
            edge[2] = new Vector3F(value.edge[2]);
            edge[3] = new Vector3F(value.edge[3]);
        }
        #endregion

        #region Public Methods

        public void SetProjection(float fov, float aspect, float hither, float yon, int width, int height)
        {
            nearClip = hither;
            farClip = yon;

            // compute projectionMatrix
            float s = 1.0f / (float)System.Math.Tan(fov / 2.0f);
            float fracHeight = 1.0f - (float)height / (float)height;
            projection.M11 = s;
            projection.M22 = (1.0f - fracHeight) * s * (float)width / (float)height;
            projection.M31 = 0.0f;
            projection.M32 = -fracHeight;
            projection.M33 = -(yon + hither) / (yon - hither);
            projection.M34 = -1.0f;
            projection.M41 = 0.0f;
            projection.M43 = -2.0f * yon * hither / (yon - hither);
            projection.M44 = 0.0f;

            projection.Transpose();

            BuildFrustum();
        }

        public void SetView(Matrix4F mat)
        {
            view = mat;
            BuildFrustum();
        }

        public void LookAt(Vector3F eye, Vector3F target)
        {
            EyePoint = new Vector3F(eye);

            // compute forward vector and normalize
            ViewDir = target - eye;
            ViewDir.Normalize();

            if (!zIsUp)
                throw new NotImplementedException();

            // compute left vector (by crossing forward with
            // world-up [0 0 1]T and normalizing)
            RightVec.X = ViewDir.Y;
            RightVec.Y = -ViewDir.X;
            float rd = 1.0f / (float)Math.Sqrt(RightVec.X * RightVec.X + RightVec.Y * RightVec.Y);
            RightVec.X *= rd;
            RightVec.Y *= rd;
            RightVec.Z = 0.0f;

            // compute local up vector (by crossing right and forward,
            // normalization unnecessary)
            Up.X = RightVec.Y * ViewDir.Z;
            Up.Y = -RightVec.X * ViewDir.Z;
            Up.Z = (RightVec.X * ViewDir.Y) - (RightVec.Y * ViewDir.X);

            // build view matrix, including a transformation bringing
            // world up [0 0 1 0]T to eye up [0 1 0 0]T, world north
            // [0 1 0 0]T to eye forward [0 0 -1 0]T.
            MatrixHelper4.m0(ref view, RightVec.X);
            MatrixHelper4.m4(ref view, RightVec.Y);
            MatrixHelper4.m8(ref view, 0.0f);

            MatrixHelper4.m1(ref view, Up.X);
            MatrixHelper4.m5(ref view, Up.Y);
            MatrixHelper4.m9(ref view, Up.Z);

            MatrixHelper4.m2(ref view, -ViewDir.X);
            MatrixHelper4.m6(ref view, -ViewDir.Y);
            MatrixHelper4.m10(ref view, -ViewDir.Z);

            MatrixHelper4.m12(ref view, -(MatrixHelper4.m0(view) * eye.X +
                               MatrixHelper4.m4(view) * eye.Y +
                               MatrixHelper4.m8(view) * eye.Z));
            MatrixHelper4.m13(ref view, -(MatrixHelper4.m1(view) * eye.X +
                               MatrixHelper4.m5(view) * eye.Y +
                               MatrixHelper4.m9(view) * eye.Z));
            MatrixHelper4.m14(ref view, -(MatrixHelper4.m2(view) * eye.X +
                               MatrixHelper4.m6(view) * eye.Y +
                               MatrixHelper4.m10(view) * eye.Z));

            MatrixHelper4.m15(ref view, 1.0f);

            MatrixHelper4.M11(ref billboard, MatrixHelper4.M11(view));
            MatrixHelper4.M12(ref billboard, MatrixHelper4.M21(view));
            MatrixHelper4.M13(ref billboard, MatrixHelper4.M31(view));
            MatrixHelper4.M21(ref billboard, MatrixHelper4.M12(view));
            MatrixHelper4.M22(ref billboard, MatrixHelper4.M22(view));
            MatrixHelper4.M23(ref billboard, MatrixHelper4.M32(view));
            MatrixHelper4.M31(ref billboard, MatrixHelper4.M13(view));
            MatrixHelper4.M32(ref billboard, MatrixHelper4.M23(view));
            MatrixHelper4.M33(ref billboard, MatrixHelper4.M33(view));

            BuildFrustum();

        }
        #endregion

        #region Protected Methods

        protected void BuildFrustum()
        {
            // save off the composite matrix to the base
            base.matrix = Matrix4F.Mult(view, projection);

            // compute vectors of frustum edges
            float xs = (float)System.Math.Abs(1.0f / projection.Column0.X);
            float ys = (float)System.Math.Abs(1.0f / projection.Column1.Y);
            edge = new Vector3F[4];

            edge[0] = ViewDir - (xs * RightVec) - (ys * Up);
            edge[1] = ViewDir + (xs * RightVec) - (ys * Up);
            edge[2] = ViewDir + (xs * RightVec) + (ys * Up);
            edge[3] = ViewDir - (xs * RightVec) + (ys * Up);

            // make frustum planes
            this.near.Normal = ViewDir;
            this.near.D = -Vector3F.Dot(EyePoint, ViewDir);

            makePlane(edge[0], edge[3], EyePoint, ref this.left);
            makePlane(edge[2], edge[1], EyePoint, ref this.right);
            makePlane(edge[1], edge[0], EyePoint, ref this.bottom);
            makePlane(edge[3], edge[2], EyePoint, ref this.top);

            this.far.Normal = near.Normal * -1;
            this.far.D = near.D + farClip;

            CreateCorners();
        }

        protected void makePlane(Vector3F v1, Vector3F v2, Vector3F eye, ref Plane plane)
        {
            if (plane == Plane.Empty)
                plane = new Plane();

            // get normal by crossing v1 and v2 and normalizing
            plane.Normal = Vector3F.Cross(v1, v2);
            plane.Normal.Normalize();
            plane.D = -Vector3F.Dot(eye, plane.Normal);
        }
        #endregion
    }

}
