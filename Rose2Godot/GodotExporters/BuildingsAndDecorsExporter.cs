using Godot;
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
        private static readonly NLog.Logger log = NLog.LogManager.GetLogger("BuildingsAndDecorsExporter");
        private ModelListFile zsc;
        private readonly string zsc_file_name;
        private StringBuilder scene;
        private StringBuilder resources;
        private StringBuilder nodes;
        public readonly string objName;

        public BuildingsAndDecorsExporter(string objectName, string zsc_file_name)
        {
            objName = objectName;
            this.zsc_file_name = zsc_file_name;
            resources = new StringBuilder();
            nodes = new StringBuilder();
        }

        private void ExportMeshes(string output_folder_path)
        {
            int mesh_idx = 0;
            foreach (string model_path in zsc.ModelFiles.Select(p => Translator.FixPath(p)))
            {
                string model_name = Path.GetFileNameWithoutExtension(model_path);
                string output_file_name_path = $"{Path.Combine(output_folder_path, model_name)}.tscn";
                SceneExporter sceneExporter = new SceneExporter(model_name, model_path);
                sceneExporter.ExportScene(output_file_name_path);
                AddExternalResource(Path.GetFileName(output_file_name_path), mesh_idx + 1);
                mesh_idx++;
            }
        }

        public void ExportMaterialFile(string output_file_path, string content)
        {
            StreamWriter fileStream = new StreamWriter(output_file_path);
            fileStream.Write(content);
            fileStream.Close();
        }

        private void ExportMaterials(string output_folder_path, int start_idx)
        {
            StringBuilder material_content = new StringBuilder();
            for (int texture_id = 1; texture_id < zsc.TextureFiles.Count; texture_id++)
            {
                material_content.Clear();
                TextureFile texture_file = zsc.TextureFiles[texture_id];
                string texture_path = Translator.FixPath(texture_file.FilePath);
                string texture_name = Path.GetFileNameWithoutExtension(texture_path);
                string output_file_name_path = $"{Path.Combine(output_folder_path, texture_name)}_MAT.tres";

                AddExternalMaterial(Path.GetFileName(output_file_name_path), start_idx);
                material_content.AppendLine("[gd_resource type=\"SpatialMaterial\" load_steps=2 format=2]\n");
                material_content.AppendLine($"[ext_resource path=\"{texture_path}\" type=\"Texture\" id=1]\n");
                material_content.AppendLine("[resource]");
                material_content.AppendLine("flags_unshaded = true");
                material_content.AppendLine("albedo_texture = ExtResource( 1 )");
                ExportMaterialFile(output_file_name_path, material_content.ToString());
                start_idx++;
            }
        }

        private void AddExternalMaterial(string resource_path, int idx)
        {
            resources.AppendLine($"[ext_resource path=\"{resource_path}\" type=\"Material\" id={idx}]\n");
        }

        private void AddExternalResource(string resource_path, int idx)
        {
            resources.AppendLine($"[ext_resource path=\"{resource_path}\" type=\"PackedScene\" id={idx}]\n");
        }

        private void AddExternalNode(string node_name, string node_parent_name, int instance_idx, GodotTransform transform)
        {
            nodes.AppendLine($"[node name=\"{node_name}\" parent=\"{node_parent_name}\" instance=ExtResource( {instance_idx} )]");
            nodes.AppendLine($"transform = {Translator.GodotTransform2String(transform)}\n");
        }

        public bool ExportScene(string output_file_name)
        {
            int resource_index = 1;

            string output_folder_path = Path.GetDirectoryName(output_file_name);
            log.Info($"Exporting assets to: \"{output_folder_path}\"");


            zsc = new ModelListFile();
            try
            {
                zsc.Load(zsc_file_name);
            }
            catch (Exception x)
            {
                log.Error(x.Message);
                throw;
            }

            ExportMeshes(output_folder_path);

            //ExportMaterials(output_folder_path, zsc.ModelFiles.Count + 1);

            ///////////////////////////////////////////

            scene = new StringBuilder();
            StringBuilder godot_resources = new StringBuilder();
            StringBuilder godot_nodes = new StringBuilder();

            StreamWriter fileStream = new StreamWriter(output_file_name);

            int num_resources = zsc.ModelFiles.Count; // + zsc.TextureFiles.Count;
            scene.AppendFormat("[gd_scene load_steps={0} format=2]\n\n", num_resources);
            try
            {
                for (int obj_idx = 0; obj_idx < zsc.Objects.Count; obj_idx++)
                {
                    ModelListObject obj = zsc.Objects[obj_idx];
                    if (obj.Parts.Count == 0) continue;
                    List<GodotTransform> model_transforms = new List<GodotTransform>();

                    foreach (ModelListPart part in obj.Parts)
                    {
                        GodotTransform part_transform = new GodotTransform(Translator.Rose2GodotRotation(part.Rotation), new GodotVector3(part.Position.Z / 10f, part.Position.Y / 100f, part.Position.X / 10f));
                        log.Info($"{part.Position} \"{zsc.ModelFiles[part.Model]}\"");
                        model_transforms.Add(part_transform);

                        string parent_name = ".";
                        if (part.Parent >= 0)
                            parent_name = Path.GetFileNameWithoutExtension(zsc.ModelFiles[part.Parent]);

                        string model_name = Path.GetFileNameWithoutExtension(zsc.ModelFiles[part.Model]);

                        if (model_name.Equals(parent_name, StringComparison.InvariantCultureIgnoreCase))
                            parent_name = ".";

                        AddExternalNode(model_name, parent_name, resource_index, part_transform);
                        resource_index++;
                    }
                }
                godot_resources.Append(resources.ToString());
                godot_nodes.Append(nodes.ToString());

                scene.AppendLine(godot_resources.ToString());

                scene.AppendLine("; scene root node");
                scene.AppendLine($"[node type=\"Spatial\" name=\"{objName}\"]");
                scene.AppendLine("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)\n");

                scene.AppendLine(godot_nodes.ToString());
            }
            catch (Exception x)
            {
                log.Error(x.Message);
                throw;
            }

            fileStream.WriteLine(scene.ToString());
            fileStream.Close();
            return true;
        }
    }
}
