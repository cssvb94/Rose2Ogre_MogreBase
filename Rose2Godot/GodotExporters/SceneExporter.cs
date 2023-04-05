using Godot;
using Revise.ZMD;
using Revise.ZMO;
using Revise.ZMS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rose2Godot.GodotExporters
{
    public class SceneExporter
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetLogger("SceneExporter");
        public StringBuilder scene { get; private set; }
        public int num_resources { get; private set; }
        public List<ModelFile> zms { get; private set; }
        public BoneFile zmd { get; private set; }
        public List<MotionFile> zmo { get; private set; }
        public string objName { get; private set; }
        public List<string> nodes { get; private set; }
        public List<string> resources { get; private set; }

        private string ZMDFile;
        private List<string> ZMSFiles;
        private List<string> ZMOFiles;
        private readonly string GodotMaterialFile;

        private void ProcessFileList(List<string> FileNameList)
        {
            ZMSFiles = new List<string>();
            ZMOFiles = new List<string>();

            zms = new List<ModelFile>();
            zmo = new List<MotionFile>();
            zmd = new BoneFile();

            // Get only files that exist
            var existing = from f in FileNameList where File.Exists(f) select f;

            // Separate by types
            IEnumerable<string> zmss = from f in existing where Path.GetExtension(f).IndexOf("ZMS", StringComparison.OrdinalIgnoreCase) >= 0 select Translator.FixPath(f);
            IEnumerable<string> zmos = from f in existing where Path.GetExtension(f).IndexOf("ZMO", StringComparison.OrdinalIgnoreCase) >= 0 select Translator.FixPath(f);
            IEnumerable<string> zmds = from f in existing where Path.GetExtension(f).IndexOf("ZMD", StringComparison.OrdinalIgnoreCase) >= 0 select Translator.FixPath(f);

            // Fill in the listboxes and lists
            try
            {
                if (zmds.Any())
                {
                    ZMDFile = zmds.First();
                    //log.Info($"Loading ZMD: {ZMDFile}");
                    zmd.Load(ZMDFile);
                }

                if (zmss.Any())
                {
                    ZMSFiles.AddRange(zmss.ToList());
                    ZMSFiles = ZMSFiles.Distinct().ToList();
                    foreach (string zms_filename in ZMSFiles)
                    {
                        //log.Info($"Loading ZMS: {zms_filename}");
                        ModelFile zms_file = new ModelFile();
                        zms_file.Load(zms_filename);
                        zms.Add(zms_file);
                    }
                }

                if (zmos.Any() && zmds.Any())
                {
                    ZMOFiles.AddRange(zmos.ToList());
                    ZMOFiles = ZMOFiles.Distinct().ToList();
                    foreach (string zmo_filename in ZMOFiles)
                    {
                        //log.Info($"Loading ZMO: {zmo_filename}");
                        MotionFile zmo_file = new MotionFile();
                        zmo_file.Load(zmo_filename);
                        zmo.Add(zmo_file);
                    }
                }
            }
            catch (Exception x)
            {
                log.Error(x);
                throw;
            }
        }

        public SceneExporter(string objectName, string model_file_path, string godot_material_file = "")
        {
            objName = objectName;
            ProcessFileList(new List<string>() { model_file_path });
            scene = new StringBuilder();
            nodes = new List<string>();
            resources = new List<string>();
            GodotMaterialFile = godot_material_file;

            // should include num of external objects
            //num_resources = (uint)(zms.Count + zms.Count + zmo.Count);
            num_resources = zms.Count;
            scene.AppendFormat("[gd_scene load_steps={0} format=2]\n", num_resources);

            if (!string.IsNullOrWhiteSpace(GodotMaterialFile))
                scene.AppendLine($"\n[ext_resource path=\"{GodotMaterialFile}\" type=\"Texture\" id=1]");
        }

        public SceneExporter(string objectName, List<string> file_paths)
        {
            objName = objectName;
            ProcessFileList(file_paths);
            scene = new StringBuilder();
            nodes = new List<string>();
            resources = new List<string>();

            // should include num of external objects
            //num_resources = (uint)(zms.Count + zms.Count + zmo.Count);
            num_resources = zms.Count;
            scene.AppendFormat("[gd_scene load_steps={0} format=2]\n", num_resources);
        }

        public bool ExportScene(string output_file_name, List<GodotTransform> transforms = null)
        {
            int resource_index = 1;
            try
            {
                StreamWriter fileStream = new StreamWriter(output_file_name);

                List<string> model_name = new List<string>();

                foreach (string model_file_name in ZMSFiles)
                    model_name.Add(Path.GetFileNameWithoutExtension(model_file_name));

                int external_material_id = string.IsNullOrWhiteSpace(GodotMaterialFile) ? -1 : 1;

                MeshExporter meshExporter = new MeshExporter(resource_index, zms, model_name, zmd.Bones.Any(), transforms, external_material_id);

                resource_index = meshExporter.LastResourceIndex;

                scene.AppendLine(meshExporter.Resources);

                AnimationExporter animExporter = new AnimationExporter(resource_index, zmo, zmd);

                // meshes & bone weights

                resource_index = animExporter.last_resource_index;

                // animations
                // normalize the rotation quats!

                if (zmo.Any())
                {
                    scene.AppendLine(animExporter.Resources);
                    resource_index = animExporter.last_resource_index;
                }

                scene.AppendLine("; scene root node");
                scene.AppendFormat("[node type=\"Spatial\" name=\"{0}\"]\n", objName);
                scene.AppendLine("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)\n");

                // skeleton

                if (zmd.Bones.Any())
                {
                    BoneExporter boneExporter = new BoneExporter(resource_index, zmd);
                    scene.AppendLine(boneExporter.ToString());
                    resource_index = animExporter.last_resource_index;
                }

                scene.AppendLine(meshExporter.Nodes);

                if (zmo.Any() && zmd.Bones.Any())
                    scene.AppendLine(animExporter.Nodes);

                fileStream.WriteLine(scene);
                fileStream.Close();

                return true;
            }
            catch (Exception x)
            {
                log.Error(x);
                throw;
            }
        }
    }
}