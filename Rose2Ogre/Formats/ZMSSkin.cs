using Mogre;
using System;

namespace RoseFormats
{
    class ZMSSkin
    {
        public Vector4 BoneWeights = new Vector4();
        public Vector4w BoneIndices = new Vector4w();

        public ZMSSkin(Vector4 Weights, Vector4w Indices)
        {
            BoneWeights = Weights;
            BoneIndices = Indices;
        }

        public override string ToString()
        {
            return String.Format("ID:{0} W: {1:0.00}, ID:{2} W: {3:0.00}, ID:{4} W: {5:0.00}, ID:{6} W: {7:0.00}", 
                BoneIndices[0], BoneWeights[0], BoneIndices[1], BoneWeights[1],
                BoneIndices[2], BoneWeights[2], BoneIndices[3], BoneWeights[3]);
        }
    }
}
