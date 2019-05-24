using Godot;
using Mogre;
using RoseFormats;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rose2Godot.GodotExporters
{

    public struct AnimationTrack
    {
        public float TimeStamp { get; set; }
        public float Transition { get; set; } // default 1.0f in transform track
        public Vector3 Translation { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public string BoneName { get; set; }
        public int BoneId { get; set; }

        private static readonly Translator translator = new Translator();

        public AnimationTrack(float timeStamp, float transition, Vector3 translation, Quaternion rotation, Vector3 scale, int boneId, string boneName)
        {
            TimeStamp = timeStamp; // secs
            Transition = transition; // 1.0f
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
            BoneId = boneId;
            BoneName = boneName;
        }

        public override string ToString()
        {
            GodotQuat gq = translator.ToQuat(Rotation);
            string v3t = string.Format("{0:G5}, {1:G5}, {2:G5}",
                 translator.Round(Translation.x),
                 translator.Round(Translation.y),
                 translator.Round(Translation.z));
            string qr = string.Format("{0:G5}, {1:G5}, {2:G5}, {3:G5}",
                translator.Round(gq.x),
                translator.Round(gq.y),
                translator.Round(gq.z),
                translator.Round(gq.w)
                );
            string v3s = string.Format("{0:G5}, {1:G5}, {2:G5}",
                translator.Round(Scale.x),
                translator.Round(Scale.y),
                translator.Round(Scale.z));

            return string.Format("{0:G5}, {1:G}, {2}, {3}, {4}",
                translator.Round(TimeStamp),
                translator.Round(Transition), v3t, qr, v3s);
        }
    }

    public class Animation
    {
        public string Name { get; set; }
        public int FramesCount { get; set; }
        public float FPS { get; set; }
        public List<AnimationTrack> Tracks { get; set; }

        public Animation(string Name, int FramesCount, float FPS)
        {
            this.Name = Name;
            this.FramesCount = FramesCount;
            this.FPS = FPS;
            Tracks = new List<AnimationTrack>();
        }

        public List<AnimationTrack> GetTracksForBoneId(int boneId)
        {
            return Tracks.Where(b => b.BoneId == boneId).OrderBy(t => t.TimeStamp).ToList();
        }
    }

    public class AnimationExporter
    {
        public int LastResourceIndex { get; }
        private readonly StringBuilder resource;
        private readonly StringBuilder nodes;

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
            ZMD zmd)
        {
            int animation_resource_idx = resource_index;

            System.Console.WriteLine("[Animation export] Start from idx: {0}", animation_resource_idx);

            nodes = new StringBuilder();
            resource = new StringBuilder();
            LastResourceIndex = resource_index;

            nodes.AppendLine("[node name=\"AnimationPlayer\" type=\"AnimationPlayer\" parent=\"Armature\"]");
            //nodes.AppendLine("root_node = NodePath(\"Armature\")");
            nodes.AppendLine("autoplay = \"\"");
            nodes.AppendLine("playback_process_mode = 1");
            nodes.AppendLine("playback_default_blend_time = 0.0");
            nodes.AppendLine("playback_speed = 1.0");
            nodes.AppendLine("blend_times = [  ]");

            List<Animation> animation = new List<Animation>();

            foreach (ZMO mo in zmo)
            {
                animation.Add(new Animation(mo.AnimationName, mo.Frames, mo.FPS));
            }

            // bones
            foreach (RoseBone bone in zmd.Bone)
            {
                foreach (BoneAnimation boneAnimation in bone.BoneAnimations)
                {
                    Animation anim = animation.Find(a => a.Name.Equals(boneAnimation.Name));
                    if (anim != null)
                    {
                        int fidx = 0;
                        foreach (BoneFrame frame in boneAnimation.Frames)
                        {
                            anim.Tracks.Add(new AnimationTrack(fidx++ / anim.FPS, 1f, frame.Position, frame.Rotation, frame.Scale, bone.ID, bone.Name));
                        }
                    }
                }
            }

            //dummies
            foreach (RoseBone bone in zmd.Dummy)
            {
                foreach (BoneAnimation boneAnimation in bone.BoneAnimations)
                {
                    Animation anim = animation.Find(a => a.Name.Equals(boneAnimation.Name));
                    if (anim != null)
                    {
                        int fidx = 0;
                        foreach (BoneFrame frame in boneAnimation.Frames)
                        {
                            anim.Tracks.Add(new AnimationTrack(fidx++ / anim.FPS, 1f, frame.Position, frame.Rotation, frame.Scale, bone.ID, bone.Name));
                        }
                    }
                }
            }

            foreach (Animation anim in animation)
            {
                nodes.AppendFormat("anims/{0} = SubResource({1})\n", anim.Name, animation_resource_idx);

                resource.AppendFormat("[sub_resource id={0} type=\"Animation\"]\n", animation_resource_idx);

                // info

                resource.AppendFormat("; FPS: {0} Frames: {1} Length: {2:G} sec\n", anim.FPS, anim.FramesCount, (float)anim.FramesCount / anim.FPS);
                resource.AppendFormat("resource_name = \"{0}\"\n", anim.Name);
                resource.AppendFormat("length = {0:G5}\n", (float)(anim.FramesCount) / anim.FPS);
                resource.AppendLine("step = 0.05");
                resource.AppendLine("loop = true");


                for (int bone_id = 0; bone_id < zmd.BonesCount; bone_id++)
                {
                    resource.AppendFormat("tracks/{0}/type = \"transform\"\n", bone_id);
                    resource.AppendFormat("tracks/{0}/path = NodePath(\".:{1}\")\n", bone_id, zmd.Bone[bone_id].Name);
                    resource.AppendFormat("tracks/{0}/interp = 2\n", bone_id);
                    resource.AppendFormat("tracks/{0}/loop_wrap = true\n", bone_id);
                    resource.AppendFormat("tracks/{0}/imported = false\n", bone_id);
                    resource.AppendFormat("tracks/{0}/enabled = true\n", bone_id);

                    List<string> transforms = new List<string>();
                    foreach (AnimationTrack track in anim.GetTracksForBoneId(bone_id))
                    {
                        transforms.Add(track.ToString());
                    }
                    resource.AppendFormat("tracks/{0}/keys = PoolRealArray({1})\n", bone_id, string.Join(", ", transforms.ToArray()));
                    //resource.AppendFormat("tracks/{0}/keys = [{1}]\n", bone_id, string.Join(", ", transforms.ToArray()));
                }
                animation_resource_idx++;
                resource.AppendLine();
            }

            LastResourceIndex = animation_resource_idx++;
        }
    }
}
