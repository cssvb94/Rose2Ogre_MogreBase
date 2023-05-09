using Godot;
using Revise.ZMD;
using System.Linq;
using System.Text;

namespace Rose2Godot.GodotExporters
{
    public class BoneExporter
    {
        public int last_resource_index { get; private set; }

        private readonly StringBuilder bone_node;
        private readonly StringBuilder bone_attachment_node;

        private void AppendGodotBone(ref int index, Bone bone)
        {
            bone.Translation *= 0.01f; // scale down
            GodotQuat rotation = new GodotQuat(bone.Rotation.X, bone.Rotation.Z, bone.Rotation.Y, bone.Rotation.W).Inverse().Normalized();
            GodotVector3 position = Translator.ToGodotVector3XZY(bone.Translation);

            GodotTransform transform = new GodotTransform(rotation, position);

            bone_node.AppendFormat($"bones/{index}/name = \"{bone.Name}\"\n");
            bone_node.AppendFormat($"bones/{index}/parent = {(index == 0 ? -1 : bone.Parent)}\n");
            bone_node.AppendFormat($"bones/{index}/rest = {Translator.GodotTransform2String(transform)}\n");
            bone_node.AppendFormat($"bones/{index}/position = Vector3{position}\n");
            bone_node.AppendFormat($"bones/{index}/rotation = Quaternion{rotation}\n");
            bone_node.AppendFormat($"bones/{index}/scale = Vector3{GodotVector3.One}\n");
            bone_node.AppendFormat($"bones/{index}/enabled = true\n");
            index++;
        }

        public BoneExporter(int resource_index, BoneFile zmd)
        {
            bone_node = new StringBuilder();
            bone_attachment_node = new StringBuilder();
            bone_node.Append("; skeleton node - mesh nodes parent\n");
            bone_node.AppendLine("[node name=\"Skeleton\" type=\"Skeleton\" parent=\".\"]");
            bone_node.AppendLine("bones_in_world_transform = true");
            bone_node.AppendLine("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)");

            int idx = 0;
            foreach (Bone bone in zmd.Bones)
            {
                bone_attachment_node.Append(MakeBoneAttachment(bone));
                AppendGodotBone(ref idx, bone);
            }

            if (zmd.DummyBones.Any())
            {
                bone_node.Append("; dummy bones\n");
                foreach (Bone dummy in zmd.DummyBones)
                {
                    bone_attachment_node.Append(MakeBoneAttachment(dummy));
                    AppendGodotBone(ref idx, dummy);
                }
            }
            last_resource_index += resource_index;

            bone_node.AppendLine();
            bone_node.AppendLine(bone_attachment_node.ToString());
        }

        private string MakeBoneAttachment(Bone bone)
        {
            StringBuilder attachment = new StringBuilder();
            attachment.AppendLine($"[node name=\"{bone.Name}\" type=\"BoneAttachment\" parent=\"Skeleton\"]");
            attachment.AppendLine($"bone_name = \"{bone.Name}\"\n");

            return attachment.ToString();
        }

        public override string ToString() => bone_node.ToString();
    }
}
