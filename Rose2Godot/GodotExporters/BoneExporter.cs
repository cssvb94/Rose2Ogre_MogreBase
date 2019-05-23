using Godot;
using Mogre;
using RoseFormats;
using System.Text;

namespace Rose2Godot.GodotExporters
{
    public class BoneExporter
    {
        private readonly StringBuilder sbone;
        private readonly Translator translator = new Translator();

        public int LastResourceIndex { get; }

        public BoneExporter(int resource_index, ZMD zmd)
        {
            sbone = new StringBuilder();
            //sbone.Append("; skelton node - mesh nodes parent\n");
            sbone.AppendLine("[node name=\"Armature\" type=\"Skeleton\" parent=\".\"]");
            sbone.AppendLine("bones_in_world_transform = true");
            sbone.AppendLine("transform = Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)");

            int idx = 0;

            GodotTransform IdentityTransform = new GodotTransform(GodotQuat.Identity, translator.ToVector3(Vector3.ZERO));

            foreach (RoseBone bone in zmd.Bone)
            {
                GodotTransform transform = new GodotTransform(translator.ToQuat(bone.Rotation), translator.ToVector3(bone.Position));
                bone.Rotation.ToAngleAxis(out Radian radian, out Vector3 axis);

                sbone.AppendFormat("bones/{0}/name = \"{1}\"\n", idx, bone.Name);
                sbone.AppendFormat("bones/{0}/parent = {1}\n", idx, idx == 0 ? -1 : bone.ParentID);
                //sbone.AppendFormat("; Position: {0}\n; Rotation: {1}\n; Angle(rad): {2:G4} Angle(deg): {3:G4} Axis: {4}\n", bone.Position, bone.Rotation, radian.ValueRadians, radian.ValueDegrees, axis);
                sbone.AppendFormat("bones/{0}/rest = {1}\n", idx, translator.Transform2String(transform));
                //sbone.AppendFormat("bones/{0}/pose = {1}\n", idx, "Transform(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0)");
                sbone.AppendFormat("bones/{0}/enabled = true\n", idx);
                sbone.AppendFormat("bones/{0}/bound_children = []\n", idx);
                idx++;
            }

            if (zmd.DummiesCount > 0)
            {
                foreach (RoseBone dummy in zmd.Dummy)
                {
                    GodotTransform transform = new GodotTransform(translator.ToQuat(dummy.Rotation), translator.ToVector3(dummy.Position));

                    sbone.AppendFormat("bones/{0}/name = \"{1}\"\n", idx, dummy.Name);
                    sbone.AppendFormat("bones/{0}/parent = {1}\n", idx, idx == 0 ? -1 : dummy.ParentID);
                    //sbone.AppendFormat("; Position: {0}\n; Rotation: {1}\n", dummy.Position, dummy.Rotation);
                    sbone.AppendFormat("bones/{0}/rest = {1}\n", idx, translator.Transform2String(transform));
                    //sbone.AppendFormat("bones/{0}/pose = {1}\n", idx, translator.Transform2String(IdentityTransform));
                    sbone.AppendFormat("bones/{0}/enabled = true\n", idx);
                    sbone.AppendFormat("bones/{0}/bound_children = []\n", idx);
                    idx++;
                }
            }

            LastResourceIndex += resource_index;
        }

        public override string ToString()
        {
            return sbone.ToString();
        }
    }
}
