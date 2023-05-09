using Godot;
using NLog;
using Revise.ZMD;
using Revise.ZMO;
using Revise.ZMO.Channels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Rose2Godot.GodotExporters
{
    public class AnimationExporter
    {
        private static readonly Logger log = LogManager.GetLogger("AnimationExporter");
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
                                                         TrackType.None,
                                                         channel.Index,
                                                         bone_name));
                        }

                        AnimationTrack anim_track = track[track_time];

                        if (channel.Type == ChannelType.Position)
                        {
                            PositionChannel pchannel = channel as PositionChannel;
                            Vector3 position = pchannel.Frames[frame_idx] * 0.01f;
                            GodotVector3 channel_position = new GodotVector3(position.Z, position.X, -position.Y);
                            anim_track.Position = channel_position;
                            anim_track.TrackType |= TrackType.Position;
                            track[track_time] = anim_track;
                            continue;
                        }

                        if (channel.Type == ChannelType.Rotation)
                        {
                            RotationChannel rotation_channel = channel as RotationChannel;
                            GodotQuat bone_rotation = Translator.Rose2GodotRotationXZYnW(bone.Rotation);
                            GodotQuat inverted_rotation = bone_rotation.UnitInverse();
                            GodotQuat frame_rotation = Translator.Rose2GodotRotationXZYnW(rotation_channel.Frames[frame_idx]);
                            //GodotQuat channel_rotation = inverted_rotation * frame_rotation; //ONLY in Godot 3!
                            GodotQuat channel_rotation = frame_rotation; // Only in Godot 4!
                            anim_track.Rotation = channel_rotation.Normalized();
                            anim_track.TrackType |= TrackType.Rotation;
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
                int track_idx = 0;
                nodes.AppendFormat("anims/{0} = SubResource({1})\n", anim.Name, animation_resource_idx);
                resource.AppendFormat("[sub_resource id={0} type=\"Animation\"]\n", animation_resource_idx);

                // animation header

                resource.AppendFormat("; FPS: {0} Frames: {1} Length: {2:G} sec\n", anim.FPS, anim.FramesCount, anim.FramesCount / anim.FPS);
                resource.AppendFormat("resource_name = \"{0}\"\n", anim.Name);
                resource.AppendFormat("length = {0:0.#####}\n", anim.FramesCount / anim.FPS);
                resource.AppendLine($"step = {1f / anim.FPS:0.#####}");
                resource.AppendLine("loop_mode = 1");

                foreach (var bone_tracks in anim.Tracks)
                {
                    List<string> transforms = new List<string>();
                    var tracks = bone_tracks.Value;
                    string bone_name = bone_tracks.Key;

                    var rotation_tracks = tracks.Where(t => (t.Value.TrackType & TrackType.Rotation) > 0);
                    if (rotation_tracks.Any())
                    {
                        resource.AppendFormat($"tracks/{track_idx}/type = \"rotation_3d\"\n"); // Godot 4
                        resource.AppendFormat("tracks/{0}/path = NodePath(\".:{1}\")\n", track_idx, bone_name);
                        resource.AppendFormat("tracks/{0}/interp = 1\n", track_idx);
                        resource.AppendFormat("tracks/{0}/loop_wrap = true\n", track_idx);
                        resource.AppendFormat("tracks/{0}/imported = true\n", track_idx);
                        resource.AppendFormat("tracks/{0}/enabled = true\n", track_idx);

                        foreach (var track in rotation_tracks)
                        {
                            AnimationTrack local_track = track.Value;
                            transforms.Add(local_track.ToRotation3D()); // Godot 4
                        }
                        resource.AppendFormat($"tracks/{track_idx}/keys = PackedFloat32Array({string.Join(", ", transforms.ToArray())})\n"); // Godot 4
                        track_idx++;
                    }

                    var position_tracks = tracks.Where(t => (t.Value.TrackType & TrackType.Position) > 0);
                    if (position_tracks.Any())
                    {
                        resource.AppendFormat($"tracks/{track_idx}/type = \"position_3d\"\n"); // Godot 4
                        resource.AppendFormat("tracks/{0}/path = NodePath(\".:{1}\")\n", track_idx, bone_name);
                        resource.AppendFormat("tracks/{0}/interp = 1\n", track_idx);
                        resource.AppendFormat("tracks/{0}/loop_wrap = true\n", track_idx);
                        resource.AppendFormat("tracks/{0}/imported = true\n", track_idx);
                        resource.AppendFormat("tracks/{0}/enabled = true\n", track_idx);

                        foreach (var track in position_tracks)
                        {
                            AnimationTrack local_track = track.Value;
                            transforms.Add(local_track.ToRotation3D()); // Godot 4
                        }
                        resource.AppendFormat($"tracks/{track_idx}/keys = PackedFloat32Array({string.Join(", ", transforms.ToArray())})\n"); // Godot 4
                        track_idx++;
                    }

                    var scale_tracks = tracks.Where(t => t.Value.TrackType == TrackType.Scale);
                    if (scale_tracks.Any())
                    {
                        resource.AppendFormat($"tracks/{track_idx}/type = \"scale_3d\"\n"); // Godot 4
                        resource.AppendFormat("tracks/{0}/path = NodePath(\".:{1}\")\n", track_idx, bone_name);
                        resource.AppendFormat("tracks/{0}/interp = 1\n", track_idx);
                        resource.AppendFormat("tracks/{0}/loop_wrap = true\n", track_idx);
                        resource.AppendFormat("tracks/{0}/imported = true\n", track_idx);
                        resource.AppendFormat("tracks/{0}/enabled = true\n", track_idx);

                        foreach (var track in scale_tracks)
                        {
                            AnimationTrack local_track = track.Value;
                            transforms.Add(local_track.ToScale3D()); // Godot 4
                        }
                        resource.AppendFormat($"tracks/{track_idx}/keys = PackedFloat32Array({string.Join(", ", transforms.ToArray())})\n"); // Godot 4
                        track_idx++;
                    }
                }
                animation_resource_idx++;
                resource.AppendLine();
            }
            last_resource_index = animation_resource_idx++;
        }
    }
}