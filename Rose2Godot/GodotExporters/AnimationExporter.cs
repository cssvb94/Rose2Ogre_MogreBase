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

            nodes.AppendLine($"[node name=\"AnimationPlayer\" type=\"AnimationPlayer\" parent=\"Skeleton\"]");
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
                            track.Add(track_time, new AnimationTrack(
                                                         track_time,
                                                         1f,                 // transition
                                                         GodotVector3.Zero,  // position
                                                         GodotQuat.Identity, // rotation
                                                         GodotVector3.One,   // scale
                                                         channel.Index,
                                                         bone_name));
                        }

                        AnimationTrack anim_track = track[track_time];

                        if (channel.Type == ChannelType.Position)
                        {
                            PositionChannel pchannel = channel as PositionChannel;
                            Vector3 position = pchannel.Frames[frame_idx] * 0.01f;
                            GodotVector3 channel_position = new GodotVector3(position.Z, position.X, -position.Y);
                            //GodotVector3 channel_position = new GodotVector3(position.Z, position.X / 1000f, -position.Y);
                            anim_track.Translation = channel_position;
                            track[track_time] = anim_track;
                            continue;
                        }

                        if (channel.Type == ChannelType.Rotation)
                        {
                            RotationChannel rotation_channel = channel as RotationChannel;
                            GodotQuat bone_rotation = Translator.Rose2GodotRotation(bone.Rotation);
                            GodotQuat inverted_rotation = bone_rotation.UnitInverse();
                            GodotQuat frame_rotation = Translator.Rose2GodotRotation(rotation_channel.Frames[frame_idx]);
                            GodotQuat channel_rotation = inverted_rotation * frame_rotation;
                            anim_track.Rotation = channel_rotation.Normalized();
                            track[track_time] = anim_track;
                            continue;
                        }
 
                        if (channel.Type == ChannelType.Scale) // float
                        {
                            //ScaleChannel scale_channel = channel as ScaleChannel;
                            //float scale = scale_channel.Frames[frame_idx];
                            //anim_track.Scale = new GodotVector3(scale);
                            track[track_time] = anim_track;
                            continue;
                        }

                        if (channel.Type == ChannelType.Alpha) // float
                        {
                            // to be implemented
                            track[track_time] = anim_track;
                            continue;
                        }

                        if (channel.Type == ChannelType.TextureAnimation) // float
                        {
                            // to be implemented
                            track[track_time] = anim_track;
                            continue;
                        }

                        if (channel.Type == ChannelType.Normal) // Vector3
                        {
                            // to be implemented
                            track[track_time] = anim_track;
                            continue;
                        }

                        if (channel.Type == ChannelType.TextureCoordinate1) // Vector2
                        {
                            // to be implemented
                            track[track_time] = anim_track;
                            continue;
                        }

                        if (channel.Type == ChannelType.TextureCoordinate2) // Vector2
                        {
                            // to be implemented
                            track[track_time] = anim_track;
                            continue;
                        }

                        if (channel.Type == ChannelType.TextureCoordinate3) // Vector2
                        {
                            // to be implemented
                            track[track_time] = anim_track;
                            continue;
                        }

                        if (channel.Type == ChannelType.TextureCoordinate4) // Vector2
                        {
                            // to be implemented
                            track[track_time] = anim_track;
                            continue;
                        }
                        
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

                    /*
                    0 (constant)
                    1 (linear)
                    2 (cubic)
                     */
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