using System.Xml;
using Mogre;
using RoseFormats;

namespace OgreRose
{
    class OgreSkeleton
    {
        public XmlDocument XMLDoc;

        private static float fscale = 0.01f;

        private Matrix4 RotationTransformMatrix = new Matrix4(new Quaternion(new Radian(-1.57079633f), new Vector3(0.0f, 0.0f, 1.0f)));

        private Matrix4 VertexTransformMatrix = new Matrix4(new Quaternion(new Radian(-1.57079633f), new Vector3(1.0f, 0.0f, 0.0f)));
            
        private XmlAttribute SetAttr(string Name, string Value)
        {
            XmlAttribute xattr = XMLDoc.CreateAttribute(Name);
            xattr.Value = Value;
            return xattr;
        }

        public OgreSkeleton(ZMD zmd)
        {
            XMLDoc = new XmlDocument();

            XmlNode skeleton = XMLDoc.CreateNode(XmlNodeType.Element, "skeleton", null);
            XmlNode bones = XMLDoc.CreateNode(XmlNodeType.Element, "bones", null);
            XmlNode bonehierarchy = XMLDoc.CreateNode(XmlNodeType.Element, "bonehierarchy", null);

            for (int boneIdx = 0; boneIdx < zmd.Bone.Count; boneIdx++)
            {
                XmlNode bone = XMLDoc.CreateNode(XmlNodeType.Element, "bone", null);
                bone.Attributes.Append(SetAttr("id", zmd.Bone[boneIdx].ID.ToString()));
                bone.Attributes.Append(SetAttr("name", zmd.Bone[boneIdx].Name));

                Vector3 pos = zmd.Bone[boneIdx].Position;
                Quaternion rot = zmd.Bone[boneIdx].Rotation;

                if (boneIdx == 0)
                {
                    Matrix4 transformMatrix = new Matrix4(rot);
                    transformMatrix.SetTrans(VertexTransformMatrix * pos);
                    transformMatrix *= RotationTransformMatrix;
                    pos = transformMatrix.GetTrans();
                    rot = transformMatrix.ExtractQuaternion();
                }

                pos *= fscale;

                rot.ToAngleAxis(out Radian Angle, out Vector3 Axis);

                XmlNode position = XMLDoc.CreateNode(XmlNodeType.Element, "position", null);
                position.Attributes.Append(SetAttr("x", string.Format("{0:0.000000000}", pos.x)));
                position.Attributes.Append(SetAttr("y", string.Format("{0:0.000000000}", pos.y)));
                position.Attributes.Append(SetAttr("z", string.Format("{0:0.000000000}", pos.z)));
                bone.AppendChild(position);

                XmlNode rotation = XMLDoc.CreateNode(XmlNodeType.Element, "rotation", null);
                rotation.Attributes.Append(SetAttr("angle", string.Format("{0:0.00000000}", Angle.ValueRadians)));

                XmlNode axis = XMLDoc.CreateNode(XmlNodeType.Element, "axis", null);
                axis.Attributes.Append(SetAttr("x", string.Format("{0:0.000000000}", Axis.x)));
                axis.Attributes.Append(SetAttr("y", string.Format("{0:0.000000000}", Axis.y)));
                axis.Attributes.Append(SetAttr("z", string.Format("{0:0.000000000}", Axis.z)));

                rotation.AppendChild(axis);
                bone.AppendChild(rotation);

                XmlNode scale = XMLDoc.CreateNode(XmlNodeType.Element, "scale", null);
                scale.Attributes.Append(SetAttr("x", "1"));
                scale.Attributes.Append(SetAttr("y", "1"));
                scale.Attributes.Append(SetAttr("z", "1"));

                bone.AppendChild(scale);

                bones.AppendChild(bone);

                if (boneIdx > 0)
                {
                    XmlNode boneparent = XMLDoc.CreateNode(XmlNodeType.Element, "boneparent", null);
                    XmlAttribute boneHName = XMLDoc.CreateAttribute("bone");
                    boneHName.Value = zmd.Bone[boneIdx].Name;
                    XmlAttribute boneHParent = XMLDoc.CreateAttribute("parent");
                    boneHParent.Value = zmd.Bone[zmd.Bone[boneIdx].ParentID].Name;
                    boneparent.Attributes.Append(boneHName);
                    boneparent.Attributes.Append(boneHParent);
                    bonehierarchy.AppendChild(boneparent);
                }

            } // for

            for (int dummyIdx = 0; dummyIdx < zmd.Dummy.Count; dummyIdx++)
            {
                XmlNode bone = XMLDoc.CreateNode(XmlNodeType.Element, "bone", null);
                bone.Attributes.Append(SetAttr("id", zmd.Dummy[dummyIdx].ID.ToString()));

                string dummy_name = string.Format("dummy_{0}", dummyIdx);
                //bone.Attributes.Append(SetAttr("name", zmd.Dummy[dummyIdx].Name));
                bone.Attributes.Append(SetAttr("name", dummy_name));

                Vector3 dummyPos = zmd.Dummy[dummyIdx].Position * fscale;
                Quaternion rot = zmd.Dummy[dummyIdx].Rotation;
                rot.ToAngleAxis(out Radian dummyAngle, out Vector3 dummyAxis);

                XmlNode position = XMLDoc.CreateNode(XmlNodeType.Element, "position", null);
                
                position.Attributes.Append(SetAttr("x", string.Format("{0:0.000000}", dummyPos.x)));
                position.Attributes.Append(SetAttr("y", string.Format("{0:0.000000}", dummyPos.y)));
                position.Attributes.Append(SetAttr("z", string.Format("{0:0.000000}", dummyPos.z)));

                bone.AppendChild(position);

                XmlNode rotation = XMLDoc.CreateNode(XmlNodeType.Element, "rotation", null);

                rotation.Attributes.Append(SetAttr("angle", string.Format("{0:0.00000}", dummyAngle.ValueRadians)));

                XmlNode axis = XMLDoc.CreateNode(XmlNodeType.Element, "axis", null);
                axis.Attributes.Append(SetAttr("x", string.Format("{0:0.000000}", dummyAxis.x)));
                axis.Attributes.Append(SetAttr("y", string.Format("{0:0.000000}", dummyAxis.y)));
                axis.Attributes.Append(SetAttr("z", string.Format("{0:0.000000}", dummyAxis.z)));

                rotation.AppendChild(axis);
                bone.AppendChild(rotation);

                XmlNode scale = XMLDoc.CreateNode(XmlNodeType.Element, "scale", null);
                scale.Attributes.Append(SetAttr("x", "1"));
                scale.Attributes.Append(SetAttr("y", "1"));
                scale.Attributes.Append(SetAttr("z", "1"));

                bone.AppendChild(scale);

                bones.AppendChild(bone);

                XmlNode boneparent = XMLDoc.CreateNode(XmlNodeType.Element, "boneparent", null);
                //boneparent.Attributes.Append(SetAttr("bone", zmd.Dummy[dummyIdx].Name));
                boneparent.Attributes.Append(SetAttr("bone", dummy_name));
                boneparent.Attributes.Append(SetAttr("parent", zmd.Bone[zmd.Dummy[dummyIdx].ParentID].Name));
                bonehierarchy.AppendChild(boneparent);
            } // for

            skeleton.AppendChild(bones);
            skeleton.AppendChild(bonehierarchy);
            XMLDoc.AppendChild(skeleton);
        } // constructor

    } // class
}
