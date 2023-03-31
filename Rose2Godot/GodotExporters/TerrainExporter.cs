using System.Numerics;
using Revise.HIM;
using Revise.TIL;
using Revise.ZON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rose2Godot.GodotExporters
{

    public class Tile
    {
        public Rect BottomRect { get; set; }
        public Rect TopRect { get; set; }
        public string BottomTex { get; set; }
        public string TopTex { get; set; }

        public Tile()
        {
            BottomRect = new Rect();
            TopRect = new Rect();
        }

        public void SetRects(Rect bottom, Rect top)
        {
            BottomRect = bottom;
            TopRect = top;
        }

        public Vector2 GetUVTop(Vector2 uv)
        {
            // adjust uv's slightly to hide seams between tiles
            if (uv.X < 0.01f) uv.X += 0.01f;
            else if (uv.X > 0.99f) uv.X *= 0.99f;
            if (uv.Y < 0.01f) uv.Y += 0.01f;
            else if (uv.Y > 0.99f) uv.Y *= 0.99f;
            return new Vector2((uv.X * TopRect.Width) + TopRect.X, (uv.Y * TopRect.Height) + TopRect.Y);
        }

        public Vector2 GetUVBottom(Vector2 uv)
        {
            if (uv.X < 0.01f) uv.X += 0.01f;
            else if (uv.X > 0.99f) uv.X *= 0.99f;
            if (uv.Y < 0.01f) uv.Y += 0.01f;
            else if (uv.Y > 0.99f) uv.Y *= 0.99f;
            return new Vector2((uv.X * BottomRect.Width) + BottomRect.X, (uv.Y * BottomRect.Height) + BottomRect.Y);
        }
    }

    public class TerrainExporter
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetLogger("TerrainExporter");

        private readonly StringBuilder resource;
        private readonly StringBuilder nodes;
        private string name;
        private readonly List<string> files;
        private readonly StringBuilder scene;
        public int LastResourceIndex { get; }
        public string MeshName { get; set; }
        public string Resources => resource.ToString();
        public string Nodes => nodes.ToString();

        public int ZoneMinimapStartY { get; set; }
        public int ZoneMinimapStartX { get; set; }

        private HeightmapFile him_file;
        private ZoneFile zon_file;
        private TileFile til_file;
        private Dictionary<string, Rect> AtlasRectHash;

        private List<List<HeightmapFile>> HIMHeightsGrid;

        private void ScanFolder(string folder_path)
        {
            List<string> HIMFiles = new List<string>(Directory.GetFiles(folder_path, "*.HIM"));

            if (!HIMFiles.Any())
                return;

            List<List<string>> HIMFilesGrid = new List<List<string>>();
            HIMHeightsGrid = new List<List<HeightmapFile>>();

            for (int map_row_id = 0; map_row_id < HIMFiles.Count; map_row_id++)
            {
                List<string> row_him = HIMFiles.OrderBy(h => h).Where(h => h.Contains($"_{ZoneMinimapStartY + map_row_id}.")).ToList();
                if (!row_him.Any())
                    break;
                HIMFilesGrid.Add(row_him);
            }

            if (!HIMFilesGrid.Any())
                return;
        }

        public TerrainExporter(List<string> file_paths)
        {
            AtlasRectHash = new Dictionary<string, Rect>();

            nodes = new StringBuilder();
            resource = new StringBuilder();
            files = file_paths;

            zon_file = new ZoneFile();
            DirectoryInfo directory_info = new DirectoryInfo(files.First());
            string root_folder = directory_info.Parent.FullName;
            log.Info($"Root folder: {root_folder}");

            string zon_path = Path.Combine(root_folder, $"{directory_info.Parent.Name}.ZON");
            log.Info($"Loading ZON file: \"{zon_path}\"");
            try
            {
                zon_file.Load(zon_path);
            }
            catch (Exception x)
            {
                log.Error(x);
                throw;
            }

            log.Info($"Scanning: \"{root_folder}\"");

            ZoneMinimapStartX = 30;
            ZoneMinimapStartY = 31;
            ScanFolder(root_folder);

            LastResourceIndex = 1;
            //Console.WriteLine($"[Mesh export] Start from idx: {LastResourceIndex}");
            scene = new StringBuilder();
            scene.AppendFormat("[gd_scene load_steps={0} format=2]\n", 1); // 1 - mesh
        }

        private string Vector3ToArray(Vector3[] vlist, float? scale)
        {
            List<string> vs = new List<string>();
            for (int v_idx = 0; v_idx < vlist.Length; v_idx++)
            {
                Vector3 v = vlist[v_idx];
                v.Z *= scale ?? 1f;
                vs.Add($"{v.X:0.###},{v.Y:0.###},{v.Z:0.###}");
            }
            return $"Vector3Array({string.Join(",", vs.ToArray())})";
        }

        private string TriangleIndices(int[] vlist)
        {
            List<string> vs = new List<string>();
            for (int idx = 0; idx < vlist.Length; idx++)
                vs.Add($"{vlist[idx]}");
            return $"IntArray({string.Join(",", vs.ToArray())})";
        }

        private string Vector2fToArray(List<Vector2> vlist)
        {
            List<string> vs = new List<string>();
            foreach (Vector2 v in vlist)
                vs.Add($"{v.X:0.####}, {v.Y:0.####}");

            return $"Vector2Array({string.Join(", ", vs.ToArray())})";
        }

        public List<Vector3> CalculateMeshTangents(List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uv, bool uv2 = false)
        {
            int vertices_num = vertices.Count;

            Vector3[] tan1 = new Vector3[vertices_num];
            Vector3[] tan2 = new Vector3[vertices_num];

            Vector3[] tangents = new Vector3[vertices_num];

            for (int triangle_idx = 0; triangle_idx < triangles.Count; triangle_idx += 3)
            {
                int index_1 = triangles[triangle_idx + 0];
                int index_2 = triangles[triangle_idx + 1];
                int index_3 = triangles[triangle_idx + 2];

                Vector3 v1 = vertices[index_1];
                Vector3 v2 = vertices[index_2];
                Vector3 v3 = vertices[index_3];

                Vector2 w1 = uv[index_1];
                Vector2 w2 = uv[index_2];
                Vector2 w3 = uv[index_3];

                float x1 = v2.X - v1.X;
                float x2 = v3.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v3.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v3.Z - v1.Z;

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;

                float r = 1.0f / (s1 * t2 - s2 * t1);

                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[index_1] += sdir;
                tan1[index_2] += sdir;
                tan1[index_3] += sdir;

                tan2[index_1] += tdir;
                tan2[index_2] += tdir;
                tan2[index_3] += tdir;
            }

            for (int vertex_idx = 0; vertex_idx < vertices_num; ++vertex_idx)
            {
                Vector3 normal = normals[vertex_idx];
                Vector3 tangent = tan1[vertex_idx];

                Vector3 orthonormalized = Vector3.Normalize(tangent - normal * Vector3.Dot(normal, tangent));
                tangents[vertex_idx] = new Vector3(orthonormalized.X, orthonormalized.Y, orthonormalized.Z);

                // OR -->
                //Vector3.OrthoNormalize(ref n, ref t);
                //tangents[a].x = t.x;
                //tangents[a].y = t.y;
                //tangents[a].z = t.z;
                //tangents[a].w = (n.Cross(t).Dot(tan2[a]) < 0.0f) ? -1.0f : 1.0f;
            }

            return tangents.ToList();
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

        private Vector3 CalculateVector3Normal(Vector3 p1, Vector3 p2, Vector3 p3) => Vector3.Normalize(Vector3.Cross(p2 - p1, p3 - p1));

        // TODO:
        public void UpdateAtlas(ref Dictionary<string, Rect> atlasRectHash /*, ref Dictionary<string, Texture2D> atlasTexHash, ref List<Texture2D> textures*/)
        {
            for (int t_x = 0; t_x < 16; t_x++)
            {
                for (int t_y = 0; t_y < 16; t_y++)
                {
                    int tileID = til_file[t_x, t_y].Tile;
                    //zon_file.Positions
                    string texture_path_1 = Translator.FixPath(zon_file.Textures[zon_file.Tiles[tileID].Layer1]);
                    string texture_path_2 = Translator.FixPath(zon_file.Textures[zon_file.Tiles[tileID].Layer2]);

                    //Texture2D tex1 = zon_file.Textures[zon_file.Tiles[tileID].Layer1].Tex;
                    //Texture2D tex2 = zon_file.Textures[zon_file.Tiles[tileID].Layer2].Tex;

                    // Adding an existing texture to atlas will cause an exception, so catch it but do nothing
                    // as this is expected to happen
                    try
                    {
                        atlasRectHash.Add(texture_path_1, new Rect());
                        //atlasRectHash.Add(texPath1, new Rect());

                        //atlasTexHash.Add(texPath1, tex1);
                        //textures.Add(tex1);
                    }
                    catch (Exception) { }

                    try
                    {
                        atlasRectHash.Add(texture_path_2, new Rect());
                        //atlasTexHash.Add(texPath2, tex2);
                        //textures.Add(tex2);
                    }
                    catch (Exception) { }
                }
            }
        }

        public bool ExportScene(string export_file_name)
        {
            int idx = 1;

            string input_file_name = files.First();
            name = Path.GetFileNameWithoutExtension(input_file_name);

            if (!File.Exists(input_file_name))
            {
                log.Error($"\"{input_file_name}\" does not exist!");
                return false;
            }

            him_file = new HeightmapFile();
            try
            {
                him_file.Load(input_file_name);
            }
            catch (Exception x)
            {
                log.Error(x);
                throw;
            }

            string til_path = Path.ChangeExtension(input_file_name, ".TIL");
            til_file = new TileFile();
            try
            {
                til_file.Load(til_path);
            }
            catch (Exception x)
            {
                log.Error(x);
                throw;
            }

            UpdateAtlas(ref AtlasRectHash);

            resource.AppendFormat("\n[sub_resource id={0} type=\"ArrayMesh\"]\n", idx);
            resource.AppendFormat("resource_name = \"Tile_{0}\"\n", name);
            resource.AppendLine("surfaces/0 = {\n\t\"primitive\":4,\n\t\"arrays\":[");

            #region Triangulation

            int vertices_num = (him_file.Width - 1) * (him_file.Height - 1) * 4;
            //int vertices_num = 64 * 64 * 4;
            int triangles_num = (him_file.Height - 1) * (him_file.Width - 1) * 6;

            Dictionary<string, List<int>> edge_vertex_lookup_dict = new Dictionary<string, List<int>>();
            Vector3[] vertices = new Vector3[vertices_num];
            List<Vector3> vertex_normals = new List<Vector3>();

            Vector2[] uvsBottom = new Vector2[vertices_num];
            Vector2[] uvsTop = new Vector2[vertices_num];
            List<int> triangles = new List<int>(triangles_num);

            int vertex_idx = 0; // vertex index

            float x_stride = 2.5f;
            float y_stride = 2.5f;
            float heightScaler = 300.0f / (x_stride * 1.2f);

            int row = 0;
            int col = 0;
            const float factor = 1f / 64f;

            float x_offset = row * x_stride * (him_file.Width - 1);
            float y_offset = col * y_stride * (him_file.Height - 1);

            /*
                float x_offset = row * m_xStride * 64.0f;
                float y_offset = col * m_yStride * 64.0f;
            */

            Vector2 center = new Vector2(x_offset + x_stride * (him_file.Width - 1) / 2f, y_offset + y_stride * (him_file.Height - 1) / 2f);

            log.Info($"Tile center: {center} Offsets: {x_offset} {y_offset} vertices: {vertices_num} triangles: {triangles_num}");

            Vector2[,] uvMatrix = new Vector2[5, 5];
            Vector2[,] uvMatrixLR = new Vector2[5, 5];
            Vector2[,] uvMatrixTB = new Vector2[5, 5];
            Vector2[,] uvMatrixLRTB = new Vector2[5, 5];
            Vector2[,] uvMatrixRotCW = new Vector2[5, 5];  // rotated 90 deg clockwise
            Vector2[,] uvMatrixRotCCW = new Vector2[5, 5];	// rotated 90 counter clockwise

            for (int uv_x = 0; uv_x < 5; uv_x++)
                for (int uv_y = 0; uv_y < 5; uv_y++)
                {
                    uvMatrix[uv_y, uv_x] = new Vector2(0.25f * uv_x, 1.0f - 0.25f * uv_y);
                    uvMatrixLR[uv_y, uv_x] = new Vector2(1.0f - 0.25f * uv_x, 1.0f - 0.25f * uv_y);
                    uvMatrixTB[uv_y, uv_x] = new Vector2(0.25f * uv_x, 0.25f * uv_y);
                    uvMatrixLRTB[uv_y, uv_x] = new Vector2(1.0f - 0.25f * uv_x, 0.25f * uv_y);
                    uvMatrixRotCCW[uv_x, uv_y] = new Vector2(0.25f * uv_x, 1.0f - 0.25f * uv_y);
                    uvMatrixRotCW[uv_x, uv_y] = new Vector2(0.25f * uv_y, 1.0f - 0.25f * uv_x);
                }

            List<Tile> tiles = new List<Tile>();

            // Populate tiles with texture references
            for (int t_x = 0; t_x < 16; t_x++)
            {
                for (int t_y = 0; t_y < 16; t_y++)
                {
                    Tile tile = new Tile();
                    int tile_idx = til_file[t_y, t_x].Tile;
                    string texPath1 = Translator.FixPath(zon_file.Textures[zon_file.Tiles[tile_idx].Layer1]);
                    string texPath2 = Translator.FixPath(zon_file.Textures[zon_file.Tiles[tile_idx].Layer2]);


                    //var image = DDSReader.DDSReader.ReadImage(texPath1);
                    tile.BottomTex = texPath1;
                    //tile.BottomRect = new Rect(new Vector2(t_y, t_x), new Vector2(image.Width, image.Height));
                    //tile.TopRect = new Rect(new Vector2(t_y, t_x), new Vector2(image.Width, image.Height));
                    tile.BottomRect = new Rect(new Vector2(t_y, t_x), new Vector2(256, 256));
                    tile.TopRect = new Rect(new Vector2(t_y, t_x), new Vector2(256, 256));

                    tile.TopTex = texPath2;

                    tiles.Add(tile);
                    log.Info($"TIL[{t_y} {t_x}] \"{texPath1}\"");
                }
            }

            // copy rects to tiles
            foreach (Tile tile in tiles)
            {
                tile.BottomRect = AtlasRectHash[tile.BottomTex];
                tile.TopRect = AtlasRectHash[tile.TopTex];
            }

            UpdateAtlas(ref AtlasRectHash);

            // Create a texture atlas from the textures of all patches and populate the rectangles in the hash

            // Figure out the required size of the atlas from the number of textures in the atlas
            uint atlas_texture_height, atlas_texture_width;  // these must be powers of 2 to be compatible with iPhone
            if (AtlasRectHash.Count <= 16) atlas_texture_width = atlas_texture_height = 4 * 256;
            else if (AtlasRectHash.Count <= 32) { atlas_texture_width = 8 * 256; atlas_texture_height = 4 * 256; }
            else if (AtlasRectHash.Count <= 64) { atlas_texture_width = 8 * 256; atlas_texture_height = 8 * 256; }
            else if (AtlasRectHash.Count <= 128) { atlas_texture_width = 16 * 256; atlas_texture_height = 8 * 256; }
            else if (AtlasRectHash.Count <= 256) { atlas_texture_width = 16 * 256; atlas_texture_height = 16 * 256; }
            else throw new Exception("Number of tiles in terrain is larger than supported by terrain atlas");

            //Texture2D atlas = new Texture2D(width, height);
            //DDSImage atlas = new DDSImage(atlas_texture_width, atlas_texture_height);


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
                    //uvsTop[a] = tiles[tileID].GetUVTop(rotMatrix[x % 4, y % 4]);
                    //uvsTop[b] = tiles[tileID].GetUVTop(rotMatrix[(x % 4 + 1) % 5, y % 4]);
                    //uvsTop[c] = tiles[tileID].GetUVTop(rotMatrix[(x % 4 + 1) % 5, (y % 4 + 1) % 5]);
                    //uvsTop[d] = tiles[tileID].GetUVTop(rotMatrix[x % 4, (y % 4 + 1) % 5]);

                    //uvsBottom[a] = tiles[tileID].GetUVBottom(rotMatrix[x % 4, y % 4]);
                    //uvsBottom[b] = tiles[tileID].GetUVBottom(rotMatrix[(x % 4 + 1) % 5, y % 4]);
                    //uvsBottom[d] = tiles[tileID].GetUVBottom(rotMatrix[(x % 4 + 1) % 5, (y % 4 + 1) % 5]);
                    //uvsBottom[c] = tiles[tileID].GetUVBottom(rotMatrix[x % 4, (y % 4 + 1) % 5]);

                    uvsTop[a] = new Vector2(x * factor, y * factor);
                    uvsTop[b] = new Vector2((x + 1f) * factor, y * factor);
                    uvsTop[c] = new Vector2((x + 1f) * factor, (y + 1f) * factor);
                    uvsTop[d] = new Vector2(x * factor, (y + 1f) * factor);

                    triangles.Add(a);
                    triangles.Add(d);
                    triangles.Add(b);

                    triangles.Add(b);
                    triangles.Add(d);
                    triangles.Add(c);

                    // add face normals as vertex normals, prior further optimization implemented after the triangle faces loops
                    Vector3 normal_triangle_one = CalculateVector3Normal(vertices[a], vertices[d], vertices[b]);
                    vertex_normals.Add(normal_triangle_one);
                    vertex_normals.Add(normal_triangle_one);
                    vertex_normals.Add(normal_triangle_one);

                    Vector3 normal_triangle_two = CalculateVector3Normal(vertices[b], vertices[d], vertices[c]);
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

            //
            #endregion
            // vertices

            resource.AppendFormat("\t\t; vertices: {0}\n", vertices.Length);
            resource.AppendFormat("\t\t{0},\n", Vector3ToArray(vertices, null));

            // normals

            resource.AppendFormat("\t\t; normals: {0}\n", vertex_normals.Count);
            resource.AppendFormat("\t\t{0},\n", Vector3ToArray(vertex_normals.ToArray(), null));

            // tangents

            resource.AppendLine("\t\tnull, ; no tangents");

            // vertex colors

            resource.AppendLine("\t\tnull, ; no vertex colors");

            // UV1

            resource.AppendFormat("\t\t; UV1: {0}\n", uvsTop.Length);
            resource.AppendFormat("\t\t{0},\n", Vector2fToArray(uvsTop.ToList()));

            // UV2

            resource.AppendFormat("\t\t; UV2: {0}\n", uvsBottom.Length);
            resource.AppendFormat("\t\t{0},\n", Vector2fToArray(uvsBottom.ToList()));

            // no bones

            resource.AppendLine("\t\tnull, ; no bone indices");
            resource.AppendLine("\t\tnull, ; no bone weights");

            // face indices


            resource.AppendFormat("\t\t; triangle faces: {0}\n", triangles.Count);
            resource.AppendFormat("\t\t{0}\n", TriangleIndices(triangles.ToArray()));

            resource.AppendLine("\t],"); // end of mesh arrays
            resource.AppendLine("\t\"morph_arrays\":[]");
            resource.AppendLine("}"); // end of surface/0

            nodes.AppendLine($"\n[node name=\"Tile_{name}\" type=\"MeshInstance\" parent=\".:\"]");
            nodes.AppendLine($"mesh = SubResource({idx})");
            nodes.AppendLine("visible = true");
            nodes.AppendLine("transform = Transform(1, 0, 0, 0, -1, 0.00000724, 0, -0.00000724, -1, 0, 0, 0)\n"); // X 180
            //nodes.AppendLine("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)\n");

            //GodotTransform transform = GodotTransform.IDENTITY;
            //transform = transform.Rotated(new GodotVector3(1f, 0f, 0f), 2 * 1.5708f);
            //Console.WriteLine($"Transform: {transform}");

            try
            {
                StreamWriter fileStream = new StreamWriter(export_file_name);

                scene.AppendLine(Resources);

                scene.AppendLine("; scene root node");
                scene.AppendLine("[node type=\"Spatial\" name=\"MapRoot\"]");
                scene.AppendLine("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)\n");

                scene.AppendLine(Nodes);

                fileStream.WriteLine(scene);
                fileStream.Close();
                return true;
            }
            catch (Exception x)
            {
                log.Error(x.Message);
                return false;
            }
        }

    }
}
