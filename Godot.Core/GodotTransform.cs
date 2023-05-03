using System;

namespace Godot
{
    public struct GodotTransform : IEquatable<GodotTransform>
    {
        public GodotBasis basis;

        public GodotVector3 origin;

        public static GodotTransform IDENTITY => new GodotTransform(new GodotVector3(1, 0, 0), new GodotVector3(0, 1, 0), new GodotVector3(0, 0, 1), new GodotVector3(0, 0, 0));

        public GodotTransform AffineInverse()
        {
            GodotBasis basis = this.basis.Inverse();
            return new GodotTransform(basis, basis.Xform(-origin));
        }

        public GodotTransform Inverse()
        {
            GodotBasis basis = this.basis.Transposed();
            return new GodotTransform(basis, basis.Xform(-origin));
        }

        public GodotTransform LookingAt(GodotVector3 target, GodotVector3 up)
        {
            GodotTransform transform = this;
            transform.SetLookAt(origin, target, up);
            return transform;
        }

        public GodotTransform Orthonormalized() => new GodotTransform(basis.Orthonormalized(), origin);

        public GodotTransform Rotated(GodotVector3 axis, float phi) => new GodotTransform(new GodotBasis(axis, phi), new GodotVector3()) * this;

        public GodotTransform Scaled(GodotVector3 scale) => new GodotTransform(basis.Scaled(scale), origin * scale);

        public void SetLookAt(GodotVector3 eye, GodotVector3 target, GodotVector3 up)
        {
            GodotVector3 vector3_1 = eye - target;
            vector3_1.Normalize();
            GodotVector3 vector3_2 = up.Cross(vector3_1);
            GodotVector3 yAxis = vector3_1.Cross(vector3_2);
            vector3_2.Normalize();
            yAxis.Normalize();
            basis = GodotBasis.CreateFromAxes(vector3_2, yAxis, vector3_1);
            origin = eye;
        }

        public GodotTransform Translated(GodotVector3 ofs)
        {
            GodotBasis basis = this.basis;
            ref GodotVector3 local1 = ref this.origin;
            ref GodotVector3 local2 = ref local1;
            float num1 = local1[0];
            float num2 = this.basis[0].Dot(ofs);
            float num3;
            float num4 = (float)(num3 = num1 + num2);
            local2[0] = (float)num3;
            float num5 = num4;
            ref GodotVector3 local3 = ref this.origin;
            ref GodotVector3 local4 = ref local3;
            float num6 = local3[1];
            float num7 = this.basis[1].Dot(ofs);
            float num8;
            float num9 = (float)(num8 = num6 + num7);
            local4[1] = (float)num8;
            float num10 = num9;
            ref GodotVector3 local5 = ref this.origin;
            ref GodotVector3 local6 = ref local5;
            float num11 = local5[2];
            float num12 = this.basis[2].Dot(ofs);
            float num13;
            float num14 = (float)(num13 = num11 + num12);
            local6[2] = (float)num13;
            float num15 = num14;
            GodotVector3 origin = new GodotVector3((float)num5, (float)num10, (float)num15);
            return new GodotTransform(basis, origin);
        }

        public GodotVector3 Xform(GodotVector3 v)
        {
            GodotVector3 basis_vector = basis[0];
            float xform_x = basis_vector.Dot(v) + origin.x;
            basis_vector = basis[1];
            float xform_y2 = basis_vector.Dot(v) + origin.y;
            basis_vector = basis[2];
            float xform_z = basis_vector.Dot(v) + origin.z;
            return new GodotVector3(xform_x, xform_y2, xform_z);
        }

        public GodotVector3 XformInv(GodotVector3 v)
        {
            GodotVector3 vector = v - origin;
            return new GodotVector3((basis[0, 0] * vector.x) + (basis[1, 0] * vector.y) + (basis[2, 0] * vector.z), (basis[0, 1] * vector.x) + (basis[1, 1] * vector.y) + (basis[2, 1] * vector.z), (basis[0, 2] * vector.x) + (basis[1, 2] * vector.y) + (basis[2, 2] * vector.z));
        }

        public GodotTransform(
            float cxx, float cyx, float czx,
            float cxy, float cyy, float czy,
            float cxz, float cyz, float czz,
            float originx, float originy, float originz)
        {
            GodotVector3 xaxis = new GodotVector3(cxx, cxy, cxz);
            GodotVector3 yaxis = new GodotVector3(cyx, cyy, cyz);
            GodotVector3 zaxis = new GodotVector3(czx, czy, czz);
            GodotVector3 origin = new GodotVector3(originx, originy, originz);
            basis = GodotBasis.CreateFromAxes(xaxis, yaxis, zaxis);
            this.origin = origin;
        }

        public GodotTransform(GodotVector3 xAxis, GodotVector3 yAxis, GodotVector3 zAxis, GodotVector3 origin)
        {
            basis = GodotBasis.CreateFromAxes(xAxis, yAxis, zAxis);
            this.origin = origin;
        }

        public GodotTransform(GodotQuat quat, GodotVector3 origin)
        {
            basis = new GodotBasis(quat);
            this.origin = origin;
        }

        public GodotTransform(GodotBasis basis, GodotVector3 origin)
        {
            this.basis = basis;
            this.origin = origin;
        }

        public static GodotTransform operator *(GodotTransform left, GodotTransform right)
        {
            left.origin = left.Xform(right.origin);
            left.basis *= right.basis;
            return left;
        }

        public static bool operator ==(GodotTransform left, GodotTransform right) => left.Equals(right);

        public static bool operator !=(GodotTransform left, GodotTransform right) => !left.Equals(right);

        public override bool Equals(object obj) => obj is GodotTransform transform && Equals(transform);

        public bool Equals(GodotTransform other) => basis.Equals(other.basis) && origin.Equals(other.origin);

        public override int GetHashCode() => basis.GetHashCode() ^ origin.GetHashCode();

        public override string ToString() => $"({basis.ToStringNoBrackets()}, {origin.ToStringNoBrackets()})";

        public string ToString(string format) => $"{basis.ToString(format)}, {origin.ToString(format)}";
    }
}
