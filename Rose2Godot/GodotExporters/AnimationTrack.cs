using Godot;

namespace Rose2Godot.GodotExporters
{

    public enum TrackType
    {
        None = 1 << 0,
        Position = 1 << 1,
        Rotation = 1 << 2,
        Scale = 1 << 10,
    }

    public struct AnimationTrack
    {
        public float TimeStamp { get; set; }
        public float Transition { get; set; } // default 1.0f in transform track
        public GodotVector3 Position { get; set; }
        public GodotQuat Rotation { get; set; }
        public GodotVector3 Scale { get; set; }
        public string BoneName { get; set; }
        public int BoneId { get; set; }
        public TrackType TrackType { get; set; }

        public AnimationTrack(float timeStamp, float transition, GodotVector3 translation, GodotQuat rotation, GodotVector3 scale, TrackType type, int boneId, string boneName)
        {
            TimeStamp = timeStamp;   // secs
            Transition = transition; // 1.0f
            Position = translation;
            Rotation = rotation;
            Scale = scale;
            BoneId = boneId;
            BoneName = boneName;
            TrackType = type;
        }

        public string ToPosition3D()
        {
            string vector3_translation = $"{Position.x:0.#####}, {Position.y:0.#####}, {Position.z:0.#####}";
            return $"{TimeStamp:0.#####}, {Transition:0.#####}, {vector3_translation}";
        }

        public string ToRotation3D()
        {
            string quad_rotation = $"{Rotation.x:0.#####}, {Rotation.y:0.#####}, {Rotation.z:0.#####}, {Rotation.w:0.#####}";
            return $"{TimeStamp:0.#####}, {Transition:0.#####}, {quad_rotation}";
        }

        public string ToScale3D()
        {
            string vector3_scale = $"{Scale.x:0.#####}, {Scale.y:0.#####}, {Scale.z:0.#####}";
            return $"{TimeStamp:0.#####}, {Transition:0.#####}, {vector3_scale}";
        }

        public override string ToString()
        {
            string vector3_translation = $"{Position.x:0.#####}, {Position.y:0.#####}, {Position.z:0.#####}";
            string quad_rotation = $"{Rotation.x:0.#####}, {Rotation.y:0.#####}, {Rotation.z:0.#####}, {Rotation.w:0.#####}";
            string vector3_scale = $"{Scale.x:0.#####}, {Scale.y:0.#####}, {Scale.z:0.#####}";
            return $"{TimeStamp:0.#####}, {Transition:0.#####}, {vector3_translation}, {quad_rotation}, {vector3_scale}";

        }

        public string TrackTypeToString()
        {
            switch (TrackType)
            {
                case TrackType.None:
                    return string.Empty;
                case TrackType.Position:
                    return "position_3d";
                case TrackType.Rotation:
                    return "rotation_3d";
                case TrackType.Scale:
                    return "scale_3d";
                default:
                    return "NOT IMPLEMENTED!";
            }
        }
    }
}
