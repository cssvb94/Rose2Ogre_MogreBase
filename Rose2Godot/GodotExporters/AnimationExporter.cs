using Godot;
using Mogre;
using RoseFormats;
using System.Collections.Generic;
using System.Text;

namespace Rose2Godot.GodotExporters
{
    public class AnimationExporter
    {
        public int LastResourceIndex { get; }
        private readonly StringBuilder resource;
        private readonly StringBuilder nodes;

        public string Resources
        {
            get
            {
                return resource.ToString();
            }
        }

        public string Nodes
        {
            get
            {
                return nodes.ToString();
            }
        }

        public AnimationExporter(
            int resource_index,
            List<ZMO> zmo,
            List<string> animationNames,
            ZMD zmd)
        {
            int idx = resource_index;

            nodes = new StringBuilder();
            resource = new StringBuilder();
            LastResourceIndex = resource_index;

            nodes.AppendLine("[node name=\"AnimationPlayer\" type=\"AnimationPlayer\" parent=\"Armature\"]");
            nodes.AppendLine("root_node = NodePath(\"..:\")");

            foreach (string animName in animationNames)
            {
                resource.AppendFormat("[sub_resource id={0} type=\"Animation\"]", idx);
            }

            LastResourceIndex = idx++;
        }
    }
}
