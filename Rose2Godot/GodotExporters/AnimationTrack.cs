using Godot;

namespace Rose2Godot.GodotExporters
{
    public struct AnimationTrack
    {
        public float TimeStamp { get; set; }
        public float Transition { get; set; } // default 1.0f in transform track
        public GodotVector3 Translation { get; set; }
        public GodotQuat Rotation { get; set; }
        public GodotVector3 Scale { get; set; }
        public string BoneName { get; set; }
        public int BoneId { get; set; }

        public AnimationTrack(float timeStamp, float transition, GodotVector3 translation, GodotQuat rotation, GodotVector3 scale, int boneId, string boneName)
        {
            TimeStamp = timeStamp;   // secs
            Transition = transition; // 1.0f
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
            BoneId = boneId;
            BoneName = boneName;
        }

        public override string ToString()
        {
            string vector3_translation = $"{Translation.x:0.00000}, {Translation.y:0.00000}, {Translation.z:0.00000}";
            string quad_rotation = $"{Rotation.x:0.00000}, {Rotation.y:0.00000}, {Rotation.z:0.00000}, {Rotation.w:0.00000}";
            string vector3_scale = $"{Scale.x:0.00000}, {Scale.y:0.00000}, {Scale.z:0.00000}";

            return $"{TimeStamp:0.00000}, {Transition:0.00000}, {vector3_translation}, {quad_rotation}, {vector3_scale}";

        }
    }
}
