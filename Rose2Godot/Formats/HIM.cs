using System;
using System.IO;
using System.Text;

namespace RoseFormats
{
    public struct HeightmapPatch
    {
        /// <summary>
        /// Gets or sets the minimum height of the patch
        /// </summary>
        public float Minimum;

        /// <summary>
        /// Gets or sets the maximum height of the patch.
        /// </summary>
        public float Maximum;
    }

    public class HIM
    {
        private const int PATCH_WIDTH = 16;
        private const int PATCH_HEIGHT = 16;
        private const int QUAD_PATCH_COUNT = 85;

        public int Width => Heights.GetLength(1);
        public int Height => Heights.GetLength(0);
        public uint GridCount { get; private set; }
        public float GridSize { get; private set; }
        public string Name { get; private set; }

        public float this[int x, int y]
        {
            get => Heights[x, y];
            set => Heights[x, y] = value;
        }

        private BinaryHelper bh;
        private Encoding koreanEncoding = Encoding.GetEncoding("EUC-KR");
        public float[,] Heights { get; private set; }
        public int PatchCount { get; private set; }
        public float PatchSize { get; private set; }
        private HeightmapPatch[,] patches;
        private HeightmapPatch[] quadPatches;

        public HIM()
        { }

        public HIM(string FileName) => Load(FileName);

        public bool Load(string FileName)
        {
            try
            {
                FileStream fileStream = File.OpenRead(FileName);
                BinaryReader br = new BinaryReader(fileStream, koreanEncoding);

                bh = new BinaryHelper(br);

                try
                {
                    uint width = bh.ReadDWord();
                    uint height = bh.ReadDWord();

                    Heights = new float[height, width];

                    GridCount = bh.ReadDWord();
                    GridSize = br.ReadSingle();

                    for (int h = 0; h < height; h++)
                        for (int w = 0; w < width; w++)
                            Heights[h, w] = br.ReadSingle();

                    Name = br.ReadString();
                    PatchCount = br.ReadInt32();

                    patches = new HeightmapPatch[PATCH_HEIGHT, PATCH_WIDTH];
                    quadPatches = new HeightmapPatch[QUAD_PATCH_COUNT];

                    for (int h = 0; h < 16; h++)
                        for (int w = 0; w < 16; w++)
                        {
                            patches[h, w].Maximum = br.ReadSingle();
                            patches[h, w].Minimum = br.ReadSingle();
                        }

                    int quadPatchCount = br.ReadInt32();

                    for (int i = 0; i < quadPatchCount; i++)
                    {
                        quadPatches[i].Maximum = br.ReadSingle();
                        quadPatches[i].Minimum = br.ReadSingle();
                    }
                }
                finally
                {
                    br.Close();
                    fileStream.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
