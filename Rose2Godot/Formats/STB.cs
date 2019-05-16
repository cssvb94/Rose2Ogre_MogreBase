using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;

namespace RoseFormats
{
    public class STB
    {
        public string FormatString;
        public string IDColumnName;

        private BinaryHelper bh;

        public List<uint> ColumnWidth;
        public List<string> ColumnHeader;
        public List<string> RowData;
        public string[,] CellData;

        private Encoding koreanEncoding = Encoding.GetEncoding("EUC-KR");

        public STB()
        {
            ColumnWidth = new List<uint>();
            ColumnHeader = new List<string>();
            RowData = new List<string>();
        }

        public STB(string FileName)
        {
            ColumnWidth = new List<uint>();
            ColumnHeader = new List<string>();
            RowData = new List<string>();
            Load(FileName);
        }

        public bool Load(string FileName)
        {
            try
            {
                FileStream fileStream = File.OpenRead(FileName);
                BinaryReader br = new BinaryReader(fileStream, koreanEncoding);

                bh = new BinaryHelper(br);

                try
                {
                    FormatString = koreanEncoding.GetString(br.ReadBytes(4));

                    uint DataOffset = bh.ReadDWord();
                    uint RowCount = bh.ReadDWord();
                    uint FieldCount = bh.ReadDWord();
                    uint UnknownDW = bh.ReadDWord();

                    CellData = new string[RowCount, FieldCount];

                    for (int i = 0; i < FieldCount; i++)
                    {
                        ColumnWidth.Add(bh.ReadWord());
                    }

                    for (int i = 0; i < FieldCount; i++)
                    {
                        ColumnHeader.Add(bh.ReadWString());
                    }

                    IDColumnName = bh.ReadWString();

                    for (int i = 0; i < RowCount; i++)
                    {
                        RowData.Add(bh.ReadWString());
                    }

                    for (int rowID = 0; rowID < RowCount; rowID++)
                    {
                        for (int colID = 0; colID < FieldCount; colID++)
                        {
                            string sdata = bh.ReadWString();
                            CellData[rowID, colID] = sdata;
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
