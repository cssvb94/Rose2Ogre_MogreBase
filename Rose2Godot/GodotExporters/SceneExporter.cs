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
        public readonly uint num_resources;
        public readonly List<ZMS> zms;
        public readonly ZMD zmd;
        public readonly List<ZMO> zmo;
        public readonly string objName;

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

            //num_resources = (uint)(zms.Count + zms.Count + zmo.Count);
            num_resources = 1;
            scene.AppendFormat("[gd_scene load_steps={0} format=2]\n\n", num_resources);
        }

        public bool ExportScene(string fileName)
        {
            int idx = 1;
            try
            {
                StreamWriter fileStream = new StreamWriter(fileName);

                MeshExporter meshExporter = new MeshExporter(objName,idx, zms);

                idx = meshExporter.LastResourceIndex;

                scene.AppendLine(meshExporter.ToString());

                if (zmd.BonesCount > 0)
                {
                    BoneExporter boneExporter = new BoneExporter(objName, idx, zmd);
                    scene.AppendLine(boneExporter.ToString());
                    idx = meshExporter.LastResourceIndex;
                }
                
                fileStream.WriteLine(scene.ToString());

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