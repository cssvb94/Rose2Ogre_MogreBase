using Godot;
using Revise.ZMD;
using Revise.ZMO;
using Revise.ZMO.Channels;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace Rose2Godot.GodotExporters
{
    public class AnimationExporter
    {
        public int last_resource_index { get; private set; }
        private readonly StringBuilder resource;
        private readonly StringBuilder nodes;

        public string Resources => resource.ToString();

        public string Nodes => nodes.ToString();

        public AnimationExporter(int resource_index, List<MotionFile> zmo_files, BoneFile zmd)
        {
            int animation_resource_idx = resource_index;

            nodes = new StringBuilder();
            resource = new StringBuilder();
            last_resource_index = resource_index;

            nodes.AppendLine($"[node name=\"AnimationPlayer\" type=\"AnimationPlayer\" parent=\"Armature\"]");
            nodes.AppendLine("autoplay = \"\"");
            nodes.AppendLine("playback_process_mode = 1");
            nodes.AppendLine("playback_default_blend_time = 0.0");
            nodes.AppendLine("playback_speed = 1.0");
            nodes.AppendLine("blend_times = [  ]");

            int bones_count = zmd.Bones.Count;

            List<Animation> animations = new List<Animation>();

            foreach (MotionFile zmo in zmo_files)
            {
                Animation animation = new Animation(Path.GetFileNameWithoutExtension(zmo.FilePath), zmo.FrameCount, zmo.FramesPerSecond);

                for (int i = 0; i < zmo.ChannelCount; i++)
                {
                    MotionChannel channel = zmo[i];
                    Bone bone;
                    if (channel.Index >= bones_count)
                    {
                        int dummy_idx = bones_count - channel.Index;
                        if (dummy_idx < 0)
                            continue;
                        bone = zmd.DummyBones[dummy_idx];
                    }
                    else
                    {
                        bone = zmd.Bones[channel.Index];
                    }

                    string bone_name = bone.Name;

                    if (!animation.Tracks.ContainsKey(bone_name))
                        animation.Tracks.Add(bone_name, new Dictionary<float, AnimationTrack>());

                    var track = animation.Tracks[bone_name];
                    for (int frame_idx = 0; frame_idx < zmo.FrameCount; ++frame_idx)
                    {
                        float track_time = frame_idx / animation.FPS;
                        if (!track.ContainsKey(track_time))
                        {
                            GodotQuat channel_rotation = GodotQuat.Identity;
                            //GodotVector3 channel_position = Translator.Rose2GodotPosition(bone.Translation / 1000f);
                            GodotVector3 channel_position = new GodotVector3(bone.Translation.Z / 1000f, bone.Translation.X / 1000f, bone.Translation.Y / 1000f);

                            track.Add(track_time, new AnimationTrack(
                                                        track_time,
                                                        1f,
                                                        channel_position,
                                                        channel_rotation,
                                                        GodotVector3.One,
                                                        channel.Index,
                                                        bone_name));

                        }

                        AnimationTrack anim_track = track[track_time];

                        if (channel.Type == ChannelType.Rotation)
                        {
                            RotationChannel rchannel = channel as RotationChannel;
                            GodotQuat bone_rotation = Translator.Rose2GodotRotation(bone.Rotation);
                            GodotQuat inverted_rotation = bone_rotation.UnitInverse();
                            GodotQuat frame_rotation = Translator.Rose2GodotRotation(rchannel.Frames[frame_idx]);
                            GodotQuat channel_rotation = inverted_rotation * frame_rotation;
                            anim_track.Rotation = channel_rotation.Normalized();

                        }

                        if (channel.Type == ChannelType.Position)
                        {
                            PositionChannel pchannel = channel as PositionChannel;
                            Vector3 position = pchannel.Frames[frame_idx] / 100f;
                            GodotVector3 channel_position = new GodotVector3(position.Z, position.X / 1000f, -position.Y);
                            anim_track.Translation = channel_position;
                        }

                        track[track_time] = anim_track;
                    }
                }
                animations.Add(animation);
            }

            foreach (Animation anim in animations)
            {
                nodes.AppendFormat("anims/{0} = SubResource({1})\n", anim.Name, animation_resource_idx);
                resource.AppendFormat("[sub_resource id={0} type=\"Animation\"]\n", animation_resource_idx);

                // animation header

                resource.AppendFormat("; FPS: {0} Frames: {1} Length: {2:G} sec\n", anim.FPS, anim.FramesCount, anim.FramesCount / anim.FPS);
                resource.AppendFormat("resource_name = \"{0}\"\n", anim.Name);
                resource.AppendFormat("length = {0:0.#####}\n", anim.FramesCount / anim.FPS);
                resource.AppendLine($"step = {1f / anim.FPS:0.#####}");
                resource.AppendLine("loop = true");

                foreach (var bone_tracks in anim.Tracks)
                {
                    List<string> transforms = new List<string>();
                    int track_num = 0;
                    var track_value = bone_tracks.Value;
                    string bone_name = bone_tracks.Key;
                    int bone_id = track_value[0].BoneId;

                    resource.AppendFormat("tracks/{0}/type = \"transform\"\n", bone_id);
                    resource.AppendFormat("tracks/{0}/path = NodePath(\".:{1}\")\n", bone_id, bone_name);
                    resource.AppendFormat("tracks/{0}/interp = 1\n", bone_id);
                    resource.AppendFormat("tracks/{0}/loop_wrap = true\n", bone_id);
                    resource.AppendFormat("tracks/{0}/imported = true\n", bone_id);
                    resource.AppendFormat("tracks/{0}/enabled = true\n", bone_id);

                    foreach (var time_track in track_value)
                    {
                        AnimationTrack track = time_track.Value;
                        transforms.Add(track.ToString());
                        track_num++;
                    }
                    resource.AppendFormat("tracks/{0}/keys = PoolRealArray({1})\n", bone_id, string.Join(", ", transforms.ToArray()));
                }
                animation_resource_idx++;
                resource.AppendLine();
            }
            last_resource_index = animation_resource_idx++;
        }
    }
}