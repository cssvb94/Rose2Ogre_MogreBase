using Mogre;
using System.Collections.Generic;

namespace RoseFormats
{
    public class BoneFrame
    {
        private float angle;

        public float Angle
        {
            get
            {
                return angle;
            }
        }

        private Vector3 axis;

        public Vector3 Axis
        {
            get
            {
                return axis;
            }
        }

        private Quaternion rotation;

        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                Radian fAngle = new Radian(0.0f);
                Vector3 fAxis = new Vector3();
                rotation = value;
                rotation.ToAngleAxis(out fAngle, out fAxis);
                this.axis = fAxis;
                this.angle = fAngle.ValueRadians;
            }
        }

        public Vector3 Position;

        public Vector3 Scale;

        public int Frame { get; set; }

        public BoneFrame()
        {
            Rotation = Quaternion.IDENTITY;
            Position = Vector3.ZERO;
            Scale = Vector3.UNIT_SCALE;
        }
    }

    public class BoneAnimation
    {
        public string Name { get; set; }
        public List<BoneFrame> Frames { get; set; }

        public BoneAnimation(string name)
        {
            Name = name;
            Frames = new List<BoneFrame>();
        }
    }

    public class RoseBone
    {
        public string Name;
        public int ParentID;
        public int ID;
        public Vector3 Position;

        private float angle;

        public float Angle
        {
            get
            {
                return angle;
            }
        }

        public List<int> ChildID = new List<int>();

        private Vector3 axis;

        public Vector3 Axis
        {
            get
            {
                return axis;
            }
        }

        private Quaternion rotation;

        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                Radian fAngle = 0.0f;
                Vector3 fAxis = new Vector3();
                rotation = value;
                rotation.ToAngleAxis(out fAngle, out fAxis);
                this.axis = fAxis;
                this.angle = fAngle.ValueRadians;
            }
        }

        public Matrix4 InverseMatrix;

        public Matrix4 TransformMatrix;

        public bool isDummy;

        // from ZMO
        // the bone animations & transformation for each frame

        public List<BoneAnimation> BoneAnimations { get; set; }

        public RoseBone()
        {
            Name = string.Empty;
            Position = new Vector3();
            Rotation = new Quaternion();
            ParentID = -1;
            ID = -1;
            isDummy = false;
            InverseMatrix = new Matrix4();
            TransformMatrix = new Matrix4();
            BoneAnimations = new List<BoneAnimation>();
        }

        // Normal bone read sequence
        public RoseBone(int ParentID, string Name, Vector3 Position, Quaternion Rotation)
        {
            this.Name = Name;
            this.Position = Position;
            this.Rotation = Rotation;
            isDummy = false;
            this.ParentID = ParentID;
            BoneAnimations = new List<BoneAnimation>();
        }

        // Dummy read sequence ZMD0003
        public RoseBone(string Name, int ParentID, Vector3 Position, Quaternion Rotation)
        {
            this.Name = Name;
            this.Position = Position;
            this.Rotation = Rotation;
            this.ParentID = ParentID;
            isDummy = true;
            BoneAnimations = new List<BoneAnimation>();
        }

        // Dummy read sequence ZMD0002
        public RoseBone(string Name, int ParentID, Vector3 Position)
        {
            this.Name = Name;
            this.Position = Position;
            Rotation = new Quaternion();
            this.ParentID = ParentID;
            isDummy = true;
            BoneAnimations = new List<BoneAnimation>();
        }

        public void AddAnimationAt(int frame_number, string animation_name, BoneFrame frame, ZMOTrack.TrackType trackType)
        {
            bool createdAnim = false;
            bool createdFare = false;
            BoneAnimation banim = BoneAnimations.Find(ba => ba.Name.Equals(animation_name, System.StringComparison.InvariantCultureIgnoreCase));

            // if new animation - create
            if (banim == null)
            {
                banim = new BoneAnimation(animation_name);
                createdAnim = true;
            }

            BoneFrame bframe = banim.Frames.Find(a => a.Frame == frame_number);
            // if new frame - create
            if (bframe == null)
            {
                bframe = new BoneFrame()
                {
                    Frame = frame_number
                };

                createdFare = true;
            }

            // add/update frame transform
            switch (trackType)
            {
                case ZMOTrack.TrackType.POSITION:
                    bframe.Position = frame.Position;
                    break;
                case ZMOTrack.TrackType.ROTATION:
                    bframe.Rotation = frame.Rotation.Normalized();
                    break;
                case ZMOTrack.TrackType.SCALE:
                    bframe.Scale = frame.Scale;
                    break;
            }

            if (createdFare)
            {
                banim.Frames.Add(bframe);
            }

            if (createdAnim)
            {
                BoneAnimations.Add(banim);
            }
        }

        public override string ToString()
        {
            return string.Format("\"{0}\"\nPOS: {1}\nROT {2}", Name, Position, Rotation);
        }
    } // class Bone
}