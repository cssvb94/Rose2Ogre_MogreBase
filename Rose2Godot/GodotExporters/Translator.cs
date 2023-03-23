using Godot;
using Revise;
using System.Collections.Generic;
using System.Numerics;

namespace Rose2Godot.GodotExporters
{
    public struct Translator
    {
        public static GodotVector3 ToGodotVector3XYZ(Vector3 v) => new GodotVector3(v.X, v.Y, v.Z);
        public static GodotVector3 ToGodotVector3ZXY(Vector3 v) => new GodotVector3(v.Z, v.X, v.Y);

        public static GodotVector3 RoseToGodotVector3(Vector3 v) => new GodotVector3(v.X, v.Z, v.Y);

        public static GodotTransform ToGodotTransform(Quaternion q, Vector3 v) =>
            new GodotTransform(Rose2GodotRotation(q), Rose2GodotPosition(v));

        //public static string Transform2String(Quaternion q, Vector3 pos)
        //{
        //    Vector3 xaxis = q.XAxis;
        //    Vector3 yaxis = q.YAxis;
        //    Vector3 zaxis = q.ZAxis;
        //    return $"Transform({xaxis.X:0.####}, {yaxis.X:0.####}, {zaxis.X:0.####}, {xaxis.Y:0.####}, {yaxis.Y:0.####}, {zaxis.Y:0.####}, {xaxis.Z:0.####}, {yaxis.Z:0.####}, {zaxis.Z:0.####}, {pos.x:0.####}, {pos.y:0.####}, {pos.z:0.####})";
        //}

        public static string GodotTransform2String(GodotTransform t) =>
            $"Transform({t.basis.ToStringNoBrackets()}, {t.origin.ToStringNoBrackets()})";

        public static GodotVector3 Convert(Vector3 vec) => new GodotVector3()
        {
            x = vec.X,
            y = vec.Y,
            z = vec.Z
        };

        public static GodotVector2 Convert(Vector2 vec) => new GodotVector2()
        {
            x = vec.X,
            y = vec.Y
        };

        public static Vector4 Convert(Revise.Types.Color4 c) => new Vector4()
        {
            X = c.Red,
            Y = c.Green,
            Z = c.Blue,
            W = c.Alpha
        };

        // Converts from ROSE (Z-up) to Godot (Y-up)
        public static GodotVector3 Rose2GodotPosition(Vector3 pos) => new GodotVector3()
        {
            x = pos.X,
            y = pos.Z,
            z = pos.Y
        };

        public static ShortVector3 Rose2GodotTriangleIndices(ShortVector3 idx) => new ShortVector3(idx.X, idx.Z, idx.Y);

        // Converts from ROSE to Godot
        public static GodotQuat Rose2GodotRotation(Quaternion rot) => new GodotQuat()
        {
            x = rot.X,
            y = rot.Z,
            z = rot.Y,
            w = -rot.W
        };

        // Converts from ROSE to Godot
        public static GodotVector3 Rose2GodotScale(Vector3 scale) => new GodotVector3()
        {
            x = scale.X,
            y = scale.Z,
            z = scale.Y
        };

        public static string FixPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;
            return path.Replace('\\', '/').Replace("//", "/").Trim().ToUpper();
        }

        public static Vector3 CalculateVector3Normal(Vector3 p1, Vector3 p2, Vector3 p3) => Vector3.Normalize(Vector3.Cross(p2 - p1, p3 - p1));

        public static string QuaternionToArray(List<Quaternion> vlist)
        {
            List<string> qs = new List<string>();
            for (int v_idx = 0; v_idx < vlist.Count; v_idx++)
            {
                Quaternion q = vlist[v_idx];
                qs.Add($"{q.X:0.###},{q.Y:0.###},{q.Z:0.###},{q.W:0.###}");
            }
            return $"FloatArray({string.Join(",", qs.ToArray())})";
        }

        public static string Vector3ToArray(List<Vector3> vlist, float? scale)
        {
            List<string> vs = new List<string>();
            for (int v_idx = 0; v_idx < vlist.Count; v_idx++)
            {
                Vector3 v = vlist[v_idx];
                v.Z *= scale ?? 1f;
                vs.Add($"{v.X:0.###},{v.Y:0.###},{v.Z:0.###}");
            }
            return $"Vector3Array({string.Join(",", vs.ToArray())})";
        }

        public static string TriangleIndices(List<int> vlist)
        {
            List<string> vs = new List<string>();
            for (int idx = 0; idx < vlist.Count; idx++)
                vs.Add($"{vlist[idx]}");
            return $"IntArray({string.Join(",", vs.ToArray())})";
        }

        public static string Vector2fToArray(List<Vector2> vlist)
        {
            List<string> vs = new List<string>();
            foreach (Vector2 v in vlist)
                vs.Add($"{v.X:0.####}, {v.Y:0.####}");

            return $"Vector2Array({string.Join(", ", vs.ToArray())})";
        }
    }
}
