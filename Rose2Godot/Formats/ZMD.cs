using Mogre;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RoseFormats
{
    public class ZMD
    {
        public string FormatString;
        public UInt32 BonesCount = 0;
        public UInt32 DummiesCount = 0;

        public bool applyRotation = true;

        public List<RoseBone> Bone = new List<RoseBone>();
        public List<RoseBone> Dummy = new List<RoseBone>();

        private BinaryHelper bh;

        public void Clear()
        {
            BonesCount = 0;
            DummiesCount = 0;
            Bone.Clear();
            Dummy.Clear();
        }

        public ZMD()
        {

        }

        public ZMD(string FileName)
        {
            Load(FileName);
        }

        public bool Load(string FileName)
        {
            Quaternion rotYneg90 = new Quaternion(new Degree(-90f), Vector3.UnitY);
            rotYneg90 = rotYneg90.Inverse();

            Encoding koreanEncoding = Encoding.GetEncoding("EUC-KR");
            try
            {
                FileStream fileStream = File.OpenRead(FileName);
                BinaryReader br = new BinaryReader(fileStream, koreanEncoding);

                bh = new BinaryHelper(br);

                try
                {
                    FormatString = koreanEncoding.GetString(br.ReadBytes(7));

                    BonesCount = br.ReadUInt32();

                    if (BonesCount > 0)
                    {
                        for (int i = 0; i < BonesCount; i++)
                        {
                            RoseBone bone = new RoseBone(br.ReadInt32(), bh.ReadZString(), bh.ReadVector3f() /** scaleFactor*/, bh.ReadQuaternion());
                            bone.ID = i;
                            bone.Position /= 100f;

                            if (i != 0)
                            {
                                Bone[bone.ParentID].ChildID.Add(i);
                            }

                            Bone.Add(bone);
                        } // for
                    } // if num_bones > 0

                    DummiesCount = br.ReadUInt32();

                    if (DummiesCount > 0)
                    {
                        for (int i = 0; i < DummiesCount; i++)
                        {
                            RoseBone dummy = null;

                            if (FormatString.Equals("ZMD0003"))
                            {
                                // dummies are read different then bones;
                                dummy = new RoseBone(bh.ReadZString(), br.ReadInt32(), bh.ReadVector3f() /* * scaleFactor*/, bh.ReadQuaternion());
                            } // if

                            if (FormatString.Equals("ZMD0002"))
                            {
                                dummy = new RoseBone(bh.ReadZString(), br.ReadInt32(), bh.ReadVector3f() /* * scaleFactor*/);
                            } // if 
                            dummy.ID = (int)BonesCount + i;
                            dummy.Position /= 100f;

                            Dummy.Add(dummy);
                        } // for
                    } // if dummies
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
    } // Class
}
