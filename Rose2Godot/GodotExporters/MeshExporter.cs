using Mogre;
using RoseFormats;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rose2Godot.GodotExporters
{
    public class MeshExporter
    {
        private readonly StringBuilder smesh;
        private StringBuilder nodes;
        private readonly string name;
        public int LastResourceIndex { get; }
        private bool rootNodeAdded = false;
        public string MeshName { get; set; }

        private Matrix3 rotatePositive90;

        public MeshExporter(string mesh_name, int resource_index, List<ZMS> zms)
        {
            rotatePositive90 = new Quaternion(new Degree(90f), new Vector3(1f, 0f, 0f)).ToRotationMatrix();
            nodes = new StringBuilder();
            smesh = new StringBuilder();
            name = mesh_name;
            LastResourceIndex = resource_index;

            if (zms.Count == 1)
            {
                MeshName = mesh_name;
                LastResourceIndex = resource_index++;
                BuildMeshData(name, zms[0], 0);
                return;
            }

            int i = 1;
            foreach (ZMS mesh in zms)
            {
                string name = string.Format("{0}_{1}", mesh_name.Trim(), i);
                BuildMeshData(name, mesh, i);
                if (mesh == zms[0]) // set 1st mesh name as MeshName
                {
                    MeshName = name;
                }
                i++;
            }
            LastResourceIndex += i;
        }

        private void BuildMeshData(string mesh_data_name, ZMS zms, int idx)
        {
            smesh.AppendFormat("\n[sub_resource id={0} type=\"ArrayMesh\"]\n", idx);
            smesh.AppendFormat("resource_name = \"{0}_mesh_data\"\n", mesh_data_name);
            smesh.AppendLine("surfaces/0 = {\n\t\"primitive\":4,\n\t\"arrays\":[");

            // vertices

            smesh.AppendFormat("\t\t; vertices: {0}\n", zms.Vertex.Count);
            smesh.AppendFormat("\t\t{0},\n", Vector3fToArray(zms.Vertex, null, null));

            // normals

            if (zms.HasNormal())
            {
                smesh.AppendFormat("\t\t; normals: {0}\n", zms.Normal.Count);
                smesh.AppendFormat("\t\t{0},\n", Vector3fToArray(zms.Normal, null, null));
            }
            else
            {
                smesh.AppendLine("\t\tnull, ; no normals");
            }

            // tangents
            // FloatArray()
            if (zms.HasTangents())
            {
                smesh.AppendFormat("\t\t; tangents: {0}\n", zms.Tangent.Count);
                smesh.AppendFormat("\t\t{0},\n", Vector3fToArray(zms.Tangent, null, null));
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
                smesh.AppendFormat("\t\t; UV1: {0}\n", zms.UV[0].Count);
                smesh.AppendFormat("\t\t{0},\n", Vector2fToArray(zms.UV[0]));
            }
            else
            {
                smesh.AppendLine("\t\tnull, ; no UV1");
            }

            // UV2

            if (zms.HasUV1())
            {
                smesh.AppendFormat("\t\t; UV2: {0}\n", zms.UV[1].Count);
                smesh.AppendFormat("\t\t{0},\n", Vector2fToArray(zms.UV[1]));
            }
            else
            {
                smesh.AppendLine("\t\tnull, ; no UV2");
            }

            // Bone indices & Bone weights per vertex

            if (zms.HasBoneIndex() && zms.HasBoneWeight())
            {
                List<int> bindices = new List<int>();
                List<string> bweights = new List<string>();

                var vgroups = zms.BoneWeights.OrderBy(bw => bw.VertexID)
                    .GroupBy(bw => bw.VertexID)
                    .Select(g => new List<BoneWeight>(g.ToList()))
                    .ToList();

                if (vgroups.Count < zms.VertexCount)
                {
                    int groups_to_add = zms.VertexCount - vgroups.Count();
                    for (int i = 0; i < groups_to_add; i++)
                    {
                        vgroups.Add(new List<BoneWeight>());
                    }
                }

                foreach (var vgroup in vgroups)
                {
                    if (vgroup.Count < 4)
                    {
                        // add up to 4 bone per vertex
                        int missing_num = 4 - vgroup.Count;
                        int vertexId = vgroup.FirstOrDefault()?.VertexID ?? 0;

                        for (int m = 0; m < missing_num; m++)
                        {
                            vgroup.Add(new BoneWeight(vertexId, 0, 0f));
                        }
                    }
                }

                foreach (var vgroup in vgroups.ToList())
                {
                    float total_weight = vgroup.Sum(bw => bw.Weight);

                    foreach (BoneWeight bw in vgroup)
                    {
                        bindices.Add(bw.BoneID);
                        // make sure combined weight applied to vertex doesnt exceed 1f
                        bweights.Add(string.Format("{0:0.0000}", bw.Weight / total_weight));
                    }
                }

                string bone_indices = string.Format("\t\tIntArray({0}),", string.Join(", ", bindices.ToArray()));
                string bone_weights = string.Format("\t\tFloatArray({0}),", string.Join(", ", bweights.ToArray()));

                smesh.AppendFormat("\t\t; bone weights: {0}, after proccessing: {1} \n", zms.BoneWeights.Count, vgroups.Count * 4);
                smesh.AppendLine(bone_indices);
                smesh.AppendLine(bone_weights);
            }
            else
            {
                smesh.AppendLine("\t\tnull, ; no bone indices");
                smesh.AppendLine("\t\tnull, ; no bone weights");
            }



            // face indices
            smesh.AppendFormat("\t\t; triangle faces: {0}\n", zms.Face.Count);
            smesh.AppendFormat("\t\t{0}\n", Vector3wToArray(zms.Face));

            smesh.AppendLine("\t],"); // end of mesh arrays
            smesh.AppendLine("\t\"morph_arrays\":[]");
            smesh.AppendLine("}"); // end of surface/0

            if (!rootNodeAdded)
            {
                nodes.AppendFormat("\n[node type=\"Spatial\" name=\"{0}\"]\n", name);
                nodes.AppendLine("transform = Transform(1, 0, 0, 0, -4.37114e-008, 1, 0, -1, -4.37114e-008, 0, 0, 0)");
                rootNodeAdded = true;
            }

            nodes.AppendFormat("\n[node name=\"{0}\" type=\"MeshInstance\" parent=\".\"]\n", mesh_data_name);

            nodes.AppendFormat("mesh = SubResource({0})\n", idx);

            nodes.AppendLine("visible = true");

            if (zms.HasBoneIndex() && zms.HasBoneWeight())
            {
                nodes.AppendLine("skeleton = NodePath(\"../Armature\")");
            }

            nodes.AppendFormat("transform = Transform(1.0000, 0.0000, 0.0000, 0.0000, 1.0000, 0.0000, 0.0000, 0.0000, 1.0000, 0.0000, 0.0000, 0.0000)\n");
        }

        private string BoneIndicesToArray(List<ushort> indices)
        {
            List<string> vs = new List<string>();
            foreach (ushort idx in indices)
            {
                vs.Add(idx.ToString());
            }
            return string.Format("IntArray({0})", string.Join(", ", vs.ToArray()));
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

                vs.Add(string.Format("{0:0.00000}, {1:0.00000}, {2:0.00000}", v.x, v.y, v.z));
            }

            return string.Format("Vector3Array({0})", string.Join(", ", vs.ToArray()));
        }


        private string Vector2fToArray(List<Vector2> vlist)
        {
            List<string> vs = new List<string>();
            foreach (Vector2 v in vlist)
            {
                vs.Add(string.Format("{0:0.00000}, {1:0.00000}", v.x, v.y));
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
            return string.Format("{0}{1}", smesh, nodes);
        }
    }
}
