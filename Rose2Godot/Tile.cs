using g4;
using Rose2Godot.GodotExporters;

namespace Rose2Godot
{
    public class Tile
    {
        public Rect BottomRect { get; set; }
        public Rect TopRect { get; set; }
        public string BottomTex { get; set; }
        public string TopTex { get; set; }
        public int TileID { get; set; }
        public int Layer1 { get; set; }
        public int Layer2 { get; set; }

        public Tile()
        {
        }

        public Tile(int TilesSize, int TileID, int Layer1ID, int Layer2ID)
        {
            TopRect = new Rect(0, 0, TilesSize, TilesSize);
            BottomRect = new Rect(0, 0, TilesSize, TilesSize);
            this.TileID = TileID;
        }

        public void SetRects(Rect bot, Rect top)
        {
            BottomRect = bot;
            TopRect = top;
        }

        public Vector2f GetUVTop(Vector2f uv)
        {
            // adjust uv's slightly to hide seams between tiles
            if (uv.x < 0.01f) uv.x += 0.01f;
            else if (uv.x > 0.99f) uv.x *= 0.99f;
            if (uv.y < 0.01f) uv.y += 0.01f;
            else if (uv.y > 0.99f) uv.y *= 0.99f;

            return new Vector2f((uv.x * TopRect.Width) + TopRect.x, (uv.y * TopRect.Height) + TopRect.y);
        }

        public Vector2f GetUVBottom(Vector2f uv)
        {
            if (uv.x < 0.01f) uv.x += 0.01f;
            else if (uv.x > 0.99f) uv.x *= 0.99f;
            if (uv.y < 0.01f) uv.y += 0.01f;
            else if (uv.y > 0.99f) uv.y *= 0.99f;

            return new Vector2f((uv.x * BottomRect.Width) + BottomRect.x, (uv.y * BottomRect.Height) + BottomRect.y);
        }
    }
}
