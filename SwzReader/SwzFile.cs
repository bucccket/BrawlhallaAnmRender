using BrawlhallaANMReader.Swz.Utils;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;

namespace BrawlhallaANMReader.Swz;

//hope I dont forget to remove this but why is the ByteStream not you know.... A STREAM
public class SwzFile
{
    public static uint DecryptKey { get; set; } = default!;

    public static void Decrypt(Stream stream)
    {
        PRNG prng;
        List<string> files = new();
        SetData(stream, out prng);

        //TODO: do something with the data here!?!?
        while (stream.Position != stream.Length)
        {
            files.Add(GetData(stream, prng));
        }
    }

    private static void SetData(Stream stream, out PRNG prng)
    {
        uint expectedHash = ReadUInt32BE(stream);
        uint seed = ReadUInt32BE(stream);

        prng = new(seed ^ DecryptKey);

        uint hash = 0x2DF4A1CDu;
        uint hashRounds = DecryptKey % 0x1Fu + 5;

        if (hashRounds != 0)
        {
            for (int i = 0; i < hashRounds; i++)
                hash ^= prng.NextRandom();
        }

        if (hash != expectedHash)
        {
            throw new SwzException($"SetData checksum mismatch. got hash {hash}, expected hash {expectedHash}.");
        }
    }

    private static string GetData(Stream stream, PRNG prng)
    {
        uint deflatedSize = ReadUInt32BE(stream) ^ prng.NextRandom();
        uint inflatedSize = ReadUInt32BE(stream) ^ prng.NextRandom();
        uint expectedHash = ReadUInt32BE(stream);

        if (deflatedSize + stream.Position > stream.Length)
        {
            throw new SwzException("deflated data truncated.");
        }

        byte[] buffer = new byte[deflatedSize];
        stream.Read(buffer);

        uint hash = prng.NextRandom();

        for (int i = 0; i < deflatedSize; i++)
        {
            buffer[i] ^= (byte)(((0xFFu << (i & 0xF)) & prng.NextRandom()) >> (i & 0xF));
            hash = buffer[i] ^ RotateRight(hash, i % 7 + 1);
        }

        if(hash != expectedHash)
        {
            throw new SwzException($"GetData checksum mismatch. got hash {hash}, expected hash {expectedHash}.");
        }

        byte[] inflatedData = Compression.InflateBuffer(buffer);
        return Encoding.UTF8.GetString(inflatedData);
    }

    private static uint RotateRight(uint v, int bits)
    {
        return (v >> bits) | (v << (32 - bits));
    }

    private static uint ReadUInt32BE(Stream stream)
    {
        byte[] buffer = new byte[4];
        stream.Read(buffer);
        return (uint)(buffer[3]
            | (buffer[2] << 8)
            | (buffer[1] << 16)
            | (buffer[0] << 24));
    }

    private static void WriteUInt32BE(Stream stream, uint value)
    {
        byte[] buffer = new byte[4]
        {
            (byte)((value >> 24) & 0xFF),
            (byte)((value >> 16) & 0xFF),
            (byte)((value >> 08) & 0xFF),
            (byte)((value >> 00) & 0xFF)
        };

        stream.Write(buffer);
    }
}
