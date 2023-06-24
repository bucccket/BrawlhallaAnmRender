using System.Text;

namespace BrawlhallaANMReader.utils
{
    ///<summary>Class <c>ByteStream</c> is used to read bytes from a byte array.</summary>
    public class ByteStream
    {
        ///<value>The byte array to read from.</value>
        protected readonly byte[] m_buffer;

        ///<value>The current position in the buffer.</value>
        protected int m_position = 0;

        ///<summary>Constructor used to create a Byte Stream.</summary>
        ///<param name="buffer">The byte array to read from.</param>
        internal ByteStream(byte[] buffer) { m_buffer = buffer; }

        ///<summary>Reads a single byte from the buffer.</summary>
        ///<returns>The byte read from the buffer.</returns>
        ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
        internal byte ReadByte()
        {
            if (m_position >= m_buffer.Length) throw new System.IndexOutOfRangeException("End of stream reached.");
            return m_buffer[m_position++];
        }

        ///<summary>Reads a specified number of bytes from the buffer.</summary>
        ///<param name="count">The number of bytes to read.</param>
        ///<returns>The bytes read from the buffer.</returns>
        ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
        internal byte[] ReadBytes(uint count)
        {
            if (m_position + count > m_buffer.Length) throw new System.IndexOutOfRangeException("End of stream reached");
            byte[] result = new byte[count];
            for (int i = 0; i < count; i++) result[i] = ReadByte();
            return result;
        }

        ///<summary>Reads a short from the buffer.</summary>
        ///<returns>The short read from the buffer.</returns>
        ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
        internal short ReadShort() => BitConverter.ToInt16(ReadBytes(2), 0);

        ///<summary>Reads a ushort from the buffer.</summary>
        ///<returns>The ushort read from the buffer.</returns>
        ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
        internal ushort ReadUShort() => BitConverter.ToUInt16(ReadBytes(2), 0);

        ///<summary>Reads an int from the buffer.</summary>
        ///<returns>The int read from the buffer.</returns>
        ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
        internal int ReadInt() => BitConverter.ToInt32(ReadBytes(4), 0);

        ///<summary>Reads a uint from the buffer.</summary>
        ///<returns>The uint read from the buffer.</returns>
        ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
        internal uint ReadUInt() => BitConverter.ToUInt32(ReadBytes(4), 0);

        ///<summary>Reads a bool from the buffer.</summary>
        ///<returns>The bool read from the buffer.</returns>
        ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
        internal bool ReadBool() => ReadByte() != 0;

        ///<summary>Reads a float from the buffer.</summary>
        ///<returns>The float read from the buffer.</returns>
        ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
        internal float ReadFloat() => BitConverter.ToSingle(ReadBytes(4), 0);

        ///<summary>Reads a double from the buffer.</summary>
        ///<returns>The double read from the buffer.</returns>
        ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
        internal double ReadDouble() => BitConverter.ToDouble(ReadBytes(8), 0);

        ///<summary>Reads a string from the buffer.</summary>
        ///<returns>The string read from the buffer.</returns>
        ///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
        ///<exception cref="System.Text.DecoderFallbackException">Thrown when the string contains invalid characters.</exception>
        ///<remarks>Strings are encoded as a short specifying the length of the string, followed by the string itself.</remarks>
        internal string ReadString() => Encoding.UTF8.GetString(ReadBytes(ReadUShort()));
    }
}
