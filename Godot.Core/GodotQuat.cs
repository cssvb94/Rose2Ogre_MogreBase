using System;

namespace Godot
{
    public struct GodotQuat : IEquatable<GodotQuat>
    {
        private static readonly GodotQuat identity = new GodotQuat(0.0f, 0.0f, 0.0f, 1f);
        private static readonly GodotQuat zero = new GodotQuat(0.0f, 0.0f, 0.0f, 0f);
        public float x;
        public float y;
        public float z;
        public float w;

        public static GodotQuat Identity => identity;
        public static GodotQuat Zero => zero;

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
                    case 3:
                        return w;
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
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public GodotQuat CubicSlerp(GodotQuat b, GodotQuat preA, GodotQuat postB, float t)
        {
            float t1 = (1.0f - t) * t * 2.0f;
            return Slerp(b, t).Slerpni(preA.Slerpni(postB, t), t1);
        }

        public float Dot(GodotQuat b) => (x * b.x) + (y * b.y) + (z * b.z) + (w * b.w);

        public GodotQuat Inverse()
        {
            float fNorm = (w * w) + (x * x) + (y * y) + (z * z);
            if (fNorm > 0.0f)
            {
                float fInvNorm = 1.0f / fNorm;
                return new GodotQuat(-x * fInvNorm, -y * fInvNorm, -z * fInvNorm, w * fInvNorm);
            }
            else
            {
                // return an invalid result to flag the error
                return Zero;
            }
        }

        public GodotQuat UnitInverse()
        {
            return new GodotQuat(-x, -y, -z, w);
        }

        public float Length()
        {
            return GodotMathf.Sqrt(LengthSquared());
        }

        public float LengthSquared()
        {
            return Dot(this);
        }

        public GodotQuat Normalized()
        {
            return this / Length();
        }

        public void Set(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public GodotQuat Slerp(GodotQuat b, float t)
        {
            float s1 = ((x * b.x) + (y * b.y) + (z * b.z) + (w * b.w));
            float[] numArray = new float[4];
            if (s1 < 0.0)
            {
                s1 = -s1;
                numArray[0] = -b.x;
                numArray[1] = -b.y;
                numArray[2] = -b.z;
                numArray[3] = -b.w;
            }
            else
            {
                numArray[0] = b.x;
                numArray[1] = b.y;
                numArray[2] = b.z;
                numArray[3] = b.w;
            }
            float num1;
            float num2;
            if (1.0 - s1 > 9.99999997475243E-07)
            {
                float s2 = GodotMathf.Acos(s1);
                float num3 = GodotMathf.Sin(s2);
                num1 = GodotMathf.Sin((1f - t) * s2) / num3;
                num2 = GodotMathf.Sin(t * s2) / num3;
            }
            else
            {
                num1 = 1f - t;
                num2 = t;
            }
            return new GodotQuat((num1 * x) + (num2 * numArray[0]), (num1 * y) + (num2 * numArray[1]), (num1 * z) + (num2 * numArray[2]), (num1 * w) + (num2 * numArray[3]));
        }

        public GodotQuat Slerpni(GodotQuat b, float t)
        {
            float s1 = Dot(b);
            if (GodotMathf.Abs(s1) > 0.999899983406067)
                return this;
            float s2 = GodotMathf.Acos(s1);
            float num1 = 1f / GodotMathf.Sin(s2);
            float num2 = GodotMathf.Sin(t * s2) * num1;
            float num3 = GodotMathf.Sin((1f - t) * s2) * num1;
            return new GodotQuat((num3 * x) + (num2 * b.x), (num3 * y) + (num2 * b.y), (num3 * z) + (num2 * b.z), (num3 * w) + (num2 * b.w));
        }

        public GodotVector3 Xform(GodotVector3 v)
        {
            GodotQuat quat = this * v * UnitInverse();
            return new GodotVector3(quat.x, quat.y, quat.z);
        }

        public GodotQuat(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public GodotQuat(GodotVector3 axis, float angle)
        {
            float num1 = axis.Length();
            if (num1 == 0.0)
            {
                x = 0.0f;
                y = 0.0f;
                z = 0.0f;
                w = 0.0f;
            }
            else
            {
                float num2 = GodotMathf.Sin(angle * 0.5f) / num1;
                x = axis.x * num2;
                y = axis.y * num2;
                z = axis.z * num2;
                w = GodotMathf.Cos(angle * 0.5f);
            }
        }

        public static GodotQuat operator *(GodotQuat left, GodotQuat right) => new GodotQuat(left.w * right.x + (left.x * right.w) + (left.y * right.z) - (left.z * right.y), (left.w * right.y) + (left.y * right.w) + (left.z * right.x) - (left.x * right.z), (left.w * right.z) + (left.z * right.w) + (left.x * right.y) - (left.y * right.x), (left.w * right.w) - (left.x * right.x) - (left.y * right.y) - (left.z * right.z));

        public static GodotQuat operator +(GodotQuat left, GodotQuat right) => new GodotQuat(left.x + right.x, left.y + right.y, left.z + right.z, left.w + right.w);

        public static GodotQuat operator -(GodotQuat left, GodotQuat right) => new GodotQuat(left.x - right.x, left.y - right.y, left.z - right.z, left.w - right.w);

        public static GodotQuat operator -(GodotQuat left) => new GodotQuat(-left.x, -left.y, -left.z, -left.w);

        public static GodotQuat operator *(GodotQuat left, GodotVector3 right) => new GodotQuat((left.w * right.x) + (left.y * right.z) - (left.z * right.y), (left.w * right.y) + (left.z * right.x) - (left.x * right.z), (left.w * right.z) + (left.x * right.y) - (left.y * right.x), (-left.x * right.x) - (left.y * right.y) - (left.z * right.z));

        public static GodotQuat operator *(GodotVector3 left, GodotQuat right) => new GodotQuat((right.w * left.x) + (right.y * left.z) - (right.z * left.y), (right.w * left.y) + (right.z * left.x) - (right.x * left.z), (right.w * left.z) + (right.x * left.y) - (right.y * left.x), (-right.x * left.x) - (right.y * left.y) - (right.z * left.z));

        public static GodotQuat operator *(GodotQuat left, float right) => new GodotQuat(left.x * right, left.y * right, left.z * right, left.w * right);

        public static GodotQuat operator *(float left, GodotQuat right) => new GodotQuat(right.x * left, right.y * left, right.z * left, right.w * left);

        public static GodotQuat operator /(GodotQuat left, float right) => left * (1f / right);

        public static bool operator ==(GodotQuat left, GodotQuat right) => left.Equals(right);

        public static bool operator !=(GodotQuat left, GodotQuat right) => !left.Equals(right);

        public override bool Equals(object obj) => obj is GodotVector2 vector && Equals(vector);

        public bool Equals(GodotQuat other) => x == other.x && y == other.y && z == other.z && w == other.w;

        public override int GetHashCode() => y.GetHashCode() ^ x.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();

        public override string ToString() => $"({x:0.#######}, {y:0.#######}, {z:0.#######}, {w:0.#######})";

        public string ToString(string format) => $"({x.ToString(format)}, {y.ToString(format)}, {z.ToString(format)}, {w.ToString(format)})";
    }
}
