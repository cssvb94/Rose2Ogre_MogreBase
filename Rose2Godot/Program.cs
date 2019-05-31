using Rose2Godot.GodotExporters;
using RoseFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rose2Godot
{
    internal static class Program
    {

        private static string ZMDFile;
        private static List<string> ZMSFiles = new List<string>();
        private static List<string> ZMOFiles = new List<string>();
        private static readonly ZMD zmd = new ZMD();
        private static readonly List<ZMS> zms = new List<ZMS>();
        private static readonly List<ZMO> zmo = new List<ZMO>();

        private static SceneExporter sceneExporter;

        private static void ProcessFileList(List<string> FileNameList)
        {
            // Get only files that exist
            var existing = from f in FileNameList where File.Exists(f) select f;
            // Separate by types
            IEnumerable<string> zmss = from f in existing where Path.GetExtension(f).IndexOf("ZMS", StringComparison.OrdinalIgnoreCase) >= 0 select f;
            IEnumerable<string> zmos = from f in existing where Path.GetExtension(f).IndexOf("ZMO", StringComparison.OrdinalIgnoreCase) >= 0 select f;
            IEnumerable<string> zmds = from f in existing where Path.GetExtension(f).IndexOf("ZMD", StringComparison.OrdinalIgnoreCase) >= 0 select f;

            // Fill in the listboxes and lists

            if (zmds.Any())
            {
                ZMDFile = zmds.ToArray()[0];
                Console.WriteLine("Loading ZMD: {0}", ZMDFile);
                zmd.Load(ZMDFile);
            }

            if (zmss.Any())
            {
                ZMSFiles.AddRange(zmss.ToList());
                ZMSFiles = ZMSFiles.Distinct().ToList();
                foreach (string zms_filename in ZMSFiles)
                {
                    Console.WriteLine("Loading ZMS: {0}", zms_filename);
                    zms.Add(new ZMS(zms_filename));
                }
            }

            if (zmos.Any() && zmds.Any())
            {
                ZMOFiles.AddRange(zmos.ToList());
                ZMOFiles = ZMOFiles.Distinct().ToList();
                foreach (string zmo_filename in ZMOFiles)
                {
                    Console.WriteLine("Loading ZMO: {0}", zmo_filename);
                    zmo.Add(new ZMO(zmo_filename, zmd));
                }
            }
        }

        [STAThread]
        private static void Main(string[] args)
        {
#if DEBUG

            //larva
            //ZMSFiles.Clear();
            //zms.Clear();
            //zmo.Clear();
            //zmd.Clear();
            //ProcessFileList(new List<string>() {
            //@"D:\Projects\3D\ROSE\PENGUIN07_BONE.ZMD",
            //@"D:\Projects\3D\ROSE\BODY07.ZMS",
            //@"D:\Projects\3D\ROSE\HEAD07.ZMS",
            //@"D:\Projects\3D\ROSE\PENGUIN07_WALK.ZMO",
            //@"D:\Projects\3D\ROSE\PENGUIN07_RUN.ZMO",
            //@"D:\Projects\3D\ROSE\PENGUIN07_WARNING.ZMO",
            //@"D:\Projects\3D\ROSE\PENGUIN07_STATUS_SKILL01.ZMO",
            //@"D:\Projects\3D\ROSE\PENGUIN07_HIT.ZMO",
            //@"D:\Projects\3D\ROSE\PENGUIN07_DIE.ZMO",
            //});
            //sceneExporter = new SceneExporter("PENGUIN", zms, zmd, zmo);
            //sceneExporter.ExportScene(@"E:\Godot\Projects\ROSE\scenes\PENGUIN.escn", ZMSFiles);

            //larva
            //ZMSFiles.Clear();
            //zms.Clear();
            //zmo.Clear();
            //zmd.Clear();
            //ProcessFileList(new List<string>() {
            //@"D:\Projects\3D\ROSE\LARVA1_BONE.ZMD",
            //  @"D:\Projects\3D\ROSE\LARVA1.ZMS",
            //@"D:\Projects\3D\ROSE\LARVA_ATTACK.ZMO",
            // @"D:\Projects\3D\ROSE\LARVA_WALK.ZMO",
            //@"D:\Projects\3D\ROSE\LARVA_WARNING.ZMO",
            //});
            //sceneExporter = new SceneExporter("Larva", zms, zmd, zmo);
            //sceneExporter.ExportScene(@"E:\Godot\Projects\ROSE\scenes\larva_exported.escn", ZMSFiles);

            // exported
            //ZMSFiles.Clear();
            //zms.Clear();
            //zmo.Clear();
            //zmd.Clear();
            //ProcessFileList(new List<string>() {
            //    @"D:\Projects\3D\ROSE\EXPORTED_BONE.ZMD",
            //    @"D:\Projects\3D\ROSE\EXPORTED.ZMS",
            //});
            //sceneExporter = new SceneExporter("Exported", zms, zmd, zmo);
            //sceneExporter.ExportScene(@"E:\Godot\Projects\ROSE\scenes\exported_exported.escn", ZMSFiles);

            // akines
            //ZMSFiles.Clear();
            //zms.Clear();
            //zmo.Clear();
            //zmd.Clear();
            //ProcessFileList(new List<string>() {
            //    @"D:\Projects\3D\ROSE\AKINES_BODY01.ZMS",
            //    @"D:\Projects\3D\ROSE\AKINES_HEAD01.ZMS",
            //    @"D:\Projects\3D\ROSE\AKINES_BONE.ZMD",
            //    @"D:\Projects\3D\ROSE\AKINES_STOP1.ZMO",
            //});
            //sceneExporter = new SceneExporter("Akines", zms, zmd, zmo);
            //sceneExporter.ExportScene(@"E:\Godot\Projects\ROSE\scenes\akines.escn", ZMSFiles);

            //ZMSFiles.Clear();
            //zms.Clear();
            //zmo.Clear();
            //zmd.Clear();
            //ProcessFileList(new List<string>()
            //{
            //    @"/home/user/Projects/Rose/3DDATA/Cowboy/BODY01.ZMS",
            //    @"/home/user/Projects/Rose/3DDATA/Cowboy/HEAD01.ZMS",
            //    @"/home/user/Projects/Rose/3DDATA/Cowboy/COWBOY_BONE.ZMD",
            //});
            //sceneExporter = new SceneExporter("Cowboy", zms, zmd, zmo);
            //sceneExporter.ExportScene(@"/home/user/Downloads/Godot/Projects/Rose/scenes/Cowboy.escn", ZMSFiles);


            ZMSFiles.Clear();
            zms.Clear();
            zmo.Clear();
            zmd.Clear();
            ProcessFileList(new List<string>()
            {
                @"/home/user/Projects/Rose/3DDATA/Larva/LARVA1.ZMS",
                @"/home/user/Projects/Rose/3DDATA/Larva/LARVA1_BONE.ZMD",
                @"/home/user/Projects/Rose/3DDATA/Larva/LARVA_WALK.ZMO",
                @"/home/user/Projects/Rose/3DDATA/Larva/LARVA_WARNING.ZMO",
                @"/home/user/Projects/Rose/3DDATA/Larva/LARVA_ATTACK.ZMO",
            });
            sceneExporter = new SceneExporter("Larva", zms, zmd, zmo);
            sceneExporter.ExportScene(@"/home/user/Downloads/Godot/Projects/Rose/scenes/LarvaAnimated.escn", ZMSFiles);

            ZMSFiles.Clear();
            zms.Clear();
            zmo.Clear();
            zmd.Clear();
            ProcessFileList(new List<string>()
            {
                @"/home/user/Projects/Rose/3DDATA/Penguin/PENGUIN07_BONE.ZMD",
                @"/home/user/Projects/Rose/3DDATA/Penguin/BODY07.ZMS",
                @"/home/user/Projects/Rose/3DDATA/Penguin/HEAD07.ZMS",
                @"/home/user/Projects/Rose/3DDATA/Penguin/PENGUIN07_ACTION_SKILL01.ZMO",
                @"/home/user/Projects/Rose/3DDATA/Penguin/PENGUIN07_ATTACK.ZMO",
                @"/home/user/Projects/Rose/3DDATA/Penguin/PENGUIN07_RUN.ZMO",
                @"/home/user/Projects/Rose/3DDATA/Penguin/PENGUIN07_WALK.ZMO",
                @"/home/user/Projects/Rose/3DDATA/Penguin/PENGUIN07_WARNING.ZMO",

            });
            sceneExporter = new SceneExporter("Penguin", zms, zmd, zmo);
            sceneExporter.ExportScene(@"/home/user/Downloads/Godot/Projects/Rose/scenes/PenguinAnimated.escn", ZMSFiles);

#else
            ProcessFileList(args.ToList());
            sceneExporter = new SceneExporter("Mesh", zms, zmd, zmo, ZMSFiles);
            sceneExporter.ExportScene(@"export.escn");
#endif

            // The [.escn] file format is identical to the TSCN file format, but is used to indicate to Godot that the file has been exported from another program 
            // and should not be edited by the user from within Godot.            

        }
    }
}
