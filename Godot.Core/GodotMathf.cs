using System;

namespace Godot
{
    public static class GodotMathf
    {
        public const float PI = 3.141593f;
        public const float Epsilon = 1E-06f;
        private const float Deg2RadConst = 0.01745329f;
        private const float Rad2DegConst = 57.29578f;

        public static float Abs(float s)
        {
            return Math.Abs(s);
        }

        public static float Acos(float s)
        {
            return (float)Math.Acos(s);
        }

        public static float Asin(float s)
        {
            return (float)Math.Asin(s);
        }

        public static float Atan(float s)
        {
            return (float)Math.Atan(s);
        }

        public static float Atan2(float x, float y)
        {
            return (float)Math.Atan2(x, y);
        }

        public static GodotVector2 Cartesian2Polar(float x, float y)
        {
            return new GodotVector2(Sqrt((float)((x * (double)x) + (y * (double)y))), Atan2(y, x));
        }

        public static float Ceil(float s)
        {
            return (float)Math.Ceiling(s);
        }

        public static float Clamp(float val, float min, float max)
        {
            if (val < (double)min)
                return min;
            if (val > (double)max)
                return max;
            return val;
        }

        public static float Cos(float s)
        {
            return (float)Math.Cos(s);
        }

        public static float Cosh(float s)
        {
            return (float)Math.Cosh(s);
        }

        public static int Decimals(float step)
        {
            return Decimals(step);
        }

        public static int Decimals(decimal step)
        {
            return BitConverter.GetBytes(decimal.GetBits(step)[3])[2];
        }

        public static float Deg2Rad(float deg)
        {
            return deg * ((float)Math.PI / 180f);
        }

        public static float Ease(float s, float curve)
        {
            if (s < 0.0)
                s = 0.0f;
            else if (s > 1.0)
                s = 1f;
            if (curve > 0.0)
            {
                if (curve < 1.0)
                    return 1f - Pow(1f - s, 1f / curve);
                return Pow(s, curve);
            }
            if (curve >= 0.0)
                return 0.0f;
            if (s < 0.5)
                return Pow(s * 2f, -curve) * 0.5f;
            return (float)(((1.0 - Pow((float)(1.0 - ((s - 0.5) * 2.0)), -curve)) * 0.5) + 0.5);
        }

        public static float Exp(float s)
        {
            return (float)Math.Exp(s);
        }

        public static float Floor(float s)
        {
            return (float)Math.Floor(s);
        }

        public static float Fposmod(float x, float y)
        {
            if (x >= 0.0)
                return x % y;
            return y - (-x % y);
        }

        public static float Lerp(float from, float to, float weight)
        {
            return from + ((to - from) * Clamp(weight, 0.0f, 1f));
        }

        public static float Log(float s)
        {
            return (float)Math.Log(s);
        }

        public static int Max(int a, int b)
        {
            if (a <= b)
                return b;
            return a;
        }

        public static float Max(float a, float b)
        {
            if (a <= (double)b)
                return b;
            return a;
        }

        public static int Min(int a, int b)
        {
            if (a >= b)
                return b;
            return a;
        }

        public static float Min(float a, float b)
        {
            if (a >= (double)b)
                return b;
            return a;
        }

        public static int NearestPo2(int val)
        {
            --val;
            val |= val >> 1;
            val |= val >> 2;
            val |= val >> 4;
            val |= val >> 8;
            val |= val >> 16;
            ++val;
            return val;
        }

        public static GodotVector2 Polar2Cartesian(float r, float th)
        {
            return new GodotVector2(r * Cos(th), r * Sin(th));
        }

        public static float Pow(float x, float y)
        {
            return (float)Math.Pow(x, y);
        }

        public static float Rad2Deg(float rad)
        {
            return rad * 57.29578f;
        }

        public static float Round(float s)
        {
            return (float)Math.Round(s);
        }

        public static float Sign(float s)
        {
            return s >= 0.0 ? 1f : -1f;
        }

        public static float Sin(float s)
        {
            return (float)Math.Sin(s);
        }

        public static float Sinh(float s)
        {
            return (float)Math.Sinh(s);
        }

        public static float Sqrt(float s)
        {
            return (float)Math.Sqrt(s);
        }

        public static float Stepify(float s, float step)
        {
            if (step != 0.0)
                s = Floor((float)((s / (double)step) + 0.5)) * step;
            return s;
        }

        public static float Tan(float s)
        {
            return (float)Math.Tan(s);
        }

        public static float Tanh(float s)
        {
            return (float)Math.Tanh(s);
        }
    }
}
