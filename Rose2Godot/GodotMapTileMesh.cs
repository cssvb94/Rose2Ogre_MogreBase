using g4;
using Rose2Godot.GodotExporters;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Rose2Godot
{
    public class GodotMapTileMesh
    {
        public DMesh3 Mesh { get; set; }
        public string Name { get; set; }
        public string LightmapPath { get; set; }
        public string AtlasPath { get; set; }
        public float[] Heights { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public Vector2f[] TilesUV { get; set; }
        public override string ToString()
        {
            StringBuilder scene = new StringBuilder();
            StringBuilder resource = new StringBuilder();
            StringBuilder nodes = new StringBuilder();

            scene.AppendFormat("[gd_scene load_steps={0} format=2]\n", 5);

            // Add texture external resource

            /*
            // res://shaders/lightmap.shader 
            shader_type spatial;

            // we are using a lightmap, we don't need realtime lighting
            render_mode unshaded, cull_disabled;

            // our input textures, a material texture, and the lightmap
            uniform sampler2D texture_albedo : hint_albedo;
            uniform sampler2D texture_lightmap : hint_albedo;

            void fragment() {
	            // lookup the colors at the uv location of our textures
	            vec4 albedo_tex = texture(texture_albedo, UV);
	            vec4 lightmap_tex = texture(texture_lightmap, UV);
                // vec4 lightmap_tex = texture(texture_lightmap,UV2);
  
	            // you can optionally use a multiplier to allow lightening areas (the 2.0 here)
	            ALBEDO = (albedo_tex * lightmap_tex).rgb * 2.0f;
            }
            
            */

            resource.AppendLine();
            resource.AppendLine($"[ext_resource path=\"{Path.Combine("LIGHTMAPS/", LightmapPath)}\" type=\"Texture\" id=1]");
            resource.AppendLine($"[ext_resource path=\"{AtlasPath}\" type=\"Texture\" id=2]");
            resource.AppendLine($"[ext_resource path=\"res://shaders/lightmap.shader\" type=\"Shader\" id=3]");

            resource.AppendFormat("\n[sub_resource id={0} type=\"ArrayMesh\"]\n", 1);
            resource.AppendFormat("resource_name = \"Tile_{0}\"\n", Name);
            resource.AppendLine("surfaces/0 = {\n\t\"primitive\":4,\n\t\"arrays\":[");

            resource.AppendFormat("\t\t; vertices: {0}\n", Mesh.Vertices().Count());
            resource.AppendFormat("\t\t{0},\n", Translator.Vector3ToArray(Mesh.Vertices().ToList(), null));

            // normals

            resource.AppendFormat("\t\t; normals: {0}\n", Mesh.Vertices().Count());

            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvsTop = new List<Vector2>();
            List<Vector2f> uvsTiles = new List<Vector2f>();

            for (int vidx = 0; vidx < Mesh.VertexCount; vidx++)
            {
                var vnormal = Mesh.GetVertexNormal(vidx);
                var uv = Mesh.GetVertexUV(vidx);
                normals.Add(new Vector3(vnormal.x, vnormal.y, vnormal.z));
                uvsTop.Add(new Vector2(uv.x, uv.y));

                //var uv2 = TilesUV[vidx];
                //uvsTiles.Add(uv2);
                uvsTiles.Add(uv);
            }

            resource.AppendFormat("\t\t{0},\n", Translator.Vector3ToArray(normals, null));

            // tangents

            resource.AppendLine("\t\tnull, ; no tangents");

            // vertex colors

            resource.AppendLine("\t\tnull, ; no vertex colors");

            // UV1

            if (uvsTop != null && uvsTop.Any())
            {
                resource.AppendFormat("\t\t; UV1: {0}\n", uvsTop.Count);
                resource.AppendFormat("\t\t{0},\n", Translator.Vector2fToArray(uvsTop));
            }
            else
            {
                resource.AppendLine("\t\tnull, ; no UV1");
            }

            // UV2

            if (uvsTop != null && uvsTop.Any())
            {
                resource.AppendFormat("\t\t; UV2: {0} - same as UV1\n", uvsTiles.Count);
                resource.AppendFormat("\t\t{0},\n", Translator.Vector2fToArray(uvsTiles));
            }
            else
            {
                resource.AppendLine("\t\tnull, ; no UV2");
            }

            // no bones

            resource.AppendLine("\t\tnull, ; no bone indices");
            resource.AppendLine("\t\tnull, ; no bone weights");

            // face indices

            resource.AppendFormat("\t\t; triangle faces: {0}\n", Mesh.Triangles().Count());
            resource.AppendFormat("\t\t{0}\n", Translator.TriangleIndices(Mesh.Triangles().ToList()));

            resource.AppendLine("\t],"); // end of mesh arrays
            resource.AppendLine("\t\"morph_arrays\":[]");
            resource.AppendLine("}"); // end of surface/0
            resource.AppendLine("");

            scene.Append(resource);

            // Shader material
            scene.AppendLine("[sub_resource type=\"ShaderMaterial\" id=2]");
            scene.AppendLine("shader = ExtResource( 3 )");
            scene.AppendLine("shader_param/texture_albedo = ExtResource( 2 )");
            scene.AppendLine("shader_param/texture_lightmap = ExtResource( 1 )");

            // Heighmap collusion shape data
            string pool_array = string.Join(", ", Heights.Select(h => $"{h:0.#####}"));
            scene.AppendLine();
            scene.AppendLine("[sub_resource type=\"HeightMapShape\" id=3]");
            scene.AppendLine("margin = 0.04"); // 0.004 default
            scene.AppendLine("map_width = 65");
            scene.AppendLine("map_depth = 65");
            scene.AppendLine($"map_data = PoolRealArray( {pool_array} )");
            scene.AppendLine();

            scene.AppendLine("; scene root node");
            scene.AppendLine($"[node type=\"KinematicBody\" name=\"Map_{Name}\"]");
            scene.AppendLine("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)");
            scene.AppendLine();

            scene.AppendLine($"[node name=\"Tile_{Name}\" type=\"MeshInstance\" parent=\".\"]");
            scene.AppendLine($"mesh = SubResource( 1 )");
            scene.AppendLine($"material/0 = SubResource( 2 )");
            scene.AppendLine("visible = true");
            scene.AppendLine("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)");
            scene.AppendLine();

            scene.AppendLine($"[node name=\"TileCollisionShape\" type=\"CollisionShape\" parent=\".\"]");
            scene.AppendLine($"shape = SubResource( 3 )");

            float x_offset = 80f + Row * 2.5f * 64f;
            float y_offset = 80f + Col * 2.5f * 64f;
            scene.AppendLine($"transform = Transform( 2.5, 0, 0, 0, 1, 0, 0, 0, 2.5, { x_offset:0.######}, 0, {y_offset:0.######} )");

            return scene.ToString();
        }
    }
}
