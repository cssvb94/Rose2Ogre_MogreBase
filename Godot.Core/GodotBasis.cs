using System;

namespace Godot
{
    public struct GodotBasis : IEquatable<GodotBasis>
    {
        private static readonly GodotBasis identity = new GodotBasis(new GodotVector3(1f, 0.0f, 0.0f), new GodotVector3(0.0f, 1f, 0.0f), new GodotVector3(0.0f, 0.0f, 1f));
        private static readonly GodotBasis[] orthoBases = new GodotBasis[24]
        {
          new GodotBasis(1f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 1f),
          new GodotBasis(0.0f, -1f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f),
          new GodotBasis(-1f, 0.0f, 0.0f, 0.0f, -1f, 0.0f, 0.0f, 0.0f, 1f),
          new GodotBasis(0.0f, 1f, 0.0f, -1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f),
          new GodotBasis(1f, 0.0f, 0.0f, 0.0f, 0.0f, -1f, 0.0f, 1f, 0.0f),
          new GodotBasis(0.0f, 0.0f, 1f, 1f, 0.0f, 0.0f, 0.0f, 1f, 0.0f),
          new GodotBasis(-1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 1f, 0.0f),
          new GodotBasis(0.0f, 0.0f, -1f, -1f, 0.0f, 0.0f, 0.0f, 1f, 0.0f),
          new GodotBasis(1f, 0.0f, 0.0f, 0.0f, -1f, 0.0f, 0.0f, 0.0f, -1f),
          new GodotBasis(0.0f, 1f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, -1f),
          new GodotBasis(-1f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, -1f),
          new GodotBasis(0.0f, -1f, 0.0f, -1f, 0.0f, 0.0f, 0.0f, 0.0f, -1f),
          new GodotBasis(1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, -1f, 0.0f),
          new GodotBasis(0.0f, 0.0f, -1f, 1f, 0.0f, 0.0f, 0.0f, -1f, 0.0f),
          new GodotBasis(-1f, 0.0f, 0.0f, 0.0f, 0.0f, -1f, 0.0f, -1f, 0.0f),
          new GodotBasis(0.0f, 0.0f, 1f, -1f, 0.0f, 0.0f, 0.0f, -1f, 0.0f),
          new GodotBasis(0.0f, 0.0f, 1f, 0.0f, 1f, 0.0f, -1f, 0.0f, 0.0f),
          new GodotBasis(0.0f, -1f, 0.0f, 0.0f, 0.0f, 1f, -1f, 0.0f, 0.0f),
          new GodotBasis(0.0f, 0.0f, -1f, 0.0f, -1f, 0.0f, -1f, 0.0f, 0.0f),
          new GodotBasis(0.0f, 1f, 0.0f, 0.0f, 0.0f, -1f, -1f, 0.0f, 0.0f),
          new GodotBasis(0.0f, 0.0f, 1f, 0.0f, -1f, 0.0f, 1f, 0.0f, 0.0f),
          new GodotBasis(0.0f, 1f, 0.0f, 0.0f, 0.0f, 1f, 1f, 0.0f, 0.0f),
          new GodotBasis(0.0f, 0.0f, -1f, 0.0f, 1f, 0.0f, 1f, 0.0f, 0.0f),
          new GodotBasis(0.0f, -1f, 0.0f, 0.0f, 0.0f, -1f, 1f, 0.0f, 0.0f)
        };

        public GodotVector3 x;
        public GodotVector3 y;
        public GodotVector3 z;

        public static GodotBasis Identity
        {
            get
            {
                return identity;
            }
        }

        public GodotVector3 Scale
        {
            get
            {
                GodotVector3 vector3 = new GodotVector3(this[0, 0], this[1, 0], this[2, 0]);
                double num1 = vector3.Length();
                vector3 = new GodotVector3(this[0, 1], this[1, 1], this[2, 1]);
                double num2 = vector3.Length();
                vector3 = new GodotVector3(this[0, 2], this[1, 2], this[2, 2]);
                double num3 = vector3.Length();
                return new GodotVector3((float)num1, (float)num2, (float)num3);
            }
        }

        public GodotVector3 this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public float this[int index, int axis]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x[axis];
                    case 1:
                        return y[axis];
                    case 2:
                        return z[axis];
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x[axis] = value;
                        break;
                    case 1:
                        y[axis] = value;
                        break;
                    case 2:
                        z[axis] = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        internal static GodotBasis CreateFromAxes(GodotVector3 xAxis, GodotVector3 yAxis, GodotVector3 zAxis)
        {
            return new GodotBasis(new GodotVector3(xAxis.x, yAxis.x, zAxis.x), new GodotVector3(xAxis.y, yAxis.y, zAxis.y), new GodotVector3(xAxis.z, yAxis.z, zAxis.z));
        }

        public float Determinant()
        {
            return (this[0, 0] * ((this[1, 1] * this[2, 2]) - (this[2, 1] * this[1, 2]))) - (this[1, 0] * ((this[0, 1] * this[2, 2]) - (this[2, 1] * this[0, 2]))) + (this[2, 0] * ((this[0, 1] * this[1, 2]) - (this[1, 1] * this[0, 2])));
        }

        public GodotVector3 GetAxis(int axis)
        {
            return new GodotVector3(this[0, axis], this[1, axis], this[2, axis]);
        }

        public GodotVector3 GetEuler()
        {
            GodotBasis basis = Orthonormalized();
            GodotVector3 vector3;
            vector3.z = 0.0f;
            float num = basis.y[2];
            if (num < 1.0)
            {
                if (num > -1.0)
                {
                    vector3.x = GodotMathf.Asin(-num);
                    vector3.y = GodotMathf.Atan2(basis.x[2], basis.z[2]);
                    vector3.z = GodotMathf.Atan2(basis.y[0], basis.y[1]);
                }
                else
                {
                    vector3.x = 1.570796f;
                    vector3.y = -GodotMathf.Atan2(-basis.x[1], basis.x[0]);
                }
            }
            else
            {
                vector3.x = -1.570796f;
                vector3.y = -GodotMathf.Atan2(basis.x[1], basis.x[0]);
            }
            return vector3;
        }

        public int GetOrthogonalIndex()
        {
            GodotBasis basis = this;
            for (int index1 = 0; index1 < 3; ++index1)
            {
                for (int index2 = 0; index2 < 3; ++index2)
                {
                    float num1 = basis[index1, index2];
                    basis[index1, index2] = num1 <= 0.5 ? (num1 >= -0.5 ? 0.0f : -1f) : 1f;
                }
            }
            for (int index = 0; index < 24; ++index)
            {
                if (orthoBases[index] == basis)
                    return index;
            }
            return 0;
        }

        public GodotBasis Inverse()
        {
            GodotBasis basis = this;
            float[] numArray = new float[3]
            {
              (basis[1, 1] *  basis[2, 2]) -  (basis[1, 2] *  basis[2, 1]),
              (basis[1, 2] *  basis[2, 0]) -  (basis[1, 0] *  basis[2, 2]),
              (basis[1, 0] *  basis[2, 1]) -  (basis[1, 1] *  basis[2, 0])
            };
            float num1 = (basis[0, 0] * numArray[0]) + (basis[0, 1] * numArray[1]) + (basis[0, 2] * numArray[2]);
            if (num1 == 0.0)
                return new GodotBasis(float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);
            float num2 = 1f / num1;
            return new GodotBasis(numArray[0] * num2, (basis[0, 2] * basis[2, 1]) - (basis[0, 1] * basis[2, 2] * num2), (basis[0, 1] * basis[1, 2]) - (basis[0, 2] * basis[1, 1] * num2), numArray[1] * num2, (basis[0, 0] * basis[2, 2]) - (basis[0, 2] * basis[2, 0] * num2), (basis[0, 2] * basis[1, 0]) - (basis[0, 0] * basis[1, 2] * num2), numArray[2] * num2, (basis[0, 1] * basis[2, 0]) - (basis[0, 0] * basis[2, 1] * num2), (basis[0, 0] * basis[1, 1]) - (basis[0, 1] * basis[1, 0] * num2));
        }

        public GodotBasis Orthonormalized()
        {
            GodotVector3 axis1 = GetAxis(0);
            GodotVector3 axis2 = GetAxis(1);
            GodotVector3 axis3 = GetAxis(2);
            axis1.Normalize();
            GodotVector3 yAxis = axis2 - (axis1 * axis1.Dot(axis2));
            yAxis.Normalize();
            GodotVector3 zAxis = axis3 - (axis1 * axis1.Dot(axis3)) - (yAxis * yAxis.Dot(axis3));
            zAxis.Normalize();
            return CreateFromAxes(axis1, yAxis, zAxis);
        }

        public GodotBasis Rotated(GodotVector3 axis, float phi)
        {
            return new GodotBasis(axis, phi) * this;
        }

        public GodotBasis Scaled(GodotVector3 scale)
        {
            GodotBasis basis = this;
            basis[0, 0] *= scale.x;
            basis[0, 1] *= scale.x;
            basis[0, 2] *= scale.x;
            basis[1, 0] *= scale.y;
            basis[1, 1] *= scale.y;
            basis[1, 2] *= scale.y;
            basis[2, 0] *= scale.z;
            basis[2, 1] *= scale.z;
            basis[2, 2] *= scale.z;
            return basis;
        }

        public float Tdotx(GodotVector3 with)
        {
            return (this[0, 0] * with[0]) + (this[1, 0] * with[1]) + (this[2, 0] * with[2]);
        }

        public float Tdoty(GodotVector3 with)
        {
            return (this[0, 1] * with[0]) + (this[1, 1] * with[1]) + (this[2, 1] * with[2]);
        }

        public float Tdotz(GodotVector3 with)
        {
            return (this[0, 2] * with[0]) + (this[1, 2] * with[1]) + (this[2, 2] * with[2]);
        }

        public GodotBasis Transposed()
        {
            GodotBasis basis = this;
            float num1 = this[0, 1];
            this[0, 1] = this[1, 0];
            this[1, 0] = num1;
            float num2 = this[0, 2];
            this[0, 2] = this[2, 0];
            this[2, 0] = num2;
            float num3 = this[1, 2];
            this[1, 2] = this[2, 1];
            this[2, 1] = num3;
            return basis;
        }

        public GodotVector3 Xform(GodotVector3 v)
        {
            GodotVector3 vector3 = this[0];
            float num1 = vector3.Dot(v);
            vector3 = this[1];
            float num2 = vector3.Dot(v);
            vector3 = this[2];
            float num3 = vector3.Dot(v);
            return new GodotVector3(num1, num2, num3);
        }

        public GodotVector3 XformInv(GodotVector3 v)
        {
            return new GodotVector3((this[0, 0] * v.x) + (this[1, 0] * v.y) + (this[2, 0] * v.z), (this[0, 1] * v.x) + (this[1, 1] * v.y) + (this[2, 1] * v.z), (this[0, 2] * v.x) + (this[1, 2] * v.y) + (this[2, 2] * v.z));
        }

        public GodotQuat Quat()
        {
            float num1 = x[0] + y[1] + z[2];
            if (num1 > 0.0)
            {
                float num2 = GodotMathf.Sqrt(num1 + 1f) * 2f;
                float num3 = 1f / num2;
                return new GodotQuat((z[1] - y[2]) * num3, (x[2] - z[0]) * num3, (y[0] - x[1]) * num3, num2 * 0.25f);
            }
            if (x[0] > y[1] && x[0] > z[2])
            {
                float num2 = GodotMathf.Sqrt((float)(x[0] - y[1] - z[2] + 1.0)) * 2f;
                float num3 = 1f / num2;
                return new GodotQuat(num2 * 0.25f, (x[1] + y[0]) * num3, (x[2] + z[0]) * num3, (z[1] - y[2]) * num3);
            }
            if (y[1] > z[2])
            {
                float num2 = GodotMathf.Sqrt((float)(-x[0] + y[1] - z[2] + 1.0)) * 2f;
                float num3 = 1f / num2;
                return new GodotQuat((x[1] + y[0]) * num3, num2 * 0.25f, (y[2] + z[1]) * num3, (x[2] - z[0]) * num3);
            }
            float num4 = GodotMathf.Sqrt((float)(-x[0] - y[1] + z[2] + 1.0)) * 2f;
            float num5 = 1f / num4;
            return new GodotQuat((x[2] + z[0]) * num5, (y[2] + z[1]) * num5, num4 * 0.25f, (y[0] - x[1]) * num5);
        }

        public GodotBasis(GodotQuat quat)
        {
            float num1 = 2f / quat.LengthSquared();
            float num2 = quat.x * num1;
            float num3 = quat.y * num1;
            float num4 = quat.z * num1;
            float num5 = quat.w * num2;
            float num6 = quat.w * num3;
            float num7 = quat.w * num4;
            float num8 = quat.x * num2;
            float num9 = quat.x * num3;
            float num10 = quat.x * num4;
            float num11 = quat.y * num3;
            float num12 = quat.y * num4;
            float num13 = quat.z * num4;
            x = new GodotVector3((float)(1.0 - (num11 + num13)), num9 - num7, num10 + num6);
            y = new GodotVector3(num9 + num7, (float)(1.0 - (num8 + num13)), num12 - num5);
            z = new GodotVector3(num10 - num6, num12 + num5, (float)(1.0 - (num8 + num11)));
        }

        public GodotBasis(GodotVector3 axis, float phi)
        {
            GodotVector3 vector3 = new GodotVector3(axis.x * axis.x, axis.y * axis.y, axis.z * axis.z);
            float num1 = GodotMathf.Cos(phi);
            float num2 = GodotMathf.Sin(phi);
            x = new GodotVector3(vector3.x + (num1 * (1f - vector3.x)), (float)((axis.x * axis.y * (1.0 - num1)) - (axis.z * num2)), (float)((axis.z * axis.x * (1.0 - num1)) + (axis.y * num2)));
            y = new GodotVector3((float)((axis.x * axis.y * (1.0 - num1)) + (axis.z * num2)), vector3.y + (num1 * (1f - vector3.y)), (float)((axis.y * axis.z * (1.0 - num1)) - (axis.x * num2)));
            z = new GodotVector3((float)((axis.z * axis.x * (1.0 - num1)) - (axis.y * num2)), (float)((axis.y * axis.z * (1.0 - num1)) + (axis.x * num2)), vector3.z + (num1 * (1f - vector3.z)));
        }

        public GodotBasis(GodotVector3 xAxis, GodotVector3 yAxis, GodotVector3 zAxis)
        {
            x = xAxis;
            y = yAxis;
            z = zAxis;
        }

        public GodotBasis(
          float xx,
          float xy,
          float xz,
          float yx,
          float yy,
          float yz,
          float zx,
          float zy,
          float zz)
        {
            x = new GodotVector3(xx, xy, xz);
            y = new GodotVector3(yx, yy, yz);
            z = new GodotVector3(zx, zy, zz);
        }

        public static GodotBasis operator *(GodotBasis left, GodotBasis right)
        {
            return new GodotBasis(right.Tdotx(left[0]), right.Tdoty(left[0]), right.Tdotz(left[0]), right.Tdotx(left[1]), right.Tdoty(left[1]), right.Tdotz(left[1]), right.Tdotx(left[2]), right.Tdoty(left[2]), right.Tdotz(left[2]));
        }

        public static bool operator ==(GodotBasis left, GodotBasis right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GodotBasis left, GodotBasis right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is GodotBasis)
                return Equals((GodotBasis)obj);
            return false;
        }

        public bool Equals(GodotBasis other)
        {
            if (x.Equals(other.x) && y.Equals(other.y))
                return z.Equals(other.z);
            return false;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", new object[3]
            {
             x.ToString(),
             y.ToString(),
             z.ToString()
            });
        }

        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2})", new object[3]
            {
             x.ToString(format),
             y.ToString(format),
             z.ToString(format)
            });
        }
    }
}
