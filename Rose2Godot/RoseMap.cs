using g4;
using Pfim;
using Revise.HIM;
using Revise.IFO;
using Revise.IFO.Blocks;
using Revise.ZMS;
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
        public DMesh3 mesh;
        public string name;
        public string lightmap_path;

        public override string ToString()
        {
            StringBuilder scene = new StringBuilder();
            StringBuilder resource = new StringBuilder();
            StringBuilder nodes = new StringBuilder();

            scene.AppendFormat("[gd_scene load_steps={0} format=2]\n", 4);

            // Add texture external resource
            resource.AppendLine();
            resource.AppendLine($"[ext_resource path=\"{Path.Combine("LIGHTMAPS/", lightmap_path)}\" type=\"Texture\" id=1]");

            resource.AppendFormat("\n[sub_resource id={0} type=\"ArrayMesh\"]\n", 1);
            resource.AppendFormat("resource_name = \"Tile_{0}\"\n", name);
            resource.AppendLine("surfaces/0 = {\n\t\"primitive\":4,\n\t\"arrays\":[");

            resource.AppendFormat("\t\t; vertices: {0}\n", mesh.Vertices().Count());
            resource.AppendFormat("\t\t{0},\n", Translator.Vector3ToArray(mesh.Vertices().ToList(), null));

            // normals

            resource.AppendFormat("\t\t; normals: {0}\n", mesh.Vertices().Count());

            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvsTop = new List<Vector2>();

            for (int vidx = 0; vidx < mesh.VertexCount; vidx++)
            {
                var vnormal = mesh.GetVertexNormal(vidx);
                var uv = mesh.GetVertexUV(vidx);
                normals.Add(new Vector3(vnormal.x, vnormal.y, vnormal.z));
                uvsTop.Add(new Vector2(uv.x, uv.y));
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
            //if (uvsBottom != null && uvsBottom.Any())
            //{
            //    resource.AppendFormat("\t\t; UV2: {0}\n", uvsBottom.Count);
            //    resource.AppendFormat("\t\t{0},\n", Translator.Vector2fToArray(uvsBottom));
            //}
            //else
            //{
            resource.AppendLine("\t\tnull, ; no UV2");
            //}

            // no bones

            resource.AppendLine("\t\tnull, ; no bone indices");
            resource.AppendLine("\t\tnull, ; no bone weights");

            // face indices

            resource.AppendFormat("\t\t; triangle faces: {0}\n", mesh.Triangles().Count());
            resource.AppendFormat("\t\t{0}\n", Translator.TriangleIndices(mesh.Triangles().ToList()));

            resource.AppendLine("\t],"); // end of mesh arrays
            resource.AppendLine("\t\"morph_arrays\":[]");
            resource.AppendLine("}"); // end of surface/0
            resource.AppendLine("");

            // Add material refering external texture resource
            resource.AppendLine("[sub_resource type=\"SpatialMaterial\" id=2]");
            resource.AppendLine("flags_unshaded = true");
            resource.AppendLine("albedo_texture = ExtResource( 1 )");

            nodes.AppendLine($"\n[node name=\"Tile_{name}\" type=\"MeshInstance\" parent=\".:\"]");
            nodes.AppendLine($"mesh = SubResource( 1 )");
            nodes.AppendLine($"material/0 = SubResource( 2 )");
            nodes.AppendLine("visible = true");
            //nodes.AppendLine("transform = Transform(1, 0, 0, 0, -1, 0.00000724, 0, -0.00000724, -1, 0, 0, 0)\n"); // X 180
            nodes.AppendLine("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)");

            scene.Append(resource);
            scene.AppendLine();

            scene.AppendLine("; scene root node");
            scene.AppendLine($"[node type=\"Spatial\" name=\"Map_{name}\"]");
            scene.AppendLine("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)");

            scene.Append(nodes);

            return scene.ToString();
        }
    }

    public class GodotObject
    {
        public int Id;
        public MapBlock MapBlock { get; set; }
        public string Name { get; private set; }
        public List<ModelListPart> Part { get; private set; }

        public GodotObject(int id, MapBlock map_block, List<ModelListPart> parts)
        {
            Id = id;
            Part = parts;
            Name = GenerateName();
            MapBlock = map_block;
        }

        private string GenerateName()
        {
            int h = 917113; // random prime number
            if (Part != null)
            {
                var hash = Part.Select(p => p.GetHashCode()).ToList();
                foreach (var item in hash)
                    h ^= item;
            }
            else
            {
                Random rnd = new Random(DateTime.UtcNow.Millisecond);
                h ^= rnd.Next();
            }
            return $"Object_{h:X}";
        }

        public override string ToString()
        {
            StringBuilder scene = new StringBuilder();

            int resource_index = 1;
            scene.AppendFormat("[gd_scene load_steps={0} format=2]\n", Part.Count);

            foreach (var part in Part)
            {
                string model_path = string.Empty;
                string model_name = string.Empty;
                //MeshExporter meshExporter = new MeshExporter(resource_index, model_path, model_name, false);
                //resource_index = meshExporter.LastResourceIndex;
                //scene.AppendLine(meshExporter.Resources);
            }

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
        private List<GodotObject> Objects;
        private List<GodotObject> Buildings;
        private ZoneFile zon_file;
        private string ObjectsPath;
        private string BuildingsPath;
        private string LightmapsPath;
        private Dictionary<int, string> buildings_mesh_files;
        private Dictionary<int, string> objects_mesh_files;
        private Dictionary<int, string> objects_material_files;
        private Dictionary<int, string> buildings_material_files;

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
                        var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                        var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
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
            material_content.AppendLine("albedo_texture = ExtResource( 1 )");

            StreamWriter fileStream = new StreamWriter(output_file_name_path);
            fileStream.Write(material_content.ToString());
            fileStream.Close();

            return $"{texture_name}.tres";
        }

        public void GenerateMapData()
        {
            Objects = new List<GodotObject>();
            Buildings = new List<GodotObject>();


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
                    //string objects_name = $"OBJECTS_{STLId}";
                    //BuildingsAndDecorsExporter obj_exporter = new BuildingsAndDecorsExporter(objects_name, ObjectsTable);
                    //obj_exporter.ExportScene($"{Path.Combine(ObjectsPath, objects_name)}.tscn");
                    // Export .tres material files
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
                    //string buildings_name = $"BUILDINGS_{STLId}";
                    //BuildingsAndDecorsExporter buildings_exporter = new BuildingsAndDecorsExporter(buildings_name, BuildingsTable);
                    //buildings_exporter.ExportScene($"{Path.Combine(BuildingsPath, buildings_name)}.tscn");
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

            zon_file = new ZoneFile();
            try
            {
                zon_file.Load(ZONPath);
                log.Info($"ZON: {ZONPath}");
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

            //log.Info(" ");

            TilesX = HIMFilesGrid[0].Count;
            TilesY = HIMFilesGrid.Count;

            #region Combined height data

            List<HeightmapFile> map_height_row;
            List<MapDataFile> map_data_row;
            List<string> til_data_row;

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
                                log.Info($"Exporting Building: {building_idx} {map_col_id} {map_row_id}");
                                StringBuilder resource = new StringBuilder();
                                StringBuilder node = new StringBuilder();

                                resource.AppendLine("[gd_scene load_steps=2 format=2]\n");

                                MapBuilding building = ifo_file.Buildings[building_idx];
                                var building_parts = zsc_buildings.Objects[building.ObjectID];

                                int resource_idx = 1;

                                for (int part_idx = 0; part_idx < building_parts.Parts.Count; part_idx++)
                                {
                                    ModelListPart bld = building_parts.Parts[part_idx];
                                    string part = buildings_mesh_files[bld.Model];
                                    string tex = buildings_material_files[bld.Texture];
                                    string part_name = Path.GetFileNameWithoutExtension(part);
                                    SceneExporter exporter = new SceneExporter($"{part_name}", part, tex);
                                    string part_scene_file = Path.Combine(BuildingsPath, $"{part_name}.tscn");
                                    exporter.ExportScene(part_scene_file);
                                    resource.AppendLine($"[ext_resource path=\"{part_name}.tscn\" type=\"PackedScene\" id={resource_idx}]");
                                    node.AppendLine($"[node name=\"PART_{part_idx:00}\" parent=\".\" instance=ExtResource( {resource_idx} )]\n");
                                    resource_idx ++;
                                }

                                resource.AppendLine($"\n[node name=\"BUILDING_{building_idx:00}_{map_col_id:00}_{map_row_id:00}\" type=\"Spatial\"]\n");
                                resource.Append(node);

                                string file_name = Path.Combine(BuildingsPath, $"BUILDING_{building_idx:00}_{map_col_id:00}_{map_row_id:00}.tscn");
                                StreamWriter fileStream = new StreamWriter(file_name);
                                fileStream.Write(resource.ToString());
                                fileStream.Close();

                                Buildings.Add(new GodotObject(building_idx, building, building_parts.Parts));
                            }
                        }

                        if (ifo_file.Objects.Any())
                        {
                            for (int object_idx = 0; object_idx < ifo_file.Objects.Count; object_idx++)
                            {
                                MapObject obj = ifo_file.Objects[object_idx];
                                var obj_parts = zsc_objects.Objects[obj.ObjectID];
                                for (int part_idx = 0; part_idx < obj_parts.Parts.Count; part_idx++)
                                {
                                    ModelListPart obj_part = obj_parts.Parts[part_idx];
                                    string part = objects_mesh_files[obj_part.Model];
                                    string tex = objects_material_files[obj_part.Texture];
                                }
                                Objects.Add(new GodotObject(object_idx, obj, obj_parts.Parts));
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

            GenerateBuildings();
            GenerateObjects();

            GenerateTerrainMesh();
        }

        private void GenerateBuildings()
        {
            foreach (GodotObject building in Buildings)
            {
                if (string.IsNullOrWhiteSpace(building.MapBlock.Name))
                {
                    building.MapBlock.Name = $"Building_{building.MapBlock.ObjectID:000}";
                }

                //log.Info($"Name: {building.MapBlock.Name} Parts: {building.Part.Count:00} ID: {building.MapBlock.ObjectID:000} Position: {building.MapBlock.Position}");
            }

            //foreach (var item in buildings_mesh_files)
            //{
            //    log.Info($"{item.Key} {item.Value}");
            //}
        }

        private void GenerateObjects()
        {
            foreach (GodotObject obj in Objects)
            {
                if (string.IsNullOrWhiteSpace(obj.MapBlock.Name))
                {
                    obj.MapBlock.Name = $"Object_{obj.MapBlock.ObjectID}";
                }
                //log.Info($"Name: {obj.MapBlock.Name} ID: {obj.MapBlock.ObjectID} Position: {obj.MapBlock.Position}");
            }

            //foreach (var item in objects_mesh_files)
            //{
            //    log.Info($"{item.Key} {item.Value}");
            //}
        }

        private void GenerateTerrainMesh()
        {
            for (int row = 0; row < HIMHeightsGrid.Count; row++)
            {
                List<HeightmapFile> him_row = HIMHeightsGrid[row];
                for (int col = 0; col < him_row.Count; col++)
                {
                    var him = him_row[col];
                    var tile_mesh = GenerateTileMesh(him, row, col);

                    string lmap_dir = Path.Combine(Path.GetDirectoryName(him.FilePath), Path.GetFileNameWithoutExtension(him.FilePath));
                    string lmap_file_dds = Path.Combine(lmap_dir, $"{Path.GetFileNameWithoutExtension(him.FilePath)}_PLANELIGHTINGMAP.DDS");
                    string png_filename = Path.ChangeExtension(Path.GetFileName(lmap_file_dds), ".PNG");
                    string png_path = Path.Combine(LightmapsPath, png_filename);
                    tile_mesh.lightmap_path = png_filename;

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
                    }// exists?

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

        private GodotMapTileMesh GenerateTileMesh(HeightmapFile him_file, int row, int col)
        {
            float x_stride = 2.5f;
            float y_stride = 2.5f;
            float heightScaler = (x_stride * 1.2f) / 300.0f;
            const float factor = 1f / 64f;

            float x_offset = row * x_stride * (him_file.Width - 1);
            float y_offset = col * y_stride * (him_file.Height - 1);

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

                    Vector2f uva = new Vector2f(x * factor, y * factor);
                    Vector2f uvb = new Vector2f((x + 1) * factor, y * factor);
                    Vector2f uvc = new Vector2f((x + 1) * factor, (y + 1) * factor);
                    Vector2f uvd = new Vector2f(x * factor, (y + 1) * factor);

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
                name = $"{Path.GetFileNameWithoutExtension(him_file.FilePath)}",
                mesh = tile_mesh,
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
