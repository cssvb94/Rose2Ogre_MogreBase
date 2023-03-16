using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mogre;

namespace RoseFormats
{
    class ZMO
    {
        public string FormatString;
        public int FPS = 0;
        public int Frames = 0;
        public int Channels = 0;
        public List<ZMOTrack> Track;
        public List<ZMOChannel> Channel;

        private BinaryHelper bh;

        public float Length
        {
            get
            {
                if (FPS == 0)
                {
                    return 0.0f;
                }
                else
                {
                    return (Frames - 1) / (float)FPS;
                }

            }
        }

        public ZMO(string FileName, ZMD zmd)
        {
            Load(FileName, zmd);
        }

        public float FrameTime(int FrameIndex)
        {
            if (Frames == 0) return 0.0f;

            return (FrameIndex / (float)FPS);
        }

        public bool Load(string FileName, ZMD zmd)
        {
            Encoding koreanEncoding = Encoding.GetEncoding("EUC-KR");
            try
            {
                FileStream fileStream = File.OpenRead(FileName);
                BinaryReader br = new BinaryReader(fileStream, koreanEncoding);
                bh = new BinaryHelper(br);

                try
                {
                    FormatString = koreanEncoding.GetString(br.ReadBytes(7));
                    br.ReadByte();

                    FPS = br.ReadInt32();
                    Frames = br.ReadInt32();
                    Channels = br.ReadInt32();

                    Track = new List<ZMOTrack>();

                    Channel = new List<ZMOChannel>();

                    // Read each track description
                    for (int i = 0; i < Channels; i++)
                    {
                        int TracType = br.ReadInt32();
                        int BoneID = br.ReadInt32();

                        Channel.Add(new ZMOChannel((ZMOTrack.TrackType)TracType, BoneID));
                    }

                    // set frames number for each bone
                    foreach (RoseBone bone in zmd.Bone)
                        bone.InitFrames(Frames);

                    // Read tracks data
                    for (int frameIDX = 0; frameIDX < Frames; frameIDX++)
                    {
                        for (int channelIDX = 0; channelIDX < Channels; channelIDX++)
                        {
                            int BoneID = Channel[channelIDX].BoneID;

                            ZMOTrack track = new ZMOTrack(Channel[channelIDX].Type, Channel[channelIDX].BoneID, frameIDX);

                            if (Channel[channelIDX].Type == ZMOTrack.TrackType.POSITION || Channel[channelIDX].Type == ZMOTrack.TrackType.NORMAL)
                            {
                                //read vector
                                track.Position = bh.ReadVector3f();
                                if (zmd != null)
                                {
                                    zmd.Bone[BoneID].Frame[frameIDX].Position = track.Position;

                                }
                            } // position

                            if (Channel[channelIDX].Type == ZMOTrack.TrackType.ROTATION)
                            {
                                //read quat
                                Quaternion q = bh.ReadQuaternion();
                                track.Rotation = q;
                                if (zmd != null)
                                {
                                    zmd.Bone[BoneID].Frame[frameIDX].Rotation = track.Rotation;
                                }
                            } // rotation

                            if (Channel[channelIDX].Type == ZMOTrack.TrackType.ALPHA || Channel[channelIDX].Type == ZMOTrack.TrackType.TEXTUREANIM)
                            {
                                //read float
                                track.Value = br.ReadSingle();
                            } // alpha

                            if (Channel[channelIDX].Type == ZMOTrack.TrackType.SCALE)
                            {
                                track.Value = br.ReadSingle();
                                if (zmd != null)
                                {
                                    zmd.Bone[BoneID].Frame[frameIDX].Scale = new Vector3(track.Value, track.Value, track.Value);
                                }
                            }

                            if (Channel[channelIDX].Type == ZMOTrack.TrackType.UV1 || Channel[channelIDX].Type == ZMOTrack.TrackType.UV2 || Channel[channelIDX].Type == ZMOTrack.TrackType.UV3 || Channel[channelIDX].Type == ZMOTrack.TrackType.UV4)
                            {
                                //read vector
                                track.UV = bh.ReadVector2f();
                            } // UV
                            Track.Add(track);
                        } // bones
                    } // frames

                } // try
                finally
                {
                    br.Close();
                    fileStream.Close();
                }

            } // try open file
            catch (Exception)
            {
                return false;
            } // catch open file
            return true;
        } // Load

    } // class
}
