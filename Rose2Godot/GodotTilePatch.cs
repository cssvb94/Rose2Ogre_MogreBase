using g4;
using Revise.ZON;
using Rose2Godot.GodotExporters;
using System.Collections.Generic;
using System.Text;

namespace Rose2Godot
{
    public class GodotTilePatch
    {
        public int layer1 { get; set; }
        public int layer2 { get; set; }
        public List<Vector3f> vertices { get; set; }
        public List<int> indices { get; set; }
        public List<Vector2f> uvs { get; set; }
        public List<Vector2f> lightmap_uvs { get; set; }
        public List<Vector3f> normals { get; set; }
        public TileRotation rotation { get; set; }

        public override string ToString()
        {
            StringBuilder scene_fragment = new StringBuilder();

            scene_fragment.AppendLine("surfaces/0 = {\n\t\"primitive\":4,\n\t\"arrays\":[");
            // vertices
            scene_fragment.AppendFormat("\t\t; vertices: {0}\n", vertices.Count);
            scene_fragment.AppendFormat("\t\t{0},\n", Translator.Vector3fToArray(vertices, null));
            // normals
            scene_fragment.AppendFormat("\t\t; normals: {0}\n", normals.Count);
            scene_fragment.AppendFormat("\t\t{0},\n", Translator.Vector3fToArray(normals, null));
            scene_fragment.AppendLine("\t\tnull, ; no tangents");
            scene_fragment.AppendLine("\t\tnull, ; no vertex colors");
            scene_fragment.AppendFormat("\t\t; UV1: {0}\n", uvs.Count);
            scene_fragment.AppendFormat("\t\t{0},\n", Translator.Vector2fToArray(uvs));

            scene_fragment.AppendFormat("\t\t; UV2: {0}\n", lightmap_uvs.Count);
            scene_fragment.AppendFormat("\t\t{0},\n", Translator.Vector2fToArray(lightmap_uvs));

            //scene_fragment.AppendLine("\t\tnull, ; no UV2");
            scene_fragment.AppendLine("\t\tnull, ; no bone indices");
            scene_fragment.AppendLine("\t\tnull, ; no bone weights");
            // face indices
            scene_fragment.AppendFormat("\t\t; triangle faces: {0}\n", indices.Count);
            scene_fragment.AppendFormat("\t\t{0}\n", Translator.TriangleIndices(indices));

            scene_fragment.AppendLine("\t],"); // end of mesh arrays
            scene_fragment.AppendLine("\t\"morph_arrays\":[]");
            scene_fragment.AppendLine("}"); // end of surface/0
            scene_fragment.AppendLine();

            return scene_fragment.ToString();
        }
    }
}