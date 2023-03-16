using System.IO;

namespace DDSReader.Internal
{
    public interface IDataDecoder
    {
        byte[] DecodeFrame(Stream dataSource, uint width, uint height);
    }
}