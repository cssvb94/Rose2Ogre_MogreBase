using System;

namespace Godot
{
    public struct GodotVector2 : IEquatable<GodotVector2>
    {
        public float x;
        public float y;

        public float this[int index]
        {
            get
            {
                if (index == 0)
                    return x;
                if (index == 1)
                    return y;
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (index != 0)
                {
                    if (index != 1)
                        throw new IndexOutOfRangeException();
                    y = value;
                }
                else
                {
                    x = value;
                }
            }
        }

        internal void Normalize()
        {
            float s = ((x * x) + (y * y));
            if (s == 0.0)
                return;
            float num = GodotMathf.Sqrt(s);
            x /= num;
            y /= num;
        }

        private float Cross(GodotVector2 b)
        {
            return (x * b.y) - (y * b.x);
        }

        public GodotVector2 Abs()
        {
            return new GodotVector2(GodotMathf.Abs(x), GodotMathf.Abs(y));
        }

        public float Angle()
        {
            return GodotMathf.Atan2(y, x);
        }

        public float AngleTo(GodotVector2 to)
        {
            return GodotMathf.Atan2(Cross(to), Dot(to));
        }

        public float AngleToPoint(GodotVector2 to)
        {
            return GodotMathf.Atan2(x - to.x, y - to.y);
        }

        public float Aspect()
        {
            return x / y;
        }

        public GodotVector2 Bounce(GodotVector2 n)
        {
            return -Reflect(n);
        }

        public GodotVector2 Clamped(float length)
        {
            GodotVector2 vector2 = this;
            float num = Length();
            if (num > 0.0 && length < (double)num)
                vector2 = vector2 / num * length;
            return vector2;
        }

        public GodotVector2 CubicInterpolate(GodotVector2 b, GodotVector2 preA, GodotVector2 postB, float t)
        {
            GodotVector2 vector2_1 = preA;
            GodotVector2 vector2_2 = this;
            GodotVector2 vector2_3 = b;
            GodotVector2 vector2_4 = postB;
            float num1 = t * t;
            float num2 = num1 * t;
            return 0.5f * ((vector2_2 * 2f) + ((-vector2_1 + vector2_3) * t) + (((2f * vector2_1) - (5f * vector2_2) + (4f * vector2_3) - vector2_4) * num1) + ((-vector2_1 + (3f * vector2_2) - (3f * vector2_3) + vector2_4) * num2));
        }

        public float DistanceSquaredTo(GodotVector2 to)
        {
            return ((x - to.x) * (x - to.x)) + ((y - to.y) * (y - to.y));
        }

        public float DistanceTo(GodotVector2 to)
        {
            return GodotMathf.Sqrt(((x - to.x) * (x - to.x)) + ((y - to.y) * (y - to.y)));
        }

        public float Dot(GodotVector2 with)
        {
            return (x * with.x) + (y * with.y);
        }

        public GodotVector2 Floor()
        {
            return new GodotVector2(GodotMathf.Floor(x), GodotMathf.Floor(y));
        }

        public bool IsNormalized()
        {
            return GodotMathf.Abs(LengthSquared() - 1f) < 9.99999997475243E-07;
        }

        public float Length()
        {
            return GodotMathf.Sqrt((x * x) + (y * y));
        }

        public float LengthSquared()
        {
            return (x * x) + (y * y);
        }

        public GodotVector2 LinearInterpolate(GodotVector2 b, float t)
        {
            GodotVector2 vector2 = this;
            vector2.x += t * (b.x - x);
            vector2.y += t * (b.y - y);
            return vector2;
        }

        public GodotVector2 Normalized()
        {
            GodotVector2 vector2 = this;
            vector2.Normalize();
            return vector2;
        }

        public GodotVector2 Reflect(GodotVector2 n)
        {
            return (2f * n * Dot(n)) - this;
        }

        public GodotVector2 Rotated(float phi)
        {
            float s = Angle() + phi;
            return new GodotVector2(GodotMathf.Cos(s), GodotMathf.Sin(s)) * Length();
        }

        public GodotVector2 Slide(GodotVector2 n)
        {
            return this - (n * Dot(n));
        }

        public GodotVector2 Snapped(GodotVector2 by)
        {
            return new GodotVector2(GodotMathf.Stepify(x, by.x), GodotMathf.Stepify(y, by.y));
        }

        public GodotVector2 Tangent()
        {
            return new GodotVector2(y, -x);
        }

        public GodotVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static GodotVector2 operator +(GodotVector2 left, GodotVector2 right)
        {
            left.x += right.x;
            left.y += right.y;
            return left;
        }

        public static GodotVector2 operator -(GodotVector2 left, GodotVector2 right)
        {
            left.x -= right.x;
            left.y -= right.y;
            return left;
        }

        public static GodotVector2 operator -(GodotVector2 vec)
        {
            vec.x = -vec.x;
            vec.y = -vec.y;
            return vec;
        }

        public static GodotVector2 operator *(GodotVector2 vec, float scale)
        {
            vec.x *= scale;
            vec.y *= scale;
            return vec;
        }

        public static GodotVector2 operator *(float scale, GodotVector2 vec)
        {
            vec.x *= scale;
            vec.y *= scale;
            return vec;
        }

        public static GodotVector2 operator *(GodotVector2 left, GodotVector2 right)
        {
            left.x *= right.x;
            left.y *= right.y;
            return left;
        }

        public static GodotVector2 operator /(GodotVector2 vec, float scale)
        {
            vec.x /= scale;
            vec.y /= scale;
            return vec;
        }

        public static GodotVector2 operator /(GodotVector2 left, GodotVector2 right)
        {
            left.x /= right.x;
            left.y /= right.y;
            return left;
        }

        public static bool operator ==(GodotVector2 left, GodotVector2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GodotVector2 left, GodotVector2 right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(GodotVector2 left, GodotVector2 right)
        {
            if (left.x.Equals(right.x))
                return left.y < right.y;
            return left.x < right.x;
        }

        public static bool operator >(GodotVector2 left, GodotVector2 right)
        {
            if (left.x.Equals(right.x))
                return left.y > right.y;
            return left.x > right.x;
        }

        public static bool operator <=(GodotVector2 left, GodotVector2 right)
        {
            if (left.x.Equals(right.x))
                return left.y <= right.y;
            return left.x <= right.x;
        }

        public static bool operator >=(GodotVector2 left, GodotVector2 right)
        {
            if (left.x.Equals(right.x))
                return left.y >= right.y;
            return left.x >= right.x;
        }

        public override bool Equals(object obj)
        {
            if (obj is GodotVector2)
                return Equals((GodotVector2)obj);
            return false;
        }

        public bool Equals(GodotVector2 other) => x == other.x && y == other.y;

        public override int GetHashCode() => y.GetHashCode() ^ x.GetHashCode();

        public override string ToString() => $"({x}, {y})";

        public string ToString(string format) => $"({x.ToString(format)}, {y.ToString(format)})";
    }
}
