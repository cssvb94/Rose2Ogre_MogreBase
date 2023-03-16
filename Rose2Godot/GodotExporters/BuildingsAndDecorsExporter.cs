using Godot;
using Revise.ZMS;
using Revise.ZSC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rose2Godot.GodotExporters
{
    public class BuildingsAndDecorsExporter
    {
        private ModelListFile zsc;
        private readonly string zsc_file_name;
        public StringBuilder scene { get; private set; }
        public readonly string objName;

        public BuildingsAndDecorsExporter(string objectName, string zsc_file_name)
        {
            objName = objectName;
            this.zsc_file_name = zsc_file_name;
        }

        private void ExportMeshes(string output_folder_path)
        {
            foreach (string model_path in zsc.ModelFiles.Select(p => Translator.FixPath(p)))
            {
                string model_name = Path.GetFileNameWithoutExtension(model_path);
                string output_file_name_path = $"{Path.Combine(output_folder_path, model_name)}.tscn";
                SceneExporter sceneExporter = new SceneExporter(model_name, model_path);
                sceneExporter.ExportScene(output_file_name_path);
            }
        }

        public bool ExportScene(string output_file_name)
        {
            int resource_index = 1;

            string output_folder_path = Path.GetFullPath(output_file_name);
            Console.WriteLine($"Exporting assets to: \"{output_folder_path}\"");

            zsc = new ModelListFile();
            try
            {
                zsc.Load(zsc_file_name);
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                throw;
            }
            scene = new StringBuilder();
            StringBuilder godot_resources = new StringBuilder();
            StringBuilder godot_nodes = new StringBuilder();

            StreamWriter fileStream = new StreamWriter(output_file_name);

            int num_resources = zsc.ModelFiles.Count;
            scene.AppendFormat("[gd_scene load_steps={0} format=2]\n\n", num_resources);

            try
            {
                for (int obj_idx = 0; obj_idx < zsc.Objects.Count; obj_idx++)
                {
                    ModelListObject obj = zsc.Objects[obj_idx];
                    if (obj.Parts.Count == 0) continue;
                    //Console.WriteLine($"  Object: {obj_idx} Parts: {obj.Parts.Count}");
                    List<GodotTransform> model_transforms = new List<GodotTransform>();

                    foreach (ModelListPart part in obj.Parts)
                    {
                        GodotTransform part_transform = new GodotTransform(Translator.Rose2GodotRotation(part.Rotation), new GodotVector3(part.Position.Z/10f, part.Position.Y/100f, part.Position.X/10f));
                        Console.WriteLine($"{part.Position} \"{zsc.ModelFiles[part.Model]}\"");
                        model_transforms.Add(part_transform);
                        //string texture_path = zsc.TextureFiles[part.Texture].FilePath;
                        //string model_path = zsc.ModelFiles[part.Model];
                        //Console.WriteLine($"    Model ID: {part.Model} Position: {part.Position} Model: \"{model_path}\" Texture: \"{texture_path}\"");
                    }
                    List<ModelFile> model_parts = new List<ModelFile>();
                    List<string> model_names = new List<string>();

                    int path_dx = 0;
                    foreach (string model_part_path in obj.Parts.Select(part => zsc.ModelFiles[part.Model]))
                    {
                        ModelFile zms_file = new ModelFile();
                        model_names.Add($"{Path.GetFileNameWithoutExtension(model_part_path)}_{obj_idx}{path_dx++}");
                        zms_file.Load(model_part_path);
                        model_parts.Add(zms_file);
                    }

                    MeshExporter meshExporter = new MeshExporter(resource_index, model_parts, model_names, false, model_transforms);

                    godot_resources.Append(meshExporter.Resources);
                    godot_nodes.Append(meshExporter.Nodes);

                    resource_index = meshExporter.LastResourceIndex;
                }

                scene.AppendLine(godot_resources.ToString());

                scene.AppendLine("; scene root node");
                scene.AppendFormat("[node type=\"Spatial\" name=\"{0}\"]\n", objName);
                scene.AppendLine("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)");

                scene.AppendLine(godot_nodes.ToString());
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                throw;
            }

            fileStream.WriteLine(scene.ToString());
            fileStream.Close();
            return true;
        }
    }
}
