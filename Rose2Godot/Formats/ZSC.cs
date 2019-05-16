using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mogre;

namespace RoseFormats
{
    public class ZSCMaterial
    {
        public bool DoubleSided;
        public bool UseAlpha;
        public uint AlphaTest;
        public float AlphaRef;
        public bool zTest;
        public bool zWrite;
        public uint BlendType; //
        public uint Specular;
        public string Path;
        public float Alpha;
        public uint GlowType;
        public float r;
        public float g;
        public float b;

        public ZSCMaterial()
        {
            Path = string.Empty;
        }
    }

    public class ZSCPart
    {
        public uint MeshID;
        public uint MaterialID;
        public int ParentID;
        public int BoneID;
        public int DummyID;
        public uint CollisionType;
        public uint RangeMode;
        public uint UseLightmap;
        public string ZMOPath;
        public Matrix4 TransformMatrix;
        public Vector3 Scale;
        public Vector3 Position;
        public Quaternion AxisRotation;
        public Quaternion Rotation;

        public ZSCPart()
        {
            BoneID = -1;
            DummyID = -1;
            ParentID = -1;
            ZMOPath = string.Empty;
            TransformMatrix = new Matrix4();
            AxisRotation = new Quaternion();
            Rotation = new Quaternion();
            Scale = new Vector3();
            Position = new Vector3();
        }
    }

    public class ZSCEffect
    {
        public uint EffectID;
        public uint EffectType;
        public int ParrentID;
        public Vector3 Scale;
        public Vector3 Position;
        public Quaternion Rotation;
        public Matrix4 TransformMatrix;

        public ZSCEffect()
        {
            ParrentID = -1;
            TransformMatrix = new Matrix4();
            Rotation = new Quaternion();
            Scale = new Vector3();
            Position = new Vector3();
        }
    }

    public class ZSCModel
    {
        public List<ZSCPart> Part;
        public List<ZSCEffect> Effect;
        public Vector3 BBoxMin;
        public Vector3 BBoxMax;
        public uint BBRadius;
        public uint BBX;
        public uint BBY;

        public ZSCModel()
        {
            Part = new List<ZSCPart>();
            Effect = new List<ZSCEffect>();
            BBoxMin = new Vector3();
            BBoxMax = new Vector3();
        }
    }

    public class ZSC
    {
        #region ZSC constants
        public const byte ZSC_PART_POSITION = 0x01;
        public const byte ZSC_PART_ROTATION = 0x02;
        public const byte ZSC_PART_SCALE = 0x03;
        public const byte ZSC_PART_AXISROTATION = 0x04;
        public const byte ZSC_PART_BONEINDEX = 0x05;
        public const byte ZSC_PART_DUMMYINDEX = 0x06;
        public const byte ZSC_PART_PARENT = 0x07;
        public const byte ZSC_PART_COLLISION = 0x1D;
        public const byte ZSC_PART_ZMOPATH = 0x1E;
        public const byte ZSC_PART_RANGEMODE = 0x1F;
        public const byte ZSC_PART_LIGHTMAPMODE = 0x20;

        public const byte ZSC_EFFECT_POSITION = 0x01;
        public const byte ZSC_EFFECT_ROTATION = 0x02;
        public const byte ZSC_EFFECT_SCALE = 0x03;
        public const byte ZSC_EFFECT_PARENT = 0x07;

        public const byte ZSC_TRANSITION_NONE = 0x00;
        public const byte ZSC_TRANSITION_ROTATE = 1 << 0x0;
        public const byte ZSC_TRANSITION_SCALE = 1 << 1;
        public const byte ZSC_TRANSITION_TRANSLATE = 1 << 2;

        public const byte ZSC_COLLISION_NONE = 0x00;
        public const byte ZSC_COLLISION_SPHERE = 0x01;
        public const byte ZSC_COLLISION_AXISALIGNEDBOUNDINGBOX = 0x02;
        public const byte ZSC_COLLISION_ORIENTEDBOUNDINGBOX = 0x03;
        public const byte ZSC_COLLISION_POLYGON = 0x04;
        #endregion

        public List<string> MeshName;
        public List<ZSCMaterial> Material;
        public List<string> EffectName;
        public List<ZSCModel> Model;

        private uint MeshCount;
        private uint MaterialCount;
        private BinaryReader br;
        private BinaryHelper bh;
        private Encoding koreanEncoding = Encoding.GetEncoding("EUC-KR");

        public ZSC()
        {
            MeshName = new List<string>();
            Material = new List<ZSCMaterial>();
            EffectName = new List<string>();
            Model = new List<ZSCModel>();
        }

        public ZSC(string FileName)
        {
            MeshName = new List<string>();
            Material = new List<ZSCMaterial>();
            EffectName = new List<string>();
            Model = new List<ZSCModel>();
            Load(FileName);
        }

        public bool Load(string FileName)
        {
            try
            {
                FileStream fileStream = File.OpenRead(FileName);
                br = new BinaryReader(fileStream, koreanEncoding);
                bh = new BinaryHelper(br);

                try
                {
                    MeshCount = bh.ReadWord();
                    for (int meshID = 0; meshID < MeshCount; meshID++)
                    {
                        MeshName.Add(bh.ReadZString());
                    }

                    MaterialCount = bh.ReadWord();
                    for (int materialID = 0; materialID < MaterialCount; materialID++)
                    {
                        ZSCMaterial mat = new ZSCMaterial();
                        mat.Path = bh.ReadZString();
                        br.ReadBytes(2); // Skip 2 bytes!
                        mat.UseAlpha = (bh.ReadWord() > 0);
                        mat.DoubleSided = (bh.ReadWord() > 0);
                        mat.AlphaTest = bh.ReadWord();
                        mat.AlphaRef = (bh.ReadWord() / 256.0f);
                        mat.zTest = (bh.ReadWord() > 0);
                        mat.zWrite = (bh.ReadWord() > 0);
                        mat.BlendType = bh.ReadWord();
                        mat.Specular = bh.ReadWord();
                        mat.Alpha = br.ReadSingle();
                        mat.GlowType = bh.ReadWord();
                        mat.r = br.ReadSingle();
                        mat.g = br.ReadSingle();
                        mat.b = br.ReadSingle();
                        Material.Add(mat);
                    }

                    uint effectsNames_count = bh.ReadWord();
                    for (int effID = 0; effID < effectsNames_count; effID++)
                    {
                        EffectName.Add(bh.ReadZString());
                    }

                    uint models_count = bh.ReadWord();
                    for (int modID = 0; modID < models_count; modID++)
                    {
                        ZSCModel model = new ZSCModel();
                        model.BBRadius = bh.ReadDWord();
                        model.BBX = bh.ReadDWord();
                        model.BBY = bh.ReadDWord();

                        uint parts_count = bh.ReadWord();
                        if (parts_count > 0)
                        {
                            // PARTS
                            for (int jparts = 0; jparts < parts_count; jparts++)
                            {
                                ZSCPart part = ReadPart();
                                if (jparts == 0)
                                {
                                    part.ParentID = -1;
                                }
                                model.Part.Add(part);
                            } // parts

                            // EFFECTS
                            uint effects_count = bh.ReadWord();
                            for (int jeffects = 0; jeffects < effects_count; jeffects++)
                            {
                                ZSCEffect effect = ReadEffect();
                                model.Effect.Add(effect);
                            }
                        } // if > 0
                        else
                        {
                            Model.Add(model);
                            continue;
                        }

                        model.BBoxMin = bh.ReadVector3f() * 0.01f;
                        model.BBoxMax = bh.ReadVector3f() * 0.01f;
                        Model.Add(model);
                    }
                }
                finally
                {
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

        private ZSCPart ReadPart()
        {
            uint transFlags = ZSC_TRANSITION_NONE;

            ZSCPart part = new ZSCPart();
            part.MeshID = bh.ReadWord();
            part.MaterialID = bh.ReadWord();

            while (true)
            {
                byte IDFlag = br.ReadByte();
                if (IDFlag == 0) break;
                byte size = br.ReadByte();
                switch (IDFlag)
                {
                    case ZSC_PART_POSITION: // $01 - Size $0C (12)
                        part.Position = bh.ReadVector3f() * 0.01f;
                        transFlags = transFlags | ZSC_TRANSITION_TRANSLATE;
                        break;
                    case ZSC_PART_ROTATION: // $02 - Size $10 (16)
                        part.Rotation = bh.ReadQuaternion();
                        transFlags = transFlags | ZSC_TRANSITION_ROTATE;
                        break;
                    case ZSC_PART_SCALE: // $03 - Size $0C (12)
                        part.Scale = bh.ReadVector3f();
                        transFlags = transFlags | ZSC_TRANSITION_SCALE;
                        break;
                    case ZSC_PART_AXISROTATION: // $04 - Size $0C (12)
                        part.AxisRotation = bh.ReadQuaternion();
                        break;
                    case ZSC_PART_BONEINDEX: // $05 - Size $02 (02)
                        part.BoneID = (int)bh.ReadWord();
                        break;
                    case ZSC_PART_DUMMYINDEX: // $06 - Size $02 (02)
                        part.DummyID = (int)bh.ReadWord();
                        break;
                    case ZSC_PART_PARENT: // $07 - Size $02 (02)
                        part.ParentID = (int)bh.ReadWord();
                        break;
                    case ZSC_PART_COLLISION: // $1D - Size $02 (02)
                        part.CollisionType = bh.ReadWord();
                        break;
                    case ZSC_PART_ZMOPATH: // $1E (30)
                        byte[] bs = br.ReadBytes(size);
                        part.ZMOPath = koreanEncoding.GetString(bs);
                        break;
                    case ZSC_PART_RANGEMODE: // $1F - Size $02 (02)
                        part.RangeMode = bh.ReadWord();
                        break;
                    case ZSC_PART_LIGHTMAPMODE: // $20 - Size $02 (02)
                        part.UseLightmap = bh.ReadWord();
                        break;
                    default:
                        br.ReadBytes(size);
                        break;
                } // switch case
            };

            part.TransformMatrix = Matrix4.IDENTITY;

            if ((transFlags & ZSC_TRANSITION_ROTATE) != 0)
            {
                part.TransformMatrix = new Matrix4(part.Rotation.ToRotationMatrix());
            }

            if ((transFlags & ZSC_TRANSITION_SCALE) != 0)
            {
                part.TransformMatrix.SetScale(part.Scale);
            }

            if ((transFlags & ZSC_TRANSITION_TRANSLATE) != 0)
            {
                part.TransformMatrix.SetTrans(part.Position);
            }
            return part;
        } // Read PART

        private ZSCEffect ReadEffect()
        {
            uint transFlags = ZSC_TRANSITION_NONE;

            ZSCEffect effect = new ZSCEffect();

            effect.EffectID = bh.ReadWord();
            effect.EffectType = bh.ReadWord();
            transFlags = ZSC_TRANSITION_NONE;

            while (true)
            {
                byte IDFlag = br.ReadByte();
                if (IDFlag == 0) break;
                byte size = br.ReadByte();
                switch (IDFlag)
                {
                    case ZSC_EFFECT_POSITION:
                        effect.Position = bh.ReadVector3f() * 0.01f;
                        transFlags = transFlags | ZSC_TRANSITION_TRANSLATE;
                        break;
                    case ZSC_EFFECT_ROTATION:
                        effect.Rotation = bh.ReadQuaternion();
                        transFlags = transFlags | ZSC_TRANSITION_ROTATE;
                        break;
                    case ZSC_EFFECT_SCALE:
                        effect.Scale = bh.ReadVector3f();
                        transFlags = transFlags | ZSC_TRANSITION_SCALE;
                        break;
                    case ZSC_EFFECT_PARENT:
                        effect.ParrentID = (int)bh.ReadWord();
                        break;
                    default:
                        br.ReadBytes(size);
                        break;
                } // switch case
            };

            effect.TransformMatrix = Matrix4.IDENTITY;

            if ((transFlags & ZSC_TRANSITION_ROTATE) != 0)
            {
                effect.TransformMatrix = new Matrix4(effect.Rotation.ToRotationMatrix());
            }

            if ((transFlags & ZSC_TRANSITION_SCALE) != 0)
            {
                effect.TransformMatrix.SetScale(effect.Scale);
            }

            if ((transFlags & ZSC_TRANSITION_TRANSLATE) != 0)
            {
                effect.TransformMatrix.SetTrans(effect.Position);
            }

            return effect;
        } // Read Effect
    }
}
