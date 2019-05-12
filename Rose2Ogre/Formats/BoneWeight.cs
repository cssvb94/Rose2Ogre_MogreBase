
namespace RoseFormats
{
    public class BoneWeight
    {
        public int VertexID;
        public int BoneID;
        public float Weight;

        public BoneWeight()
        {
        }

        public BoneWeight(int VertexID, int BoneID, float Weight)
        {
            this.VertexID = VertexID;
            this.BoneID = BoneID;
            this.Weight = Weight;
        }

        public override string ToString()
        {
            return string.Format("VertexID: {0}, BoneID: {1}, Weight: {2}", VertexID, BoneID, Weight);
        }
    }
}
