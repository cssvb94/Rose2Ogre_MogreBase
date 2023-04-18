using System.Numerics;

namespace Rose2Godot.GodotExporters
{
    public class Rect
    {
        public Vector2 Size { get; set; }
        public float Height { get => Size.Y; set => SetHeight(value); }
        public float Width { get => Size.X; set => SetWidth(value); }
        public float X { get => Position.X; set => SetX(value); }
        public float Y { get => Position.Y; set => SetY(value); }
        public float x { get => Position.X; set => SetX(value); }
        public float y { get => Position.Y; set => SetY(value); }
        public Rect Zero => new Rect();
        public Vector2 Position { get; set; }
        public Vector2 Min => new Vector2(Position.X, Position.Y);
        public Vector2 Max => (Position + Size);

        public Vector2 Center => new Vector2(Position.X + Size.X / 2f, Position.Y + Size.Y / 2);

        public Rect()
        {
            Position = Vector2.Zero;
            Size = Vector2.Zero;
        }

        public Rect(Vector2 position, Vector2 size)
        {
            Position = new Vector2(System.Math.Min(position.X, size.X), System.Math.Min(position.Y, size.Y));
            Size = new Vector2(System.Math.Abs(size.X), System.Math.Abs(size.Y));
        }

        public Rect(float x, float y, float width, float height)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
        }

        private void SetHeight(float new_height) => Size = new Vector2(Size.X, new_height);

        private void SetWidth(float new_width) => Size = new Vector2(new_width, Size.Y);

        private void SetX(float value) => Position = new Vector2(value, Position.Y);

        private void SetY(float value) => Position = new Vector2(Position.X, value);

        public bool Contains(Vector2 point) => point.X > x && point.X < (x + Width) && point.Y > y && point.Y < (y + Height);

        public bool Overlaps(Rect secondRect)
        {
            if (x + Width * 0.5f < secondRect.x - secondRect.Width * 0.5f)
                return false;
            if (secondRect.x + secondRect.Width * 0.5f < x - Width * 0.5f)
                return false;
            if (y + Height * 0.5f < secondRect.y - secondRect.Height * 0.5f)
                return false;
            if (secondRect.y + secondRect.Height * 0.5f < y - Height * 0.5f)
                return false;
            return true;
        }

        public static bool IsContainedIn(Rect a, Rect b)
        {
            return a.x >= b.x && a.y >= b.y
                && a.x + a.Width <= b.x + b.Width
                && a.y + a.Height <= b.y + b.Height;
        }

        public bool IsContainedIn(Rect other)
        {
            return x >= other.x && y >= other.y
               && x + Width <= other.x + other.Width
               && y + Height <= other.y + other.Height;
        }
    }
}