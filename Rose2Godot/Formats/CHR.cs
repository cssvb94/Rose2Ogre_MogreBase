using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mogre;

namespace RoseFormats
{
    public class CHRAnimation
    {
        public uint ID;
        public uint Type;
    }

    public class CHREffect
    {
        public uint BoneID;
        public uint EffectID;
    }

    public class CHRCharacter
    {
        public bool isActive;
        public uint SkeletonID;
        public string Name;
        public List<uint> Model;
        public List<CHRAnimation> Animation;
        public List<CHREffect> Effect;

        public CHRCharacter()
        {
            Model = new List<uint>();
            Animation = new List<CHRAnimation>();
            Effect = new List<CHREffect>();
            Name = string.Empty;
            SkeletonID = 0;
            isActive = false;
        }
    }

    public class CHR
    {
        public List<string> Skeleton;
        public List<string> Animation;
        public List<string> Effect;

        public List<CHRCharacter> Character;

        private BinaryReader br;
        private BinaryHelper bh;
        private Encoding koreanEncoding = Encoding.GetEncoding("EUC-KR");

        public CHR()
        {
            Skeleton = new List<string>();
            Animation = new List<string>();
            Effect = new List<string>();
            Character = new List<CHRCharacter>();
        }

        public CHR(string FileName)
        {
            Skeleton = new List<string>();
            Animation = new List<string>();
            Effect = new List<string>();
            Character = new List<CHRCharacter>();

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
                    uint skeleton_count = bh.ReadWord();
                    for (int skeltonID = 0; skeltonID < skeleton_count; skeltonID++)
                    {
                        Skeleton.Add(bh.ReadZString());
                    }

                    uint animation_count = bh.ReadWord();
                    for (int animationID = 0; animationID < animation_count; animationID++)
                    {
                        Animation.Add(bh.ReadZString());
                    }

                    uint effect_count = bh.ReadWord();
                    for (int effectID = 0; effectID < effect_count; effectID++)
                    {
                        Effect.Add(bh.ReadZString());
                    }

                    uint char_count = bh.ReadWord();
                    for (int charID = 0; charID < char_count; charID++)
                    {
                        CHRCharacter chr = new CHRCharacter();
                        chr.isActive = (br.ReadByte() != 0);

                        if (chr.isActive)
                        {
                            chr.SkeletonID = bh.ReadWord();
                            chr.Name = bh.ReadZString();
                            
                            for (int modelID = 0; modelID < bh.ReadWord(); modelID++)
                            {
                                chr.Model.Add(bh.ReadWord());
                            }

                            for (int charanimID = 0; charanimID < bh.ReadWord(); charanimID++)
                            {
                                chr.Animation.Add(new CHRAnimation()
                                {
                                   Type = bh.ReadWord(),
                                   ID = bh.ReadWord()
                                });
                            }

                            for (int chareffectID = 0; chareffectID < bh.ReadWord(); chareffectID++)
                            {
                                chr.Effect.Add(new CHREffect()
                                {
                                    BoneID = bh.ReadWord(),
                                    EffectID = bh.ReadWord()
                                });
                            }
                        } // isActive
                        Character.Add(chr);
                    } // for char
                } // tryf
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

    }
}
