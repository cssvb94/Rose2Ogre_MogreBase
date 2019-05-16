using Mogre;
using System.Collections.Generic;

namespace Rose2Godot.GodotExporters
{
    public struct Translator
    {
        public string Matrix4ToTransform(Matrix4 m)
        {
            Matrix4 mtx = FixMatrix(m);
            // Matrix4 mtx = m.Clone();

            List<string> vals = new List<string>();

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    vals.Add(string.Format("{0:0.0000}", mtx[row, col]));
                }
            }

            for (int axis = 0; axis < 3; axis++)
            {
                vals.Add(string.Format("{0:0.0000}", mtx[axis, 3]));
            }

            return "Transform(" + string.Join(", ", vals.ToArray()) + ")";
        }

        private Matrix4 FixMatrix(Matrix4 m)
        {
            Matrix4 trans = m.Clone();
            const int up_axis = 2; // from y up to z up
            for (int i = 0; i < 3; i++)
            {
                float t1 = trans[1, i];
                trans[1, i] = trans[up_axis, i];
                trans[up_axis, i] = t1;
            }

            for (int i = 0; i < 3; i++)
            {
                float t2 = trans[i, 1];
                trans[i, 1] = trans[i, up_axis];
                trans[up_axis, i] = t2;
            }

            float t = trans[1, 3];
            trans[1, 3] = trans[up_axis, 3];
            trans[up_axis, 3] = t;

            trans[up_axis, 0] = -trans[up_axis, 0];
            trans[up_axis, 1] = -trans[up_axis, 1];
            trans[0, up_axis] = -trans[0, up_axis];
            trans[1, up_axis] = -trans[1, up_axis];
            trans[up_axis, 3] = -trans[up_axis, 3];

            return trans;
        }
    }
}
