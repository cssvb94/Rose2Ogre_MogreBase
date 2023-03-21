using System.Numerics;
using System;

namespace Rose2Godot.GodotExporters
{
    public class Rect
    {
        public float Height { get => Size.Y; }
        public float Width { get => Size.X; }
        public float X { get => Position.X; }
        public float Y { get => Position.Y; }
        public Rect Zero => new Rect();
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
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
            Position = new Vector2(Math.Min(position.X, size.X), Math.Min(position.Y, size.Y));
            Size = new Vector2(Math.Abs(size.X), Math.Abs(size.Y));
        }
    }
}
