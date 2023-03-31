using g4;
using Pfim;
using Revise.HIM;
using Revise.IFO;
using Revise.IFO.Blocks;
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
            resource.AppendLine($"[ext_resource path=\"{lightmap_path}\" type=\"Texture\" id=1]");

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
        public string Name { get; private set; }
        public List<ModelListPart> Part { get; private set; }

        public GodotObject(int id, List<ModelListPart> parts)
        {
            Id = id;
            Part = parts;
            Name = GenerateName();
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

        // CW
        private readonly Quaternion cw = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1f), -90f * 0.01745329251994329f);
        // CCW
        private readonly Quaternion ccw = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1f), 90f * 0.01745329251994329f);

        private ModelListFile zsc_objects;
        private ModelListFile zsc_buildings;
        private List<GodotObject> objects;
        private List<GodotObject> buildings;
        private ZoneFile zon_file;

        public void GenerateMapData()
        {
            if (!string.IsNullOrEmpty(ObjectsTable))
            {
                zsc_objects = new ModelListFile();
                try
                {
                    zsc_objects.Load(ObjectsTable);
                    objects = new List<GodotObject>();
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
                    buildings = new List<GodotObject>();
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

            log.Info("");

            List<List<string>> HIMFilesGrid = new List<List<string>>();
            HIMHeightsGrid = new List<List<HeightmapFile>>();
            IFODataGrid = new List<List<MapDataFile>>();
            TILDataGrid = new List<List<string>>();

            for (int map_row_id = 0; map_row_id < HIMFiles.Count; map_row_id++)
            {
                List<string> row_him = HIMFiles.Where(h => h.Contains($"{ZoneMinimapStartX + map_row_id}_")).OrderBy(h => h).Select(hf => Translator.FixPath(hf)).ToList();
                if (!row_him.Any())
                    break;

                //log.Info($"{string.Join(" ", row_him.Select(r => Path.GetFileNameWithoutExtension(r)).ToArray())}");

                HIMFilesGrid.Add(row_him);
            }

            if (!HIMFilesGrid.Any())
                return;

            HIMFilesGrid.Reverse();

            log.Info(" ");

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
                                MapBuilding building = ifo_file.Buildings[building_idx];
                                var building_parts = zsc_buildings.Objects[building.ObjectID];
                                buildings.Add(new GodotObject(building_idx, building_parts.Parts));
                            }
                        }

                        if (ifo_file.Objects.Any())
                        {
                            for (int object_idx = 0; object_idx < ifo_file.Objects.Count; object_idx++)
                            {
                                MapObject obj = ifo_file.Objects[object_idx];
                                var obj_parts = zsc_objects.Objects[obj.ObjectID];

                                objects.Add(new GodotObject(object_idx, obj_parts.Parts));
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
            string godot_path = @"C:\Applications\Godot\GodotProjects\ImportTest\scenes\JDT01\";

            for (int row = 0; row < HIMHeightsGrid.Count; row++)
            {
                List<HeightmapFile> him_row = HIMHeightsGrid[row];
                for (int col = 0; col < him_row.Count; col++)
                //for (int col = 0; col < 1; col++)
                {
                    var him = him_row[col];
                    var tile_mesh = GenerateTileMeshG4(him, row, col);
                    // tile_mesh.lightmap_path = "res://textures/uv_map.png";

                    string lmap_dir = Path.Combine(Path.GetDirectoryName(him.FilePath), Path.GetFileNameWithoutExtension(him.FilePath));
                    string lmap_file_dds = Path.Combine(lmap_dir, $"{Path.GetFileNameWithoutExtension(him.FilePath)}_PLANELIGHTINGMAP.DDS");
                    log.Info($"{lmap_file_dds}");
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
                            string png_filename = Path.ChangeExtension(Path.GetFileName(lmap_file_dds), ".PNG");
                            string png_path = Path.Combine(godot_path, png_filename);
                            tile_mesh.lightmap_path = png_filename;
                            bitmap.Save(png_path, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        finally
                        {
                            handle.Free();
                        }
                    }

                    string file_name = Path.Combine(godot_path, Path.GetFileNameWithoutExtension(him.FilePath) + ".tscn");

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

        private GodotMapTileMesh GenerateTileMeshG4(HeightmapFile him_file, int row, int col)
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
