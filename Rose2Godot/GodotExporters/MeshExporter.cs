using Godot;
using Revise;
using Revise.Types;
using Revise.ZMS;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Rose2Godot.GodotExporters
{
    public class MeshExporter
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetLogger("MeshExporter");
        private readonly StringBuilder resource;
        private readonly StringBuilder nodes;
        private readonly string name;
        private readonly List<GodotTransform> transforms;
        private readonly int ExternalMaterialId;

        public int LastResourceIndex { get; private set; }
        public string MeshName { get; set; }

        public string Resources => resource.ToString();

        public string Nodes => nodes.ToString();

        private string GodotVector3fToArray(IEnumerable<GodotVector3> vlist)
        {
            List<string> vs = new List<string>();
            foreach (GodotVector3 vertex in vlist)
                vs.Add($"{vertex.x:0.####}, {vertex.y:0.####}, {vertex.z:0.####}");
            return $"Vector3Array({string.Join(", ", vs.ToArray())})";
        }

        private string VerticesColorsToArray(IEnumerable<Color4> color_list)
        {
            List<string> vcolors = new List<string>();
            foreach (Color4 color in color_list)
                vcolors.Add($"{color.Red:0.####}, {color.Green:0.####}, {color.Blue:0.####}, {color.Alpha:0.####}");
            return $"ColorArray({string.Join(", ", vcolors.ToArray())})";
        }

        private string Vector2fToArray(IEnumerable<Vector2> vlist)
        {
            List<string> vs = new List<string>();
            foreach (Vector2 v in vlist)
                vs.Add($"{v.X:0.####}, {v.Y:0.####}");

            return $"Vector2Array({string.Join(", ", vs.ToArray())})";
        }

        private string ShortVector3ToArray(IEnumerable<ShortVector3> vlist)
        {
            List<string> vs = new List<string>();
            foreach (ShortVector3 v in vlist)
                vs.Add($"{v.X}, {v.Z}, {v.Y}");

            return $"IntArray({string.Join(", ", vs.ToArray())})";
        }

        public MeshExporter(int resource_index, List<ModelFile> zms, List<string> mesh_names, bool exportWithBones, List<GodotTransform> transforms = null, int ExternalMaterialId = -1)
        {
            nodes = new StringBuilder();
            resource = new StringBuilder();
            name = mesh_names.First();
            LastResourceIndex = resource_index;
            this.transforms = transforms;
            this.ExternalMaterialId = ExternalMaterialId;

            if (zms.Count == 1)
            {
                MeshName = mesh_names[0];
                BuildMeshData(name, zms[0], LastResourceIndex, exportWithBones, 0);
                LastResourceIndex++;
                return;
            }

            int i = 1;
            foreach (ModelFile mesh in zms)
            {
                string mesh_name = $"{mesh_names[i - 1]}";
                BuildMeshData(mesh_name, mesh, LastResourceIndex + i - 1, exportWithBones, i - 1);
                if (mesh == zms[0]) // set 1st mesh name as MeshName
                    MeshName = mesh_name;
                i++;
            }
            LastResourceIndex = LastResourceIndex + i - 1;
        }

        private void BuildMeshData(string mesh_data_name, ModelFile zms, int idx, bool exportWithBones, int transform_idx)
        {
            resource.AppendFormat("\n[sub_resource id={0} type=\"ArrayMesh\"]\n", idx);
            resource.AppendFormat($"resource_name = \"{mesh_data_name}\"\n");

            resource.AppendLine("surfaces/0 = {\n\t\"primitive\":4,");

            if (ExternalMaterialId > 0)
                resource.AppendLine($"\t\"material\": ExtResource( {ExternalMaterialId} ),");

            resource.AppendLine("\t\"arrays\":[");
            // vertices

            resource.AppendFormat("\t\t; vertices: {0}\n", zms.Vertices.Count);
            resource.AppendFormat("\t\t{0},\n", GodotVector3fToArray(zms.Vertices.Select(v => Translator.ToGodotVector3XZY(v.Position))));

            // normals

            if (zms.NormalsEnabled)
            {
                resource.AppendFormat("\t\t; normals: {0}\n", zms.Vertices.Count);
                resource.AppendFormat("\t\t{0},\n", GodotVector3fToArray(zms.Vertices.Select(v => Translator.ToGodotVector3XYZ(v.Normal))));
            }
            else
                resource.AppendLine("\t\tnull, ; no normals");

            // tangents
            // FloatArray()
            if (zms.TangentsEnabled)
            {
                resource.AppendFormat("\t\t; tangents: {0}\n", zms.Vertices.Count);
                resource.AppendFormat("\t\t{0},\n", GodotVector3fToArray(zms.Vertices.Select(v => Translator.ToGodotVector3XZY(v.Tangent))));
            }
            else
                resource.AppendLine("\t\tnull, ; no tangents");

            // vertex colors
            // FORMAT: ColorArray(r, g, b, alpha)

            if (zms.ColoursEnabled)
            {
                resource.AppendFormat("\t\t; vertex colors: {0}\n", zms.Vertices.Count);
                resource.AppendFormat("\t\t{0},\n", VerticesColorsToArray(zms.Vertices.Select(v => v.Colour)));
            }
            else
                resource.AppendLine("\t\tnull, ; no vertex colors");

            // UV1

            if (zms.TextureCoordinates1Enabled)
            {
                resource.AppendFormat("\t\t; UV1: {0}\n", zms.Vertices.Count);
                resource.AppendFormat("\t\t{0},\n", Vector2fToArray(zms.Vertices.Select(v => v.TextureCoordinates[0])));
            }
            else
                resource.AppendLine("\t\tnull, ; no UV1");

            // UV2

            if (zms.TextureCoordinates2Enabled)
            {
                resource.AppendFormat("\t\t; UV2: {0}\n", zms.Vertices.Count);
                resource.AppendFormat("\t\t{0},\n", Vector2fToArray(zms.Vertices.Select(v => v.TextureCoordinates[1])));
            }
            else
                resource.AppendLine("\t\tnull, ; no UV2");

            // Bone indices & Bone weights per vertex

            if (zms.BonesEnabled && exportWithBones)
            {
                List<int> bone_indices_list = new List<int>();
                List<float> bone_weights_list = new List<float>();

                foreach (var vertex in zms.Vertices)
                {
                    bone_indices_list.AddRange(new List<int>() { zms.BoneTable[vertex.BoneIndices.X], zms.BoneTable[vertex.BoneIndices.Y], zms.BoneTable[vertex.BoneIndices.Z], zms.BoneTable[vertex.BoneIndices.W] });
                    bone_weights_list.AddRange(new List<float>() { vertex.BoneWeights.X, vertex.BoneWeights.Y, vertex.BoneWeights.Z, vertex.BoneWeights.W });
                }

                string bone_indices = $"\t\tIntArray({string.Join(", ", bone_indices_list.ToArray())}),";
                string bone_weights = $"\t\tFloatArray({string.Join(", ", bone_weights_list.Select(w => $"{w:0.######}").ToArray())}),";

                resource.AppendFormat("\t\t; bone weights: {0} \n", bone_weights_list.Count);
                resource.AppendLine(bone_indices);
                resource.AppendLine(bone_weights);
            }
            else
            {
                resource.AppendLine("\t\tnull, ; no bone indices");
                resource.AppendLine("\t\tnull, ; no bone weights");
            }

            // face indices
            resource.AppendFormat("\t\t; triangle faces: {0}\n", zms.Indices.Count);
            resource.AppendFormat("\t\t{0}\n", ShortVector3ToArray(zms.Indices.Select(vidx => Translator.Rose2GodotTriangleIndices(vidx))));

            resource.AppendLine("\t],"); // end of mesh arrays
            resource.AppendLine("\t\"morph_arrays\":[]");
            resource.AppendLine("}"); // end of surface/0

            if (exportWithBones)
                nodes.AppendFormat("\n[node name=\"{0}\" type=\"MeshInstance\" parent=\"Skeleton\"]\n", mesh_data_name);
            else
                nodes.AppendFormat("\n[node name=\"{0}\" type=\"MeshInstance\" parent=\".\"]\n", mesh_data_name);

            nodes.AppendFormat("mesh = SubResource({0})\n", idx);

            nodes.AppendLine("visible = true");

            if (zms.BonesEnabled && exportWithBones)
                nodes.AppendLine("skeleton = NodePath(\"..:Armature\")");

            if (transforms != null && transforms.Any())
            {
                nodes.AppendLine($"transform = {Translator.GodotTransform2String(transforms[transform_idx])}");
            }
            else
            {
                nodes.AppendFormat("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)\n");
            }
        }

        public override string ToString() => $"{resource}{nodes}";
    }
}
