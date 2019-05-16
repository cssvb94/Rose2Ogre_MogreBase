using System;

namespace RoseFormats
{
    public class Vector3w
    {
        private uint[] element = new uint[3] { 0, 0, 0 };
        public uint this[int i]
        {
            get
            {
                return element[i];
            }
            set
            {
                element[i] = value;
            }
        }
        public uint x
        {
            get
            {
                return element[0];
            }
            set
            {
                element[0] = value;
            }
        }

        public uint y
        {
            get
            {
                return element[1];
            }
            set
            {
                element[1] = value;
            }
        }

        public uint z
        {
            get
            {
                return element[2];
            }
            set
            {
                element[2] = value;
            }
        }

        public float Length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y + z * z);
            }
        }

        public Vector3w()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public Vector3w(int x, int y, int z)
        {
            this[0] = (uint)x;
            this[1] = (uint)y;
            this[2] = (uint)z;
        }

        public Vector3w(uint x, uint y, uint z)
        {
            this[0] = x;
            this[1] = y;
            this[2] = z;
        }

        public Vector3w Clone()
        {
            return new Vector3w(x, y, z);
        }

        public override string ToString()
        {
            return String.Format("Vector3w({0}, {1}, {2})", x, y, z);
        }
    } // class
}
