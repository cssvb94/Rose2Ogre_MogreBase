using g4;
using Revise.ZON;
using System.Collections.Generic;
using System.Text;

namespace Rose2Godot.GodotExporters
{
    public class GodotTilePatch
    {
        public int Layer1 { get; set; }
        public int Layer2 { get; set; }
        public List<Vector3f> Vertices { get; set; }
        public List<int> Indices { get; set; }
        public List<Vector2f> UVs { get; set; }
        public List<Vector2f> LightmapUVs { get; set; }
        public List<Vector3f> Normals { get; set; }
        public TileRotation Rotation { get; set; }
        public int SurfaceID { get; set; }
        public int MaterialID { get; set; }
        public GodotTilePatch() : this(1) { }
        public GodotTilePatch(int surface_id) => SurfaceID = surface_id;

        public override string ToString()
        {
            StringBuilder scene_fragment = new StringBuilder();

            scene_fragment.AppendLine($"surfaces/{SurfaceID} = {{\n\t\"primitive\":4,\n\t\"arrays\":[");
            // vertices
            scene_fragment.AppendFormat("\t\t; vertices: {0}\n", Vertices.Count);
            scene_fragment.AppendFormat("\t\t{0},\n", Translator.Vector3fToArray(Vertices, null));
            // normals
            scene_fragment.AppendFormat("\t\t; normals: {0}\n", Normals.Count);
            scene_fragment.AppendFormat("\t\t{0},\n", Translator.Vector3fToArray(Normals, null));
            scene_fragment.AppendLine("\t\tnull, ; no tangents");
            scene_fragment.AppendLine("\t\tnull, ; no vertex colors");

            // tile UV
            scene_fragment.AppendFormat("\t\t; UV1: {0}\n", UVs.Count);
            scene_fragment.AppendFormat("\t\t{0},\n", Translator.Vector2fToArray(UVs));
            // lightmap UV
            scene_fragment.AppendFormat("\t\t; UV2: {0}\n", LightmapUVs.Count);
            scene_fragment.AppendFormat("\t\t{0},\n", Translator.Vector2fToArray(LightmapUVs));
            // terrain mesh has no bone data
            scene_fragment.AppendLine("\t\tnull, ; no bone indices");
            scene_fragment.AppendLine("\t\tnull, ; no bone weights");
            // face indices
            scene_fragment.AppendFormat("\t\t; triangle faces: {0}\n", Indices.Count);
            scene_fragment.AppendFormat("\t\t{0}\n", Translator.TriangleIndices(Indices));

            scene_fragment.AppendLine("\t],"); // end of mesh arrays
            // material
            scene_fragment.AppendLine($"\t\"material\": SubResource( {MaterialID} ),");
            scene_fragment.AppendLine("\t\"morph_arrays\": [ ]");
            scene_fragment.AppendLine("}"); // end of surface/0
            scene_fragment.AppendLine();

            return scene_fragment.ToString();
        }
    }
}