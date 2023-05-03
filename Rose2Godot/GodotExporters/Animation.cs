using System.Collections.Generic;

namespace Rose2Godot.GodotExporters
{
    public class Animation
    {
        public string Name { get; set; }
        public int FramesCount { get; set; }
        public float FPS { get; set; }
        public Dictionary<string, Dictionary<float, AnimationTrack>> Tracks { get; set; }

        public Animation(string Name, int FramesCount, float FPS)
        {
            this.Name = Name;
            this.FramesCount = FramesCount;
            this.FPS = FPS;
            Tracks = new Dictionary<string, Dictionary<float, AnimationTrack>>();
        }
    }
}
