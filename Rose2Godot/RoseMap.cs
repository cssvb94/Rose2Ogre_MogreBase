using g4;
using Godot;
using Pfim;
using Revise.HIM;
using Revise.IFO;
using Revise.IFO.Blocks;
using Revise.TIL;
using Revise.ZON;
using Revise.ZSC;
using Rose2Godot.GodotExporters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Rose2Godot
{

    public enum MapType
    {
        Outdoor = 0,
        Underground = 1,
        GameArena = 2,
    }

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

                var uv2 = TilesUV[vidx];
                uvsTiles.Add(uv2);
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

    public class RoseMap
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetLogger("RoseMap");
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string ZONPath { get; set; }
        public MapType Type { get; set; }
        public string BackgroundMusicMidday { get; set; }
        public string BackgroundMusicNigth { get; set; }
        public string MiniMap { get; set; }
        public uint ZoneMinimapStartX { get; set; }
        public uint ZoneMinimapStartY { get; set; }
        public string ObjectsTable { get; set; }
        public string BuildingsTable { get; set; }
        public uint MapSize { get; set; }
        public string STLId { get; set; }
        public List<List<HeightmapFile>> HIMHeightsGrid { get; private set; }
        public List<List<MapDataFile>> IFODataGrid { get; private set; }
        public List<List<string>> TILDataGrid { get; private set; }
        public int TilesX { get; private set; }
        public int TilesY { get; private set; }
        public string GodotScenePath { get; set; }

        private ModelListFile zsc_objects;
        private ModelListFile zsc_buildings;
        private ZoneFile zon_file;
        private string ObjectsPath;
        private string BuildingsPath;
        private string LightmapsPath;
        private string AtlasPath;
        private Dictionary<int, string> buildings_mesh_files;
        private Dictionary<int, string> objects_mesh_files;
        private Dictionary<int, string> objects_material_files;
        private Dictionary<int, string> buildings_material_files;

        public RoseMap() { }

        private string ExportMaterial(TextureFile texture_file, string export_path)
        {
            StringBuilder material_content = new StringBuilder();

            string dds_texture_path = Translator.FixPath(texture_file.FilePath);

            var parent_info = new DirectoryInfo(dds_texture_path).Parent;
            string parent_folder = parent_info.Name;
            string full_godot_texture_path = Path.Combine(export_path, parent_folder);
            Directory.CreateDirectory(full_godot_texture_path);
            string png_filename = Path.ChangeExtension(Path.GetFileName(dds_texture_path), ".PNG");
            string full_godot_png_path = Path.Combine(full_godot_texture_path, png_filename);

            if (!File.Exists(full_godot_png_path))
            {
                log.Info($"Exporting to \"{full_godot_png_path}\"");
                using (var image = Pfimage.FromFile(dds_texture_path))
                {
                    PixelFormat format;
                    switch (image.Format)
                    {
                        case Pfim.ImageFormat.Rgba32:
                            format = PixelFormat.Format32bppArgb;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                    try
                    {
                        IntPtr data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                        Bitmap bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                        bitmap.Save(full_godot_png_path, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    catch (Exception x)
                    {
                        log.Error(x);
                    }
                    finally
                    {
                        handle.Free();
                    }
                }
            }

            string texture_name = Path.GetFileNameWithoutExtension(dds_texture_path);
            string output_file_name_path = $"{Path.Combine(export_path, texture_name)}.tres";

            material_content.AppendLine("[gd_resource type=\"SpatialMaterial\" load_steps=2 format=2]\n");
            material_content.AppendLine($"[ext_resource path=\"{parent_folder}/{png_filename}\" type=\"Texture\" id=1]\n");
            material_content.AppendLine("[resource]");
            material_content.AppendLine("flags_unshaded = true");
            if (texture_file.TwoSided)
                material_content.AppendLine("params_cull_mode = 2"); // dual sided
            if (texture_file.AlphaEnabled)
                material_content.AppendLine("flags_transparent = true"); // dual sided

            material_content.AppendLine("albedo_texture = ExtResource( 1 )");

            StreamWriter fileStream = new StreamWriter(output_file_name_path);
            fileStream.Write(material_content.ToString());
            fileStream.Close();

            return $"{texture_name}.tres";
        }

        public void GenerateMapData()
        {
            objects_material_files = new Dictionary<int, string>();
            buildings_material_files = new Dictionary<int, string>();

            buildings_mesh_files = new Dictionary<int, string>();
            objects_mesh_files = new Dictionary<int, string>();

            if (!string.IsNullOrEmpty(ObjectsTable))
            {
                zsc_objects = new ModelListFile();
                try
                {
                    zsc_objects.Load(ObjectsTable);
                    ObjectsPath = Path.Combine(GodotScenePath, "OBJECTS");
                    Directory.CreateDirectory(ObjectsPath);

                    for (int tex_idx = 0; tex_idx < zsc_objects.TextureFiles.Count; tex_idx++)
                    {
                        TextureFile mat = zsc_objects.TextureFiles[tex_idx];
                        string exported_mat_file = ExportMaterial(mat, ObjectsPath);
                        objects_material_files.Add(tex_idx, exported_mat_file);
                    }
                    for (int model_idx = 0; model_idx < zsc_objects.ModelFiles.Count; model_idx++)
                    {
                        string zms_file = Translator.FixPath(zsc_objects.ModelFiles[model_idx]);
                        objects_mesh_files.Add(model_idx, zms_file);
                    }
                }
                catch (Exception x)
                {
                    log.Error(x.Message);
                    throw;
                }
            }

            if (!string.IsNullOrEmpty(BuildingsTable))
            {
                zsc_buildings = new ModelListFile();
                try
                {
                    zsc_buildings.Load(BuildingsTable);
                    BuildingsPath = Path.Combine(GodotScenePath, "BUILDINGS");
                    Directory.CreateDirectory(BuildingsPath);

                    for (int tex_idx = 0; tex_idx < zsc_buildings.TextureFiles.Count; tex_idx++)
                    {
                        TextureFile mat = zsc_buildings.TextureFiles[tex_idx];
                        string exported_mat_file = ExportMaterial(mat, BuildingsPath);
                        buildings_material_files.Add(tex_idx, exported_mat_file);
                    }
                    for (int model_idx = 0; model_idx < zsc_buildings.ModelFiles.Count; model_idx++)
                    {
                        string zms_file = Translator.FixPath(zsc_buildings.ModelFiles[model_idx]);
                        buildings_mesh_files.Add(model_idx, zms_file);
                    }
                }
                catch (Exception x)
                {
                    log.Error(x.Message);
                    throw;
                }
            }

            zon_file = new ZoneFile();
            try
            {
                zon_file.Load(ZONPath);
                log.Info($"ZON: \"{ZONPath}\"");
            }
            catch (Exception x)
            {
                log.Error(x);
                throw;
            }

            string zon_folder = Path.GetDirectoryName(ZONPath);
            List<string> HIMFiles = new List<string>(Directory.GetFiles(zon_folder, "*.HIM"));

            if (!HIMFiles.Any())
                return;

            LightmapsPath = Path.Combine(GodotScenePath, "LIGHTMAPS");

            try
            {
                Directory.CreateDirectory(LightmapsPath);
            }
            catch (Exception x)
            {
                log.Error(x);
                throw;
            }

            AtlasPath = Path.Combine(GodotScenePath, "ATLAS");
            try
            {
                Directory.CreateDirectory(AtlasPath);
            }
            catch (Exception x)
            {
                log.Error(x);
                throw;
            }

            List<List<string>> HIMFilesGrid = new List<List<string>>();
            HIMHeightsGrid = new List<List<HeightmapFile>>();
            IFODataGrid = new List<List<MapDataFile>>();
            TILDataGrid = new List<List<string>>();

            for (int map_row_id = 0; map_row_id < HIMFiles.Count; map_row_id++)
            {
                List<string> row_him = HIMFiles.Where(h => h.Contains($"{ZoneMinimapStartX + map_row_id}_")).OrderBy(h => h).Select(hf => Translator.FixPath(hf)).ToList();

                if (!row_him.Any())
                    break;

                HIMFilesGrid.Add(row_him);
            }

            if (!HIMFilesGrid.Any())
                return;

            HIMFilesGrid.Reverse();

            TilesX = HIMFilesGrid[0].Count;
            TilesY = HIMFilesGrid.Count;

            #region Combined height data

            List<HeightmapFile> map_height_row;
            List<MapDataFile> map_data_row;
            List<string> til_data_row;

            List<GodotTransform> transforms = new List<GodotTransform>();

            // y
            for (int map_row_id = HIMFilesGrid.Count - 1; map_row_id >= 0; map_row_id--)
            {
                List<string> map_row = HIMFilesGrid[map_row_id];
                map_height_row = new List<HeightmapFile>();
                map_data_row = new List<MapDataFile>();
                til_data_row = new List<string>();
                // x
                for (int map_col_id = 0; map_col_id < map_row.Count; map_col_id++)
                {
                    HeightmapFile him_file = new HeightmapFile();
                    MapDataFile ifo_file = new MapDataFile();
                    try
                    {
                        him_file.Load(map_row[map_col_id]);
                        ifo_file.Load(Path.ChangeExtension(map_row[map_col_id], ".IFO"));

                        if (ifo_file.Buildings.Any())
                        {
                            for (int building_idx = 0; building_idx < ifo_file.Buildings.Count; building_idx++)
                            {
                                //log.Info($"Exporting Building: {building_idx} Tile: [{map_col_id}, {map_row_id}]");
                                StringBuilder resource = new StringBuilder();
                                StringBuilder node = new StringBuilder();

                                resource.AppendLine("[gd_scene load_steps=2 format=2]\n");

                                MapBuilding building = ifo_file.Buildings[building_idx];

                                //log.Debug($"Building name: {building.Name} WarpID: {building.WarpID} EventID: {building.EventID} ObjectType: {building.ObjectType} ObjectID: {building.ObjectID} MapPosition: {building.MapPosition}");
                                //log.Debug($"Position: {building.Position / 100f} Rotation: {building.Rotation} Scale: {building.Scale}");

                                ModelListObject building_model = zsc_buildings.Objects[building.ObjectID];

                                transforms.Clear();
                                GodotTransform model_transform = GodotTransform.IDENTITY;

                                model_transform = model_transform.Scaled(Translator.Rose2GodotScale(building.Scale));
                                transforms.Add(model_transform);

                                int resource_idx = 1;

                                for (int part_idx = 0; part_idx < building_model.Parts.Count; part_idx++)
                                {
                                    ModelListPart building_part = building_model.Parts[part_idx];

                                    //log.Debug($"\tModelID: {building_part.Model} AxisRotation: {building_part.AxisRotation} Parent: {building_part.Parent} Collision: {building_part.Collision}");
                                    //log.Debug($"\tPosition: {building_part.Position / 100f} Rotation: {building_part.Rotation} Scale: {building_part.Scale}");

                                    /*
                                    tool
                                    extends Node

                                    func _ready():
	                                    for child in get_node(".").get_children():
		                                    child.create_trimesh_collision()
                                    */

                                    string gd_script_collision_gen = string.Empty;
                                    if (building_part.Collision != CollisionType.None)
                                        gd_script_collision_gen = "res://scenes/generate_collision_mesh.gd";

                                    string part = buildings_mesh_files[building_part.Model];
                                    string tex = buildings_material_files[building_part.Texture];
                                    string part_name = Path.GetFileNameWithoutExtension(part);
                                    SceneExporter exporter = new SceneExporter($"{part_name}", part, tex, gd_script_collision_gen);
                                    string part_scene_file = Path.Combine(BuildingsPath, $"{part_name}.tscn");
                                    exporter.ExportScene(part_scene_file, transforms);
                                    resource.AppendLine($"[ext_resource path=\"{part_name}.tscn\" type=\"PackedScene\" id={resource_idx}]\n");
                                    node.AppendLine($"[node name=\"{part_name}_{part_idx:00}\" parent=\".\" instance=ExtResource( {resource_idx} )]");

                                    Vector3 part_scaled_position = building_part.Scale * building_part.Position / 100f;
                                    GodotTransform part_transform = Translator.ToGodotTransform(building_part.Rotation, part_scaled_position); //.Scaled(GodotVector3.NegXScale);

                                    node.AppendLine($"transform = {Translator.GodotTransform2String(part_transform)}");
                                    node.AppendLine();
                                    resource_idx++;
                                }

                                resource.AppendLine($"[node name=\"BUILDING_{building_idx:00}_{map_col_id:00}_{map_row_id:00}\" type=\"Spatial\"]");

                                // ******************
                                Vector3 building_scaled_position = building.Position / 100f;
                                GodotTransform building_transform = Translator.ToGodotTransform(building.Rotation, building_scaled_position).Scaled(GodotVector3.NegXScale);
                                building_transform.basis = building_transform.basis.Scaled(Translator.Rose2GodotScale(building.Scale));
                                // ******************

                                resource.AppendLine($"transform = {Translator.GodotTransform2String(building_transform)}");

                                resource.AppendLine();
                                resource.Append(node);

                                string file_name = Path.Combine(BuildingsPath, $"BUILDING_{building_idx:00}_{map_col_id:00}_{map_row_id:00}.tscn");
                                StreamWriter fileStream = new StreamWriter(file_name);
                                fileStream.Write(resource.ToString());
                                fileStream.Close();

                                //log.Debug($"Exported: \"{file_name}\"");
                                //log.Debug("**********************************************************************************************");
                            }
                        }

                        if (ifo_file.Objects.Any())
                        {
                            for (int object_idx = 0; object_idx < ifo_file.Objects.Count; object_idx++)
                            {
                                StringBuilder resource = new StringBuilder();
                                StringBuilder node = new StringBuilder();

                                resource.AppendLine("[gd_scene load_steps=2 format=2]\n");

                                MapObject obj = ifo_file.Objects[object_idx];
                                var object_parts = zsc_objects.Objects[obj.ObjectID];

                                transforms.Clear();
                                GodotTransform part_mesh_transform = GodotTransform.IDENTITY;
                                part_mesh_transform = part_mesh_transform.Scaled(Translator.Rose2GodotScale(obj.Scale));
                                transforms.Add(part_mesh_transform);

                                int resource_idx = 1;

                                for (int part_idx = 0; part_idx < object_parts.Parts.Count; part_idx++)
                                {
                                    ModelListPart object_part = object_parts.Parts[part_idx];
                                    string part = objects_mesh_files[object_part.Model];
                                    string tex = objects_material_files[object_part.Texture];
                                    string part_name = Path.GetFileNameWithoutExtension(part);

                                    string gd_script_collision_gen = string.Empty;
                                    if (object_part.Collision != CollisionType.None)
                                        gd_script_collision_gen = "res://scenes/generate_collision_mesh.gd";

                                    SceneExporter exporter = new SceneExporter($"{part_name}", part, tex, gd_script_collision_gen);
                                    string part_scene_file = Path.Combine(ObjectsPath, $"{part_name}.tscn");
                                    exporter.ExportScene(part_scene_file, transforms);
                                    resource.AppendLine($"[ext_resource path=\"{part_name}.tscn\" type=\"PackedScene\" id={resource_idx}]\n");
                                    node.AppendLine($"[node name=\"{part_name}_{part_idx:00}\" parent=\".\" instance=ExtResource( {resource_idx} )]");

                                    Vector3 scaled_position = object_part.Scale * object_part.Position / 100f;
                                    GodotTransform part_transform = Translator.ToGodotTransform(object_part.Rotation, scaled_position);
                                    part_transform.basis = part_transform.basis.Scaled(Translator.ToGodotVector3XZY(object_part.Scale));

                                    node.AppendLine($"transform = {Translator.GodotTransform2String(part_transform)}");
                                    node.AppendLine();
                                    resource_idx++;
                                }
                                resource.AppendLine($"[node name=\"OBJECT_{object_idx:00}_{map_col_id:00}_{map_row_id:00}\" type=\"Spatial\"]");

                                Vector3 object_scaled_position = obj.Position / 100f;
                                GodotTransform object_transform = Translator.ToGodotTransform(obj.Rotation, object_scaled_position).Scaled(GodotVector3.NegXScale);
                                object_transform.basis = object_transform.basis.Scaled(Translator.Rose2GodotScale(obj.Scale));

                                resource.AppendLine($"transform = {Translator.GodotTransform2String(object_transform)}");
                                resource.AppendLine();
                                resource.Append(node);

                                string file_name = Path.Combine(ObjectsPath, $"OBJECT_{object_idx:00}_{map_col_id:00}_{map_row_id:00}.tscn");
                                StreamWriter fileStream = new StreamWriter(file_name);
                                fileStream.Write(resource.ToString());
                                fileStream.Close();
                            }
                        }

                        map_height_row.Add(him_file);
                        map_data_row.Add(ifo_file);
                        til_data_row.Add(Path.ChangeExtension(map_row[map_col_id], ".TIL"));
                    }
                    catch (Exception x)
                    {
                        log.Info(x);
                        continue;
                    }
                }
                // add row to the grid
                HIMHeightsGrid.Add(map_height_row);
                IFODataGrid.Add(map_data_row);
                TILDataGrid.Add(til_data_row);
            }
            #endregion

            GenerateTerrainMesh();
        }

        private void AddBitmapTileToLookup(ref Dictionary<int, Bitmap> bitmaps, string path, int index)
        {
            if (!bitmaps.ContainsKey(index))
            {
                using (IImage image = Pfimage.FromFile(path))
                {
                    PixelFormat format;
                    switch (image.Format)
                    {
                        case Pfim.ImageFormat.Rgba32:
                            format = PixelFormat.Format32bppArgb;
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    GCHandle handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);

                    try
                    {
                        var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                        Bitmap bitmap = new Bitmap(new Bitmap(image.Width, image.Height, image.Stride, format, data));
                        bitmaps.Add(index, bitmap);
                    }
                    catch (Exception x)
                    {
                        log.Error(x);
                        throw;
                    }
                    finally
                    {
                        handle.Free();
                    }
                }
            }
        }

        private Bitmap ResizeImage(Bitmap imgToResize, Size size)
        {
            try
            {
                Bitmap bitmap = new Bitmap(size.Width, size.Height);
                using (Graphics graphics = Graphics.FromImage((Image)bitmap))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }
                return bitmap;
            }
            catch
            {
                Console.WriteLine("Bitmap could not be resized");
                return imgToResize;
            }
        }

        public string GenerateTileAtlas(TileFile til)
        {
            int atlas_image_width = til.Width * 256;
            int atlas_image_height = til.Height * 256;

            string atlas_file_name = $"ATLAS_{Path.GetFileNameWithoutExtension(til.FilePath)}.PNG";
            string atlas_full_path = Path.Combine(AtlasPath, atlas_file_name);

            //

            return $"ATLAS/{atlas_file_name}";

            //


            log.Debug($"Generating atlas image texture for \"{Path.GetFileName(til.FilePath)}\" Width: {til.Width} Height: {til.Height} Image: [{atlas_image_width}x{atlas_image_height} px]");

            Font font = new Font("Arial", 24, FontStyle.Regular, GraphicsUnit.Pixel);

            Bitmap tile_atlas_bitmap = new Bitmap(atlas_image_width, atlas_image_height, PixelFormat.Format32bppArgb);

            if (tile_atlas_bitmap is null)
                throw new ArgumentNullException(nameof(tile_atlas_bitmap));

            Graphics atlas = Graphics.FromImage(tile_atlas_bitmap);

            Dictionary<int, Bitmap> bitmaps = new Dictionary<int, Bitmap>();

            RotateFlipType bitmap_rotation = RotateFlipType.RotateNoneFlipNone;

            #region arrow helper

            Bitmap arrow_bitmap = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
            Graphics arrow_graphic = Graphics.FromImage(arrow_bitmap);

            arrow_graphic.DrawLine(new Pen(Brushes.LightBlue, 3f), 1, 1, 30, 1);
            arrow_graphic.DrawLine(new Pen(Brushes.LightBlue, 3f), 30, 0, 20, 10);
            arrow_graphic.DrawLine(new Pen(Brushes.LightBlue, 3f), 20, 10, 20, 1);

            arrow_graphic.DrawLine(new Pen(Brushes.Coral, 3f), 1, 1, 1, 30);

            #endregion

            for (int y = 0; y < til.Height; y++)
            {
                for (int x = 0; x < til.Width; x++)
                {
                    int tileID = til[til.Height - y - 1, x].Tile;

                    TileRotation rotation = zon_file.Tiles[tileID].Rotation;

                    var arrow = (Bitmap)arrow_bitmap.Clone();

                    switch (rotation)
                    {
                        case TileRotation.FlipHorizontal:
                            bitmap_rotation = RotateFlipType.RotateNoneFlipX;
                            break;
                        case TileRotation.FlipVertical:
                            bitmap_rotation = RotateFlipType.RotateNoneFlipY;
                            break;
                        case TileRotation.Flip:
                            bitmap_rotation = RotateFlipType.RotateNoneFlipXY;
                            break;
                        case TileRotation.Clockwise90Degrees:
                            bitmap_rotation = RotateFlipType.Rotate90FlipNone;
                            break;
                        case TileRotation.CounterClockwise90Degrees:
                            bitmap_rotation = RotateFlipType.Rotate270FlipNone;
                            break;
                        case TileRotation.None:
                            bitmap_rotation = RotateFlipType.RotateNoneFlipNone;
                            break;
                        default:
                            break;
                    }

                    ZoneTile tile = zon_file.Tiles[tileID];
                    int image_offset1 = tile.Layer1 + tile.Offset1;
                    int image_offset2 = tile.Layer2 + tile.Offset2;

                    string texPath1 = Translator.FixPath(zon_file.Textures[image_offset1]);
                    string texPath2 = Translator.FixPath(zon_file.Textures[image_offset2]);

                    AddBitmapTileToLookup(ref bitmaps, texPath1, image_offset1);
                    AddBitmapTileToLookup(ref bitmaps, texPath2, image_offset2);

                    // log.Debug($"TileID: {tileID} [{x}, {y}] Rotation: [{rotation}] Offset1: {image_offset1} Offset2: {image_offset2}");
                    // log.Debug($"\tTexture1: \"{texPath1}\"");
                    // log.Debug($"\tTexture2: \"{texPath2}\"\n");

                    int x_bitmap = x * 256;
                    int y_bitmap = y * 256;

                    if (!bitmaps.TryGetValue(image_offset1, out Bitmap bitmap1))
                    {
                        throw new Exception($"Index: {image_offset1} not in dictionary!");
                    }

                    if (bitmap1 is null)
                        throw new ArgumentNullException(nameof(bitmap1));

                    if (!bitmaps.TryGetValue(image_offset2, out Bitmap bitmap2))
                    {
                        throw new Exception($"Index: {image_offset2} not in dictionary!");
                    }

                    if (bitmap2 is null)
                        throw new ArgumentNullException(nameof(bitmap2));

                    try
                    {
                        // NOTE: Only bitmap2 is rotated!
                        //bitmap2.RotateFlip(bitmap_rotation);
                        bitmap1.RotateFlip(bitmap_rotation);
                        atlas.DrawImage(bitmap1, x_bitmap, y_bitmap);
                        atlas.DrawImage(bitmap2, x_bitmap, y_bitmap);
                    }
                    catch (Exception dex)
                    {
                        log.Error(dex);
                        throw;
                    }

                    //atlas.DrawString($"[{x:00} {y:00}]", font, Brushes.FloralWhite, x_bitmap + 5, y_bitmap + 5);

                    arrow.RotateFlip(bitmap_rotation);
                    atlas.DrawImage(arrow, x_bitmap + 5, y_bitmap + 95 + 35 + 35);

                    if (rotation != TileRotation.None)
                    {
                        atlas.DrawString($"{rotation}", font, Brushes.Black, x_bitmap + 6, y_bitmap + 36);
                        atlas.DrawString($"{rotation}", font, Brushes.FloralWhite, x_bitmap + 5, y_bitmap + 35);
                        // atlas.DrawString($"{bitmap_rotation}", font, Brushes.FloralWhite, x_bitmap + 5, y_bitmap + 65);
                    }

                    atlas.DrawString($"L1: {Path.GetFileNameWithoutExtension(texPath1)}", font, Brushes.FloralWhite, x_bitmap + 5, y_bitmap + 95);
                    atlas.DrawString($"L2: {Path.GetFileNameWithoutExtension(texPath2)}", font, Brushes.FloralWhite, x_bitmap + 5, y_bitmap + 95 + 35);
                }
            }

            //log.Debug($"Total bitmaps in dictionary: {bitmaps.Count}");



            try
            {
                //atlas.DrawRectangle(new Pen(Color.BlueViolet, 10), 0, 0, 100, 100);
                tile_atlas_bitmap = ResizeImage(tile_atlas_bitmap, new Size(2048, 2048));

                tile_atlas_bitmap.Save(atlas_full_path, System.Drawing.Imaging.ImageFormat.Png);
                log.Debug($"Saved patch atlas: \"ATLAS/{atlas_file_name}\"");
            }
            catch (Exception a)
            {
                log.Error(a);
                throw;
            }
            return $"ATLAS/{atlas_file_name}";
        }

        private void GenerateTerrainMesh()
        {
            for (int row = 0; row < HIMHeightsGrid.Count; row++)
            {
                List<HeightmapFile> him_row = HIMHeightsGrid[row];
                List<string> til_row = TILDataGrid[row];
                for (int col = 0; col < him_row.Count; col++)
                {
                    var him = him_row[col];
                    string til_file_path = til_row[col];

                    //log.Debug($"Row: {row} Col: {col}");
                    TileFile til = new TileFile();
                    string atlas_filename;
                    try
                    {
                        til.Load(til_file_path);
                        atlas_filename = GenerateTileAtlas(til);
                    }
                    catch (Exception x)
                    {
                        log.Error(x);
                        throw;
                    }

                    GodotMapTileMesh tile_mesh = GenerateTileMesh(him, til, row, col);
                    tile_mesh.AtlasPath = atlas_filename;

                    string lmap_dir = Path.Combine(Path.GetDirectoryName(him.FilePath), Path.GetFileNameWithoutExtension(him.FilePath));
                    string lmap_file_dds = Path.Combine(lmap_dir, $"{Path.GetFileNameWithoutExtension(him.FilePath)}_PLANELIGHTINGMAP.DDS");
                    string png_filename = Path.ChangeExtension(Path.GetFileName(lmap_file_dds), ".PNG");
                    string png_path = Path.Combine(LightmapsPath, png_filename);
                    tile_mesh.LightmapPath = png_filename;

                    if (!Directory.Exists(png_path))
                    {
                        using (var image = Pfimage.FromFile(lmap_file_dds))
                        {
                            PixelFormat format;
                            switch (image.Format)
                            {
                                case Pfim.ImageFormat.Rgba32:
                                    format = PixelFormat.Format32bppArgb;
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                            var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                            try
                            {
                                var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                                var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                                bitmap.Save(png_path, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            finally
                            {
                                handle.Free();
                            }
                        }
                    } // exists?

                    string file_name = Path.Combine(GodotScenePath, Path.GetFileNameWithoutExtension(him.FilePath) + ".tscn");

                    try
                    {
                        StreamWriter sw = new StreamWriter(file_name);
                        sw.WriteLine(tile_mesh.ToString());
                        sw.Close();
                    }
                    catch (Exception x)
                    {
                        log.Error(x);
                        throw;
                    }
                }
            }
        }

        private Vector2f RotationToCoordinates(TileRotation rotation, Vector2f texCoord)
        {
            switch (rotation)
            {
                case TileRotation.FlipHorizontal: // 2
                    texCoord.x = 1.0f - texCoord.x;
                    break;
                case TileRotation.FlipVertical: // 3
                    texCoord.y = 1.0f - texCoord.y;
                    break;
                case TileRotation.Flip: // 4
                    {
                        texCoord.x = 1.0f - texCoord.x;
                        texCoord.y = 1.0f - texCoord.y;
                    }
                    break;
                case TileRotation.Clockwise90Degrees: // 5
                    {
                        float tempX = texCoord.x;
                        texCoord.x = texCoord.y;
                        texCoord.y = 1.0f - tempX;
                    }
                    break;
                case TileRotation.CounterClockwise90Degrees: // 6
                    {
                        float tempX = texCoord.x;
                        texCoord.x = texCoord.y;
                        texCoord.y = tempX;
                    }
                    break;
            }
            return texCoord;
        }

        private string ExportChunk(List<Vector3f> verts, List<Vector3f> normals, List<Vector2f> uvs, List<int> indices, string tex1, string tex2, TileRotation rotation)
        {
            StringBuilder scene = new StringBuilder();

            scene.AppendLine("surfaces/0 = {\n\t\"primitive\":4,\n\t\"arrays\":[");
            // vertices
            scene.AppendFormat("\t\t; vertices: {0}\n", verts.Count);
            scene.AppendFormat("\t\t{0},\n", Translator.Vector3fToArray(verts, null));
            // normals
            scene.AppendFormat("\t\t{0},\n", Translator.Vector3fToArray(normals, null));
            scene.AppendLine("\t\tnull, ; no tangents");
            scene.AppendLine("\t\tnull, ; no vertex colors");
            scene.AppendFormat("\t\t; UV1: {0}\n", uvs.Count);
            scene.AppendFormat("\t\t{0},\n", Translator.Vector2fToArray(uvs));
            scene.AppendLine("\t\tnull, ; no UV2");
            scene.AppendLine("\t\tnull, ; no bone indices");
            scene.AppendLine("\t\tnull, ; no bone weights");
            // face indices
            scene.AppendFormat("\t\t; triangle faces: {0}\n", indices.Count);
            scene.AppendFormat("\t\t{0}\n", Translator.TriangleIndices(indices));

            scene.AppendLine("\t],"); // end of mesh arrays
            scene.AppendLine("\t\"morph_arrays\":[]");
            scene.AppendLine("}"); // end of surface/0
            scene.AppendLine("");
            scene.AppendLine($"; {tex1}");
            scene.AppendLine($"; {tex2}");
            scene.AppendLine($"; {rotation}");
            scene.AppendLine();

            return scene.ToString();
        }

        private string GenerateTilesMap(List<string> tile_scene, string translation)
        {
            StringBuilder scene = new StringBuilder();
            StringBuilder node = new StringBuilder();
            /*
            // res://shaders/tile.shader
            shader_type spatial;
            render_mode unshaded, cull_disabled;

            uniform sampler2D layer1;
            uniform sampler2D layer2;
            uniform int rotation = 0;

            void fragment() {
                vec2 rotated_uv2 = vec2(UV.x, UV.y);
                
                if (rotation == 2) { // Flip Horizontal
                    rotated_uv2 = vec2(1.0 - UV2.x, UV2.y);
                }
                if (rotation == 3) { // Flip Vertical
                    rotated_uv2 = vec2(rotated_uv2.x, 1.0 - rotated_uv2.y);
                }
                if (rotation == 4) { // Flip
                    rotated_uv2 = vec2(1.0 - rotated_uv2.x, 1.0 - rotated_uv2.y);
                }
                if (rotation == 5) { // Clockwise 90
                    rotated_uv2 = vec2(-rotated_uv2.y, rotated_uv2.x)
                }
                if (rotation == 6) { // CounterClockwise 90
                    rotated_uv2 = vec2(rotated_uv2.y, -rotated_uv2.x)
                }
                
                vec4 layer1_tex = texture(layer1, UV);
                vec4 layer2_tex = texture(layer2, rotated_uv2);
                
                ALBEDO = mix(layer1_tex.rgba, layer2_tex.rgba, layer2_tex.a).rgb;
                //ALBEDO = layer1_tex.rgb * layer2_tex.rgb;
            }

            */

            scene.AppendLine("[gd_scene format=2]\n");
            scene.AppendLine($"[ext_resource path=\"res://shaders/tile.shader\" type=\"Shader\" id=1]");

            int resource = 1;
            for (int tile_id = 0; tile_id < tile_scene.Count; tile_id++)
            {
                scene.AppendLine($"\n[sub_resource id={resource} type=\"ArrayMesh\"]");
                scene.AppendLine($"resource_name = \"Tile_{tile_id:00}\"");
                scene.Append(tile_scene[tile_id]);

                node.AppendLine($"[node name=\"TileMesh {tile_id:0000}\" type=\"MeshInstance\" parent=\".\"]");
                node.AppendLine($"mesh = SubResource( {resource} )");
                node.AppendLine($"material/0 = SubResource( {resource + 1} )");
                node.AppendLine("visible = true");
                node.AppendLine("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)");
                node.AppendLine();

                resource++;
                // Shader material
                scene.AppendLine($"[sub_resource type=\"ShaderMaterial\" id={resource}]");
                scene.AppendLine("shader = ExtResource( 1 )");
                // scene.AppendLine("shader_param/texture_albedo = ExtResource( 2 )");
                // scene.AppendLine("shader_param/texture_lightmap = ExtResource( 1 )");

                scene.AppendLine();
                resource++;
            }

            scene.AppendLine("; scene root node");
            scene.AppendLine($"[node type=\"Spatial\" name=\"CHUNK\"]");
            scene.AppendLine(translation);
            scene.AppendLine();

            scene.Append(node);

            return scene.ToString();
        }
        private GodotMapTileMesh GenerateTileMesh(HeightmapFile him_file, TileFile tile_file, int row, int col)
        {
            // from godot zon_importer @ line 201

            List<string> tile_scene = new List<string>();

            int tile_width = (him_file.Width - 1) / tile_file.Width + 1; // = 5
            int tile_height = (him_file.Height - 1) / tile_file.Height + 1; // = 5

            for (int h = 0; h < (him_file.Height - 1); h += (tile_height - 1))
            {
                for (int w = 0; w < (him_file.Height - 1); w += (tile_width - 1))
                {
                    List<Vector3f> tile_vertices = new List<Vector3f>();
                    List<int> tile_indices = new List<int>();
                    List<Vector2f> tile_uv = new List<Vector2f>();
                    List<Vector3f> tile_normals = new List<Vector3f>();

                    // arrays
                    for (int y = 0; y < tile_height; y++)
                    {
                        for (int x = 0; x < tile_width; x++)
                        {
                            Vector3f vert = new Vector3f(w + x, him_file[h + y, w + x] / 300f, h + y);

                            //FIXME: not calculated properly
                            Vector2f uv = new Vector2f(x * 0.2f, y * 0.2f);
                            Vector3f normal = new Vector3f(0, 1f, 0);

                            tile_vertices.Add(vert);
                            tile_uv.Add(uv);
                            tile_normals.Add(normal);
                        }
                    }

                    for (int y = 0; y < tile_height - 1; y++)
                    {
                        for (int x = 0; x < tile_width - 1; x++)
                        {
                            var i = (y * tile_width) + x;
                            tile_indices.Add(i);
                            tile_indices.Add(i + 1);
                            tile_indices.Add(i + tile_width);

                            tile_indices.Add(i + 1);
                            tile_indices.Add(i + tile_width + 1);
                            tile_indices.Add(i + tile_width);
                        }
                    }

                    var tile_x = (int)Math.Floor((double)(w / (tile_width - 1)));
                    var tile_y = (int)Math.Floor((double)(h / (tile_height - 1)));
                    
                    int tile_id = tile_file[tile_y, tile_x].Tile; // ??? reversed x y !!!!!
                    int layer1 = zon_file.Tiles[tile_id].Layer1 + zon_file.Tiles[tile_id].Offset1;
                    int layer2 = zon_file.Tiles[tile_id].Layer2 + zon_file.Tiles[tile_id].Offset2;
                    Tile tile = new Tile(1, tile_id, layer1, layer2);
                    string texture1_path = Translator.FixPath(zon_file.Textures[layer1]);
                    string texture2_path = Translator.FixPath(zon_file.Textures[layer2]);

                    TileRotation rotation = zon_file.Tiles[tile_id].Rotation;

                    tile_scene.Add(ExportChunk(tile_vertices, tile_normals, tile_uv, tile_indices, texture1_path, texture2_path, rotation));

                }
            }

            // Translate each tile mesh chunk
            string chunk_transform = $"transform = Transform( 1, 0, 0, 0, 1, 0, 0, 0, 1, { row * (him_file.Width - 1):0.######}, 0, {col * (him_file.Height - 1):0.######} )";

            string scene = GenerateTilesMap(tile_scene, chunk_transform);

            string chunk_filename = Path.Combine(GodotScenePath, $"CHUNK_{col:00}.{row:00}.tscn");

            try
            {
                StreamWriter sw = new StreamWriter(chunk_filename);
                sw.WriteLine(scene);
                sw.Close();
            }
            catch (Exception x)
            {
                log.Error(x);
                throw;
            }

            // *******************************************
            float x_stride = 2.5f;
            float y_stride = 2.5f;
            float heightScaler = x_stride * 1.2f / 300.0f;
            const float factor = 1f / 64f;

            float x_offset = row * x_stride * (him_file.Width - 1);
            float y_offset = col * y_stride * (him_file.Height - 1);

            float[] heights = new float[him_file.Width * him_file.Height];
            int h_idx = 0;
            for (int y = 0; y < him_file.Height; y++)
            {
                for (int x = 0; x < him_file.Width; x++)
                {
                    heights[h_idx] = him_file[y, x] * heightScaler;
                    h_idx++;
                }
            }

            // UV Tiles

            //  Uv mapping for tiles
            //	x%5 =   0     1	     2	   3     4	 
            //   	 (0,1) (.25,1)(.5,1)(.75,1)(1,1)
            //		 	*-----*-----*-----*-----*
            //			|   / |   / |   / |   / |
            //		    | /   | /   | /   | /   |
            //  (0,.75)	*-----*-----*-----*-----*
            //			|   / |   / |   / |   / |
            //			| /   | /   | /   | /   |
            //	(0,.5)	*-----*-----*-----*-----*
            //			|   / |   / |   / |   / |
            //			| /   | /   | /   | /   |
            //	(0,.25)	*-----*-----*-----*-----*
            //			|   / |   / |   / |   / |
            //			| /   | /   | /   | /   |
            //  (0,0)	*-----*-----*-----*-----*
            //			   (.25,0)(.5,0)(.75,0)(1,0)

            int i_v = 0;

            Vector2f[,] uvMatrix = new Vector2f[5, 5];
            Vector2f[,] uvMatrixLR = new Vector2f[5, 5];
            Vector2f[,] uvMatrixTB = new Vector2f[5, 5];
            Vector2f[,] uvMatrixLRTB = new Vector2f[5, 5];
            Vector2f[,] uvMatrixRotCW = new Vector2f[5, 5];  // rotated 90 deg clockwise
            Vector2f[,] uvMatrixRotCCW = new Vector2f[5, 5];    // rotated 90 counter clockwise

            for (int uv_x = 0; uv_x < 5; uv_x++)
            {
                for (int uv_y = 0; uv_y < 5; uv_y++)
                {
                    uvMatrix[uv_y, uv_x] = new Vector2f(0.25f * uv_x, 1.0f - 0.25f * uv_y);
                    uvMatrixLR[uv_y, uv_x] = new Vector2f(1.0f - 0.25f * uv_x, 1.0f - 0.25f * uv_y);
                    uvMatrixTB[uv_y, uv_x] = new Vector2f(0.25f * uv_x, 0.25f * uv_y);
                    uvMatrixLRTB[uv_y, uv_x] = new Vector2f(1.0f - 0.25f * uv_x, 0.25f * uv_y);
                    uvMatrixRotCCW[uv_x, uv_y] = new Vector2f(0.25f * uv_x, 1.0f - 0.25f * uv_y);
                    uvMatrixRotCW[uv_x, uv_y] = new Vector2f(0.25f * uv_y, 1.0f - 0.25f * uv_x);
                }
            }

            int nVertices = (him_file.Width - 1) * (him_file.Height - 1) * 4;
            Vector2f[] uvsTop = new Vector2f[nVertices];
            Vector2f[] uvsBottom = new Vector2f[nVertices];

            List<Tile> m_tiles = new List<Tile>();

            // Populate tiles with texture references
            for (int t_x = 0; t_x < 16; t_x++)
            {
                for (int t_y = 0; t_y < 16; t_y++)
                {
                    int tile_id = tile_file[t_y, t_x].Tile;
                    int layer1 = zon_file.Tiles[tile_id].Layer1 + zon_file.Tiles[tile_id].Offset1;
                    int layer2 = zon_file.Tiles[tile_id].Layer2 + zon_file.Tiles[tile_id].Offset2;
                    Tile tile = new Tile(1, tile_id, layer1, layer2);
                    string texture1_path = Translator.FixPath(zon_file.Textures[layer1]);
                    string texture2_path = Translator.FixPath(zon_file.Textures[layer2]);
                    tile.BottomTex = texture1_path;
                    tile.TopTex = texture2_path;
                    m_tiles.Add(tile);
                }
            }

            // MESH ************************************************************************************************

            Quaterniond vrot = new Quaterniond(new Vector3d(0, 0, 1), -90);

            DMesh3 tile_mesh = new DMesh3(true, true, true);
            for (int y = 0; y < him_file.Height - 1; y++)
            {
                for (int x = 0; x < him_file.Width - 1; x++)
                {

                    int a = i_v++;
                    int b = i_v++;
                    int c = i_v++;
                    int d = i_v++;

                    int tileX = x / 4;
                    int tileY = y / 4;
                    int tileID = tileY * 16 + tileX;

                    TileRotation rotation = zon_file.Tiles[tile_file[tileX, tileY].Tile].Rotation;
                    Vector2f[,] rotMatrix;
                    switch (rotation)
                    {
                        case TileRotation.None:
                            rotMatrix = uvMatrix;
                            break;
                        case TileRotation.FlipHorizontal:
                            rotMatrix = uvMatrixLR;
                            break;
                        case TileRotation.FlipVertical:
                            rotMatrix = uvMatrixLRTB;
                            break;
                        case TileRotation.Clockwise90Degrees:
                            rotMatrix = uvMatrixRotCW;
                            break;
                        case TileRotation.CounterClockwise90Degrees:
                            rotMatrix = uvMatrixRotCCW;
                            break;
                        case TileRotation.Flip:
                            rotMatrix = uvMatrixTB;
                            break;
                        default:
                            rotMatrix = uvMatrix;
                            break;
                    }

                    //uvsTop[a] = m_tiles[tileID].GetUVTop(rotMatrix[x % 4, y % 4]);
                    //uvsTop[b] = m_tiles[tileID].GetUVTop(rotMatrix[(x % 4 + 1) % 5, y % 4]);
                    //uvsTop[c] = m_tiles[tileID].GetUVTop(rotMatrix[(x % 4 + 1) % 5, (y % 4 + 1) % 5]);
                    //uvsTop[d] = m_tiles[tileID].GetUVTop(rotMatrix[x % 4, (y % 4 + 1) % 5]);

                    //uvsBottom[a] = m_tiles[tileID].GetUVBottom(rotMatrix[x % 4, y % 4]);
                    //uvsBottom[b] = m_tiles[tileID].GetUVBottom(rotMatrix[(x % 4 + 1) % 5, y % 4]);
                    //uvsBottom[c] = m_tiles[tileID].GetUVBottom(rotMatrix[(x % 4 + 1) % 5, (y % 4 + 1) % 5]);
                    //uvsBottom[d] = m_tiles[tileID].GetUVBottom(rotMatrix[x % 4, (y % 4 + 1) % 5]);

                    //    Each quad will be split into two triangles:
                    //
                    //          a         b
                    //            *-----*
                    //            |   / |
                    //            |  /  |
                    //            | /   |
                    //          d *-----* c         
                    //
                    //  The triangles used are: bda and cdb

                    Vector3d vxa = new Vector3d(x, y, -him_file[x, y] * heightScaler);
                    Vector3d vxb = new Vector3d(x + 1, y, -him_file[x + 1, y] * heightScaler);
                    Vector3d vxc = new Vector3d(x + 1, y + 1, -him_file[x + 1, y + 1] * heightScaler);
                    Vector3d vxd = new Vector3d(x, y + 1, -him_file[x, y + 1] * heightScaler);

                    vxa = vrot * vxa;
                    vxb = vrot * vxb;
                    vxc = vrot * vxc;
                    vxd = vrot * vxd;

                    vxa.x = vxa.x * x_stride + x_offset;
                    vxa.y = vxa.y * y_stride - y_offset;

                    vxb.x = vxb.x * x_stride + x_offset;
                    vxb.y = vxb.y * y_stride - y_offset;

                    vxc.x = vxc.x * x_stride + x_offset;
                    vxc.y = vxc.y * y_stride - y_offset;

                    vxd.x = vxd.x * x_stride + x_offset;
                    vxd.y = vxd.y * y_stride - y_offset;

                    int index_va = tile_mesh.AppendVertex(vxa);
                    int index_vb = tile_mesh.AppendVertex(vxb);
                    int index_vc = tile_mesh.AppendVertex(vxc);
                    int index_vd = tile_mesh.AppendVertex(vxd);

                    // Tiles

                    uvsTop[index_va] = m_tiles[tileID].GetUVTop(rotMatrix[x % 4, y % 4]);
                    uvsTop[index_vb] = m_tiles[tileID].GetUVTop(rotMatrix[(x % 4 + 1) % 5, y % 4]);
                    uvsTop[index_vc] = m_tiles[tileID].GetUVTop(rotMatrix[(x % 4 + 1) % 5, (y % 4 + 1) % 5]);
                    uvsTop[index_vd] = m_tiles[tileID].GetUVTop(rotMatrix[x % 4, (y % 4 + 1) % 5]);

                    //

                    // Generate and scale up UVs to hide the seams
                    Vector2f uva = new Vector2f((x + 0.01) * factor, (y + 0.01) * factor);
                    Vector2f uvb = new Vector2f((x + 0.99) * factor, (y + 0.01) * factor);
                    Vector2f uvc = new Vector2f((x + 0.99) * factor, (y + 0.99) * factor);
                    Vector2f uvd = new Vector2f((x + 0.01) * factor, (y + 0.99) * factor);

                    RotateUV(ref uva, 1.5708);
                    RotateUV(ref uvb, 1.5708);
                    RotateUV(ref uvc, 1.5708);
                    RotateUV(ref uvd, 1.5708);

                    tile_mesh.SetVertexUV(index_va, uva);
                    tile_mesh.SetVertexUV(index_vb, uvb);
                    tile_mesh.SetVertexUV(index_vc, uvc);
                    tile_mesh.SetVertexUV(index_vd, uvd);

                    tile_mesh.AppendTriangle(index_vb, index_vd, index_va);
                    tile_mesh.AppendTriangle(index_vc, index_vd, index_vb);
                }
            }

            var mn = new MeshNormals(tile_mesh);
            mn.Compute();
            mn.CopyTo(tile_mesh);

            MeshTransforms.ConvertYUpToZUp(tile_mesh);
            MeshTransforms.FlipLeftRightCoordSystems(tile_mesh);

            return new GodotMapTileMesh()
            {
                Name = $"{Path.GetFileNameWithoutExtension(him_file.FilePath)}",
                Mesh = tile_mesh,
                Heights = heights,
                Row = row,
                Col = col,
                TilesUV = uvsTop,
            };
        }

        private void RotateUV(ref Vector2f uv, double radians)
        {
            float rotMatrix00 = (float)Math.Cos(radians);
            float rotMatrix01 = (float)-Math.Sin(radians);
            float rotMatrix10 = (float)Math.Sin(radians);
            float rotMatrix11 = (float)Math.Cos(radians);

            Vector2f halfVector = new Vector2f(0.5f, 0.5f);

            // Switch coordinates to be relative to center of the plane
            uv -= halfVector;
            // Apply the rotation matrix
            float u = rotMatrix00 * uv.x + rotMatrix01 * uv.y;
            float v = rotMatrix10 * uv.x + rotMatrix11 * uv.y;
            uv.x = 1f - u;
            uv.y = v;
            // Switch back coordinates to be relative to edge
            uv += halfVector;
        }
    }
}
