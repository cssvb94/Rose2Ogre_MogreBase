using NLog;
using Rose2Godot.GodotExporters;
using System;
using System.Collections.Generic;

namespace Rose2Godot
{
    internal static class Program
    {
        private static readonly Logger log = LogManager.GetLogger("App");
        private static SceneExporter sceneExporter;

        [STAThread]
        private static void Main(string[] args)
        {
            log.Info("App start");
#if DEBUG

            // 1   - Zant
            // 2   - Junon
            // 3   - Dolphin island
            // 10  - Adventure plains (EVO)
            // 20  - Birth Island
            // 21  - Valley of Luxem Tower
            // 22  - Adventurers Plains
            // 23  - Breezy Hills
            // 24  - El Verloon Desert
            // 25  - Anima Lake
            // 26  - Forest of Wisdom
            // 27  - Kenji Beach
            // 28  - Gorge of Silence
            // 51  - Magic City of the Eucar
            // 61  - Xita Refuge
            // 62  - Shady Jungle
            // 63  - Forest of Wandering
            // 64  - Marsh of Ghosts
  
            string godot_project_path = string.Empty;

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                godot_project_path = @"C:\Applications\Godot\GodotProjects\ImportTest\scenes";
            }
            else
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                godot_project_path = "/home/user/Projects/GodotProjects/TestAssImp/scenes";
            }

            MapExporter mapExporter = new MapExporter(23, godot_project_path);

/*
            // Avatar

            sceneExporter = new SceneExporter("Avatar_male", new List<string>()
                        {
                            @"3DDATA\AVATAR\ARMS\ARM1_00100.ZMS",
                            @"3DDATA\AVATAR\BODY\BODY1_00300.ZMS",
                            @"3DDATA\AVATAR\BODY\BODY1_00310.ZMS",
                            @"3DDATA\AVATAR\FOOT\FOOT1_00300.ZMS",
                       
                            //@"3DDATA\AVATAR\FACE\FACE1_00200.ZMS",
                            //@"3DDATA\AVATAR\HAIR\HAIR01_00100.ZMS",
                            @"3DDATA\AVATAR\MALE.ZMD",
                            @"3DDATA\MOTION\AVATAR\EMPTY_RUN_M1.ZMO",
                            @"3DDATA\MOTION\AVATAR\EMPTY_WALK_M1.ZMO",
                            @"3DDATA\MOTION\AVATAR\EMPTY_STOP1_M1.ZMO",
                            @"3DDATA\MOTION\AVATAR\EMPTY_STOP2_M1.ZMO",
                            @"3DDATA\MOTION\AVATAR\SKILL_MAGIC01_M1.ZMO",
                            @"3DDATA\MOTION\AVATAR\SKILL_UPPERCUT_01.ZMO",
                        });
            sceneExporter.ExportScene(@"C:\Applications\Godot\GodotProjects\ImportTest\scenes\Avatar_male.tscn");

            sceneExporter = new SceneExporter("Avatar_female", new List<string>()
                        {
                            @"3DDATA\AVATAR\ARMS\ARM2_00100.ZMS",
                            @"3DDATA\AVATAR\BODY\BODY2_00100.ZMS",
                            @"3DDATA\AVATAR\BODY\BODY2_00110.ZMS",
                            @"3DDATA\AVATAR\FOOT\FOOT2_05700.ZMS",
                            @"3DDATA\AVATAR\FOOT\FOOT2_05710.ZMS",
                            //@"3DDATA\AVATAR\FACE\FACE2_00800.ZMS",
                            //@"3DDATA\AVATAR\HAIR\HAIR02_00200.ZMS",
                            @"3DDATA\AVATAR\FEMALE.ZMD",
                            @"3DDATA\MOTION\AVATAR\EMPTY_RUN_F1.ZMO",
                            @"3DDATA\MOTION\AVATAR\EMPTY_WALK_F1.ZMO",
                            @"3DDATA\MOTION\AVATAR\EMPTY_STOP1_F1.ZMO",
                            @"3DDATA\MOTION\AVATAR\EMPTY_STOP2_F1.ZMO",
                        });
            sceneExporter.ExportScene(@"C:\Applications\Godot\GodotProjects\ImportTest\scenes\Avatar_female.tscn");
*/

            /*
            

            // Akines

            ZMSFiles.Clear();
            ZMOFiles.Clear();
            zms.Clear();
            zmo.Clear();
            zmd.Clear();
            ProcessFileList(new List<string>()
                        {
                            @"3DDATA\NPC\NPC\AKINES\HEAD01.ZMS",
                            @"3DDATA\NPC\NPC\AKINES\BODY01.ZMS",
                            @"3DDATA\NPC\NPC\AKINES\AKINES_BONE.ZMD",
                            @"3DDATA\MOTION\NPC\AKINES\AKINES_STOP1.ZMO",
                        });
            sceneExporter = new SceneExporter("Akines", zms, zmd, zmo);
            sceneExporter.ExportScene(@"C:\Applications\Godot\GodotProjects\ImportTest\scenes\Akines.tscn", ZMSFiles);
            */

            // Dragon

            //ZMSFiles.Clear();
            //ZMOFiles.Clear();
            //zms.Clear();
            //zmo.Clear();
            //zmd.Clear();
            //ProcessFileList(new List<string>() {
            //                @"3DDATA\NPC\ANIMAL\DRAGON01\DRAGON01_BONE.ZMD",
            //                @"3DDATA\NPC\ANIMAL\DRAGON01\BODY01.ZMS",
            //                @"3DDATA\NPC\ANIMAL\DRAGON01\BODY01_1.ZMS",
            //                @"3DDATA\NPC\ANIMAL\DRAGON01\BODY02.ZMS",
            //                @"3DDATA\NPC\ANIMAL\DRAGON01\BODY03.ZMS",
            //                @"3DDATA\NPC\ANIMAL\DRAGON01\BODY03_1.ZMS",
            //                @"3DDATA\MOTION\NPC\DRAGON01\DIE_01.ZMO",
            //                @"3DDATA\MOTION\NPC\DRAGON01\HIT_01.ZMO",
            //                @"3DDATA\MOTION\NPC\DRAGON01\RUN_01.ZMO",
            //                @"3DDATA\MOTION\NPC\DRAGON01\SKILL_A_01.ZMO",
            //                @"3DDATA\MOTION\NPC\DRAGON01\SKILL_S_01.ZMO",
            //                @"3DDATA\MOTION\NPC\DRAGON01\WALK_01.ZMO",
            //                @"3DDATA\MOTION\NPC\DRAGON01\WARNING_01.ZMO",
            //                @"3DDATA\MOTION\NPC\DRAGON01\ATTACK_01.ZMO",
            //                @"3DDATA\MOTION\NPC\DRAGON01\CASTING_01.ZMO",
            //            });
            //sceneExporter = new SceneExporter("Dragon", zms, zmd, zmo);
            //sceneExporter.ExportScene(@"C:\Applications\Godot\GodotProjects\ImportTest\scenes\Dragon.tscn", ZMSFiles);

            // Penguin

            //ZMSFiles.Clear();
            //ZMOFiles.Clear();
            //zms.Clear();
            //zmo.Clear();
            //zmd.Clear();
            //ProcessFileList(new List<string>() {
            //                @"3DDATA\NPC\ANIMAL\PENGUIN07\PENGUIN07_BONE.ZMD",
            //                @"3DDATA\NPC\ANIMAL\PENGUIN07\BODY07.ZMS",
            //                @"3DDATA\NPC\ANIMAL\PENGUIN07\HEAD07.ZMS",
            //                @"3DDATA\MOTION\NPC\PENGUIN07\PENGUIN07_WALK.ZMO",
            //                @"3DDATA\MOTION\NPC\PENGUIN07\PENGUIN07_RUN.ZMO",
            //                @"3DDATA\MOTION\NPC\PENGUIN07\PENGUIN07_WARNING.ZMO",
            //                @"3DDATA\MOTION\NPC\PENGUIN07\PENGUIN07_ACTION_SKILL01.ZMO",
            //                @"3DDATA\MOTION\NPC\PENGUIN07\PENGUIN07_STATUS_SKILL01.ZMO",
            //                @"3DDATA\MOTION\NPC\PENGUIN07\PENGUIN07_ATTACK.ZMO",
            //                @"3DDATA\MOTION\NPC\PENGUIN07\PENGUIN07_HIT.ZMO",
            //            });
            //sceneExporter = new SceneExporter("PENGUIN", zms, zmd, zmo);
            //sceneExporter.ExportScene(@"C:\Applications\Godot\GodotProjects\ImportTest\scenes\PENGUIN.tscn", ZMSFiles);


            // Larva

            //ZMSFiles.Clear();
            //ZMOFiles.Clear();
            //zms.Clear();
            //zmo.Clear();
            //zmd.Clear();
            //ProcessFileList(new List<string>() {
            //                @"3DDATA\NPC\ANIMAL\LARVA\LARVA1.ZMS",
            //                @"3DDATA\NPC\ANIMAL\LARVA\LARVA1_BONE.ZMD",
            //                @"3DDATA\MOTION\NPC\LARVA\LARVA_WALK.ZMO",
            //                @"3DDATA\MOTION\NPC\LARVA\LARVA_ATTACK.ZMO",
            //                @"3DDATA\MOTION\NPC\LARVA\LARVA_WARNING.ZMO",
            //            });
            //sceneExporter = new SceneExporter("Larva", zms, zmd, zmo);
            //sceneExporter.ExportScene(@"C:\Applications\Godot\GodotProjects\ImportTest\scenes\Larva.tscn", ZMSFiles);

#else
            ProcessFileList(args.ToList());
            sceneExporter = new SceneExporter("Mesh", zms, zmd, zmo, ZMSFiles);
            sceneExporter.ExportScene(@"export.tscn");
#endif
        }
    }
}
