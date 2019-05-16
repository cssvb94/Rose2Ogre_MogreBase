using Mogre;
using RoseFormats;
using System.Collections.Generic;
using System.Text;

namespace Rose2Godot.GodotExporters
{
    public class MeshExporter
    {
        private readonly StringBuilder smesh;
        private readonly string name;
        public int LastResourceIndex { get; }
        private bool rootNodeAdded = false;

        private Matrix3 rotatePositive90;

        public MeshExporter(string mesh_name, int resource_index, List<ZMS> zms)
        {
            rotatePositive90 = new Quaternion(new Degree(90f), new Vector3(1f, 0f, 0f)).ToRotationMatrix();

            smesh = new StringBuilder();
            name = mesh_name;
            LastResourceIndex = resource_index;

            int i = 0;
            foreach (ZMS mesh in zms)
            {
                //BuildMeshData(string.Format("{0}_{1}", name, i++), mesh, i);
                BuildMeshData(string.Format("{0}", name), mesh, i);
                i++;
            }
            LastResourceIndex += i;
        }

        private void BuildMeshData(string name, ZMS zms, int idx)
        {
            smesh.AppendFormat("[sub_resource id={0} type=\"ArrayMesh\"]\n\n", idx);
            smesh.AppendFormat("resource_name = \"{0}\"\n", name);
            smesh.AppendLine("surfaces/0 = {\n\t\"primitive\":4,\n\t\"arrays\":[");

            // vertices

            smesh.AppendFormat("\t\t{0},\n", Vector3fToArray(zms.Vertex, rotatePositive90, null));

            // normals

            if (zms.HasNormal())
            {
                smesh.AppendFormat("\t\t{0},\n", Vector3fToArray(zms.Normal, rotatePositive90, null));
            }
            else
            {
                smesh.AppendLine("\t\tnull, ; no normals");
            }

            // tangents
            // FloatArray()
            if (zms.HasTangents())
            {
                smesh.AppendFormat("\t\t{0},\n", Vector3fToArray(zms.Tangent, rotatePositive90, null));
            }
            else
            {
                smesh.AppendLine("\t\tnull, ; no tangents");
            }

            // vertex colors

            // FORMAT: ColorArray(r, g, b, alpha)

            smesh.AppendLine("\t\tnull, ; no vertex colors");

            // UV1

            if (zms.HasUV0())
            {
                smesh.AppendFormat("\t\t{0},\n", Vector2fToArray(zms.UV[0]));
            }
            else
            {
                smesh.AppendLine("\t\tnull, ; no UV1");
            }

            // UV2

            if (zms.HasUV1())
            {
                smesh.AppendFormat("\t\t{0},\n", Vector2fToArray(zms.UV[1]));
            }
            else
            {
                smesh.AppendLine("\t\tnull, ; no UV2");
            }

            // Bone indices
            smesh.AppendLine("\t\tnull, ; no bone indices");

            // Bone weights
            // FloatArray()

            smesh.AppendLine("\t\tnull, ; no bone weights");

            // face indices

            smesh.AppendFormat("\t\t{0}\n", Vector3wToArray(zms.Face));

            smesh.AppendLine("\t],"); // end of mesh arrays
            smesh.AppendLine("\t\"morph_arrays\":[]");
            smesh.AppendLine("}\n"); // end of surface/0

            if (!rootNodeAdded)
            {
                smesh.AppendLine("[node type=\"Spatial\" name=\"Scene\"]\n");
                rootNodeAdded = true;
            }

            smesh.AppendFormat("[node name=\"{0}\" type=\"MeshInstance\" parent=\".\"]\n", name);

            smesh.AppendFormat("mesh = SubResource({0})\n", idx);

            smesh.AppendLine("visible = true");

            smesh.AppendLine("transform = Transform(1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0)");
            //smesh.AppendLine("transform = Transform( 1, 0, 0, 0, -4.37114e-008, 1, 0, -1, -4.37114e-008, 0, 0, 0 )");
        }

        private string Vector3fToArray(List<Vector3> vlist, Matrix3? transform, float? scale)
        {
            List<string> vs = new List<string>();
            foreach (Vector3 vertex in vlist)
            {
                Vector3 v = vertex;
                if (scale.HasValue)
                {
                    v *= scale.Value;
                }

                if (transform.HasValue)
                {
                    v *= transform.Value;
                }

                vs.Add(string.Format("{0:0.0000}, {1:0.0000}, {2:0.0000}", v.x, v.y, v.z));
            }

            return string.Format("Vector3Array({0})", string.Join(", ", vs.ToArray()));
        }


        private string Vector2fToArray(List<Vector2> vlist)
        {
            List<string> vs = new List<string>();
            foreach (Vector2 v in vlist)
            {
                vs.Add(string.Format("{0:0.0000}, {1:0.0000}", v.x, v.y));
            }

            return string.Format("Vector2Array({0})", string.Join(", ", vs.ToArray()));
        }

        private string Vector3wToArray(List<Vector3w> vlist)
        {
            List<string> vs = new List<string>();
            foreach (Vector3w v in vlist)
            {
                vs.Add(string.Format("{0}, {1}, {2}", v.x, v.z, v.y));
            }

            return string.Format("IntArray({0})", string.Join(", ", vs.ToArray()));
        }

        public override string ToString()
        {
            return smesh.ToString();
        }
    }
}
