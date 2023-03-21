using Revise.HIM;
using Revise.IFO;
using Revise.IFO.Blocks;
using Revise.TIL;
using Revise.ZON;
using Revise.ZSC;
using Rose2Godot.GodotExporters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
        public List<Vector3> vertex;
        public List<Vector3> normal;
        public List<int> triangle;
        public List<Vector2> uvsBottom;
        public List<Vector2> uvsTop;
        public string Name;

        public override string ToString()
        {
            StringBuilder scene = new StringBuilder();
            StringBuilder resource = new StringBuilder();
            StringBuilder nodes = new StringBuilder();

            scene.AppendFormat("[gd_scene load_steps={0} format=2]\n", 1);

            resource.AppendFormat("\n[sub_resource id={0} type=\"ArrayMesh\"]\n", 1);
            resource.AppendFormat("resource_name = \"Tile_{0}\"\n", Name);
            resource.AppendLine("surfaces/0 = {\n\t\"primitive\":4,\n\t\"arrays\":[");

            resource.AppendFormat("\t\t; vertices: {0}\n", vertex.Count);
            resource.AppendFormat("\t\t{0},\n", Translator.Vector3ToArray(vertex, null));

            // normals

            resource.AppendFormat("\t\t; normals: {0}\n", normal.Count);
            resource.AppendFormat("\t\t{0},\n", Translator.Vector3ToArray(normal, null));

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
            if (uvsBottom != null && uvsBottom.Any())
            {
                resource.AppendFormat("\t\t; UV2: {0}\n", uvsBottom.Count);
                resource.AppendFormat("\t\t{0},\n", Translator.Vector2fToArray(uvsBottom));
            }
            else
            {
                resource.AppendLine("\t\tnull, ; no UV2");
            }

            // no bones

            resource.AppendLine("\t\tnull, ; no bone indices");
            resource.AppendLine("\t\tnull, ; no bone weights");

            // face indices

            resource.AppendFormat("\t\t; triangle faces: {0}\n", triangle.Count);
            resource.AppendFormat("\t\t{0}\n", Translator.TriangleIndices(triangle));

            resource.AppendLine("\t],"); // end of mesh arrays
            resource.AppendLine("\t\"morph_arrays\":[]");
            resource.AppendLine("}"); // end of surface/0

            nodes.AppendLine($"\n[node name=\"Tile_{Name}\" type=\"MeshInstance\" parent=\".:\"]");
            nodes.AppendLine($"mesh = SubResource({1})");
            nodes.AppendLine("visible = true");
            nodes.AppendLine("transform = Transform(1, 0, 0, 0, -1, 0.00000724, 0, -0.00000724, -1, 0, 0, 0)\n"); // X 180

            scene.AppendLine();
            scene.Append(resource);
            scene.AppendLine();

            scene.AppendLine("; scene root node");
            scene.AppendLine("[node type=\"Spatial\" name=\"MapRoot\"]");
            scene.AppendLine("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)\n");

            scene.AppendLine();
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

            List<List<string>> HIMFilesGrid = new List<List<string>>();
            HIMHeightsGrid = new List<List<HeightmapFile>>();
            IFODataGrid = new List<List<MapDataFile>>();
            TILDataGrid = new List<List<string>>();

            for (int map_row_id = 0; map_row_id < HIMFiles.Count; map_row_id++)
            {
                List<string> row_him = HIMFiles.OrderBy(h => h).Where(h => h.Contains($"_{ZoneMinimapStartY + map_row_id}.")).Select(hf => Translator.FixPath(hf)).ToList();
                if (!row_him.Any())
                    break;
                HIMFilesGrid.Add(row_him);
            }

            if (!HIMFilesGrid.Any())
                return;

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

        private void AddVertexToLookup(Dictionary<string, List<int>> lookup, string vertex, int index)
        {
            if (!lookup.ContainsKey(vertex))
            {
                List<int> ids = new List<int>
                {
                    index
                };
                lookup.Add(vertex, ids);
            }
            else
                lookup[vertex].Add(index);
        }

        private void GenerateTerrainMesh()
        {
            for (int row = 0; row < HIMHeightsGrid.Count; row++)
            {
                List<HeightmapFile> him_row = HIMHeightsGrid[row];
                for (int col = 0; col < him_row.Count; col++)
                {
                    var him = him_row[col];
                    log.Info($"Generating 3D mesh for: {him.FilePath}");
                    var tile_mesh = GenerateTileMesh(him, row, col);

                    string godot_path = @"C:\Applications\Godot\GodotProjects\ImportTest\scenes\JDT01\";
                    string file_name = Path.Combine(godot_path, Path.GetFileNameWithoutExtension(him.FilePath)+ ".tscn");

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
            TileFile til_file = new TileFile();
            try
            {
                string til_path = TILDataGrid[row][col];
                til_file.Load(til_path);
                log.Info($"TIL: {til_path}");
            }
            catch (Exception x)
            {
                log.Error(x);
                throw;
            }

            List<Tile> tiles = new List<Tile>();
            int vertices_num = (him_file.Width - 1) * (him_file.Height - 1) * 4;
            int triangles_num = (him_file.Height - 1) * (him_file.Width - 1) * 6;
            Dictionary<string, List<int>> edge_vertex_lookup_dict = new Dictionary<string, List<int>>();
            Vector3[] vertices = new Vector3[vertices_num];
            List<Vector3> vertex_normals = new List<Vector3>();
            List<int> triangles = new List<int>(triangles_num);

            Vector2[] uvsBottom = new Vector2[vertices_num];
            Vector2[] uvsTop = new Vector2[vertices_num];

            int vertex_idx = 0; // vertex index

            float x_stride = 2.5f;
            float y_stride = 2.5f;
            float heightScaler = 300.0f / (x_stride * 1.2f);

            float x_offset = row * x_stride * (him_file.Width - 1);
            float y_offset = col * y_stride * (him_file.Height - 1);

            Vector2[,] uvMatrix = new Vector2[5, 5];
            Vector2[,] uvMatrixLR = new Vector2[5, 5];
            Vector2[,] uvMatrixTB = new Vector2[5, 5];
            Vector2[,] uvMatrixLRTB = new Vector2[5, 5];
            Vector2[,] uvMatrixRotCW = new Vector2[5, 5];  // rotated 90 deg clockwise
            Vector2[,] uvMatrixRotCCW = new Vector2[5, 5];	// rotated 90 counter clockwise

            for (int uv_x = 0; uv_x < 5; uv_x++)
            {
                for (int uv_y = 0; uv_y < 5; uv_y++)
                {
                    uvMatrix[uv_y, uv_x] = new Vector2(0.25f * uv_x, 1.0f - 0.25f * uv_y);
                    uvMatrixLR[uv_y, uv_x] = new Vector2(1.0f - 0.25f * uv_x, 1.0f - 0.25f * uv_y);
                    uvMatrixTB[uv_y, uv_x] = new Vector2(0.25f * uv_x, 0.25f * uv_y);
                    uvMatrixLRTB[uv_y, uv_x] = new Vector2(1.0f - 0.25f * uv_x, 0.25f * uv_y);
                    uvMatrixRotCCW[uv_x, uv_y] = new Vector2(0.25f * uv_x, 1.0f - 0.25f * uv_y);
                    uvMatrixRotCW[uv_x, uv_y] = new Vector2(0.25f * uv_y, 1.0f - 0.25f * uv_x);
                }
            }

            // Populate tiles with texture references
            for (int t_x = 0; t_x < 16; t_x++)
            {
                for (int t_y = 0; t_y < 16; t_y++)
                {
                    Tile tile = new Tile();
                    int tile_idx = til_file[t_y, t_x].Tile;
                    string texPath1 = Translator.FixPath(zon_file.Textures[zon_file.Tiles[tile_idx].Layer1]);
                    string texPath2 = Translator.FixPath(zon_file.Textures[zon_file.Tiles[tile_idx].Layer2]);

                    var image = DDSReader.DDSReader.ReadImage(texPath1);

                    tile.BottomTex = texPath1;
                    tile.BottomRect = new Rect(new Vector2(t_y, t_x), new Vector2(image.Width, image.Height));

                    tile.TopTex = texPath2;
                    tile.TopRect = new Rect(new Vector2(t_y, t_x), new Vector2(image.Width, image.Height));

                    tiles.Add(tile);
                    //log.Info($"TIL[{t_y} {t_x}] \"{texPath1}\"");
                }
            }

            /*
            // copy rects to tiles
            foreach (Tile tile in tiles)
            {
                tile.BottomRect = AtlasRectHash[tile.BottomTex];
                tile.TopRect = AtlasRectHash[tile.TopTex];
            }
            */

            for (int x = 0; x < him_file.Height - 1; x++)
            {
                for (int y = 0; y < him_file.Width - 1; y++)
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
                    //  The triangles used are: adb and bdc

                    int a = vertex_idx + 0;
                    int b = vertex_idx + 1;
                    int c = vertex_idx + 2;
                    int d = vertex_idx + 3;
                    vertex_idx += 4;

                    // Calculate vertices
                    vertices[a] = new Vector3(x * x_stride + x_offset, him_file.Heights[x, y] / heightScaler, y * y_stride + y_offset);
                    vertices[b] = new Vector3((x + 1) * x_stride + x_offset, him_file.Heights[x + 1, y] / heightScaler, y * y_stride + y_offset);
                    vertices[c] = new Vector3((x + 1) * x_stride + x_offset, him_file.Heights[x + 1, y + 1] / heightScaler, (y + 1) * y_stride + y_offset);
                    vertices[d] = new Vector3(x * x_stride + x_offset, him_file.Heights[x, y + 1] / heightScaler, (y + 1) * y_stride + y_offset);

                    if (y == 0)
                    {
                        AddVertexToLookup(edge_vertex_lookup_dict, vertices[a].ToString(), a);
                        AddVertexToLookup(edge_vertex_lookup_dict, vertices[a].ToString(), b);
                    }
                    if (y == him_file.Width - 1)
                    {
                        AddVertexToLookup(edge_vertex_lookup_dict, vertices[a].ToString(), d);
                        AddVertexToLookup(edge_vertex_lookup_dict, vertices[a].ToString(), c);
                    }
                    if (x == 0)
                    {
                        AddVertexToLookup(edge_vertex_lookup_dict, vertices[a].ToString(), a);
                        AddVertexToLookup(edge_vertex_lookup_dict, vertices[a].ToString(), d);
                    }
                    if (x == him_file.Height - 1)
                    {
                        AddVertexToLookup(edge_vertex_lookup_dict, vertices[a].ToString(), b);
                        AddVertexToLookup(edge_vertex_lookup_dict, vertices[a].ToString(), c);
                    }

                    int tileX = x / 4;
                    int tileY = y / 4;
                    int tileID = tileY * 16 + tileX;

                    // TODO:
                    TileRotation rotation = zon_file.Tiles[til_file[tileX, tileY].Tile].Rotation;
                    Vector2[,] rotMatrix;

                    //  if (rotation == TileRotation.Clockwise90Degrees || rotation == TileRotation.CounterClockwise90Degrees)
                    //      Debug.Log("Rotation: " + (int)rotation);

                    switch (rotation)
                    {
                        case TileRotation.None:
                            rotMatrix = uvMatrix;
                            break;
                        case TileRotation.FlipHorizontal:
                            rotMatrix = uvMatrixLR;
                            break;
                        case TileRotation.Flip:
                            rotMatrix = uvMatrixLRTB;
                            break;
                        case TileRotation.Clockwise90Degrees:
                            rotMatrix = uvMatrixRotCW;
                            break;
                        case TileRotation.CounterClockwise90Degrees:
                            rotMatrix = uvMatrixRotCCW;
                            break;
                        case TileRotation.FlipVertical:
                            rotMatrix = uvMatrixTB;
                            break;
                        default:
                            rotMatrix = uvMatrix;
                            break;
                    }

                    // Get top and bottom UV's using texture atlas and rotation adjustments
                    uvsTop[a] = tiles[tileID].GetUVTop(rotMatrix[x % 4, y % 4]);
                    uvsTop[b] = tiles[tileID].GetUVTop(rotMatrix[(x % 4 + 1) % 5, y % 4]);
                    uvsTop[c] = tiles[tileID].GetUVTop(rotMatrix[(x % 4 + 1) % 5, (y % 4 + 1) % 5]);
                    uvsTop[d] = tiles[tileID].GetUVTop(rotMatrix[x % 4, (y % 4 + 1) % 5]);

                    uvsBottom[a] = tiles[tileID].GetUVBottom(rotMatrix[x % 4, y % 4]);
                    uvsBottom[b] = tiles[tileID].GetUVBottom(rotMatrix[(x % 4 + 1) % 5, y % 4]);
                    uvsBottom[c] = tiles[tileID].GetUVBottom(rotMatrix[(x % 4 + 1) % 5, (y % 4 + 1) % 5]);
                    uvsBottom[d] = tiles[tileID].GetUVBottom(rotMatrix[x % 4, (y % 4 + 1) % 5]);

                    triangles.Add(a);
                    triangles.Add(d);
                    triangles.Add(b);

                    triangles.Add(b);
                    triangles.Add(d);
                    triangles.Add(c);

                    // add face normals as vertex normals, prior further optimization implemented after the triangle faces loops
                    Vector3 normal_triangle_one = Translator.CalculateVector3Normal(vertices[a], vertices[d], vertices[b]);
                    vertex_normals.Add(normal_triangle_one);
                    vertex_normals.Add(normal_triangle_one);
                    vertex_normals.Add(normal_triangle_one);

                    Vector3 normal_triangle_two = Translator.CalculateVector3Normal(vertices[b], vertices[d], vertices[c]);
                    vertex_normals.Add(normal_triangle_two);
                } // y
            } // x

            #region Calculate shared vertex normals

            // CalculateSharedNormals: fix all normals as follows:
            // Several triangles share same vertex, but it is duplicated
            // We want to:
            //	1. search the vertex array for shared vertices
            //	2. store each shared vertex id in a data structure comprising rows of shared vertices
            //  3. go through each row of shared vertices and calculate the average normal
            //	4. store the avg normal and all corresponding vertex id's in different data structure
            //	5. traverse the new data structure and assign the new normal to all the vertices it belongs to

            Dictionary<string, List<int>> vertex_lookup_dict = new Dictionary<string, List<int>>();
            // 1. and 2.
            for (int i = 0; i < vertices_num; i++)
                AddVertexToLookup(vertex_lookup_dict, vertices[i].ToString(), i);

            // traverse the shared vertex list and calculate new normals
            // 3.,4. & 5.
            foreach (KeyValuePair<string, List<int>> vertex in vertex_lookup_dict)
            {
                Vector3 avg = Vector3.Zero;
                foreach (int id in vertex.Value)
                    avg += vertex_normals[id];
                avg = Vector3.Normalize(avg);
                foreach (int id in vertex.Value)
                    vertex_normals[id] = avg;
            }
            #endregion

            log.Info($"\t verts: {vertices.Length} normals: {vertex_normals.Count} triangles: {triangles.Count} UVs1: {uvsTop.Length} UVs2 {uvsBottom.Length}");

            return new GodotMapTileMesh()
            {
                Name = $"{Path.GetFileNameWithoutExtension(him_file.FilePath)}",
                vertex = vertices.ToList(),
                normal = vertex_normals,
                triangle = triangles
            };
        }
    }
}
