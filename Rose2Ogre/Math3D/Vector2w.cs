using System;

namespace RoseFormats
{
    class Vector2w
    {
        private int[] element = new int[2] { 0, 0 };

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

        public float Length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
        }

        public Vector2w()
        {
            x = 0;
            y = 0;
        }

        public Vector2w(int x, int y)
        {
            this[0] = x;
            this[1] = y;
        }

        public Vector2w Clone()
        {
            return new Vector2w(x, y);
        }

    } // class
}
