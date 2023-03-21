using Revise.STB;
using Rose2Godot.GodotExporters;
using System;
using System.Collections.Generic;

namespace Rose2Godot
{
    public class MapExporter
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetLogger("MapExporter");
        private readonly int zone_id;
        private readonly DataFile stb_file_zone;
        public RoseMap Map { get; private set; }
        public MapExporter(int ZoneId)
        {
            zone_id = ZoneId;
            stb_file_zone = new DataFile();
            try
            {
                stb_file_zone.Load("3DDATA/STB/LIST_ZONE.stb");
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

            Map.GenerateMapData();
        }
    }
}
