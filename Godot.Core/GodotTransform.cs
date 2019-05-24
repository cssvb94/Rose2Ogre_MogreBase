using System;

namespace Godot
{
    public struct GodotTransform : IEquatable<GodotTransform>
    {
        public GodotBasis basis;

        public GodotVector3 origin;

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

        public GodotTransform Orthonormalized()
        {
            return new GodotTransform(basis.Orthonormalized(), origin);
        }

        public GodotTransform Rotated(GodotVector3 axis, float phi)
        {
            return new GodotTransform(new GodotBasis(axis, phi), new GodotVector3()) * this;
        }

        public GodotTransform Scaled(GodotVector3 scale)
        {
            return new GodotTransform(basis.Scaled(scale), origin * scale);
        }

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
            double num1 = local1[0];
            double num2 = this.basis[0].Dot(ofs);
            double num3;
            float num4 = (float)(num3 = num1 + num2);
            local2[0] = (float)num3;
            double num5 = num4;
            ref GodotVector3 local3 = ref this.origin;
            ref GodotVector3 local4 = ref local3;
            double num6 = local3[1];
            double num7 = this.basis[1].Dot(ofs);
            double num8;
            float num9 = (float)(num8 = num6 + num7);
            local4[1] = (float)num8;
            double num10 = num9;
            ref GodotVector3 local5 = ref this.origin;
            ref GodotVector3 local6 = ref local5;
            double num11 = local5[2];
            double num12 = this.basis[2].Dot(ofs);
            double num13;
            float num14 = (float)(num13 = num11 + num12);
            local6[2] = (float)num13;
            double num15 = num14;
            GodotVector3 origin = new GodotVector3((float)num5, (float)num10, (float)num15);
            return new GodotTransform(basis, origin);
        }

        public GodotVector3 Xform(GodotVector3 v)
        {
            GodotVector3 basi = basis[0];
            double num1 = basi.Dot(v) + (double)origin.x;
            basi = basis[1];
            double num2 = basi.Dot(v) + (double)origin.y;
            basi = basis[2];
            double num3 = basi.Dot(v) + (double)origin.z;
            return new GodotVector3((float)num1, (float)num2, (float)num3);
        }

        public GodotVector3 XformInv(GodotVector3 v)
        {
            GodotVector3 vector3 = v - origin;
            return new GodotVector3((basis[0, 0] * vector3.x) + (basis[1, 0] * vector3.y) + (basis[2, 0] * vector3.z), (basis[0, 1] * vector3.x) + (basis[1, 1] * vector3.y) + (basis[2, 1] * vector3.z), (basis[0, 2] * vector3.x) + (basis[1, 2] * vector3.y) + (basis[2, 2] * vector3.z));
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

        public static bool operator ==(GodotTransform left, GodotTransform right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GodotTransform left, GodotTransform right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is GodotTransform)
                return Equals((GodotTransform)obj);
            return false;
        }

        public bool Equals(GodotTransform other)
        {
            if (basis.Equals(other.basis))
                return origin.Equals(other.origin);
            return false;
        }

        public override int GetHashCode()
        {
            return basis.GetHashCode() ^ origin.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", new object[2]
            {
             basis.ToString(),
             origin.ToString()
            });
        }

        public string ToString(string format)
        {
            return string.Format("{0} - {1}", new object[2]
            {
             basis.ToString(format),
             origin.ToString(format)
            });
        }
    }
}
