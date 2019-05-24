using System;

namespace Godot
{
    public struct GodotPlane : IEquatable<GodotPlane>
    {
        private GodotVector3 normal;
        private float d;

        public float x
        {
            get
            {
                return normal.x;
            }
            set
            {
                normal.x = value;
            }
        }

        public float y
        {
            get
            {
                return normal.y;
            }
            set
            {
                normal.y = value;
            }
        }

        public float z
        {
            get
            {
                return normal.z;
            }
            set
            {
                normal.z = value;
            }
        }

        public GodotVector3 Center
        {
            get
            {
                return normal * d;
            }
        }

        public float DistanceTo(GodotVector3 point)
        {
            return normal.Dot(point) - d;
        }

        public GodotVector3 GetAnyPoint()
        {
            return normal * d;
        }

        public bool HasPoint(GodotVector3 point, float epsilon = 1E-06f)
        {
            return GodotMathf.Abs(normal.Dot(point) - d) <= (double)epsilon;
        }

        public GodotVector3 Intersect3(GodotPlane b, GodotPlane c)
        {
            float s = normal.Cross(b.normal).Dot(c.normal);
            if (GodotMathf.Abs(s) <= 9.99999997475243E-07)
                return new GodotVector3();
            return ((b.normal.Cross(c.normal) * d) + (c.normal.Cross(normal) * b.d) + (normal.Cross(b.normal) * c.d)) / s;
        }

        public GodotVector3 IntersectRay(GodotVector3 from, GodotVector3 dir)
        {
            float s = normal.Dot(dir);
            if (GodotMathf.Abs(s) <= 9.99999997475243E-07)
                return new GodotVector3();
            float num = (normal.Dot(from) - d) / s;
            if (num > 9.99999997475243E-07)
                return new GodotVector3();
            return from + (dir * -num);
        }

        public GodotVector3 IntersectSegment(GodotVector3 begin, GodotVector3 end)
        {
            GodotVector3 b = begin - end;
            float s = normal.Dot(b);
            if (GodotMathf.Abs(s) <= 9.99999997475243E-07)
                return new GodotVector3();
            float num = (normal.Dot(begin) - d) / s;
            if (num < -9.99999997475243E-07 || num > 1.00000095367432)
                return new GodotVector3();
            return begin + (b * -num);
        }

        public bool IsPointOver(GodotVector3 point)
        {
            return normal.Dot(point) > d;
        }

        public GodotPlane Normalized()
        {
            float num = normal.Length();
            if (num == 0.0)
                return new GodotPlane(0.0f, 0.0f, 0.0f, 0.0f);
            return new GodotPlane(normal / num, d / num);
        }

        public GodotVector3 Project(GodotVector3 point)
        {
            return point - (normal * DistanceTo(point));
        }

        public GodotPlane(float a, float b, float c, float d)
        {
            normal = new GodotVector3(a, b, c);
            this.d = d;
        }

        public GodotPlane(GodotVector3 normal, float d)
        {
            this.normal = normal;
            this.d = d;
        }

        public GodotPlane(GodotVector3 v1, GodotVector3 v2, GodotVector3 v3)
        {
            normal = (v1 - v3).Cross(v1 - v2);
            normal.Normalize();
            d = normal.Dot(v1);
        }

        public static GodotPlane operator -(GodotPlane plane)
        {
            return new GodotPlane(-plane.normal, -plane.d);
        }

        public static bool operator ==(GodotPlane left, GodotPlane right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GodotPlane left, GodotPlane right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is GodotPlane)
                return Equals((GodotPlane)obj);
            return false;
        }

        public bool Equals(GodotPlane other)
        {
            if (normal == other.normal)
                return d == (double)other.d;
            return false;
        }

        public override int GetHashCode()
        {
            return normal.GetHashCode() ^ d.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", new object[2]
            {
                 normal.ToString(),
                 d.ToString()
            });
        }

        public string ToString(string format)
        {
            return string.Format("({0}, {1})", new object[2]
            {
                 normal.ToString(format),
                 d.ToString(format)
            });
        }
    }
}
