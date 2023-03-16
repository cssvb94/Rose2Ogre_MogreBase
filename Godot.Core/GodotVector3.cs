using System;
using System.Runtime.CompilerServices;

namespace Godot
{
    public partial struct GodotVector3 : IEquatable<GodotVector3>
    {
        public float x;
        public float y;
        public float z;

        public float this[int index]
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

        internal void Normalize()
        {
            float num = Length();
            if (num == 0.0)
            {
                x = y = z = 0.0f;
            }
            else
            {
                x /= num;
                y /= num;
                z /= num;
            }
        }

        public static GodotVector3 One => new GodotVector3(1f, 1f, 1f);
        public static GodotVector3 Zero => new GodotVector3(0f, 0f, 0f);

        public GodotVector3 Abs()
        {
            return new GodotVector3(GodotMathf.Abs(x), GodotMathf.Abs(y), GodotMathf.Abs(z));
        }

        public float AngleTo(GodotVector3 to)
        {
            return GodotMathf.Atan2(Cross(to).Length(), Dot(to));
        }

        public GodotVector3 Bounce(GodotVector3 n)
        {
            return -Reflect(n);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GodotVector3 Ceil()
        {
            return new GodotVector3(GodotMathf.Ceil(x), GodotMathf.Ceil(y), GodotMathf.Ceil(z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GodotVector3 Cross(GodotVector3 b)
        {
            return new GodotVector3((y * b.z) - (z * b.y), (z * b.x) - (x * b.z), (x * b.y) - (y * b.x));
        }

        public GodotVector3 CubicInterpolate(GodotVector3 b, GodotVector3 preA, GodotVector3 postB, float t)
        {
            GodotVector3 vector3_1 = preA;
            GodotVector3 vector3_2 = this;
            GodotVector3 vector3_3 = b;
            GodotVector3 vector3_4 = postB;
            float num1 = t * t;
            float num2 = num1 * t;
            return 0.5f * ((vector3_2 * 2f) + ((-vector3_1 + vector3_3) * t) + (((2f * vector3_1) - (5f * vector3_2) + (4f * vector3_3) - vector3_4) * num1) + ((-vector3_1 + (3f * vector3_2) - (3f * vector3_3) + vector3_4) * num2));
        }

        public float DistanceSquaredTo(GodotVector3 b)
        {
            return (b - this).LengthSquared();
        }

        public float DistanceTo(GodotVector3 b)
        {
            return (b - this).Length();
        }

        public float Dot(GodotVector3 b)
        {
            return (x * b.x) + (y * b.y) + (z * b.z);
        }

        public GodotVector3 Floor()
        {
            return new GodotVector3(GodotMathf.Floor(x), GodotMathf.Floor(y), GodotMathf.Floor(z));
        }

        public GodotVector3 Inverse()
        {
            return new GodotVector3(1f / x, 1f / y, 1f / z);
        }

        public bool IsNormalized()
        {
            return GodotMathf.Abs(LengthSquared() - 1f) < 9.99999997475243E-07;
        }

        public float Length()
        {
            double num1 = x * (double)x;
            float num2 = y * y;
            float num3 = z * z;
            double num4 = num2;
            return GodotMathf.Sqrt((float)(num1 + num4) + num3);
        }

        public float LengthSquared()
        {
            double num1 = x * (double)x;
            float num2 = y * y;
            float num3 = z * z;
            double num4 = num2;
            return (float)(num1 + num4) + num3;
        }

        public GodotVector3 LinearInterpolate(GodotVector3 b, float t)
        {
            return new GodotVector3(x + (t * (b.x - x)), y + (t * (b.y - y)), z + (t * (b.z - z)));
        }

        public Axis MaxAxis()
        {
            if (x >= y)
                return x >= z ? Axis.X : Axis.Z;
            return y >= z ? Axis.Y : Axis.Z;
        }

        public Axis MinAxis()
        {
            if (x >= y)
                return y >= z ? Axis.Z : Axis.Y;
            return x >= z ? Axis.Z : Axis.X;
        }

        public GodotVector3 Normalized()
        {
            GodotVector3 vector3 = this;
            vector3.Normalize();
            return vector3;
        }

        public GodotBasis Outer(GodotVector3 b)
        {
            return new GodotBasis(new GodotVector3(x * b.x, x * b.y, x * b.z), new GodotVector3(y * b.x, y * b.y, y * b.z), new GodotVector3(z * b.x, z * b.y, z * b.z));
        }

        public GodotVector3 Reflect(GodotVector3 n)
        {
            return (2f * n * Dot(n)) - this;
        }

        public GodotVector3 Rotated(GodotVector3 axis, float phi)
        {
            return new GodotBasis(axis, phi).Xform(this);
        }

        public GodotVector3 Slide(GodotVector3 n)
        {
            return this - (n * Dot(n));
        }

        public GodotVector3 Snapped(GodotVector3 by)
        {
            return new GodotVector3(GodotMathf.Stepify(x, by.x), GodotMathf.Stepify(y, by.y), GodotMathf.Stepify(z, by.z));
        }

        public GodotBasis ToDiagonalMatrix()
        {
            return new GodotBasis(x, 0.0f, 0.0f, 0.0f, y, 0.0f, 0.0f, 0.0f, z);
        }

        public GodotVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static GodotVector3 operator +(GodotVector3 left, GodotVector3 right)
        {
            left.x += right.x;
            left.y += right.y;
            left.z += right.z;
            return left;
        }

        public static GodotVector3 operator -(GodotVector3 left, GodotVector3 right)
        {
            left.x -= right.x;
            left.y -= right.y;
            left.z -= right.z;
            return left;
        }

        public static GodotVector3 operator -(GodotVector3 vec)
        {
            vec.x = -vec.x;
            vec.y = -vec.y;
            vec.z = -vec.z;
            return vec;
        }

        public static GodotVector3 operator *(GodotVector3 vec, float scale)
        {
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            return vec;
        }

        public static GodotVector3 operator *(float scale, GodotVector3 vec)
        {
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            return vec;
        }

        public static GodotVector3 operator *(GodotVector3 left, GodotVector3 right)
        {
            left.x *= right.x;
            left.y *= right.y;
            left.z *= right.z;
            return left;
        }

        public static GodotVector3 operator /(GodotVector3 vec, float scale)
        {
            vec.x /= scale;
            vec.y /= scale;
            vec.z /= scale;
            return vec;
        }

        public static GodotVector3 operator /(GodotVector3 left, GodotVector3 right)
        {
            left.x /= right.x;
            left.y /= right.y;
            left.z /= right.z;
            return left;
        }

        public static bool operator ==(GodotVector3 left, GodotVector3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GodotVector3 left, GodotVector3 right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(GodotVector3 left, GodotVector3 right)
        {
            if (left.x != right.x)
                return left.x < right.x;
            if (left.y == right.y)
                return left.z < right.z;
            return left.y < right.y;
        }

        public static bool operator >(GodotVector3 left, GodotVector3 right)
        {
            if (left.x != right.x)
                return left.x > right.x;
            if (left.y == right.y)
                return left.z > right.z;
            return left.y > right.y;
        }

        public static bool operator <=(GodotVector3 left, GodotVector3 right)
        {
            if (left.x != right.x)
                return left.x < right.x;
            if (left.y == right.y)
                return left.z <= right.z;
            return left.y < right.y;
        }

        public static bool operator >=(GodotVector3 left, GodotVector3 right)
        {
            if (left.x != right.x)
                return left.x > right.x;
            if (left.y == right.y)
                return left.z >= right.z;
            return left.y > right.y;
        }

        public override bool Equals(object obj) => obj is GodotVector3 vector && Equals(vector);

        public bool Equals(GodotVector3 other) => x == other.x && y == other.y && z == other.z;

        public override int GetHashCode() => y.GetHashCode() ^ x.GetHashCode() ^ z.GetHashCode();

        public string ToStringNoBrackets() => $"{x:g8}, {y:g8}, {z:g8}";

        public override string ToString() => $"({ToStringNoBrackets()})";

        public string ToString(string format) => $"({x.ToString(format)}, {y.ToString(format)}, {z.ToString(format)})";
    }
}
