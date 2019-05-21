using Godot;
using Mogre;
using RoseFormats;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rose2Godot.GodotExporters
{

    public struct AnimationKey
    {
        public float TimeStap { get; set; }
        public float Transition { get; set; } // default 1.0f in transform track
        public Vector3 Translation { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public AnimationKey(float timeStamp, float transition, Vector3 translation, Quaternion rotation, Vector3 scale)
        {
            TimeStap = timeStamp; // secs
            Transition = transition; // 1.0f
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
        }

        public override string ToString()
        {
            string v3t = string.Format("{0:0.0000}, {1:0.0000}, {2:0.0000}", Translation.x, Translation.y, Translation.z);
            string qr = string.Format("{0:0.0000}, {1:0.0000}, {2:0.0000}, {3:0.0000}", Rotation.x, Rotation.y, Rotation.z, Rotation.w);
            string v3s = string.Format("{0:0.0000}, {1:0.0000}, {2:0.0000}", Scale.x, Scale.y, Scale.z);

            return string.Format("{0:0.0000}, {1:0000}, {2}, {3}, {4}", TimeStap, Transition, v3t, qr, v3s);
        }
    }

    public class AnimationExporter
    {
        public int LastResourceIndex { get; }
        private readonly StringBuilder resource;
        private readonly StringBuilder nodes;
        private readonly Translator translator = new Translator();

        public string Resources
        {
            get
            {
                return resource.ToString();
            }
        }

        public string Nodes
        {
            get
            {
                return nodes.ToString();
            }
        }

        public AnimationExporter(
            int resource_index,
            List<ZMO> zmo,
            List<string> animationNames,
            ZMD zmd)
        {
            int idx = resource_index;

            nodes = new StringBuilder();
            resource = new StringBuilder();
            LastResourceIndex = resource_index;

            nodes.AppendLine("[node name=\"AnimationPlayer\" type=\"AnimationPlayer\" parent=\"Armature\"]");
            //nodes.AppendLine("root_node = NodePath(\"..:\")");
            nodes.AppendLine("root_node = NodePath(\"..:Armature\")");

            for (int i = 0; i < zmo.Count; i++)
            {
                resource.AppendFormat("[sub_resource id={0} type=\"Animation\"]\n", idx);
                resource.AppendFormat("resource_name = \"{0}\"\n", animationNames[i]);
                resource.AppendFormat("length = {0:0.0000}\n", (float)zmo[i].Frames / zmo[i].FPS);
                resource.AppendLine("step = 0.1");
                resource.AppendLine("loop = true");

                var godot_tracks = zmo[i].Track.Where(t => t.Type == ZMOTrack.TrackType.POSITION || t.Type == ZMOTrack.TrackType.ROTATION || t.Type == ZMOTrack.TrackType.SCALE)
                    .ToList()
                    .OrderBy(t => t.BoneID)
                    .GroupBy(t => t.BoneID)
                    .Select(t => new {
                        bone_name = zmd.Bone[t.FirstOrDefault().BoneID].Name,
                        tracks = t
                    });

                for (int track_id = 0; track_id < zmo[i].Track.Count; track_id++)
                {
                    string bone_name = zmd.Bone[zmo[i].Track[track_id].BoneID].Name;
                    GodotTransform transform = new GodotTransform(translator.ToQuat(zmo[i].Track[track_id].Rotation), translator.ToVector3(zmo[i].Track[track_id].Position));
                    zmo[i].Track[track_id].Rotation.ToAngleAxis(out Radian radian, out Vector3 axis);
                    resource.AppendFormat("track/{0}/type = \"transform\"\n", track_id);
                    resource.AppendFormat("track/{0}/path = NodePath=(\".:{1}\")\n", track_id, bone_name);
                    resource.AppendLine("track/{0}/interp = 1");
                }

                resource.AppendLine();
                idx++;
            }

            LastResourceIndex = idx++;
        }
    }
}
