using System;

namespace Godot
{
    public struct GodotAABB : IEquatable<GodotAABB>
    {
        private GodotVector3 position;
        private GodotVector3 size;

        public GodotVector3 Position
        {
            get
            {
                return position;
            }
        }

        public GodotVector3 Size
        {
            get
            {
                return size;
            }
        }

        public GodotVector3 End
        {
            get
            {
                return position + size;
            }
        }

        public bool Encloses(GodotAABB with)
        {
            GodotVector3 position1 = position;
            GodotVector3 vector3_1 = position + size;
            GodotVector3 position2 = with.position;
            GodotVector3 vector3_2 = with.position + with.size;
            if (position1.x <= position2.x && vector3_1.x > (double)vector3_2.x && (position1.y <= (double)position2.y && vector3_1.y > (double)vector3_2.y) && position1.z <= (double)position2.z)
                return vector3_1.z > vector3_2.z;
            return false;
        }

        public GodotAABB Expand(GodotVector3 to_point)
        {
            GodotVector3 position = this.position;
            GodotVector3 vector3 = this.position + size;
            if (to_point.x < (double)position.x)
                position.x = to_point.x;
            if (to_point.y < (double)position.y)
                position.y = to_point.y;
            if (to_point.z < (double)position.z)
                position.z = to_point.z;
            if (to_point.x > (double)vector3.x)
                vector3.x = to_point.x;
            if (to_point.y > (double)vector3.y)
                vector3.y = to_point.y;
            if (to_point.z > (double)vector3.z)
                vector3.z = to_point.z;
            return new GodotAABB(position, vector3 - position);
        }

        public float GetArea()
        {
            return size.x * size.y * size.z;
        }

        public GodotVector3 GetEndpoint(int idx)
        {
            switch (idx)
            {
                case 0:
                    return new GodotVector3(position.x, position.y, position.z);
                case 1:
                    return new GodotVector3(position.x, position.y, position.z + size.z);
                case 2:
                    return new GodotVector3(position.x, position.y + size.y, position.z);
                case 3:
                    return new GodotVector3(position.x, position.y + size.y, position.z + size.z);
                case 4:
                    return new GodotVector3(position.x + size.x, position.y, position.z);
                case 5:
                    return new GodotVector3(position.x + size.x, position.y, position.z + size.z);
                case 6:
                    return new GodotVector3(position.x + size.x, position.y + size.y, position.z);
                case 7:
                    return new GodotVector3(position.x + size.x, position.y + size.y, position.z + size.z);
                default:
                    throw new ArgumentOutOfRangeException(nameof(idx), string.Format("Index is {0}, but a value from 0 to 7 is expected.", idx));
            }
        }

        public GodotVector3 GetLongestAxis()
        {
            GodotVector3 vector3 = new GodotVector3(1f, 0.0f, 0.0f);
            float num = size.x;
            if (size.y > num)
            {
                vector3 = new GodotVector3(0.0f, 1f, 0.0f);
                num = size.y;
            }
            if (size.z > num)
            {
                vector3 = new GodotVector3(0.0f, 0.0f, 1f);
                float z = size.z;
            }
            return vector3;
        }

        public GodotVector3.Axis GetLongestAxisIndex()
        {
            GodotVector3.Axis axis = GodotVector3.Axis.X;
            float num = size.x;
            if (size.y > num)
            {
                axis = GodotVector3.Axis.Y;
                num = size.y;
            }
            if (size.z > num)
            {
                axis = GodotVector3.Axis.Z;
                float z = size.z;
            }
            return axis;
        }

        public float GetLongestAxisSize()
        {
            float num = size.x;
            if (size.y > num)
                num = size.y;
            if (size.z > num)
                num = size.z;
            return num;
        }

        public GodotVector3 GetShortestAxis()
        {
            GodotVector3 vector3 = new GodotVector3(1f, 0.0f, 0.0f);
            float num = size.x;
            if (size.y < num)
            {
                vector3 = new GodotVector3(0.0f, 1f, 0.0f);
                num = size.y;
            }
            if (size.z < num)
            {
                vector3 = new GodotVector3(0.0f, 0.0f, 1f);
                float z = size.z;
            }
            return vector3;
        }

        public GodotVector3.Axis GetShortestAxisIndex()
        {
            GodotVector3.Axis axis = GodotVector3.Axis.X;
            float num = size.x;
            if (size.y < num)
            {
                axis = GodotVector3.Axis.Y;
                num = size.y;
            }
            if (size.z < num)
            {
                axis = GodotVector3.Axis.Z;
                float z = size.z;
            }
            return axis;
        }

        public float GetShortestAxisSize()
        {
            float num = size.x;
            if (size.y < num)
                num = size.y;
            if (size.z < num)
                num = size.z;
            return num;
        }

        public GodotVector3 GetSupport(GodotVector3 dir)
        {
            GodotVector3 vector3 = size * 0.5f;
            return position + vector3 + new GodotVector3(dir.x > 0.0 ? -vector3.x : vector3.x, dir.y > 0.0 ? -vector3.y : vector3.y, dir.z > 0.0 ? -vector3.z : vector3.z);
        }

        public GodotAABB Grow(float by)
        {
            GodotAABB aabb = this;
            aabb.position.x -= by;
            aabb.position.y -= by;
            aabb.position.z -= by;
            aabb.size.x += 2f * by;
            aabb.size.y += 2f * by;
            aabb.size.z += 2f * by;
            return aabb;
        }

        public bool HasNoArea()
        {
            if (size.x > 0.0 && size.y > 0.0)
                return size.z <= 0.0;
            return true;
        }

        public bool HasNoSurface()
        {
            if (size.x <= 0.0 && size.y <= 0.0)
                return size.z <= 0.0;
            return false;
        }

        public bool HasPoint(GodotVector3 point)
        {
            return point.x >= position.x && point.y >= position.y && (point.z >= position.z && point.x <= position.x + size.x) && (point.y <= position.y + size.y && point.z <= position.z + size.z);
        }

        public GodotAABB Intersection(GodotAABB with)
        {
            GodotVector3 position1 = position;
            GodotVector3 vector3_1 = position + size;
            GodotVector3 position2 = with.position;
            GodotVector3 vector3_2 = with.position + with.size;
            if (position1.x > vector3_2.x || vector3_1.x < position2.x)
                return new GodotAABB();
            GodotVector3 position3;
            position3.x = position1.x > position2.x ? position1.x : position2.x;
            GodotVector3 vector3_3;
            vector3_3.x = vector3_1.x < vector3_2.x ? vector3_1.x : vector3_2.x;
            if (position1.y > vector3_2.y || vector3_1.y < position2.y)
                return new GodotAABB();
            position3.y = position1.y > position2.y ? position1.y : position2.y;
            vector3_3.y = vector3_1.y < vector3_2.y ? vector3_1.y : vector3_2.y;
            if (position1.z > vector3_2.z || vector3_1.z < position2.z)
                return new GodotAABB();
            position3.z = position1.z > position2.z ? position1.z : position2.z;
            vector3_3.z = vector3_1.z < vector3_2.z ? vector3_1.z : vector3_2.z;
            return new GodotAABB(position3, vector3_3 - position3);
        }

        public bool Intersects(GodotAABB with)
        {
            return position.x < with.position.x + with.size.x && position.x + size.x > with.position.x && (position.y < with.position.y + with.size.y && position.y + size.y > with.position.y) && (position.z < with.position.z + with.size.z && position.z + size.z > with.position.z);
        }

        public bool IntersectsPlane(GodotPlane plane)
        {
            GodotVector3[] vector3Array = new GodotVector3[8]
            {
                new GodotVector3(position.x, position.y, position.z),
                new GodotVector3(position.x, position.y, position.z + size.z),
                new GodotVector3(position.x, position.y + size.y, position.z),
                new GodotVector3(position.x, position.y + size.y, position.z + size.z),
                new GodotVector3(position.x + size.x, position.y, position.z),
                new GodotVector3(position.x + size.x, position.y, position.z + size.z),
                new GodotVector3(position.x + size.x, position.y + size.y, position.z),
                new GodotVector3(position.x + size.x, position.y + size.y, position.z + size.z)
            };
            bool flag1 = false;
            bool flag2 = false;
            for (int index = 0; index < 8; ++index)
            {
                if (plane.DistanceTo(vector3Array[index]) > 0.0)
                    flag1 = true;
                else
                    flag2 = true;
            }
            return flag2 & flag1;
        }

        public bool IntersectsSegment(GodotVector3 from, GodotVector3 to)
        {
            float num1 = 0.0f;
            float num2 = 1f;
            for (int index = 0; index < 3; ++index)
            {
                float num3 = from[index];
                float num4 = to[index];
                float num5 = position[index];
                float num6 = num5 + size[index];
                float num7;
                float num8;
                if (num3 < num4)
                {
                    if (num3 > num6 || num4 < num5)
                        return false;
                    float num9 = num4 - num3;
                    num7 = num3 < num5 ? (num5 - num3) / num9 : 0.0f;
                    num8 = num4 > num6 ? (num6 - num3) / num9 : 1f;
                }
                else
                {
                    if (num4 > num6 || num3 < num5)
                        return false;
                    float num9 = num4 - num3;
                    num7 = num3 > num6 ? (num6 - num3) / num9 : 0.0f;
                    num8 = num4 < num5 ? (num5 - num3) / num9 : 1f;
                }
                if (num7 > num1)
                    num1 = num7;
                if (num8 < num2)
                    num2 = num8;
                if (num2 < num1)
                    return false;
            }
            return true;
        }

        public GodotAABB Merge(GodotAABB with)
        {
            GodotVector3 position1 = position;
            GodotVector3 position2 = with.position;
            GodotVector3 vector3_1 = new GodotVector3(size.x, size.y, size.z) + position1;
            GodotVector3 vector3_2 = new GodotVector3(with.size.x, with.size.y, with.size.z) + position2;
            GodotVector3 position3 = new GodotVector3(position1.x < position2.x ? position1.x : position2.x, position1.y < position2.y ? position1.y : position2.y, position1.z < position2.z ? position1.z : position2.z);
            GodotVector3 vector3_3 = new GodotVector3(vector3_1.x > vector3_2.x ? vector3_1.x : vector3_2.x, vector3_1.y > vector3_2.y ? vector3_1.y : vector3_2.y, vector3_1.z > vector3_2.z ? vector3_1.z : vector3_2.z);
            return new GodotAABB(position3, vector3_3 - position3);
        }

        public GodotAABB(GodotVector3 position, GodotVector3 size)
        {
            this.position = position;
            this.size = size;
        }

        public static bool operator ==(GodotAABB left, GodotAABB right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GodotAABB left, GodotAABB right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is GodotAABB)
                return Equals((GodotAABB)obj);
            return false;
        }

        public bool Equals(GodotAABB other)
        {
            if (position == other.position)
                return size == other.size;
            return false;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode() ^ size.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", new object[2]
            {
             position.ToString(),
             size.ToString()
            });
        }

        public string ToString(string format)
        {
            return string.Format("{0} - {1}", new object[2]
            {
             position.ToString(format),
             size.ToString(format)
            });
        }
    }
}
