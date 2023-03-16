using System.IO;
using System.Text;
using Mogre;

namespace RoseFormats
{
    class BinaryHelper
    {
        private BinaryReader br;
        private BinaryWriter bw;

        private readonly Encoding koreanEncoding = Encoding.GetEncoding("EUC-KR");

        public BinaryHelper(BinaryReader aBinaryReader) => br = aBinaryReader;

        public BinaryHelper(BinaryWriter aBinaryWriter) => bw = aBinaryWriter;

        public uint ReadWord() => (uint)(br.ReadByte() | (br.ReadByte() << 8));

        public uint ReadDWord() => (uint)(br.ReadByte() | (br.ReadByte() << 8) | (br.ReadByte() << 16) | (br.ReadByte() << 32));

        public Vector2 ReadUVVector2f() => new Vector2(br.ReadSingle(), 1.0f - br.ReadSingle()); // ReadVector2

        public Vector2 ReadVector2f() => new Vector2(br.ReadSingle(), br.ReadSingle()); // ReadVector2

        public void WriteVector2f(Vector2 v)
        {
            bw.Write(v.x);
            bw.Write(v.y);
        }

        public void WriteUVVector2f(Vector2 v)
        {
            bw.Write(v.x);
            bw.Write(1.0f - v.y);
        }

        public Vector3 ReadVector3f() => new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()); // ReadVector3

        public void WriteVector3f(Vector3 v)
        {
            bw.Write(v.x);
            bw.Write(v.y);
            bw.Write(v.z);
        }

        public Vector3w ReadVector3w() => new Vector3w(br.ReadUInt16(), br.ReadUInt16(), br.ReadUInt16()); // ReadVector3w

        //public void WriteVector3w(Vector3w v)
        //{
        //    bw.Write(v.x);
        //    bw.Write(v.y);
        //    bw.Write(v.z);
        //}

        public Vector4 ReadVector4f() => new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()); // ReadVector4f

        public void WriteVector4f(Vector4 v)
        {
            bw.Write(v.x);
            bw.Write(v.y);
            bw.Write(v.z);
            bw.Write(v.w);
        }

        public Quaternion ReadQuaternion()
        {
            float w, x, y, z;
            w = br.ReadSingle();
            x = br.ReadSingle();
            y = br.ReadSingle();
            z = br.ReadSingle();
            return new Quaternion(w, x, y, z);
        } // ReadQuaternion

        public void WriteQuaternion(Quaternion q)
        {
            bw.Write(q.w);
            bw.Write(q.x);
            bw.Write(q.y);
            bw.Write(q.z);
        }

        public Vector4w ReadVector4w() => new Vector4w(br.ReadUInt16(), br.ReadUInt16(), br.ReadUInt16(), br.ReadUInt16());

        //public void WriteVector4w(Vector4w v)
        //{
        //    bw.Write(v.x);
        //    bw.Write(v.y);
        //    bw.Write(v.z);
        //    bw.Write(v.w);
        //}

        // Read zero terminated string
        public string ReadZString()
        {
            string string_value = string.Empty;
            while (true)
            {
                byte byte_value = br.ReadByte();
                if (byte_value == 0)
                    return string_value;
                string_value += (char)byte_value;
            }
        } // ReadZString

        public void WriteZString(string Text)
        {
            for (int c = 0; c < Text.Length; c++)
                bw.Write(Text[c]);
            bw.Write((byte)0);
        }

        public string ReadWString()
        {
            uint string_length = ReadWord();
            return koreanEncoding.GetString(br.ReadBytes((int)string_length));
        }

        public void WriteWString(string Text) => bw.Write(Text);

    } // class
}
