using Mogre;

namespace RoseFormats
{
    class ZMOChannel
    {
        public ZMOTrack.TrackType Type;
        public int BoneID;

        public ZMOChannel()
        {
            BoneID = -1;
            Type = ZMOTrack.TrackType.NONE;
        }

        public ZMOChannel(ZMOTrack.TrackType TrackType, int BoneID)
        {
            Type = TrackType;
            this.BoneID = BoneID;
        }

    } // class ZMOChannel 

    class ZMOTrack
    {
        public enum TrackType : int
        {
            NONE = 0x00,
            POSITION = 0x2, // Vector3f
            ROTATION = 0x4, // Quaternion
            NORMAL = 0x8, // Vector3f
            ALPHA = 0x10, // Float
            UV1 = 0x20, // Vector2f
            UV2 = 0x40,
            UV3 = 0x80,
            UV4 = 0x100,
            TEXTUREANIM = 0x200, // Float
            SCALE = 0x400 // Float
        }

        public TrackType Type;
        public int BoneID;
        public int ID;
        public Vector3 Position; // also used for NORMAL type
        public Vector2 UV;
        public Quaternion Rotation;
        public float Value; // Multy value, for all float values

        public ZMOTrack()
        {
            Type = TrackType.NONE;
            BoneID = -1;
            ID = -1;
        }

        public ZMOTrack(TrackType Type, int BoneID, int ID)
        {
            this.Type = Type;
            this.BoneID = BoneID;
            this.ID = ID;
        }
    }
}
