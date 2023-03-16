using System.IO;

namespace DDSReader.Internal.Decoders
{
    public class DXT4Decoder : DXT5Decoder
    {
        public DXT4Decoder(DDSHeader header) : base(header)
        {
        }

        public byte[] DecodeFrame(Stream dataSource, uint width, uint height)
        {
            var dxt5Data = base.DecodeFrame(dataSource, width, height);

            DXT2Decoder.CorrectPreMult(dxt5Data);

            return dxt5Data;
        }
    }
}