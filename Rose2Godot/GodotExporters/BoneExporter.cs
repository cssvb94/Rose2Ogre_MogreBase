using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogre;
using RoseFormats;

namespace Rose2Godot.GodotExporters
{
    public class BoneExporter
    {
        private readonly StringBuilder sbone;

        public int LastResourceIndex { get; }

        public BoneExporter(string mesh_name, int resource_index, ZMD zmd)
        {
            Matrix3 rotate_M3 = new Quaternion(new Degree(90f), new Vector3(0f, 0f, 1f)).ToRotationMatrix();
            Matrix4 rotate = new Matrix4(rotate_M3);


            sbone = new StringBuilder();
            sbone.AppendFormat("[node name=\"Armature\" type=\"Skeleton\" parent=\"{0}\"]\n\n", mesh_name);
            sbone.AppendLine("bones_in_world_transform = true");
            sbone.AppendLine("transform = Transform(1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0)");

            int idx = 0;

            Translator translator = new Translator();

            foreach (RoseBone bone in zmd.Bone)
            {
                Matrix4 m = Matrix4.Identity;

                //Vector3 transform = bone.TransformMatrix.GetTrans() * 0.01f;
                //bone.TransformMatrix.SetTrans(transform);

                sbone.AppendFormat("bones/{0}/name = \"{1}\"\n", idx, bone.Name);
                sbone.AppendFormat("bones/{0}/parent = {1}\n", idx, idx == 0 ? -1 : bone.ParentID);
                sbone.AppendFormat("bones/{0}/rest = {1}\n", idx, translator.Matrix4ToTransform(bone.TransformMatrix));
               // sbone.AppendFormat("bones/{0}/pose = {1}\n", idx, Matrix4ToString(Matrix4.Identity));
                sbone.AppendFormat("bones/{0}/enabled = true\n", idx);
                sbone.AppendFormat("bones/{0}/bound_children = []\n", idx);
                idx++;
            }

            LastResourceIndex += resource_index;
        }

        private string Matrix4ToString(Matrix4 m)
        {
            List<string> ms = new List<string>();

            for (int i = 0; i < 12; i++)
            {
                ms.Add(string.Format("{0:0.0000}", m[i]));
            }

            return string.Format("Transform({0})", string.Join(", ", ms.ToArray()));
        }

        public override string ToString()
        {
            return sbone.ToString();
        }
    }
}
