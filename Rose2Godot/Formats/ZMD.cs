using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mogre;

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
                            bone.TransformMatrix = Matrix4.Identity;
                            bone.TransformMatrix.MakeTransform(bone.Position, Vector3.One, bone.Rotation);
                            bone.InverseMatrix = bone.TransformMatrix.Inverse();

                            Console.WriteLine(bone);
                            Console.WriteLine(bone.Rotation.ToRotationMatrix());
                            Console.WriteLine(bone.TransformMatrix);

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
                            dummy.TransformMatrix = Matrix4.Identity;
                            dummy.TransformMatrix.MakeTransform(dummy.Position, Vector3.One, dummy.Rotation);
                            dummy.InverseMatrix = dummy.TransformMatrix.Inverse();

                            //if (dummy.ParentID < BonesCount)
                            //{
                            //    Bone[dummy.ParentID].ChildID.Add(dummy.ID);
                            //} else
                            //{
                            //    Dummy[dummy.ParentID - (int)BonesCount].ChildID.Add(dummy.ID);
                            //}

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

            //TransformChildren(0);

            TransformBones(Bone[0]);

            return true;
        } // Load

        private void TransformBones(RoseBone bone)
        {
            bone.TransformMatrix.MakeTransform(bone.Position, Vector3.One, bone.Rotation);

            if (bone.ID != 0)
            {
                RoseBone parent = Bone[bone.ParentID];
                Matrix4 parentInvTransformMatrix = Matrix4.Identity;
                parentInvTransformMatrix.MakeInverseTransform(parent.Position, Vector3.One, parent.Rotation);
                bone.TransformMatrix = parentInvTransformMatrix * bone.TransformMatrix;
            }

            foreach (int childID in bone.ChildID)
            {
                TransformBones(Bone[childID]);
            }
        }

        //private void TransformChildren(int ParentID)
        //{
        //    for (int i = 0; i < Bone.Count; i++)
        //    {
        //        if (i == ParentID) continue;
        //        if (Bone[i].ParentID != ParentID) continue;
        //        Bone[i].TransformMatrix *= Bone[Bone[i].ParentID].TransformMatrix;
        //        // TODO: Dummies?
        //        TransformChildren(i);
        //    }
        //}
    } // Class
}
