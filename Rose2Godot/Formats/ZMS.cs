using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mogre;

namespace RoseFormats
{
    public class ZMS
    {
        public const uint ZMS_UVMAP3 = 1024;
        public const uint ZMS_UVMAP2 = 512;
        public const uint ZMS_UVMAP1 = 256;
        public const uint ZMS_UVMAP0 = 128;
        public const uint ZMS_TANGENT = 64;
        public const uint ZMS_BONEINDICES = 32;
        public const uint ZMS_BLENDWEIGHT = 16;
        public const uint ZMS_COLOR = 8;
        public const uint ZMS_NORMAL = 4;
        public const uint ZMS_POSITION = 2;

        public string FormatString;
        public uint VertexFormat;
        public byte ZMSFileType;

        public Vector3 MinBounds;
        public Vector3 MaxBounds;

        public ushort BonesCount;
        public ushort VertexCount;
        public ushort FaceCount;
        public ushort StripCount;
        public ushort MaterialCount;
        public ushort MaterialType;
        public string MaterialName;

        public List<ushort> BoneIndices = new List<ushort>();
        public List<BoneWeight> BoneWeights = new List<BoneWeight>();

        public List<Vector3w> Face = new List<Vector3w>();
        public List<Vector3> Vertex = new List<Vector3>();
        public List<Vector3> Normal = new List<Vector3>();
        public List<Vector3> Tangent = new List<Vector3>();
        public List<Vector4> Color = new List<Vector4>();
        public List<UInt16> Strip = new List<UInt16>();
        public List<Vector2>[] UV = new List<Vector2>[4] { new List<Vector2>(), new List<Vector2>(), new List<Vector2>(), new List<Vector2>() };

        private BinaryHelper bh;
        //private float scaleFactor = 10.0f;

        public ZMS(string FileName)
        {
            MaterialName = Path.GetFileNameWithoutExtension(FileName);
            Load(FileName);
        }

        public ZMS()
        {
            MaterialName = string.Empty;
        }

        public void Clear()
        {
            FormatString = string.Empty;
            VertexFormat = 0;
            VertexCount = 0;
            BonesCount = 0;
            FaceCount = 0;
            StripCount = 0;
            MaterialCount = 0;
            MaterialType = 0;
            ZMSFileType = 0;
            BoneIndices.Clear();
            Face.Clear();
            Vertex.Clear();
            Normal.Clear();
            Tangent.Clear();
            Color.Clear();
            Strip.Clear();
            BoneWeights.Clear();
            for (int i = 0; i < 4; i++)
            {
                UV[i].Clear();
            }
        }

        public bool Load(string FileName)
        {
            Encoding koreanEncoding = Encoding.GetEncoding("EUC-KR");
            try
            {
                FileStream fileStream = File.OpenRead(FileName);
                BinaryReader br = new BinaryReader(fileStream, koreanEncoding);
                bh = new BinaryHelper(br);

                try
                {
                    FormatString = koreanEncoding.GetString(br.ReadBytes(8));

                    ZMSFileType = byte.Parse(FormatString[6].ToString());

                    VertexFormat = br.ReadUInt32();

                    MinBounds = bh.ReadVector3f();

                    MaxBounds = bh.ReadVector3f();

                    BonesCount = br.ReadUInt16();

                    for (int i = 0; i < BonesCount; i++)
                    {
                        BoneIndices.Add(br.ReadUInt16());
                    }

                    if (ZMSFileType >= 7)
                    {

                        LoadMeshType8(br);
                    }
                    else
                    {
                        LoadMeshType6(br);
                    }

                    StripCount = br.ReadUInt16();

                    for (int sindx = 0; sindx < StripCount; sindx++)
                    {
                        Strip.Add(br.ReadUInt16());
                    }

                    MaterialType = br.ReadUInt16();
                }
                finally
                {
                    // Close
                    br.Close();
                    fileStream.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void LoadMeshType8(BinaryReader br)
        {
            VertexCount = br.ReadUInt16();

            if (HasPosition())
            {
                for (int i = 0; i < VertexCount; i++)
                {
                    Vertex.Add(bh.ReadVector3f());
                }
            }

            if (HasNormal())
            {
                for (int i = 0; i < VertexCount; i++)
                {
                    Normal.Add(bh.ReadVector3f());
                }
            }

            if (HasColor())
            {
                for (int i = 0; i < VertexCount; i++)
                {
                    Color.Add(bh.ReadVector4f());
                }
            }

            if (HasSkin() && HasPosition())
            {

                for (int i = 0; i < VertexCount; i++)
                {
                    Vector4 weights = bh.ReadVector4f();
                    Vector4w ids = bh.ReadVector4w();

                    for (int wi = 0; wi < 4; wi++)
                    {
                        // do not optimize - add all 4 bone weights per vertex
                        //if (weights[wi] != 0.0f)
                        //{
                        BoneWeights.Add(new BoneWeight(i, BoneIndices[ids[wi]], weights[wi]));
                        //}
                    }

                }
            }

            if (HasTangents())
            {
                for (int i = 0; i < VertexCount; i++)
                {
                    Tangent.Add(bh.ReadVector3f());
                }
            }

            for (int c = 0; c < 4; c++)
            {
                if (HasUVChannel(c))
                {
                    for (int i = 0; i < VertexCount; i++)
                    {
                        UV[c].Add(bh.ReadVector2f());
                    }
                }
            }

            FaceCount = br.ReadUInt16();

            for (int findex = 0; findex < FaceCount; findex++)
            {
                Face.Add(bh.ReadVector3w());
            }
        } // Load8

        private void LoadMeshType6(BinaryReader br)
        {
            UInt16 vidx;
            VertexCount = br.ReadUInt16();


            if (HasPosition())
            {
                for (int i = 0; i < VertexCount; i++)
                {
                    vidx = br.ReadUInt16();
                    Vertex.Add(bh.ReadVector3f());
                }
            }

            if (HasNormal())
            {
                for (int i = 0; i < VertexCount; i++)
                {
                    vidx = br.ReadUInt16();
                    Normal.Add(bh.ReadVector3f());
                }
            }

            if (HasColor())
            {
                for (int i = 0; i < VertexCount; i++)
                {
                    Color.Add(bh.ReadVector4f());
                }
            }

            if (HasSkin())
            {

                for (int i = 0; i < VertexCount; i++)
                {
                    vidx = br.ReadUInt16();
                    Vector4 weights = bh.ReadVector4f();
                    Vector4w ids = bh.ReadVector4w();

                    for (int wi = 0; wi < 4; wi++)
                    {
                        if (weights[wi] != 0.0f)
                        {
                            BoneWeights.Add(new BoneWeight(i, BoneIndices[ids[wi]], weights[wi]));
                        }
                    }

                }
            }

            if (HasTangents())
            {
                for (int i = 0; i < VertexCount; i++)
                {
                    vidx = br.ReadUInt16();
                    Tangent.Add(bh.ReadVector3f());
                }
            }

            for (int c = 0; c < 4; c++)
            {
                if (HasUVChannel(c))
                {
                    for (int i = 0; i < VertexCount; i++)
                    {
                        vidx = br.ReadUInt16();
                        // (x, 1.0f - y)
                        UV[c].Add(bh.ReadUVVector2f());
                    }
                }
            }

            FaceCount = br.ReadUInt16();

            for (int findex = 0; findex < FaceCount; findex++)
            {
                vidx = br.ReadUInt16();
                Face.Add(bh.ReadVector3w());
            }
        } // Load6

        #region Properties checks

        public bool HasPosition()
        {
            return (VertexFormat & ZMS_POSITION) > 0;
        }

        public bool HasNormal()
        {
            return (VertexFormat & ZMS_NORMAL) > 0;
        }

        public bool HasColor()
        {
            return (VertexFormat & ZMS_COLOR) > 0;
        }

        public bool HasBoneWeight()
        {
            return (VertexFormat & ZMS_BLENDWEIGHT) > 0;
        }

        public bool HasBoneIndex()
        {
            return (VertexFormat & ZMS_BONEINDICES) > 0;
        }

        public bool HasSkin()
        {
            return (HasBoneWeight() & HasBoneIndex());
        }

        public bool HasTangents()
        {
            return (VertexFormat & ZMS_TANGENT) > 0;
        }

        public bool HasUVChannel(int Channel)
        {
            return (VertexFormat & (ZMS_UVMAP0 << Channel)) > 0;
        }

        public bool HasUV0()
        {
            return (VertexFormat & ZMS_UVMAP0) > 0;
        }

        public bool HasUV1()
        {
            return (VertexFormat & ZMS_UVMAP1) > 0;
        }

        public bool HasUV2()
        {
            return (VertexFormat & ZMS_UVMAP2) > 0;
        }

        public bool HasUV3()
        {
            return (VertexFormat & ZMS_UVMAP3) > 0;
        }

        #endregion
    }
}
