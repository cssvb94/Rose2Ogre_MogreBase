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
            string v3t = string.Format("{0:0.000}, {1:0.000}, {2:0.000}", Translation.x, Translation.y, Translation.z);
            string qr = string.Format("{0:0.000}, {1:0.000}, {2:0.000}, {3:0.000}", Rotation.x, Rotation.y, Rotation.z, Rotation.w);
            string v3s = string.Format("{0:0.000}, {1:0.000}, {2:0.000}", Scale.x, Scale.y, Scale.z);

            return string.Format("{0:0.00000}, {1:G}, {2}, {3}, {4}", TimeStamp, Transition, v3t, qr, v3s);
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
            int animation_resource_idx = resource_index;

            nodes = new StringBuilder();
            resource = new StringBuilder();
            LastResourceIndex = resource_index;

            nodes.AppendLine("[node name=\"AnimationPlayer\" type=\"AnimationPlayer\" parent=\"Armature\"]");
            //nodes.AppendLine("root_node = NodePath(\"..:Armature\")");
            //nodes.AppendLine("autoplay = \"\"");
            //nodes.AppendLine("playback_process_mode = 1");
            //nodes.AppendLine("playback_default_blend_time = 0.0");
            //nodes.AppendLine("playback_speed = 1.0");
            //nodes.AppendLine("blend_times = [  ]");

            List<Animation> animation = new List<Animation>();

            foreach (ZMO mo in zmo)
            {
                animation.Add(new Animation(mo.AnimationName, mo.Frames, mo.FPS));
            }

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

            foreach (Animation anim in animation)
            {
                nodes.AppendFormat("anims/{0} = SubResource({1})\n", anim.Name, animation_resource_idx);

                resource.AppendFormat("[sub_resource id={0} type=\"Animation\"]\n", animation_resource_idx);

                // info

                resource.AppendFormat("; FPS: {0} Frames: {1} Length: {2:G} sec\n", anim.FPS, anim.FramesCount, (float)anim.FramesCount / anim.FPS);

                resource.AppendFormat("resource_name = \"{0}\"\n", anim.Name);
                resource.AppendFormat("length = {0:0.00000}\n", (float)(anim.FramesCount-1) / anim.FPS);
                //resource.AppendFormat("step = {0:0.0000}\n", (float)anim.FPS / anim.FramesCount);
                //resource.AppendLine("loop = true");


                for (int bone_id = 0; bone_id < zmd.BonesCount; bone_id++)
                {
                    resource.AppendFormat("tracks/{0}/type = \"transform\"\n", bone_id);
                    resource.AppendFormat("tracks/{0}/path = NodePath=(\".:{1}\")\n", bone_id, zmd.Bone[bone_id].Name);
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
                }
                animation_resource_idx++;
                resource.AppendLine();
            }

            LastResourceIndex = animation_resource_idx++;
        }
    }
}
