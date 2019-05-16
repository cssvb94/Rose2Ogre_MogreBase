using System;
using System.IO;
using System.Text;

namespace RoseFormats
{
    public class HIM
    {
        public uint Height;
        public uint Width;
        public uint GridCount;
        public float GridSize;

        public float MinValue = 0f;
        public float MaxValue = 0f;
        public float this[int x, int y]
        {
            get
            {
                return element[x, y];
            }
            set
            {
                element[x, y] = value;
            }
        }

        private BinaryHelper bh;
        private Encoding koreanEncoding = Encoding.GetEncoding("EUC-KR");
        private float[,] element = new float[65, 65];

        public HIM()
        {
        }

        public HIM(string FileName)
        {
            Load(FileName);
        }

        public bool Load(string FileName)
        {
            MaxValue = 0f;
            MinValue = 0f;

            try
            {
                FileStream fileStream = File.OpenRead(FileName);
                BinaryReader br = new BinaryReader(fileStream, koreanEncoding);

                bh = new BinaryHelper(br);

                try
                {
                    Height = bh.ReadDWord();
                    Width = bh.ReadDWord();
                    GridCount = bh.ReadDWord();
                    GridSize = br.ReadSingle();
                    for (int i = 0; i < 65; i++)
                    {
                        for (int j = 0; j < 65; j++)
                        {
                            element[i, j] = br.ReadSingle();
                            if (element[i, j] > MaxValue)
                                MaxValue = element[i, j];
                            if (element[i, j] < MinValue)
                                MinValue = element[i, j];
                        }
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
