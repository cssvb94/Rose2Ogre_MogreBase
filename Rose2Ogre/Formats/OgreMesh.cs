using System.Xml;
using Mogre;
using RoseFormats;

namespace OgreRose
{
    class OgreMesh
    {
        public XmlDocument XMLDoc;

        private Matrix4 VertexTransformMatrix()
        {
            Matrix4 m = new Matrix4(new Quaternion(new Radian(-1.57079633f), new Vector3(1.0f, 0.0f, 0.0f)));
            return m;
        }

        private XmlAttribute SetAttr(string Name, string Value)
        {
            XmlAttribute xattr = XMLDoc.CreateAttribute(Name);
            xattr.Value = Value;
            return xattr;
        }

        private XmlNode SetVertexNode(Vector3 pos, Vector3 norm)
        {
            XmlNode vertex = XMLDoc.CreateNode(XmlNodeType.Element, "vertex", null);
            XmlNode position = XMLDoc.CreateNode(XmlNodeType.Element, "position", null);
            position.Attributes.Append(SetAttr("x", string.Format("{0:0.000000}", pos.x)));
            position.Attributes.Append(SetAttr("y", string.Format("{0:0.000000}", pos.y)));
            position.Attributes.Append(SetAttr("z", string.Format("{0:0.000000}", pos.z)));

            XmlNode normal = XMLDoc.CreateNode(XmlNodeType.Element, "normal", null);
            normal.Attributes.Append(SetAttr("x", string.Format("{0:0.000000}", norm.x)));
            normal.Attributes.Append(SetAttr("y", string.Format("{0:0.000000}", norm.y)));
            normal.Attributes.Append(SetAttr("z", string.Format("{0:0.000000}", norm.z)));

            vertex.AppendChild(position);
            vertex.AppendChild(normal);

            return vertex;
        }

        private XmlNode SetVertexUVNode(Vector2 uv)
        {
            XmlNode xmluv = XMLDoc.CreateNode(XmlNodeType.Element, "vertex", null);
            XmlNode xmluvcoord = XMLDoc.CreateNode(XmlNodeType.Element, "texcoord", null);
            xmluvcoord.Attributes.Append(SetAttr("u", string.Format("{0:0.000000}", uv.x)));
            xmluvcoord.Attributes.Append(SetAttr("v", string.Format("{0:0.000000}", uv.y)));
            xmluv.AppendChild(xmluvcoord);
            return xmluv;
        }

        private XmlNode SetFaceNode(Vector3w f)
        {
            XmlNode face = XMLDoc.CreateNode(XmlNodeType.Element, "face", null);
            face.Attributes.Append(SetAttr("v1", f.x.ToString()));
            face.Attributes.Append(SetAttr("v2", f.y.ToString()));
            face.Attributes.Append(SetAttr("v3", f.z.ToString()));
            return face;
        }

        public OgreMesh(ZMS zms)
        {
            XMLDoc = new XmlDocument();

            XmlNode mesh = XMLDoc.CreateNode(XmlNodeType.Element, "mesh", null);
            XmlNode submeshes = XMLDoc.CreateNode(XmlNodeType.Element, "submeshes", null);
            XmlNode submesh = XMLDoc.CreateNode(XmlNodeType.Element, "submesh", null);
            submesh.Attributes.Append(SetAttr("material", zms.MaterialName));
            submesh.Attributes.Append(SetAttr("usesharedvertices", "false"));
            submesh.Attributes.Append(SetAttr("use32bitindexes", "false"));
            submesh.Attributes.Append(SetAttr("operationtype", "triangle_list"));

            XmlNode geometry = XMLDoc.CreateNode(XmlNodeType.Element, "geometry", null);
            XmlAttribute vertexcount = SetAttr("vertexcount", zms.Vertex.Count.ToString());
            geometry.Attributes.Append(vertexcount);

            XmlNode vertexbuffer = XMLDoc.CreateNode(XmlNodeType.Element, "vertexbuffer", null);
            XmlAttribute positions = SetAttr("positions", "true");
            XmlAttribute normals = SetAttr("normals", zms.HasNormal().ToString().ToLower());
            vertexbuffer.Attributes.Append(positions);
            vertexbuffer.Attributes.Append(normals);

            for (int vidx = 0; vidx < zms.Vertex.Count; vidx++)
            {
                Vector3 v = new Vector3(zms.Vertex[vidx].x, zms.Vertex[vidx].y, zms.Vertex[vidx].z);
                XmlNode vertex = SetVertexNode(VertexTransformMatrix() * v, zms.Normal[vidx]);
                vertexbuffer.AppendChild(vertex);
            }

            geometry.AppendChild(vertexbuffer);

            XmlNode uvbuffer = XMLDoc.CreateNode(XmlNodeType.Element, "vertexbuffer", null);
            XmlAttribute uvdim = SetAttr("texture_coord_dimensions_0", "2");
            XmlAttribute texture_coords = SetAttr("texture_coords", "1");
            uvbuffer.Attributes.Append(uvdim);
            uvbuffer.Attributes.Append(texture_coords);

            for (int uvidx = 0; uvidx < zms.UV[0].Count; uvidx++)
            {
                XmlNode uv = SetVertexUVNode(zms.UV[0][uvidx]);
                uvbuffer.AppendChild(uv);
            }

            geometry.AppendChild(uvbuffer);

            submesh.AppendChild(geometry);

            XmlNode faces = XMLDoc.CreateNode(XmlNodeType.Element, "faces", null);
            XmlAttribute fcount = SetAttr("count", zms.Face.Count.ToString());
            faces.Attributes.Append(fcount);

            for (int fidx = 0; fidx < zms.Face.Count; fidx++)
            {
                XmlNode face = SetFaceNode(zms.Face[fidx]);
                faces.AppendChild(face);
            }

            submesh.AppendChild(faces);

            if (zms.BonesCount > 0)
            {
                XmlNode boneassignments = XMLDoc.CreateNode(XmlNodeType.Element, "boneassignments", null);

                for (int bwi = 0; bwi < zms.BoneWeights.Count; bwi++)
                {
                    XmlNode vertexboneassignment = XMLDoc.CreateNode(XmlNodeType.Element, "vertexboneassignment", null);
                    vertexboneassignment.Attributes.Append(SetAttr("vertexindex", zms.BoneWeights[bwi].VertexID.ToString()));
                    vertexboneassignment.Attributes.Append(SetAttr("boneindex", zms.BoneWeights[bwi].BoneID.ToString()));
                    vertexboneassignment.Attributes.Append(SetAttr("weight", string.Format("{0:0.000000}", zms.BoneWeights[bwi].Weight)));

                    boneassignments.AppendChild(vertexboneassignment);
                }
                submesh.AppendChild(boneassignments);
            }
            submeshes.AppendChild(submesh);
            mesh.AppendChild(submeshes);
            XMLDoc.AppendChild(mesh);
        }
    }
}
