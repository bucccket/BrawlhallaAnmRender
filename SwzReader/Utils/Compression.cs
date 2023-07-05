using System.IO.Compression;

namespace BrawlhallaANMReader.Swz.Utils;

///<summary>Class <c>Compression</c> provides compression and decompression functionality.</summary>
internal static class Compression
{
    ///<summery>Decompresses a buffer using the ZLib compression algorithm.</summery>
    ///<param name="compressed_buffer">The compressed stream to decompress.</param>
    ///<returns>The decompressed buffer.</returns>
    internal static byte[] InflateBuffer(byte[] compressed_buffer)
    {
        using MemoryStream compressed_stream = new(compressed_buffer);
        ZLibStream decompressor = new(compressed_stream, CompressionMode.Decompress);
        byte[] array = new byte[1024];
        using MemoryStream decompressed_stream = new();
        using (decompressor)
        {
            int count;
            while ((count = decompressor.Read(array, 0, array.Length)) != 0) decompressed_stream.Write(array, 0, count);
        }
        return decompressed_stream.ToArray();
    }

    ///<summery>Compresses a buffer using the ZLib compression algorithm.</summery>
    ///<param name="uncompressed_buffer">The uncompressed stream to compress.</param>
    ///<returns>The compressed buffer.</returns>
    internal static byte[] DeflateBuffer(byte[] uncompressed_buffer)
    {
        using MemoryStream compressed_stream = new();
        ZLibStream compressor = new(compressed_stream, CompressionLevel.SmallestSize);
        using (compressor) compressor.Write(uncompressed_buffer, 0, uncompressed_buffer.Length);
        return compressed_stream.ToArray();
    }
}
