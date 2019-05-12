using System.Xml;
using Mogre;
using RoseFormats;

namespace OgreRose
{
    class OgreAnimation
    {
        private static float fscale = 0.01f;

        private Matrix4 VertexTransformMatrix = new Matrix4(new Quaternion(new Radian(-1.57079633f), new Vector3(1.0f, 0.0f, 0.0f)));

        private XmlAttribute SetAttr(XmlDocument XMLDoc, string Name, string Value)
        {
            XmlAttribute xattr = XMLDoc.CreateAttribute(Name);
            xattr.Value = Value;
            return xattr;
        }

        public OgreAnimation(ZMD zmd, ZMO zmo, XmlDocument XMLDoc, string AnimationName)
        {
            XmlNode animations;

            // Get/Create <animations> tag
            XmlNodeList animationslist = XMLDoc.GetElementsByTagName("animations");
            if (animationslist.Count == 0)
            {
                animations = XMLDoc.CreateNode(XmlNodeType.Element, "animations", null);
                XMLDoc.DocumentElement.AppendChild(animations);
            }
            else
            {
                // Get the first node
                animations = animationslist[0];
            }

            XmlNode animation = XMLDoc.CreateNode(XmlNodeType.Element, "animation", null);
            animation.Attributes.Append(SetAttr(XMLDoc, "name", AnimationName));
            animation.Attributes.Append(SetAttr(XMLDoc, "length", string.Format("{0:0.000000}", zmo.Length * 1.5f)));

            XmlNode tracks = XMLDoc.CreateNode(XmlNodeType.Element, "tracks", null);

            for (int boneidx = 0; boneidx < zmd.Bone.Count; boneidx++)
            {
                XmlNode track = XMLDoc.CreateNode(XmlNodeType.Element, "track", null);
                RoseBone bone = zmd.Bone[boneidx];

                track.Attributes.Append(SetAttr(XMLDoc, "bone", bone.Name));
                XmlNode keyframes = XMLDoc.CreateNode(XmlNodeType.Element, "keyframes", null);

                for (int frameidx = 0; frameidx < zmo.Frames; frameidx++)
                {
                    XmlNode keyframe = XMLDoc.CreateNode(XmlNodeType.Element, "keyframe", null);
                    keyframe.Attributes.Append(SetAttr(XMLDoc, "time", string.Format("{0:0.000000}", zmo.FrameTime(frameidx) * 1.5f)));

                    XmlNode translate = XMLDoc.CreateNode(XmlNodeType.Element, "translate", null);

                    Vector3 translateVector = bone.Frame[frameidx].Position;
                    if (boneidx == 0)
                    {
                        translateVector = VertexTransformMatrix * translateVector;
                        translateVector *= fscale;
                    }

                    translate.Attributes.Append(SetAttr(XMLDoc, "x", string.Format("{0:0.000000}", translateVector.x)));
                    translate.Attributes.Append(SetAttr(XMLDoc, "y", string.Format("{0:0.000000}", translateVector.y)));
                    translate.Attributes.Append(SetAttr(XMLDoc, "z", string.Format("{0:0.000000}", translateVector.z)));
                    keyframe.AppendChild(translate);

                    // Rotations

                    Quaternion qRot = bone.Rotation.UnitInverse() * bone.Frame[frameidx].Rotation;

                    Radian RotAngle;
                    Vector3 RotAxis;
                    qRot.ToAngleAxis(out RotAngle, out RotAxis);

                    XmlNode rotate = XMLDoc.CreateNode(XmlNodeType.Element, "rotate", null);
                    rotate.Attributes.Append(SetAttr(XMLDoc, "angle", string.Format("{0:0.00000000}", RotAngle.ValueRadians)));

                    XmlNode axis = XMLDoc.CreateNode(XmlNodeType.Element, "axis", null);
                    axis.Attributes.Append(SetAttr(XMLDoc, "x", string.Format("{0:0.000000000}", RotAxis.x)));
                    axis.Attributes.Append(SetAttr(XMLDoc, "y", string.Format("{0:0.000000000}", RotAxis.y)));
                    axis.Attributes.Append(SetAttr(XMLDoc, "z", string.Format("{0:0.000000000}", RotAxis.z)));

                    rotate.AppendChild(axis);
                    keyframe.AppendChild(rotate);

                    XmlNode scale = XMLDoc.CreateNode(XmlNodeType.Element, "scale", null);
                    scale.Attributes.Append(SetAttr(XMLDoc, "x", string.Format("{0:0.000000000}", bone.Frame[frameidx].Scale.x)));
                    scale.Attributes.Append(SetAttr(XMLDoc, "y", string.Format("{0:0.000000000}", bone.Frame[frameidx].Scale.y)));
                    scale.Attributes.Append(SetAttr(XMLDoc, "z", string.Format("{0:0.000000000}", bone.Frame[frameidx].Scale.z)));

                    keyframe.AppendChild(scale);

                    keyframes.AppendChild(keyframe);
                }

                track.AppendChild(keyframes);
                tracks.AppendChild(track);
            }

            animation.AppendChild(tracks);
            animations.AppendChild(animation);
        }

    }
}
