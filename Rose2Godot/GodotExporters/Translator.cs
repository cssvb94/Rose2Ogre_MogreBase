using Godot;
using Mogre;

namespace Rose2Godot.GodotExporters
{
    public struct Translator
    {
        public GodotQuat ToQuat(Quaternion q)
        {
            return new GodotQuat(q.x, q.y, q.z, q.w);
        }

        public GodotVector3 ToVector3(Vector3 v)
        {
            return new GodotVector3(v.x, v.y, v.z);
        }

        public GodotVector2 ToVector2(Vector2 v)
        {
            return new GodotVector2(v.x, v.y);
        }

        public float Round(float val)
        {
            return (float)System.Math.Round(val, 4, System.MidpointRounding.AwayFromZero);
        }

        public float Round(double val)
        {
            return (float)System.Math.Round(val, 4, System.MidpointRounding.AwayFromZero);
        }

        public string Vector32String(GodotVector3 v)
        {
            return string.Format("{0:G4}, {1:G4}, {2:G4}",
                Round(v.x),
                Round(v.y),
                Round(v.z));
        }

        public string Transform2String(GodotTransform t)
        {
            return string.Format("Transform({0}, {1}, {2}, {3})",
                Vector32String(t.basis.x),
                Vector32String(t.basis.y),
                Vector32String(t.basis.z),
                Vector32String(t.origin));
        }
    }
}
