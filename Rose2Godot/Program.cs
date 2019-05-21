﻿using Rose2Godot.GodotExporters;
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
            // avatar
            //ProcessFileList(new List<string>() {
            //    "Example\\MALE.ZMD",
            //    "Example\\BODY1_00110.ZMS",
            //    "Example\\ARM1_00100.ZMS",
            //    "Example\\FACE1_00100.ZMS",
            //    "Example\\HAIR01_00500.ZMS",
            //    "Example\\FOOT1_00200.ZMS",
            //});
            //sceneExporter = new SceneExporter("Avatar", zms, zmd, zmo);
            //sceneExporter.ExportScene(@"E:\Godot\Projects\ROSE\scenes\avatar.escn");

            //larva
            ProcessFileList(new List<string>() {
                @"D:\Projects\3D\ROSE\LARVA1_BONE.ZMD",
                @"D:\Projects\3D\ROSE\LARVA1.ZMS",
                @"D:\Projects\3D\ROSE\LARVA_ATTACK.ZMO",
                @"D:\Projects\3D\ROSE\LARVA_WALK.ZMO",
                @"D:\Projects\3D\ROSE\LARVA_WARNING.ZMO",
            });
            sceneExporter = new SceneExporter("Larva", zms, zmd, zmo);
            sceneExporter.ExportScene(@"E:\Godot\Projects\ROSE\scenes\larva_exported.escn", ZMOFiles, ZMSFiles);

            // exported
            //ProcessFileList(new List<string>() {
            //    @"D:\Projects\3D\EXPORTED_BONE.ZMD",
            //    @"D:\Projects\3D\EXPORTED.ZMS",
            //});
            //sceneExporter = new SceneExporter("Larva", zms, zmd, zmo);
            //sceneExporter.ExportScene(@"E:\Godot\Projects\ROSE\scenes\exported_exported.escn");

            // akines
            //ProcessFileList(new List<string>() {
            //    @"D:\Projects\3D\AKINES_BODY01.ZMS",
            //    @"D:\Projects\3D\AKINES_HEAD01.ZMS",
            //    @"D:\Projects\3D\AKINES_BONE.ZMD",
            //});
            //sceneExporter = new SceneExporter("Akines", zms, zmd, zmo);
            //sceneExporter.ExportScene(@"C:\Users\Irix\Downloads\Godot\Projects\RoseImport\scenes\akines.escn");

            //ProcessFileList(new List<string>()
            //{
            //    @"/home/user/Projects/Rose/3DDATA/Cowboy/BODY01.ZMS",
            //    @"/home/user/Projects/Rose/3DDATA/Cowboy/HEAD01.ZMS",
            //    @"/home/user/Projects/Rose/3DDATA/Cowboy/COWBOY_BONE.ZMD",

            //});
            //sceneExporter = new SceneExporter("Cowboy", zms, zmd, zmo);
            //sceneExporter.ExportScene(@"/home/user/Downloads/Godot/Projects/Rose/scenes/cowboy.escn");

#else
            ProcessFileList(args.ToList());
            sceneExporter = new SceneExporter("Mesh", zms, zmd, zmo);
            sceneExporter.ExportScene(@"export.escn");
#endif

            // The [.escn] file format is identical to the TSCN file format, but is used to indicate to Godot that the file has been exported from another program 
            // and should not be edited by the user from within Godot.            

        }
    }
}
