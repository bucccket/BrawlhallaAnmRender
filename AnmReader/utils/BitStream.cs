using System.Text;

namespace BrawlhallaANMReader.Anm.Utils;

///<summary>Class <c>BitStream</c> is used to read bits from a byte array.</summary>
internal class BitStream
{
    ///<value>The buffer to read from.</value>
    protected readonly byte[] m_buffer;

    ///<value>The current position in the buffer.</value>
    protected int m_position;

    ///<value>The current bit position in the buffer.</value>
    protected byte m_bit_position;

    ///<summary>Initialises a new bit stream.</summary>
    ///<param name="buffer">The buffer to read from.</param>
    internal BitStream(byte[] buffer)
    {
        m_buffer = buffer;
        m_position = 0;
        m_bit_position = 0;
    }

    ///<value>The number of bits remaining in the buffer.</value>
    internal int RemainingBytes => m_buffer.Length - m_position;

    ///<summary>Reads a specified number of bits from the buffer.</summary>
    ///<param name="count">The number of bits to read.</param>
    ///<returns>The bits read from the buffer.</returns>
    ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
    internal int ReadBits(int count)
    {

        int result = 0;
        while (count != 0)
        {
            if (m_position >= m_buffer.Length * 8) throw new System.IndexOutOfRangeException("End of stream reached.");
            bool bit = (m_buffer[m_position] & (1 << (7 - m_bit_position))) != 0;
            result |= (bit ? 1 : 0) << (count - 1);
            count--;
            m_bit_position++;
            if (m_bit_position == 8)
            {
                m_position++;
                m_bit_position = 0;
            }
        }
        return result;
    }

    ///<summary>Reads a single bit from the buffer.</summary>
    ///<returns>The bit read from the buffer.</returns>
    ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
    internal bool ReadBool() => ReadBits(1) != 0;

    ///<summary>Reads a single byte from the buffer.</summary>
    ///<returns>The byte read from the buffer.</returns>
    ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
    internal byte ReadByte() => (byte)ReadBits(8);

    ///<summary>Reads a specified number of bytes from the buffer.</summary>
    ///<param name="count">The number of bytes to read.</param>
    ///<returns>The bytes read from the buffer.</returns>
    ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
    internal byte[] ReadBytes(uint count)
    {
        byte[] result = new byte[count];
        for (int i = 0; i < count; i++) result[i] = ReadByte();
        return result;
    }

    ///<summary>Reads a single short from the buffer.</summary>
    ///<returns>The short read from the buffer.</returns>
    ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
    internal short ReadShort() => (short)ReadBits(16);

    ///<summary>Reads a single int from the buffer.</summary>
    ///<returns>The int read from the buffer.</returns>
    ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
    internal int ReadInt() => ReadBits(32);

    ///<summary>Reads a single long from the buffer.</summary>
    ///<returns>The long read from the buffer.</returns>
    ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
    internal long ReadLong()
    {
        byte[] bytes = ReadBytes(8);
        return BitConverter.ToInt64(bytes, 0);
    }

    ///<summary>Reads a single char from the buffer.</summary>
    ///<returns>The char read from the buffer.</returns>
    ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
    internal char ReadChar() => (char)ReadBits(8);

    ///<summary>Reads a string from the buffer.</summary>
    ///<returns>The string read from the buffer.</returns>
    ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
    ///<exception cref="System.Text.DecoderFallbackException">Thrown when the string contains invalid characters.</exception>
    ///<remarks>Strings are encoded as a short specifying the length of the string, followed by the string itself.</remarks>
    internal string ReadString() => Encoding.UTF8.GetString(ReadBytes((ushort)ReadShort()));

    ///<summary>Reads a single float from the buffer.</summary>
    ///<returns>The float read from the buffer.</returns>
    ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
    internal float ReadFloat() => BitConverter.ToSingle(BitConverter.GetBytes(ReadInt()), 0);

    ///<summary>Reads a double float from the buffer.</summary>
    ///<returns>The double float read from the buffer.</returns>
    ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
    internal double ReadDouble() => BitConverter.Int64BitsToDouble(ReadLong());
}
