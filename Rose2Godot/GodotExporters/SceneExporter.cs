using RoseFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rose2Godot.GodotExporters
{
    public class SceneExporter
    {
        public readonly StringBuilder scene;
        public readonly int num_resources;
        public readonly List<ZMS> zms;
        public readonly ZMD zmd;
        public readonly List<ZMO> zmo;
        public readonly string objName;

        public readonly List<string> nodes;
        public readonly List<string> resources;

        public SceneExporter(
            string objectName,
            List<ZMS> mesh,
            ZMD skeleton,
            List<ZMO> animation)
        {
            scene = new StringBuilder();
            zms = mesh;
            zmd = skeleton;
            zmo = animation;
            objName = objectName;

            nodes = new List<string>();
            resources = new List<string>();

            // should include num of external objects
            //num_resources = (uint)(zms.Count + zms.Count + zmo.Count);
            num_resources = zms.Count;
            scene.AppendFormat("[gd_scene load_steps={0} format=2]\n", num_resources);
        }

        public bool ExportScene(string fileName, List<string> zmsFileNames)
        {
            int idx = 1;
            try
            {
                StreamWriter fileStream = new StreamWriter(fileName);

                //List<string> animationNames = new List<string>();

                //foreach (string fname in zmoFileNames)
                //{
                //    animationNames.Add(Path.GetFileNameWithoutExtension(fname));
                //}

                List<string> meshNames = new List<string>();

                foreach (string fname in zmsFileNames)
                {
                    meshNames.Add(Path.GetFileNameWithoutExtension(fname));
                }

                MeshExporter meshExporter = new MeshExporter(idx, zms, meshNames, zmd.BonesCount > 0);

                idx = meshExporter.LastResourceIndex;

                scene.AppendLine(meshExporter.Resources);

                AnimationExporter animExporter = new AnimationExporter(idx, zmo, zmd);

                // meshes & bone weights

                idx = animExporter.LastResourceIndex;


                // animations
                // normalize the rotation quats!

                if (zmo.Count > 0)
                {
                    scene.AppendLine(animExporter.Resources);

                    idx = animExporter.LastResourceIndex;
                }

                scene.AppendLine("; scene root node");
                scene.AppendFormat("[node type=\"Spatial\" name=\"{0}\"]\n", objName);
                scene.AppendLine("transform = Transform(1, 0, 0, 0, 0, 1, 0, -1, 0, 0, 0, 0)\n");

                // skeleton

                if (zmd.BonesCount > 0)
                {
                    BoneExporter boneExporter = new BoneExporter(idx, zmd);
                    scene.AppendLine(boneExporter.ToString());
                    idx = animExporter.LastResourceIndex;
                }

                scene.AppendLine(meshExporter.Nodes);

                if (zmo.Count > 0)
                {
                    scene.AppendLine(animExporter.Nodes);
                }

                fileStream.WriteLine(scene);

                fileStream.Close();

                return true;
            }
            catch (Exception x)
            {
                // log?
                Console.WriteLine(x.Message);
                return false;
            }
        }
    }
}