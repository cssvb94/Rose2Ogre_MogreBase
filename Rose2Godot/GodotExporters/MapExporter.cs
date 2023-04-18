using Revise.STB;
using Revise.TIL;
using Rose2Godot.GodotExporters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Rose2Godot
{
    public class MapExporter
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetLogger("MapExporter");
        private readonly int zone_id;
        private readonly DataFile stb_file_zone;
        private Dictionary<string, Rect> atlasRectHash;
        private Dictionary<string, Bitmap> atlasTexHash;
        private List<Bitmap> textures;

        public RoseMap Map { get; private set; }
        public MapExporter(int ZoneId, string GodotProjectPah)
        {
            zone_id = ZoneId;
            stb_file_zone = new DataFile();
            try
            {
                stb_file_zone.Load("3DDATA/STB/LIST_ZONE.STB");
            }
            catch (Exception x)
            {
                log.Error(x.Message);
                throw;
            }

            List<string> cell = stb_file_zone.RowsData(zone_id);
            MapType type;
            string type_cell = cell[5];
            switch (type_cell)
            {
                case "0":
                    type = MapType.Outdoor;
                    break;
                case "1":
                    type = MapType.Underground;
                    break;
                case "2":
                    type = MapType.GameArena;
                    break;
                default:
                    type = MapType.Outdoor;
                    break;
            }

            Map = new RoseMap()
            {
                ShortName = cell[0],
                LongName = cell[1],
                ZONPath = Translator.FixPath(cell[2]),
                Type = type,
                BackgroundMusicMidday = Translator.FixPath(cell[6]),
                BackgroundMusicNigth = Translator.FixPath(cell[7]),
                MiniMap = Translator.FixPath(cell[9]),
                ZoneMinimapStartX = uint.Parse(cell[10]),
                ZoneMinimapStartY = uint.Parse(cell[11]),
                ObjectsTable = Translator.FixPath(cell[12]),
                BuildingsTable = Translator.FixPath(cell[13]),
                MapSize = uint.Parse(cell[26]),
                STLId = cell[27],
            };

            Map.GodotScenePath = $"{Path.Combine(GodotProjectPah, cell[27])}";
            Directory.CreateDirectory(Map.GodotScenePath);

            Map.GenerateMapData();

            // **********************************************************
            atlasRectHash = new Dictionary<string, Rect>();
            atlasTexHash = new Dictionary<string, Bitmap>();
            textures = new List<Bitmap>();

            TileFile til = new TileFile();
            try
            {
                til.Load(@"3DDATA/MAPS/JUNON/JDT01/31_30.TIL");
                log.Info($"TIL: Width: {til.Width} Height: {til.Height}  \"{til.FilePath}\"");
            }
            catch (Exception x)
            {
                log.Error(x);
                throw;
            }
            Map.UpdateAtlas(til, ref atlasRectHash, ref atlasTexHash, ref textures);

            // Figure out the required size of the atlas from the number of textures in the atlas
            int height, width;  // these must be powers of 2 to be compatible with iPhone
            if (atlasRectHash.Count <= 16) width = height = 4 * 256;
            else if (atlasRectHash.Count <= 32) { width = 8 * 256; height = 4 * 256; }
            else if (atlasRectHash.Count <= 64) { width = 8 * 256; height = 8 * 256; }
            else if (atlasRectHash.Count <= 128) { width = 16 * 256; height = 8 * 256; }
            else if (atlasRectHash.Count <= 256) { width = 16 * 256; height = 16 * 256; }
            else throw new Exception("Number of tiles in terrain is larger than supported by terrain atlas");

            log.Info($"New atlas texure [{width}x{height}] px");

            //Bitmap atlas = TextureAtlasser.MakeAtlas(ref textures, out Rect[] rects, 0);
            //atlas.Save(@"C:\Applications\Godot\GodotProjects\ImportTest\scenes\LZON001\ATLAS.PNG", System.Drawing.Imaging.ImageFormat.Png);

            Map.GenerateTileAtlas(til, 0, 0);
        }
    }
}
