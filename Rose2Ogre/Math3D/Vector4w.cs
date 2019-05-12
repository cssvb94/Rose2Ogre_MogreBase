using System;

namespace RoseFormats
{
    class Vector4w
    {
        private int[] element = new int[4] { 0, 0, 0, 0 };

        public int this[int i]
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

        public int x
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

        public int y
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

        public int z
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

        public int w
        {
            get
            {
                return element[3];
            }
            set
            {
                element[3] = value;
            }
        }

        public float Length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y + z * z + w * w);
            }
        }

        public Vector4w()
        {
            x = 0;
            y = 0;
            z = 0;
            w = 0;
        }

        public Vector4w(int x, int y, int z, int w)
        {
            this[0] = x;
            this[1] = y;
            this[2] = z;
            this[3] = w;
        }

        public Vector4w Clone()
        {
            return new Vector4w(x, y, z, w);
        }

        public override string ToString()
        {
            return String.Format("Vector4w({0}, {1}, {2}, {3})", x, y, z, w);
        }

    } // class
}
