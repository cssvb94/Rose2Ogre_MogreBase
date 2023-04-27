using g4;
using Godot;
//using Pfim;
using Revise.HIM;
using Revise.IFO;
using Revise.IFO.Blocks;
using Revise.TIL;
using Revise.ZON;
using Revise.ZSC;
using Rose2Godot.GodotExporters;
using S16.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Rose2Godot
{
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
        public string GodotProjectPath { get; set; }
        public string GodotScenePath => Path.Combine(GodotProjectPath, LongName);
        public string GodotTilesPath => Path.Combine(GodotProjectPath, "TERRAIN/TILES");

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

                //FileStream stream = new FileStream(dds_texture_path, FileMode.Open);
                //try
                //{
                //    DDSImage dds = new DDSImage(stream);
                //    dds.BitmapImage.Save(full_godot_png_path, ImageFormat.Png);
                //}
                //finally
                //{
                //    stream.Close();
                //}

                using (var image = Pfim.Pfimage.FromFile(dds_texture_path))
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
                        bitmap.Save(full_godot_png_path, ImageFormat.Png);
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

                                    System.Numerics.Vector3 part_scaled_position = building_part.Scale * building_part.Position / 100f;
                                    GodotTransform part_transform = Translator.ToGodotTransform(building_part.Rotation, part_scaled_position); //.Scaled(GodotVector3.NegXScale);

                                    node.AppendLine($"transform = {Translator.GodotTransform2String(part_transform)}");
                                    node.AppendLine();
                                    resource_idx++;
                                }

                                resource.AppendLine($"[node name=\"BUILDING_{building_idx:00}_{map_col_id:00}_{map_row_id:00}\" type=\"Spatial\"]");

                                // ******************
                                System.Numerics.Vector3 building_scaled_position = building.Position / 100f;
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

                                    System.Numerics.Vector3 scaled_position = object_part.Scale * object_part.Position / 100f;
                                    GodotTransform part_transform = Translator.ToGodotTransform(object_part.Rotation, scaled_position);
                                    part_transform.basis = part_transform.basis.Scaled(Translator.ToGodotVector3XZY(object_part.Scale));

                                    node.AppendLine($"transform = {Translator.GodotTransform2String(part_transform)}");
                                    node.AppendLine();
                                    resource_idx++;
                                }
                                resource.AppendLine($"[node name=\"OBJECT_{object_idx:00}_{map_col_id:00}_{map_row_id:00}\" type=\"Spatial\"]");

                                System.Numerics.Vector3 object_scaled_position = obj.Position / 100f;
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
                    //string atlas_filename;
                    try
                    {
                        til.Load(til_file_path);
                        //atlas_filename = GenerateTileAtlas(til);
                    }
                    catch (Exception x)
                    {
                        log.Error(x);
                        throw;
                    }


                    //GodotMapTileMesh tile_mesh = GenerateTileMesh(him, til, row, col);
                    //tile_mesh.AtlasPath = atlas_filename;

                    string lmap_dir = Path.Combine(Path.GetDirectoryName(him.FilePath), Path.GetFileNameWithoutExtension(him.FilePath));
                    string lmap_file_dds = Path.Combine(lmap_dir, $"{Path.GetFileNameWithoutExtension(him.FilePath)}_PLANELIGHTINGMAP.DDS");
                    string png_filename = Path.ChangeExtension(Path.GetFileName(lmap_file_dds), ".PNG");
                    string png_path = Path.Combine(LightmapsPath, png_filename);
                    //tile_mesh.LightmapPath = png_filename;

                    if (!Directory.Exists(png_path))
                    {
                        using (var image = Pfim.Pfimage.FromFile(lmap_file_dds))
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
                                bitmap.Save(png_path, ImageFormat.Png);
                            }
                            finally
                            {
                                handle.Free();
                            }
                        }
                    } // exists?

                    GenerateChunkMesh(him, til, row, col, png_filename);

                    //string file_name = Path.Combine(GodotScenePath, Path.GetFileNameWithoutExtension(him.FilePath) + ".tscn");

                    //try
                    //{
                    //    StreamWriter sw = new StreamWriter(file_name);
                    //    sw.WriteLine(tile_mesh.ToString());
                    //    sw.Close();
                    //}
                    //catch (Exception x)
                    //{
                    //    log.Error(x);
                    //    throw;
                    //}

                }
            }
        }

        private string GenerateTilesMap(List<GodotTilePatch> tile_scene, string translation, ref Dictionary<int, string> tiles_paths, string lightmap_path)
        {
            StringBuilder root = new StringBuilder();
            StringBuilder scene = new StringBuilder();
            StringBuilder external_resources = new StringBuilder();
            StringBuilder node = new StringBuilder();
            /*
            // res://shaders/tile.shader
            shader_type spatial;
            render_mode unshaded, cull_disabled;

            uniform sampler2D layer1;
            uniform sampler2D layer2;
            uniform sampler2D lightmap;
            uniform int rotation : hint_range(1, 7, 1) = 1;

            void fragment() {
                vec2 uv = vec2(UV);
	            vec2 rotated_uv = vec2(uv);
    
                if (rotation == 2) { // Flip Horizontal
                    rotated_uv = vec2(1.0 - uv.x, uv.y);
                }
                if (rotation == 3) { // Flip Vertical
                    rotated_uv = vec2(uv.x, 1.0 - uv.y);
                }
                if (rotation == 4) { // Flip
                    rotated_uv = vec2(1.0 - uv.x, 1.0 - uv.y);
                }
                if (rotation == 5) { // Clockwise 90
                    rotated_uv = vec2(-uv.y, uv.x);
                }
                if (rotation == 6) { // CounterClockwise 90
                    rotated_uv = vec2(uv.y, -uv.x);
                }
    
                vec4 layer1_tex = texture(layer1, uv);
                vec4 layer2_tex = texture(layer2, rotated_uv);
	            vec4 lmap_tex = texture(lightmap, UV2);
	
	            ALBEDO = mix(layer1_tex , layer2_tex,  layer2_tex.a).rgb * lmap_tex.rgb;
            }
            */

            /*
            // res://scenes/generate_collision_mesh.gd
            tool
            extends Node

            func _ready():
	            for child in get_node(".").get_children():
		            child.create_trimesh_collision()
            */

            root.AppendLine("[gd_scene format=2]\n");
            root.AppendLine($"[ext_resource path=\"res://shaders/tile.shader\" type=\"Shader\" id=1]");
            root.AppendLine($"[ext_resource path=\"{Path.Combine("LIGHTMAPS/", lightmap_path)}\" type=\"Texture\" id=2]");
            root.AppendLine("[ext_resource path=\"res://scenes/generate_collision_mesh.gd\" type=\"Script\" id=3]");
            root.AppendLine();
            int ext_resource = 4; // 1 is the tile_shader, 2nd lightmap, 3d script

            Dictionary<int, int> used_resources = new Dictionary<int, int>();

            int resource = 1;
            for (int tile_id = 0; tile_id < tile_scene.Count; tile_id++)
            {
                int texture_id1 = tile_scene[tile_id].layer1;

                if (!used_resources.ContainsKey(texture_id1))
                {
                    used_resources.Add(texture_id1, ext_resource);
                    string texture_path = tiles_paths[texture_id1];
                    external_resources.AppendLine($"[ext_resource path =\"../{texture_path}\" type=\"Texture\" id={ext_resource}]");
                    ext_resource++;
                }
                int shader_param_id1 = used_resources[texture_id1];

                int texture_id2 = tile_scene[tile_id].layer2;
                if (!used_resources.ContainsKey(texture_id2))
                {
                    used_resources.Add(texture_id2, ext_resource);
                    string texture_path = tiles_paths[texture_id2];
                    external_resources.AppendLine($"[ext_resource path =\"../{texture_path}\" type=\"Texture\" id={ext_resource}]");
                    ext_resource++;
                }
                int shader_param_id2 = used_resources[texture_id2];

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
                scene.AppendLine($"shader_param/lightmap = ExtResource( 2 )");
                scene.AppendLine($"shader_param/layer1 = ExtResource( {shader_param_id1} ) ; {tiles_paths[texture_id1]}");
                scene.AppendLine($"shader_param/layer2 = ExtResource( {shader_param_id2} ) ; {tiles_paths[texture_id2]}");
                scene.AppendLine($"shader_param/rotation = {(int)tile_scene[tile_id].rotation} ; {tile_scene[tile_id].rotation}");
                scene.AppendLine();

                resource++;
            }

            scene.AppendLine("; scene root node");
            scene.AppendLine($"[node type=\"Spatial\" name=\"CHUNK\"]");
            scene.AppendLine("script = ExtResource( 3 )"); // add generate_collision_mesh.gd
            scene.AppendLine(translation);
            scene.AppendLine();

            root.Append(external_resources);
            scene.Append(node);
            root.Append(scene);

            return root.ToString();
        }

        private void AddTileToLookup(ref Dictionary<int, string> tiles_paths, string path, int index)
        {
            string png_path = path.Replace("3DDATA/", string.Empty);
            png_path = Translator.FixPath(Path.ChangeExtension(png_path, ".PNG"));
            string absolute_png_path = Translator.FixPath(Path.Combine(GodotProjectPath, png_path));

            string folder = Path.GetDirectoryName(absolute_png_path);
            try
            {
                if (!Directory.Exists(folder))
                {
                    log.Info($"Creating folder: \"{absolute_png_path}\"");
                    Directory.CreateDirectory(folder);
                }
            }
            catch (Exception dx)
            {
                log.Fatal(dx);
                throw;
            }

            if (!tiles_paths.ContainsKey(index))
            {
                tiles_paths.Add(index, png_path);
                if (!File.Exists(absolute_png_path))
                {

                    FileStream stream = new FileStream(path, FileMode.Open);
                    try
                    {
                        DDSImage dds = new DDSImage(stream);
                        dds.BitmapImage.Save(absolute_png_path, ImageFormat.Png);
                    }
                    finally
                    {
                        stream.Close();
                    }
                } // exists
            }
        }

        private void GenerateChunkMesh(HeightmapFile him_file, TileFile tile_file, int row, int col, string lightmap_path)
        {
            //  UV mapping for tiles
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
            //			(.25,0)(.5,0)(.75,0)(1,0)

            // from godot zon_importer @ line 201

            Dictionary<int, string> tiles_paths = new Dictionary<int, string>();

            List<GodotTilePatch> tile_scene = new List<GodotTilePatch>();

            int tile_width = (him_file.Width - 1) / tile_file.Width + 1; // = 5
            int tile_height = (him_file.Height - 1) / tile_file.Height + 1; // = 5
            const float height_scale = 1f / 300f; //
            const float factor = 1f / 64f;

            const float scale_factor = 2.5f;
            Vector3f scale_vector = new Vector3f(scale_factor, 3f, scale_factor);

            for (int h = 0; h < (him_file.Height - 1); h += tile_height - 1)
            {
                for (int w = 0; w < (him_file.Width - 1); w += tile_width - 1)
                {
                    List<Vector3f> tile_vertices = new List<Vector3f>();
                    List<int> tile_indices = new List<int>();
                    List<Vector2f> tile_uv = new List<Vector2f>();
                    List<Vector2f> lightmap_uv = new List<Vector2f>();
                    List<Vector3f> tile_normals = new List<Vector3f>();

                    // arrays
                    for (int y = 0; y < tile_height; y++)
                    {
                        for (int x = 0; x < tile_width; x++)
                        {
                            Vector3f vertex = new Vector3f(w + x, him_file[h + y, w + x] * height_scale, h + y);
                            Vector2f uv = new Vector2f(x * 0.25f, y * 0.25f);

                            // Hide seams
                            if (uv.x == 0f)
                                uv.x = 0.01f;
                            if (uv.y == 0f)
                                uv.y = 0.01f;

                            if (uv.x >= 0.99f)
                                uv.x = 0.99f;
                            if (uv.y >= 0.99f)
                                uv.y = 0.99f;

                            float lm_x = ((w + x) * factor);
                            float lm_y = ((h + y) * factor);

                            Vector2f lmap_uv = new Vector2f(lm_x, lm_y);
                            Vector3f normal = new Vector3f(0, 1f, 0);

                            tile_vertices.Add(vertex * scale_vector);
                            tile_uv.Add(uv);
                            tile_normals.Add(normal);

                            lightmap_uv.Add(lmap_uv);
                        }
                    }

                    // build triangle indices
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

                    //var tile_x = (int)Math.Floor((double)(w / (tile_width - 1)));
                    //var tile_y = (int)Math.Floor((double)(h / (tile_height - 1)));

                    int tile_x = w / 4;
                    int tile_y = (him_file.Height - 1) / 4 - h / 4 - 1;
                    int tile_id = tile_file[tile_y, tile_x].Tile;

                    ZoneTile tile = zon_file.Tiles[tile_id];
                    int layer1 = tile.Layer1 + tile.Offset1; // is the bottom image
                    int layer2 = tile.Layer2 + tile.Offset2; // is the top image

                    string texture1_path = Translator.FixPath(zon_file.Textures[layer1]); // bottom
                    string texture2_path = Translator.FixPath(zon_file.Textures[layer2]); // top

                    TileRotation rotation = tile.Rotation;

                    AddTileToLookup(ref tiles_paths, texture1_path, layer1);
                    AddTileToLookup(ref tiles_paths, texture2_path, layer2);

                    GodotTilePatch patch = new GodotTilePatch()
                    {
                        layer1 = layer1,
                        layer2 = layer2,
                        vertices = tile_vertices,
                        indices = tile_indices,
                        uvs = tile_uv,
                        normals = tile_normals,
                        rotation = rotation,
                        lightmap_uvs = lightmap_uv,
                    };

                    tile_scene.Add(patch);
                }
            }

            // Translate each tile mesh chunk
            string chunk_transform = $"transform = Transform( 1, 0, 0, 0, 1, 0, 0, 0, 1, { row * (him_file.Width - 1) * 2.5f:0.######}, 0, {col * (him_file.Height - 1) * 2.5f:0.######} )";

            string scene = GenerateTilesMap(tile_scene, chunk_transform, ref tiles_paths, lightmap_path);

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
        }

        /*
        private GodotMapTileMesh GenerateTileMesh(HeightmapFile him_file, TileFile tile_file, int row, int col)
        {
            float x_stride = 2.5f;
            float y_stride = 2.5f;
            float heightScaler = x_stride * 1.2f / 300.0f; // = 100f
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

            // MESH ************************************************************************************************

            Quaterniond vrot = new Quaterniond(new Vector3d(0, 0, 1), -90);

            DMesh3 tile_mesh = new DMesh3(true, true, true);
            for (int y = 0; y < him_file.Height - 1; y++)
            {
                for (int x = 0; x < him_file.Width - 1; x++)
                {
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
                //TilesUV = uvsTop,
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
        */
    }
}
