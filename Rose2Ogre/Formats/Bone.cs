
using System.Collections.Generic;
using Mogre;

namespace RoseFormats
{
    class BoneAnimation
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

        public BoneAnimation()
        {
            Rotation = Quaternion.IDENTITY;
            Position = Vector3.ZERO;
            Scale = Vector3.UNIT_SCALE;
        }
    }

    class RoseBone
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
        // the bone animation transformation for each frame
        public BoneAnimation[] Frame;

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
        }

        // Normal bone read sequence
        public RoseBone(int ParentID, string Name, Vector3 Position, Quaternion Rotation)
        {
            this.Name = Name;
            this.Position = Position;
            this.Rotation = Rotation;
            isDummy = false;
            this.ParentID = ParentID;
        }

        // Dummy read sequence ZMD0003
        public RoseBone(string Name, int ParentID, Vector3 Position, Quaternion Rotation)
        {
            this.Name = Name;
            this.Position = Position;
            this.Rotation = Rotation;
            this.ParentID = ParentID;
            isDummy = true;
        }

        // Dummy read sequence ZMD0002
        public RoseBone(string Name, int ParentID, Vector3 Position)
        {
            this.Name = Name;
            this.Position = Position;
            this.Rotation = new Quaternion();
            this.ParentID = ParentID;
            isDummy = true;
        }

        public void InitFrames(int FramesCount)
        {
            Frame = new BoneAnimation[FramesCount];

            for (int i = 0; i < FramesCount; i++)
            {
                Frame[i] = new BoneAnimation();
            }
        }

    } // class Bone
}
